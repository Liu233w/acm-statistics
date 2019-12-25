const _ = require('lodash')

const crawlerNames = _.map(_.range(5), i => `crawler${i}`)
const metas = _.zipObject(
  crawlerNames,
  _.map(crawlerNames, str => ({
    title: str,
  })),
)
metas.withDescription = {
  title: 'with description',
  description: 'a description',
}
metas.withUrl = {
  title: 'with url',
  url: 'http://test.aaa.com',
}
metas.withAll = {
  title: 'with all',
  description: 'with all description and url',
  url: 'http://test.aaa.com',
}

module.exports = {
  readMetaConfigs: () => {
    return Promise.resolve(metas)
  },
  generateBrowserCrawlerFunctions: () => {

    const result = {}
    for (let key in metas) {
      result[key] = 'username => ({submissions: 0, solved: 0})'
    }

    return Promise.resolve(result)
  },
}
