const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const res = await request
    .get('http://acm.csu.edu.cn/csuoj/user/userinfo')
    .query({user_id: username})

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  const $ = cheerio.load(res.text)

  if (/<p class="error">No such user<\/p>/.test(res.text)) {
    throw new Error('The user does not exist')
  }

  try {
    const tds = $('#info_left tbody td')

    return {
      solved: Number($(tds[1]).text()),
      submissions: Number($(tds[2]).text()),
      solvedList: $('#userinfo_left a[href^="/csuoj/problemset/problem?pid="]')
        .map((i,elem)=>$(elem).text().trim())
        .get(),
    }
  } catch (e) {
    throw new Error('Error while parsing')
  }
}