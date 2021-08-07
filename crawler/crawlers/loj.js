const request = require('superagent')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const res = await request
    .post('https://api.loj.ac/api/user/getUserDetail')
    .send({
      now: new Date().toISOString(),
      timezone: 'UTC',
      username,
    })

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  if (res.body.error) {
    throw new Error('The user does not exist')
  }

  const submissions = res.body.meta.submissionCount
  const solvedList = await resolveSolvedList(username)
  // if a submission is not public, it can be different from the value of the user api
  const solved = solvedList.length 
  // const solved = res.body.meta.acceptedProblemCount

  return {
    submissions,
    solved,
    solvedList,
  }
}

async function resolveSolvedList(username) {
  const acSet = new Set()
  let maxId = null
  // eslint-disable-next-line no-constant-condition
  while (true) {
    const res = await request
      .post('https://api.loj.ac/api/submission/querySubmission')
      .send({
        submitter: username, 
        status: 'Accepted', 
        maxId, 
        locale: 'en_US', 
        takeCount: 10, // cannot be changed
      })

    if (!res.body.submissions || res.body.submissions.length === 0) {
      break
    }

    res.body.submissions.forEach(item => {
      acSet.add(item.problem.id + '')
      maxId = item.id - 1
    })
  }

  return [...acSet]
}