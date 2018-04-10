/* eslint-disable no-undef */

const configReader = require('../lib/configReader')
const _ = require('lodash')

test('ensureConfigAndRead 能够正确读取配置', async () => {
  const config = await configReader.ensureConfigAndRead()
  expect(config.crawlers.length).not.toBe(0)

  const sguConfig = _.find(config.crawlers, item => item.name === 'sgu')
  expect(sguConfig).toBeTruthy()
  expect(sguConfig.meta).toBeTruthy()

})

test('readMetaConfigs 能够读取元信息', async () => {
  const meta = await configReader.readMetaConfigs()
  expect(meta.sgu).toBeTruthy()
  expect(meta.sgu.title).toBe('SGU')
  expect(meta.sgu.description).toBeTruthy()
})
