const mock = require('./lib/mock')

module.exports = async mocks => {
  const actived = [
    mocks.busuanzi,
  ]

  for (let item of actived) {
    await mock(item)
  }
}