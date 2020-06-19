const mock = require('./lib/mock')

module.exports = async mocks => {
  const actived = [
    mocks.busuanzi,
    mocks.tajs,
    mocks.googleAnalysis,
    mocks.googleAds,
  ]

  for (let item of actived) {
    await mock(item)
  }
}