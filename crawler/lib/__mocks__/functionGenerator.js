/* eslint-disable no-undef */

exports.generateServerCrawlerFunctions = async () => {

  const crawlerFunc = username => {
    if (username === 'reject') {
      throw new Error('用户不存在')
    } else {
      return {
        solved: 101,
        submissions: 230,
      }
    }
  }

  const ret = {}
  for (let item of ['crawler1', 'crawler2', 'crawler_for_server']) {
    ret[item] = crawlerFunc
  }
  return ret
}
