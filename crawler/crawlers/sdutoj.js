const request = require('superagent')

async function fetchSDUTOJ(api, data) {
  const res = await request
    .post(`https://oj.sdutacm.cn/onlinejudge3/api/${api}`)
    .set('Content-Type', 'application/json;charset=utf-8')
    .send(data)
  if (!(res.ok && res.body && res.body.success)) {
    throw new Error(`Server Response Error: ${res.status}, code: ${res.body ? res.body.code : ''}`)
  }
  return res.body.data
}

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const userSearchRes = await fetchSDUTOJ('getUserList', {
    username,
    page: 1, 
    order: [['accepted','DESC']],
    limit: 1000,
  })
  const user = userSearchRes.rows.find(user => user.username === username)
  if (!user) {
    throw new Error('The user does not exist')
  }
  const userId = user.userId

  const [statsRes, detailRes] = await Promise.all([
    fetchSDUTOJ('getUserProblemResultStats', { userId }),
    fetchSDUTOJ('getUserDetail', { userId }),
  ])

  return {
    submissions: detailRes.submitted,
    solved: detailRes.accepted,
    solvedList: statsRes.acceptedProblemIds.map(pid => `${pid}`),
  }
}
