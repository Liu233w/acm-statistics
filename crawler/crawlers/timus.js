const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  let res = await request
    .get('http://acm.timus.ru/search.aspx')
    .query({Str: username})

  let $ = cheerio.load(res.text)
  const name = $('td.name').filter((i, el) => $(el).text() === username)
  if (name.length === 0) {
    throw new Error('用户不存在')
  }

  res = await request
    .get('http://acm.timus.ru/' + name.find('a').attr('href'))
  $ = cheerio.load(res.text)

  let solved = null
  try {
    solved = Number($($('td.author_stats_value')[1]).text().split(' ')[0])
  } catch (e) {
    throw new Error('无法解析数据')
  }

  const submissionPageUri = $('a').filter((i, el) => $(el).text() === 'Recent submissions')
  const submissions = await queryList(submissionPageUri.attr('href'))

  return {
    solved: solved,
    submissions: submissions,
  }
}

/**
 * 进入当前页面，统计页面上的提交数
 * @param uri 要进入的页面的链接
 * @returns {Promise<Number>} - 提交数
 */
async function queryList(uri) {
  const res = await request
    .get('http://acm.timus.ru/' + uri)

  const $ = cheerio.load(res.text)
  const num = $('td.problem').length
  if (num === 0) {
    return 0
  }

  const nextLink = $('a').filter((i, el) => $(el).text().startsWith('Next'))
  if (nextLink.length === 0) {
    return num
  } else {
    return num + await queryList(nextLink.attr('href'))
  }
}
