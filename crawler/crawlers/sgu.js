const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  let res = await request
    .get('http://acm.sgu.ru/find.php')
    .query({find_id: username})

  let $ = cheerio.load(res.text)
  const name = $('a').filter((i, el) => $(el).text().startsWith(username))
  if (name.length === 0) {
    throw new Error('用户不存在')
  }

  res = await request
    .get('http://acm.sgu.ru/' + name.attr('href')) // name 含有多个元素的时候，attr 会返回第一个元素的
  $ = cheerio.load(res.text)

  try {
    const text = $('td').filter((i, el) => $(el).text() === 'Statistic').next().text()
    // text同时包含普通空格和宽字符空格，这两个都要分开
    const splitted = text.split(/ | /) // [':', 'Submitted:', '25325;', 'Accepted:', '406', '[details]']
    const submitStr = splitted[2]
    return {
      solved: Number(splitted[4]),
      submissions: Number(submitStr.substr(0, submitStr.length - 1))
    }
  } catch (e) {
    throw new Error('无法解析数据')
  }
}
