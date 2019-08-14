const request = require('superagent')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('请输入用户名')
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
    throw new Error('用户不存在')
  }

  return {
    solved: data.submissionProgress.acTotal,
    submissions: data.submissionProgress.totalSubmissions,
  }
}