const _ = require('lodash')

const poj = require('../crawlers/poj')
const hdu = require('../crawlers/hdu')
const zoj = require('../crawlers/zoj')
const vjudge = require('../crawlers/vjudge')
const {ensureConfigAndRead} = require('../lib/configReader')

jest.setTimeout(10000) // 最多10秒

const notExistUsername = 'fmV84ZCQ3hwu'
// 感谢 @leoloveacm 同学提供帐号
const username = 'leoloveacm'

describe('poj', () => {

  test('test poj - 用户不存在时抛出异常', async () => {
    await expect(poj(null, notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test poj', async () => {
    const res = await poj(null, username)
    checkRes(res)
  })

})

describe('hdu', () => {

  test('test hdu - 用户不存在时抛出异常', async () => {
    await expect(hdu(null, notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test hdu', async () => {
    const res = await hdu(null, username)
    checkRes(res)
  })

})

describe('zoj', () => {

  test('test zoj - 用户不存在时抛出异常', async () => {
    await expect(zoj(null, notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test zoj', async () => {
    const res = await zoj(null, '2013300262')
    checkRes(res)
  })

})

test.skip('test vjudge', async () => {
  const config = await ensureConfigAndRead()
  // 需要读取设置，因此没法在 ci 里面测试
  const vjConfig = _.find(config.crawlers, item => item.name === 'vjudge')

  // 必须要保证配置文件正确才能进行此项测试
  expect(vjConfig).toBeTruthy()
  expect(vjConfig.crawler_login_user).toBeTruthy()
  expect(vjConfig.crawler_login_user).not.toBe('用户名')

  console.log(vjConfig)

  const res = await vjudge(vjConfig, username)
  checkRes(res)
})

function checkRes(res) {
  expect(typeof res.solved).toBe('number')
  expect(typeof res.submissions).toBe('number')

  expect(res.solved).toBeGreaterThan(0)
  expect(res.submissions).toBeGreaterThan(0)
}