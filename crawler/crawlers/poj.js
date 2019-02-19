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

  if (/<li>Sorry,.* doesn't exist<\/li>/.test(res.text)) {
    throw new Error('用户不存在')
  }

  try {

    // function p(id)\n{\n......\np(1000)\np(1001)\n...\n
    // 这个题目列表是前端渲染的
    const acListScript = $('td[rowspan=4] > script').html()
    // js 不支持零宽后发断言，所以没法加上 (?<=p\() 表达式
    const solvedList = acListScript.match(/\d+(?=\)\n)/g)

    return {
      solved: Number($('a[href^="status?result=0&user_id="]').text()),
      submissions: Number($('a[href^="status?user_id="]').text()),
      solvedList,
    }
  } catch (e) {
    throw new Error('无法解析数据')
  }
}