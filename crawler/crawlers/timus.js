const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const idRes = await request
    .get('http://acm.timus.ru/search.aspx')
    .query({ Str: username })

  const id$ = cheerio.load(idRes.text)
  const name = id$('td.name').filter((i, el) => id$(el).text() === username)
  if (name.length === 0) {
    throw new Error('The user does not exist')
  }

  const res = await request
    .get('http://acm.timus.ru/' + name.find('a').attr('href'))
  const $ = cheerio.load(res.text)

  let solved = null
  try {
    solved = Number($('td.author_stats_name:contains("Problems solved") + td').text().match(/\d+/g)[0])
  } catch (e) {
    throw new Error('Error while parsing')
  }

  const submissionPageUri = $('a').filter((i, el) => $(el).text() === 'Recent submissions')
  const submissions = await queryList(submissionPageUri.attr('href'))

  const solvedList = $('td.accepted > a')
    .map((i, elem) => $(elem).text().trim())
    .get()

  return {
    solved,
    submissions,
    solvedList,
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

  const nextLink = $('a:contains("Next")')
  if (nextLink.length === 0) {
    return num
  } else {
    return num + await queryList(nextLink.attr('href'))
  }
}
