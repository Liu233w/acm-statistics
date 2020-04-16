const request = require('superagent')
const cheerio = require('cheerio')

module.exports = async function (config, username) {

  if (!username) {
    throw new Error('请输入用户ID')
  }

  if (isNaN(username)) {
    throw new Error('牛客网的输入必须是用户ID（数字格式）')
  }

  username = Number(username) + ''

  let solved = null
  let submissions = null
  const solvedList = new Set()

  let lastSubmissionId = Infinity
  let page = 1

  // eslint-disable-next-line no-constant-condition
  while (true) {
    const res = await request
      .get(`https://ac.nowcoder.com/acm/contest/profile/${username}/practice-coding`)
      .query({
        pageSize: 200,
        statusTypeFilter: 5,
        languageCategoryFilter: -1,
        orderType: 'DESC',
        page,
      })

    if (!res.ok) {
      throw new Error(`Server Response Error: ${res.status}`)
    }

    const $ = cheerio.load(res.text)

    if ($('.null-tip').text().trim() === '用户不存在') {
      throw new Error('The user does not exist')
    }

    try {

      if (solved == null) {
        solved = Number($('span:contains("题已通过")').prev().text())
        submissions = Number($('span:contains("次提交")').prev().text())
      }

      const newSubmissionId = Number($($('a[href^="/acm/contest/view-submission"]')[0]).text())
      if (newSubmissionId === lastSubmissionId) {
        // 已经读完了列表，开始循环了
        break
      }
      lastSubmissionId = newSubmissionId

      // 题目名称有可能重复，用题目编号
      $('a[href^="/acm/problem/"]')
        .map((i, elem) => $(elem).attr('href').slice(13))
        .get()
        .slice(1) // 第一项是 "list"
        .forEach(solvedList.add, solvedList)

    } catch (e) {
      throw new Error('Error while parsing')
    }

    page += 1
  }

  return {
    solved,
    submissions,
    solvedList: [...solvedList],
  }
}