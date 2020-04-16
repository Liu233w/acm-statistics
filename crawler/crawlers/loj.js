const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const res = await request
    .get('http://loj.ac/find_user')
    .query({nickname: username})

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }
  const $ = cheerio.load(res.text)

  if ($('.header').filter((i, el) => $(el).text().trim() === '无此用户。').length >= 1) {
    throw new Error('The user does not exist')
  }
  try {
    const acList = []
    $('[href^="/problem/"]').each(function (i, el) {
      acList.push($(el).text().trim())
    })

    const text = $('script:contains("new Chart")').html().replace(/\s/gi, '')
    const submitMatchArray = text.match(/data:\[(\d+),(\d+),(\d+),(\d+),(\d+),(\d+),\]/i)
    const submissions = submitMatchArray
      .slice(1, submitMatchArray.length)
      .reduce((sum, a) => sum + parseInt(a), 0)

    return {
      submissions,
      solved: parseInt($('a:has(i.check.icon)').text().trim().split(' ')[1]),
      solvedList: acList,
    }
  }
  catch (e) {
    throw new Error('Error while parsing')
  }

}