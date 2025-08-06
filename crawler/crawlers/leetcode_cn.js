const request = require('superagent')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const input = {
    query: `
    query userSessionProgress($userSlug:String!){
      userProfileUserQuestionSubmitStats(userSlug:$userSlug){
        acSubmissionNum {
          difficulty 
          count
        }
        totalSubmissionNum {
          difficulty
          count
        }
      }
    }`,
    variables: { userSlug: username },
  }

  const res = await request
    .post('https://leetcode.cn/graphql/')
    .set('User-Agent', 'ojhunt/1.0.0')
    .send(input)

  const data = res.body.data.userProfileUserQuestionSubmitStats
  const acList = data.acSubmissionNum
  const subList = data.totalSubmissionNum

  if (acList.length === 0 && subList.length === 0) {
    throw new Error('The user does not exist')
  }

  const solved = acList.map(item => item.count).reduce((a, b) => a + b)
  const submissions = subList.map(item => item.count).reduce((a, b) => a + b)

  return {
    solved,
    submissions,
  }
}