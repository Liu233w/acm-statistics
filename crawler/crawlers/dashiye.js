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

  // 返回的页面不是一个完整的 html，没有后面的 </body></html>
  // 所以用 jquery 解析会出错
  if (res.text.endsWith('No such User!')) {
    throw new Error('用户不存在')
  }

  try {

    // p(1000);p(1001);p(1002)....p(XXXX);
    // 这个题目列表是前端渲染的
    const acListScript = $('td[rowspan=14] > script').html().split('\n')[2]
    // ['p(1000)', 'p(1001)' ...]
    const singleList = acListScript.split(';').slice(0, -1)
    const acList = singleList.map(item => item.substring(2, item.length - 1))

    return {
      solved: Number($('td:contains("Solved") + td > a').text()),
      submissions: Number($('td:contains("Submit") + td > a').text()),
      solvedList: acList,
    }
  } catch (e) {
    throw new Error('无法解析数据')
  }
}