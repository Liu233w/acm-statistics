const request = require('superagent')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  // request atcoder to detect if user exists, to reduce throughput of kenkoooo's api
  try {
    await request
      .get('https://atcoder.jp/users/' + username)
  } catch (e) {
    if (e.message === 'Not Found') {
      throw new Error('The user does not exist')
    } else {
      throw e
    }
  }

  // Thank @kenkoooo for the api
  // Source code: https://github.com/kenkoooo/AtCoderProblems
  const res = await request
    .get('https://kenkoooo.com/atcoder/atcoder-api/v3/user/ac_rank')
    .query({ user: username })

  const solved = res.body.count
  const  submissions = solved

  return {
    submissions,
    solved,
  }
}