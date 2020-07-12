const request = require('superagent')
const cheerio = require('cheerio')

const hostName = 'vjudge.net'

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('Please enter username')
  }

  let res
  try {
    res = await request
      .get(`https://${hostName}/user/${username}`)
  } catch (err) {
    if (err.response && err.response.status === 500) {
      throw new Error('The user does not exist')
    } else {
      throw err
    }
  }

  if (!res.ok) {
    throw new Error(`Server Response Error: ${res.status}`)
  }

  const $ = cheerio.load(res.text)

  const acRes = await request
    .get(`https://${hostName}/user/solveDetail/${username}`)

  try {
    const submissions = $('a[title="Overall attempted"]').text()
    const acSet = new Set()

    for (const oj in acRes.body.acRecords) {
      const list = acRes.body.acRecords[oj]
      const ojName = mapOjName(oj)
      for (const problem of list) {
        acSet.add(ojName + '-' + problem)
      }
    }

  } catch (e) {
    throw new Error('Error while parsing')
  }


  return {
    solved: acSet.size,
    submissions: parseInt(submissions),
    solvedList: [...acSet],
    submissionsByCrawlerName,
  }
}

/**
 * take a oj name in virtual judge and map its name to crawler name
 * 
 * if name cannot be mapped, return original name
 * @param {string} nameInVjudge 
 */
function mapOjName(nameInVjudge) {

  // oj that can map its name to crawler name by changing into lower case
  const simpleMapOj = new Set([
    'codeforces',
    'uva',
    'uvalive',
    'poj',
    'hdu',
    'zoj',
    'fzu',
    'spoj',
    'timus',
    'csu',
    // 'hust',
    'atcoder',
    'aizu',
    'codechef',
    'nbut',
  ])
  // crawler name map
  const ojMap = {
    '': 'NO_NAME',
    'LibreOJ': 'loj',
    'URAL': 'timus',
    'HYSBZ': 'dashiye',
    // it looks like a typo of vjudge
    'EIJudge': 'eljudge',
    'Gym': 'codeforces',
    '51Nod': 'nod',
  }

  if (simpleMapOj.has(nameInVjudge.toLowerCase())) {
    return nameInVjudge.toLowerCase()
  } else if (nameInVjudge in ojMap) {
    return ojMap[nameInVjudge]
  } else {
    return nameInVjudge
  }
}