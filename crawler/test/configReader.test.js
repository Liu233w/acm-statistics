import test from 'ava'

import {ensureConfigAndRead} from "../lib/configReader"

test('ensureConfigAndRead 能够正确读取配置', async t => {
  const config = await ensureConfigAndRead()
  t.not(config.crawlers.length, 0)
  t.not(config.crawlers[0].name, null)
})
