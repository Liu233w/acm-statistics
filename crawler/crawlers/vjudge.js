const request = require('superagent')

const hostName = 'vjudge.net'

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
module.exports = async function (config, username) {

  // console.log(config)

  const agent = request.agent()

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
    const error = new Error('vjudge 爬虫登录失败')
    error.innerError = err
    throw error
  }
  if (loginStatus.text !== 'success') {
    throw new Error('vjudge 爬虫登录失败')
  }

  console.log('vjudge 登陆成功')

  const acSet = new Set()
  const submissions = await queryForNumber(agent, username, null, acSet)

  return {
    solved: acSet.size,
    submissions: submissions,
  }
}

const MAX_PAGE_SIZE = 500

/**
 * 递归查询题数
 * @param agent
 * @param username
 * @param maxId
 * @param acSet {Set<{String}>} - ac的题目列表，会修改此对象
 * @returns {Promise<Number>}
 */
async function queryForNumber(agent, username, maxId, acSet) {

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

  // 处理结果 /////////////////////////////////////////////////////////////
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
    if (element[4] === 'AC') {
      const title = element[2] + '-' + element[3]
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
    const ret = await queryForNumber(agent, username, newMaxId, acSet)
    return ret + total
  }
}