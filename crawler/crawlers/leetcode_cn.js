const request = require('superagent')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const input = {
    query: 'query userPublicProfile($userSlug:String!){userProfilePublicProfile(userSlug:$userSlug){submissionProgress{totalSubmissions acTotal}}}',
    variables: { userSlug: username },
  }

  const res = await request
    .post('https://leetcode-cn.com/graphql')
    .send(input)

  const data = res.body.data.userProfilePublicProfile

  if (!data) {
    throw new Error('The user does not exist')
  }

  return {
    solved: data.submissionProgress.acTotal,
    submissions: data.submissionProgress.totalSubmissions,
  }
}