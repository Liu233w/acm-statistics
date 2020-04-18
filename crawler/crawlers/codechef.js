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

  if (res.req.path === '/') {
    // redirected to /
    throw new Error('The user does not exist')
  }

  const $ = cheerio.load(res.text)

  try {
    const solvedText = $('h5:contains("Fully Solved")').text().match(/\((\d+)\)/)[1]
    const solvedList = []
    $('h5:contains("Fully Solved") + article a').each(function (i, el) {
      solvedList.push($(el).text().trim())
    })

    const submissionText = $('script:contains("Highcharts.chart")').html().replace(/\s/gi, '')
    const submitMatchArray = submissionText.match(/{name:'solutions_partially_accepted',y:(\d+),color:colorForSections\['solutions_partially_accepted'\]},{name:'compile_error',y:(\d+),color:colorForSections\['compile_error'\]},{name:'runtime_error',y:(\d+),color:colorForSections\['runtime_error'\]},{name:'time_limit_exceeded',y:(\d+),color:colorForSections\['time_limit_exceeded'\]},{name:'wrong_answers',y:(\d+),color:colorForSections\['wrong_answers'\]},{name:'solutions_accepted',y:(\d+),color:colorForSections\['solutions_accepted'\],sliced:false,selected:true},\]/)
    const submissions = submitMatchArray
      .slice(1, submitMatchArray.length)
      .reduce((sum, a) => sum + parseInt(a), 0)

    return {
      solved: parseInt(solvedText),
      solvedList,
      submissions,
    }
  } catch (err) {
    throw new Error('Error while parsing')
  }
}