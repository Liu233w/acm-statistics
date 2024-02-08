const request = require('superagent')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const submissionRes = await request
    .get('/api/ohunt/submissions')
    .query({
      oj: 'zoj',
      $filter: `UserName eq '${username}'`,
      $count: true,
      $top: 0,
    })

  const submissions = submissionRes.body['@odata.count']

  if (submissions === 0) {
    throw new Error('The user does not exist')
  }

  try {
    const solvedSet = new Set()

    let skip = 0
    let solvedRes
    do {
      solvedRes = await request
        .get('https://ojhunt.com/api/ohunt/submissions')
        .query({
          oj: 'zoj',
          $filter: `UserName eq '${username}' and Status eq 'Accepted'`,
          $skip: skip,
          $select: 'ProblemLabel',
        })
      solvedRes.body.value.forEach(item => {
        solvedSet.add(item.ProblemLabel)
      })

      skip += 500
    } while (solvedRes.body['@odata.nextLink'])

    return {
      solved: solvedSet.size,
      submissions,
      solvedList: [...solvedSet],
    }
  } catch (e) {
    throw new Error('Error while parsing')
  }
}