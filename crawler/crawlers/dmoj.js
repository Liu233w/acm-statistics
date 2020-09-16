const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  let res

  try {
    res = await request
      .get('https://dmoj.ca/submissions/user/' + username)
  } catch (err) {
    if (err.response && err.response.status === 404) {
      throw new Error('The user does not exist')
    } else {
      throw err
    }
  }

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  const $ = cheerio.load(res.text)

  let submissions
  try {
    const codes = $('script:contains("window.results_json")').html().trim().split('\n')
    const line = codes[codes.length - 1]
    const json = JSON.parse(line.match(/\{.*\}/)[0])
    submissions = json.total
  } catch (e) {
    throw new Error('Error while parsing')
  }

  res = await request
    .get('https://dmoj.ca/api/v2/user/' + username)

  try {
    const solvedList = res.body.data.object.solved_problems
    // const solved = res.body.data.object.problem_count

    return {
      solved: solvedList.length,
      submissions,
      solvedList,
    }
  } catch (e) {
    throw new Error('Error while parsing')
  }
}