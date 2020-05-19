const request = require('superagent')

/**
 * It has the same interface as uva.
 * 
 * See uva.js for more information.
 */
module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const uidRes = await request.get('https://icpcarchive.ecs.baylor.edu/uhunt/api/uname2uid/' + username)

  if (uidRes.body === 0) {
    throw new Error('The user does not exist')
  }

  const res = await request
    .get('https://icpcarchive.ecs.baylor.edu/uhunt/api/subs-user/' + uidRes.body)

  const acSet = new Set()
  const problemArray = res.body.subs
  problemArray.forEach(function (element) {
    if (element[2] === 90) {
      acSet.add(element[1])
    }
  })

  const solvedList = []
  for (let item of acSet) {
    solvedList.push(String(item))
  }

  return {
    solved: acSet.size,
    submissions: problemArray.length,
    solvedList,
  }
}
