const {ensureConfigAndRead} = require("../lib/configReader")
/* eslint-disable no-undef */

test('ensureConfigAndRead 能够正确读取配置', async () => {
  const config = await ensureConfigAndRead()
  expect(config.crawlers.length).not.toBe(0)
  expect(config.crawlers[0].name).toBeTruthy()
})
