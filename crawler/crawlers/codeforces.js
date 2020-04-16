const request = require('superagent')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const acSet = new Set()
  const submissions = await queryForNumber(username, 1, acSet)

  return {
    solved: acSet.size,
    submissions: submissions,
    solvedList: [...acSet],
  }
}

const MAX_PAGE_SIZE = 10000

/**
 * 递归查询题数
 * @param username
 * @param pageCount {Number} - 这个是页数，数字是几就是第几页
 * @param acSet {Set<{String}>} - ac的题目列表，会修改此对象
 * @returns {Promise<Number>}
 */
async function queryForNumber(username, pageCount, acSet) {

  // 发起请求 /////////////////////////////////////////////////////////////
  const queryObject = {
    handle: username,
    from: (pageCount - 1) * MAX_PAGE_SIZE + 1,
    count: MAX_PAGE_SIZE,
  }

  let res = null
  try {
    res = await request
      .get('http://codeforces.com/api/user.status')
      .query(queryObject)
  } catch (e) {

    if (e.response && e.response.body.status) {// 有 response 一定有 body
      // 有 response 且以 json 的格式相应
      const comment = e.response.body.comment
      if (/handle: User with handle .* not found/.test(comment)) {
        throw new Error('The user does not exist')
      } else {
        throw new Error(comment)
      }
    } else if (e.response) {
      throw new Error(e.response.body)
    } else {
      throw e
    }
  }

  // 处理结果 /////////////////////////////////////////////////////////////
  const problemArray = res.body.result

  if (problemArray.length === 0) {
    return 0
  }

  problemArray.forEach(function (element) {
    if (element.verdict === 'OK') {
      const problem = element.problem
      const title = problem.contestId + '-' + problem.index
      acSet.add(title)
    }
  })

  const total = problemArray.length

  // 递归处理（返回结果或再发起请求） ////////////////////////////////////////////
  if (total < MAX_PAGE_SIZE) {
    // 已经读完
    return total
  } else {
    const ret = await queryForNumber(username, pageCount + 1, acSet)
    return ret + total
  }
}