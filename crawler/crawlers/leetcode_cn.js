const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('请输入用户名')
  }

  let res
  try {
    res = await request
      .get('https://leetcode-cn.com/' + username)
  } catch (e) {
    if (e.response && e.response.status == 404) {
      throw new Error('用户不存在')
    } else {
      throw e
    }
  }

  const $ = cheerio.load(res.text)

  try {
    const solvedElem = $('li.list-group-item:contains("解决的题目") > span')
    const submissionsElem = $('li.list-group-item:contains("通过的提交") > span')
    return {
      solved: Number($(solvedElem).text().replace(/[ \n]/g, '').split('/')[0]),
      submissions: Number($(submissionsElem).text().replace(/[ \n]/g, '').split('/')[1]),
    }
  } catch (e) {
    throw new Error('无法解析数据')
  }
}