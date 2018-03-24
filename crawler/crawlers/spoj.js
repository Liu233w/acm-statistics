const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('请输入用户名')
  }

  let res = null
  try {
    res = await request
      .get('http://www.spoj.com/users/' + username)
  } catch (e) {
    if (e.message === 'Not Found') {
      throw new Error('用户不存在')
    } else {
      throw e
    }
  }

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  const $ = cheerio.load(res.text)

  try {
    return {
      submissions: Number($('span.fa.fa-send.fa-fw').parent().next().text()),
      solved: Number($('span.fa.fa-check.fa-fw').parent().next().text()),
    }
  } catch (e) {
    throw new Error('无法解析数据')
  }
}