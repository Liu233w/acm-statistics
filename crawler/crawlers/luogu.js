const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('请输入用户名')
  }

  const uidRes = await request
    .get('https://www.luogu.org/space/ajax_getuid')
    .query({username: username})

  if (!uidRes.ok) {
    throw new Error(`Server Response Error: ${uidRes.status}`)
  }

  const uidJSON = JSON.parse(uidRes.text)

  if (uidJSON.code === 404) {
    throw new Error('用户不存在')
  }

  const uid = uidJSON.more.uid;
  const res = await request
    .get('https://www.luogu.org/space/show')
    .query({uid: uid})

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  const $ = cheerio.load(res.text);

  try {
    const acList = []
    $('.lg-article > [href^="/problem/show?pid="]').each(function (i, el) {
      acList.push($(el).text().trim())
    })
    const submissions = $('li:contains("提交") > span.lg-bignum-num').text();

    return {
      submissions: submissions,
      solved: acList.length, //个人主页的计数好像不是实时更新，但是过了的题目好像是实时更新..
      solvedList: acList,
    }
  }
  catch (e) {
    throw new Error('无法解析数据')
  }

}