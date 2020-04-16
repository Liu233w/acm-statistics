const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const res = await request
    .get('http://acm.fzu.edu.cn/user.php')
    .query({uname: username})

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  const $ = cheerio.load(res.text)

  if ($('font').filter((i, el) => $(el).text() === 'No such user or user has been deleted!').length >= 1) {
    throw new Error('The user does not exist')
  }

  try {

    const acList = $('b > a[href^="problem.php?pid="]')
      .map((i, elem) => $(elem).text())
      .get()

    return {
      submissions: Number($('td:contains("Total Submitted") + td').text()),
      solved: Number($('td:contains("Total Accepted") + td').text()),
      solvedList: acList,
    }
  } catch (e) {
    throw new Error('Error while parsing')
  }
}