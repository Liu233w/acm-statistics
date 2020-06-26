const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  if (/\s/.test(username)) {
    throw new Error('The crawler does not support username with space')
  }

  const submissionPage = await request
    .get('https://ac.2333.moe/Problem/status.xhtml')
    .query({ username })

  if (!submissionPage.ok) {
    throw new Error(`Server Response Error: ${submissionPage.status}`)
  }

  const $s = cheerio.load(submissionPage.text)

  const usernameElement = $s('tbody tr a[href^="/User/view_user.xhtml?id="]')
  if (usernameElement.length === 0) {
    throw new Error('The user does not exist')
  }

  let userId
  try {
    userId = parseInt(usernameElement.first().attr('href').match(/\d+/)[0])
  } catch (err) {
    throw new Error('Error while parsing')
  }

  const res = await request
    .get('https://ac.2333.moe/User/view_user.xhtml')
    .query({ id: userId })

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  const $ = cheerio.load(res.text)

  try {
    const [submissions, solved] = $('li#limit').text().match(/\d+/g)

    const solvedList = $('li#solvedlist .fl a[href^="/Problem/view.xhtml?id="]')
      .map((i, e) => $(e).text())
      .get()

    return {
      solved: parseInt(solved),
      solvedList,
      submissions: parseInt(submissions),
    }
  } catch (err) {
    throw new Error('Error while parsing')
  }
}