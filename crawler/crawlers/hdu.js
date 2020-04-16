const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  let res

  const RETRY_TIMES = config.env === 'server' ? 1 : 2
  for (let i = 0; i < RETRY_TIMES; ++i) {
    try {
      res = await request
        .get('http://acm.hdu.edu.cn/userstatus.php')
        .query({user: username})
      break
    } catch (e) {
      if (i >= RETRY_TIMES - 1) {
        throw e
      }
      console.log('HDU connection error, retry ...')
    }
  }

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  const $ = cheerio.load(res.text)

  if ($('div').filter((i, el) => $(el).text() === 'No such user.').length >= 1) {
    throw new Error('The user does not exist')
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
    throw new Error('Error while parsing')
  }
}
