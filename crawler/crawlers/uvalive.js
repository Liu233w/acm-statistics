const request = require('superagent')

const prefix = 'https://icpcarchive.ecs.baylor.edu/uhunt'

/**
 * It has the same interface as uva.
 * 
 * See uva.js for more information.
 */
module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const uidRes = await request.get(prefix + '/api/uname2uid/' + username)

  if (uidRes.body === 0) {
    throw new Error('The user does not exist')
  }

  const res = await request
    .get(prefix + '/api/subs-user/' + uidRes.body)

  const acSet = new Set()
  const problemArray = res.body.subs
  problemArray.forEach(function (element) {
    if (element[2] === 90) {
      acSet.add(element[1])
    }
  })

  const ohuntRes = await request
    .post('https://ojhunt.com/api/ohunt/problems/resolve-label')
    .send({
      onlineJudge: 'uvalive',
      list: [...acSet],
    })

  return {
    solved: acSet.size,
    submissions: problemArray.length,
    solvedList: Object.values(ohuntRes.body.result),
  }
}
