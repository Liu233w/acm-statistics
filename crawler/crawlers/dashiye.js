const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('请输入用户名')
  }

  const res = await request
    .get('http://www.lydsy.com/JudgeOnline/userinfo.php')
    .query({user: username})

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  const $ = cheerio.load(res.text)

  if ($('body').text().endsWith('No such User!')) {
    throw new Error('用户不存在')
  }

  try {
    const solvedTd = $('td').filter((i, el) => $(el).text() === 'Solved')
    const submitTd = $('td').filter((i, el) => $(el).text() === 'Submit')

    return {
      solved: Number(solvedTd.next().children().text()),
      submissions: Number(submitTd.next().children().text()),
    }
  } catch (e) {
    throw new Error('无法解析数据')
  }
}