const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('请输入用户名')
  }

  const res = await request
    .get('http://acm.hdu.edu.cn/userstatus.php')
    .query({user: username})

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  const $ = cheerio.load(res.text)

  if ($('div').filter((i, el) => $(el).text() === 'No such user.').length >= 1) {
    throw new Error('用户不存在')
  }

  try {
    return {
      submissions: Number($('td').filter((i,el)=>$(el).text()==='Submissions').next().text()),
      solved: Number($('td').filter((i,el)=>$(el).text()==='Accepted').next().text())
    }
  } catch (e) {
    throw new Error('无法解析数据')
  }
}