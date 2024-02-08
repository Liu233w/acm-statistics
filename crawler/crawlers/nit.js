const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  const res = await request
    .get('https://www.nitacm.com/userinfo.php')
    .query({ name: username })

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }
  const $ = cheerio.load(res.text)

  if ($('.alert-error:contains("No such user!")').length >= 1) {
    throw new Error('The user does not exist')
  }

  let submissions, solved, acList
  try {
    submissions = $('th:contains("Total Submissions")')
      .next().text()
    solved = $('th:contains("Accepted")')
      .next().text()

    acList = $('#userac a').map((_, el) => parseInt($(el).text())).toArray()
  }
  catch (e) {
    throw new Error('Error while parsing')
  }

  const ohuntRes = await request
    .post('https://ojhunt.com/api/ohunt/problems/resolve-label')
    .send({
      onlineJudge: 'OurOJ',
      list: [...acList],
    })

  try {
    const solvedList = Object.values(ohuntRes.body.result).map(label => {
      const [oj, num] = label.split('-')
      return `${ojMap(oj)}-${num}`
    })

    return {
      submissions: parseInt(submissions),
      solved: parseInt(solved),
      solvedList,
    }
  }
  catch (e) {
    throw new Error('Error while parsing')
  }

}

function ojMap(oj) {

  // oj that can map its name to crawler name by changing into lower case
  const simpleMapOj = new Set([
    'codeforces',
    'hdu',
    'fzu',
    'nbut',

    'uva',
    'uvalive',
    // 'zoj',
    'spoj',
    // 'timus',
    // 'csu',
    // 'hust',
    // 'atcoder',
    'aizu',
    'codechef',
  ])
  // crawler name map
  const ojMap = {
    '': 'NO_NAME',
    'PKU': 'poj',

    'URAL': 'timus',
    // 'HYSBZ': 'dashiye',
    // it looks like a typo of vjudge
    // 'EIJudge': 'eljudge',
    // 'Gym': 'codeforces',
    // '51Nod': 'nod',
  }

  if (simpleMapOj.has(oj.toLowerCase())) {
    return oj.toLowerCase()
  } else if (oj in ojMap) {
    return ojMap[oj]
  } else {
    return oj
  }
}