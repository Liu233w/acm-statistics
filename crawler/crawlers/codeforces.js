const request = require('superagent')

module.exports = async function (config, username) {

  return queryForNumber(username, 1)
}

const MAX_PAGE_SIZE = 10000

async function queryForNumber(username, pageCount) {

  // 发起请求 /////////////////////////////////////////////////////////////
  const queryObject = {
    handle: username,
    from: pageCount, // 这个是页数，数字是几就是第几页
    count: MAX_PAGE_SIZE
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
        throw new Error('用户不存在')
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
    return {
      solved: 0,
      submissions: 0
    }
  }

  let solved = 0
  problemArray.forEach(function (element) {
    if (element.verdict === 'OK') {
      ++solved
    }
  })

  const total = problemArray.length

  // 递归处理（返回结果或再发起请求） ////////////////////////////////////////////
  if (total < MAX_PAGE_SIZE) {
    // 已经读完
    return {
      solved: solved,
      submissions: total
    }
  } else {
    const ret = await queryForNumber(username, pageCount + 1)
    return {
      solved: ret.solved + solved,
      submissions: ret.submissions + total
    }
  }
}