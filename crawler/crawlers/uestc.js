const request = require('superagent')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const res = await request
    .post('https://acm.uestc.edu.cn/graphql')
    .send({
      operationName: 'ProfileGQL',
      variables: { username },
      query: `query ProfileGQL($username: String!) {
        user(username: $username) {
          solved
          statistics {
            ac
            tle
            ce
            wa
            re
            ole
            mle      
            solve {
              pk
              status
            }
          }
        }
      }
      `,
    })

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  if (res.body.errors) {
    throw new Error('The user does not exist')
  }

  try {

    const solvedList = res.body.data.user.statistics.solve
      .filter(x => x.status)
      .map(x => x.pk)

    const s = res.body.data.user.statistics

    return {
      submissions: s.ac + s.tle + s.ce + s.wa + s.re + s.ole + s.mle,
      solved: res.body.data.user.solved,
      solvedList,
    }
  }
  catch (e) {
    throw new Error('Error while parsing')
  }
}