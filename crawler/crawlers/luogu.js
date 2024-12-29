const request = require('superagent')

async function getUserId(username) {
  const uidRes = await request
    .get('https://www.luogu.com.cn/api/user/search')
    .query({keyword: username})

  if (!uidRes.ok) {
    throw new Error(`Server Response Error: ${uidRes.status}`)
  }

  const uidJSON = JSON.parse(uidRes.text)

  if (uidJSON.users[0] == null) {
    throw new Error('The user does not exist')
  }

  return uidJSON.users[0].uid
}

function getUserJson(text) {
  try {
    return JSON.parse(decodeURIComponent(text.match(/decodeURIComponent\("(.*?)"\)/i)[1]))
  } catch (e) {
    throw new Error('Error while parsing')
  }
}

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  let res = await request
    .get('https://www.luogu.com.cn/user/' + username)
  if (!res.ok || getUserJson(res.text).code === 404) {
    const uid = await getUserId(username)
    res = await request
      .get('https://www.luogu.com.cn/user/' + uid)
  }

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  const userJson = getUserJson(res.text)
  if (userJson.code === 404) {
    throw new Error('User not found')
  }

  if (userJson.code !== 200) {
    throw new Error(`Parse error. Message from luogu: ${userJson.currentTitle}`)
  }

  try {
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