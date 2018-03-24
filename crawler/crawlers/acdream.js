const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('请输入用户名')
  }

  const res = await request
    .get('http://acdream.info/user/' + username)

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  const $ = cheerio.load(res.text)

  const data = $('div[style="padding-top:20px;text-align:center"]').text()
  if (data.startsWith('页面不存在，请点击')) {
    throw new Error('用户不存在')
  }

  try {
    return {
      solved: Number($('img[src="/img/yes.png"]').next().text()),
      submissions: Number($('img[src="/img/submit.png"]').next().text()),
    }
  } catch (e) {
    throw new Error('无法解析数据')
  }
}