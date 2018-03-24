const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('请输入用户名')
  }

  const res = await request
    .get('http://acm.zju.edu.cn/onlinejudge/showUserStatus.do')
    .query({handle: username})

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  const $ = cheerio.load(res.text)

  const data = $('font[color=red]').text()
  if (data === 'No such user.') {
    throw new Error('用户不存在')
  }

  try {
    const num = data.split('/')
    return {
      solved: Number(num[0]),
      submissions: Number(num[1]),
    }
  } catch (e) {
    throw new Error('无法解析数据')
  }
}