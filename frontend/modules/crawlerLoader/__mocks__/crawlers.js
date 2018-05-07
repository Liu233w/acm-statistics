import _ from 'lodash'

export default () => {
// eslint-disable-next-line no-unused-vars
  const mockFuncs = username => ({submissions: 0, solved: 0})

  const crawlerNames = _.map(_.range(5), i => `crawler${i}`)
  const metas = _.zipObject(
    crawlerNames,
    _.map(crawlerNames, str => ({
      title: str,
    }))
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

  return {
    metas: metas,
    crawlers: _.mapValues(metas, () => mockFuncs),
  }
}
