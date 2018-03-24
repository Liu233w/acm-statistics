const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('请输入用户名')
  }

  const res = await request
    .get('http://poj.org/userstatus')
    .query({user_id: username})

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  const $ = cheerio.load(res.text)

  if ($('title').text() === 'Error -- no user found') {
    throw new Error('用户不存在')
  }

  if (/<li>Sorry,\w* doesn't exist<\/li>/.test(res.text)) {
    throw new Error('用户不存在')
  }

  let ret = null
  try {
    ret = {
      solved: Number($('a[href^="status?result=0&user_id="]').text()),
      submissions: Number($('a[href^="status?user_id="]').text()),
    }
  } catch (e) {
    throw new Error('无法解析数据')
  }

  return ret
}