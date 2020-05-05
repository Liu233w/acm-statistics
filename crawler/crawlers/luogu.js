const request = require('superagent')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const uidRes = await request
    .get('https://www.luogu.com.cn/fe/api/user/search')
    .query({keyword: username})

  if (!uidRes.ok) {
    throw new Error(`Server Response Error: ${uidRes.status}`)
  }

  const uidJSON = JSON.parse(uidRes.text)

  if (uidJSON.users[0] == null) {
    throw new Error('The user does not exist')
  }

  const uid = uidJSON.users[0].uid
  const res = await request
    .get('https://www.luogu.com.cn/user/' + uid)

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  try {

    const userJson = JSON.parse(decodeURIComponent(res.text.match(/decodeURIComponent\("(.*?)"\)/i)[1]))
    const solvedJson = userJson.currentData.passedProblems
    const acList = solvedJson.map((p) => p.pid)

    return {
      submissions: userJson.currentData.user.submittedProblemCount,
      solved: userJson.currentData.user.passedProblemCount,
      solvedList: acList,
    }
  }
  catch (e) {
    throw new Error('Error while parsing')
  }

}