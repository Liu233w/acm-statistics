/* eslint-disable no-undef */

module.exports = {
  readMetaConfigs: async () => {
    return {
      crawler1: {
        title: 'Crawler1',
        description: 'Description1',
        url: 'http://www.c1.com',
      },
      crawler2: {
        title: 'Crawler2',
      },
      crawler_for_server: {
        title: 'CrawlerForServer',
      },
    }
  },
  generateServerCrawlerFunctions: async () => {

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
  },
}