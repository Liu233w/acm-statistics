const request = require('superagent')

// 51nod
module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  let res = await request
    .get('https://www.51nod.com/SearchReader/TitleOnly')
    .query({ searchStr: username })

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  if (!res.body) {
    throw new Error('The user does not exist')
  }

  // find user id in search panel
  let userId = null
  for (const item of res.body) {
    if (item.Content === username && item.ContentType === 2) {
      userId = item.LinkId
      break
    }
  }

  if (userId === null) {
    throw new Error('The user does not exist')
  }

  try {
    res = await request
      .get('https://www.51nod.com/Challenge/UserIndex')
      .query({ userId })
  } catch (err) {
    if (err.response && err.response.status === 404) {
      throw new Error('The user does not exist')
    }
  }

  try {
    const solvedList = []
    res.body.ProblemTables.forEach(table => {
      table.ProblemInfos.forEach(problem => {
        if (problem.UserProblemSimplify && problem.UserProblemSimplify.IsAccepted) {
          solvedList.push(problem.ProblemSimplify.ProblemId.toString())
        }
      })
    })

    return {
      solved: res.body.UserStat.ProblemAcceptedCount,
      submissions: res.body.UserStat.ProblemSubmitCount,
      solvedList,
    }
  } catch (e) {
    throw new Error('Error while parsing')
  }
}