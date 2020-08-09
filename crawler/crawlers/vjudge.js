const request = require('superagent')

const hostName = 'vjudge.net'

// put the object at module level to cache the session
let agent = request.agent()
let config = {}

/**
 * vjudge 的设置项
 * @typedef vjudgeCrawlerConfig
 * @type {Object}
 * @param {String} crawler_login_user - 登录 vjudge 使用的用户名，vj需要一个账户才能访问api
 * @param {String} crawler_login_password
 */

/**
 * vjudge 的爬虫函数
 * @param {vjudgeCrawlerConfig} config
 * @param username 要爬取的用户名
 * @returns {Promise<crawlerReturns>} - 见 configReader
 */
module.exports = async function (localConfig, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  config = localConfig
  // console.log(config)

  const acSet = new Set()
  const submissionsByCrawlerName = {}
  const submissions = await queryForNumber(username, null, acSet, submissionsByCrawlerName)

  return {
    solved: acSet.size,
    submissions: submissions,
    solvedList: [...acSet],
    submissionsByCrawlerName,
  }
}

async function tryLogin() {
  let loginStatus
  try {
    loginStatus = await agent
      .post(`https://${hostName}/user/login`)
      .type('form')
      .send({
        'username': config.crawler_login_user,
        'password': config.crawler_login_password,
      })
  } catch (err) {
    const error = new Error('vjudge login failed')
    error.innerError = err
    throw error
  }
  if (loginStatus.text !== 'success') {
    throw new Error('vjudge login failed')
  }

  // console.log('vjudge login success')
}

/**
 * take a oj name in virtual judge and map its name to crawler name
 * 
 * if name cannot be mapped, return original name
 * @param {string} nameInVjudge 
 */
function mapOjName(nameInVjudge) {

  // oj that can map its name to crawler name by changing into lower case
  const simpleMapOj = new Set([
    'codeforces',
    'uva',
    'uvalive',
    'poj',
    'hdu',
    'zoj',
    'fzu',
    'spoj',
    'timus',
    'csu',
    // 'hust',
    'atcoder',
    'aizu',
    'codechef',
    'nbut',
  ])
  // crawler name map
  const ojMap = {
    '': 'NO_NAME',
    'LibreOJ': 'loj',
    'URAL': 'timus',
    'HYSBZ': 'dashiye',
    // it looks like a typo of vjudge
    'EIJudge': 'eljudge',
    'Gym': 'codeforces',
    '51Nod': 'nod',
  }

  if (simpleMapOj.has(nameInVjudge.toLowerCase())) {
    return nameInVjudge.toLowerCase()
  } else if (nameInVjudge in ojMap) {
    return ojMap[nameInVjudge]
  } else {
    return nameInVjudge
  }
}

const MAX_PAGE_SIZE = 500

/**
 * 递归查询题数
 * @param username
 * @param maxId
 * @param {Set<{String}>} acSet The problem list that accepted, function will change this object
 * @param {Object.{string, number}} submissionsByCrawlerName submissions in each oj, function will change this object
 * @returns {Promise<Number>}
 */
async function queryForNumber(username, maxId, acSet, submissionsByCrawlerName) {

  // 发起请求 /////////////////////////////////////////////////////////////
  const queryObject = {
    username: username,
    pageSize: MAX_PAGE_SIZE,
  }

  if (maxId) {
    queryObject.maxId = maxId
  }

  // console.log ('queryObject', queryObject)

  const res = await agent
    .get(`https://${hostName}/user/submissions`)
    .query(queryObject)

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  if (res.body.error && res.body.error === 'Please login and confirm your email first') {
    await tryLogin()
    return queryForNumber(username, maxId, acSet, submissionsByCrawlerName)
  }

  // 处理结果 /////////////////////////////////////////////////////////////
  if (res.body.error && /User .* does not exist/.test(res.body.error)) {
    throw new Error('The user does not exist')
  }

  const problemArray = res.body.data

  /*
  delete res.body.data
  console.log('body except data', res.body)
  */

  // console.log(probremArray)
  // console.log(probremArray[0][0])

  if (problemArray.length === 0) {
    return 0
  }

  problemArray.forEach(function (element) {
    const crawlerName = mapOjName(element[2])
    if (!submissionsByCrawlerName[crawlerName]) {
      submissionsByCrawlerName[crawlerName] = 1
    } else {
      submissionsByCrawlerName[crawlerName] += 1
    }
    if (element[4] === 'AC') {
      const title = crawlerName + '-' + element[3]
      acSet.add(title)
    }
  })

  const total = problemArray.length

  // vj以id从大到小的顺序返回题目情况，把最后一个题目的id-1作为下次查询的MaxId
  // id必须要减一，否则最后一题会重复（2018-3-17日最后一次实验）
  const newMaxId = problemArray[total - 1][0] - 1

  /*
  console.log({
      newMaxId: newMaxId,
      solved: solved,
      total: total
  })
  */

  // 递归处理（返回结果或再发起请求） ////////////////////////////////////////////
  if (total < MAX_PAGE_SIZE) {
    // 已经读完
    return total
  } else {
    const ret = await queryForNumber(username, newMaxId, acSet, submissionsByCrawlerName)
    return ret + total
  }
}