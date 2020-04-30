const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const res = await request
    .get('http://acm.mipt.ru/judge/users.pl')
    .query({ user: username })

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  if (/User .+ does not exist/.test(res.text)) {
    throw new Error('The user does not exist')
  }

  const $ = cheerio.load(res.text)

  try {
    const solvedElem = $('font:contains("solved:")')
    const solvedText = solvedElem.children('b').text()
    const solvedList = []
    solvedElem.nextUntil('font').find('a').each((i, el) => {
      solvedList.push($(el).text().trim())
    })

    const submissionElems = $('font:contains("Statistics:")')
      .nextAll('table')
      .first()
      .find('div')
    let submissions = 0
    submissionElems.each((i, el) => {
      submissions += parseInt($(el).text())
    })

    return {
      solved: parseInt(solvedText),
      solvedList,
      submissions,
    }
  } catch (err) {
    throw new Error('Error while parsing')
  }
}