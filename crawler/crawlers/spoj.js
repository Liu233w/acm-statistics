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

  if ($('#user-profile-left').length == 0) {
    throw new Error('用户不存在')
  }

  try {
    return {
      submissions: Number($('dt:contains("Solutions submitted") + dd').text().trim()),
      solved: Number($('dt:contains("Problems solved") + dd').text().trim()),
      solvedList: $('h4:contains("List of solved classical problems") + table a')
        .map((i, elem) => $(elem).text().trim())
        .get()
        // 移除表格中的空位
        .filter(item => item.length > 0),
    }
  } catch (e) {
    throw new Error('无法解析数据')
  }
}