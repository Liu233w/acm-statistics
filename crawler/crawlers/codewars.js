const request = require('superagent')

// api url : https://dev.codewars.com/#get-user:-completed-challenges
module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  // TODO: get submission

  let totalPage = undefined
  let currentPage = 0
  const acSet = new Set()

  do {
    let res
    try {
      res = await request
        .get(`https://www.codewars.com/api/v1/users/${username}/code-challenges/completed`)
        .query({ page: 0 })
    } catch (err) {
      if (err.response && err.response.status === 404) {
        throw new Error('The user does not exist')
      }
      throw err
    }

    res.body.data.forEach(d => {
      acSet.add(d.slug)
    })

    totalPage = res.body.totalPages
    ++currentPage
  } while (currentPage < totalPage)


  return {
    solved: acSet.size,
    submissions: acSet.size,
    solvedList: [...acSet],
  }
}