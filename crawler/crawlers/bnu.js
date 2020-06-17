const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const res = await request
    .get('http://www.bnuoj.com/userinfo.php')
    .query({ name: username })

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }
  const $ = cheerio.load(res.text)

  if ($('.alert-error:contains("No such user!")').length >= 1) {
    throw new Error('The user does not exist')
  }
  try {
    // TODO: ac list
    const acList = null

    const submissions = $('th:contains("Total Submissions")')
      .next().text()
    const solved = $('th:contains("Accepted")')
      .next().text()

    return {
      submissions: parseInt(submissions),
      solved: parseInt(solved),
      solvedList: acList,
    }
  }
  catch (e) {
    throw new Error('Error while parsing')
  }

}