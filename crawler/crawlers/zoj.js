const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const res = await request
    .get('http://acm.zju.edu.cn/onlinejudge/showUserStatus.do')
    .query({handle: username})

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  const $ = cheerio.load(res.text)

  const data = $('font[color=red]').text()
  if (data === 'No such user.') {
    throw new Error('The user does not exist')
  }

  try {
    const num = data.split('/')

    const solvedList = $('div:has(b:contains("Solved Problems")) > a')
      .map((i, elem) => $(elem).text().trim())
      .get()

    return {
      solved: Number(num[0]),
      submissions: Number(num[1]),
      solvedList,
    }
  } catch (e) {
    throw new Error('Error while parsing')
  }
}