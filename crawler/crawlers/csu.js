const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('请输入用户名')
  }

  const res = await request
    .get('http://acm.csu.edu.cn:20080/csuoj/user/userinfo')
    .query({user_id: username})

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  const $ = cheerio.load(res.text)

  if (/<p class="error">No such user<\/p>/.test(res.text)) {
    throw new Error('用户不存在')
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
    throw new Error('无法解析数据')
  }
}