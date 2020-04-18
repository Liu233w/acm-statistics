const request = require('superagent')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  let statusRes
  try {
    statusRes = await request('https://judgeapi.u-aizu.ac.jp/users/' + username)
  } catch (err) {
    if (err.response && err.response.status === 404) {
      throw new Error('The user does not exist')
    }
    throw err
  }
  // seems that the list is big enough, we don't need pages
  const listRes = await request(`https://judgeapi.u-aizu.ac.jp/solutions/users/${username}?page=0&size=10000`)

  const solvedSet = new Set(listRes.body.map(a => a.problemId))

  return {
    solved: statusRes.body.status.solved,
    submissions: statusRes.body.status.submissions,
    solvedList: [...solvedSet],
  }
}