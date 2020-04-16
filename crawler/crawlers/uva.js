const request = require('superagent')

/** uva 的 api 接口：https://uhunt.onlinejudge.org/api
 * 由于uva不支持分页处理，每次必须把全部信息拉取下来，在前端查询时有可能超过 cors-proxy 的长度限制
 * 不过普通用户的话也不会有太多数据
 */
module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const uidRes = await request.get('https://uhunt.onlinejudge.org/api/uname2uid/' + username)

  if (uidRes.body === 0) {
    throw new Error('The user does not exist')
  }

  const res = await request
    .get('https://uhunt.onlinejudge.org/api/subs-user/' + uidRes.body)

  const acSet = new Set()
  const problemArray = res.body.subs
  problemArray.forEach(function (element) {
    if (element[2] === 90) {
      acSet.add(element[1])
    }
  })

  const solvedList = []
  for (let item of acSet) {
    solvedList.push(String(item))
  }

  return {
    solved: acSet.size,
    submissions: problemArray.length,
    solvedList,
  }
}
