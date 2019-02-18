const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('请输入用户名')
  }

  if (config.env === 'server') {
    return await doCrawler(username)
  } else {

    for (let i = 0; i < 2; ++i) {
      try {
        console.log('browser')
        return await doCrawler(username)
      } catch (e) {
        if (e.message !== 'Not Found' || i >= 1) {
          throw e
        }
      }
    }
  }
}

async function doCrawler(username) {
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

    // p(1000,3588,11274);p(1001,1951,7721);p(1002,1535,6550);...
    // 这个题目列表是前端渲染的
    const acListScript = $('h3:has(font:contains("List of solved problems")) + p > script').html()
    // ['p(1000,3588,11274)', 'p(1001,1951,7721)' ...]
    const singleList = acListScript.split(';').slice(0, -1)
    const acList = singleList.map(item => item.match(/\d+/g)[0])

    return {
      submissions: Number($('td:contains("Submissions") + td').text()),
      solved: Number($('td:contains("Problems Solved") + td').text()),
      solvedList: acList,
    }
  } catch (e) {
    throw new Error('无法解析数据')
  }
}