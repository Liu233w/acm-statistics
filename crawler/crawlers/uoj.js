const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  let res
  try {
    res = await request
      .get('https://uoj.ac/user/profile/' + username)
  } catch (err) {
    if (err.response && err.response.status === 404) {
      throw new Error('The user does not exist')
    } else {
      throw err
    }
  }

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  const $ = cheerio.load(res.text)

  if ($('.panel-danger:contains("不存在该用户")').length === 1) {
    throw new Error('The user does not exist')
  }

  const submissionPage = await request
    .get('https://uoj.ac/submissions')
    .query({
      submitter: username,
      page: 999,
    })

  const $$ = cheerio.load(submissionPage.text)

  try {

    const solved = $('h4:contains("AC 过的题目：")')
      .text()
      .match(/\d+/)[0]
    const solvedList = $('a[href^="https://uoj.ac/problem/"]')
      .map((_, el) => $(el).text())
      .toArray()

    let submissions
    if ($$('.uoj-content tr td').first().text() === '无') {
      submissions = 0
    } else {
      const pageNum = parseInt($$('li.active').text()) || 1
      submissions = (pageNum - 1) * 10 + $$('.uoj-content tbody tr').length
    }

    return {
      solved: parseInt(solved),
      submissions,
      solvedList,
    }
  } catch (e) {
    throw new Error('Error while parsing')
  }
}