const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const res = await request.get('https://www.codechef.com/users/' + username)

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  const $ = cheerio.load(res.text)
  if ($('a[href="/"]:contains("Home")').length === 0) {
    // not in user profile page
    throw new Error('The user does not exist')
  }

  try {
    const solvedText = $('h5:contains("Fully Solved")').text().match(/\((\d+)\)/)[1]
    const solvedList = []
    $('h5:contains("Fully Solved") + article a').each(function (i, el) {
      solvedList.push($(el).text().trim())
    })

    const submissionText = $('script:contains("Highcharts.chart")').html().replace(/\s/gi, '')
    const matchObjectText = submissionText.match(/data:\[(.*),\]/)[1]
    const submitMatchIterator = matchObjectText.matchAll(/y:(\d+)/g)
    let submissions = 0
    for (const a of submitMatchIterator) {
      submissions += parseInt(a[1])
    }

    return {
      solved: parseInt(solvedText),
      solvedList,
      submissions,
    }
  } catch (err) {
    throw new Error('Error while parsing')
  }
}