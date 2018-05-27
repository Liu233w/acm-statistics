/* eslint-disable no-undef */

const _ = require('lodash')

const poj = require('../crawlers/poj')
const hdu = require('../crawlers/hdu')
const zoj = require('../crawlers/zoj')
const acdream = require('../crawlers/acdream')
const dashiye = require('../crawlers/dashiye')
const codeforces = require('../crawlers/codeforces')
const uva = require('../crawlers/uva')
const fzu = require('../crawlers/fzu')
const spoj = require('../crawlers/spoj')
const timus = require('../crawlers/timus')
const sgu = require('../crawlers/sgu')
const leetcode_cn = require('../crawlers/leetcode_cn')
const vjudge = require('../crawlers/vjudge')
const csu = require('../crawlers/csu')
const {ensureConfigAndRead} = require('../lib/configReader')

jest.setTimeout(10000) // 最多10秒

const notExistUsername = 'fmV84ZCQ3hwu'
const username = 'vjudge5'

// 另外，感谢 @leoloveacm, @2013300262 同学提供测试帐号

describe('poj', () => {

  test('test poj - 用户不存在时抛出异常', async () => {
    await expect(poj(null, notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test poj - 能够正确识别带有空格的用户名', async () => {
    await expect(poj(null, ' ' + notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test poj', async () => {
    const res = await poj(null, username)
    checkRes(res)
  })

})

describe('hdu', () => {

  const config = {
    env: 'server',
  }

  test('test hdu - 用户不存在时抛出异常', async () => {
    await expect(hdu(config, notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test hdu - 能够正确识别带有空格的用户名', async () => {
    await expect(hdu(config, ' ' + notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test hdu', async () => {
    const res = await hdu(config, 'vjudge4')
    checkRes(res)
  })

})

describe('zoj', () => {

  test('test zoj - 用户不存在时抛出异常', async () => {
    await expect(zoj(null, notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test zoj - 能够正确识别带有空格的用户名', async () => {
    await expect(zoj(null, ' ' + notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test zoj', async () => {
    const res = await zoj(null, username)
    checkRes(res)
  })

})

describe('acdream', () => {

  test('test acdream - 用户不存在时抛出异常', async () => {
    await expect(acdream(null, notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test acdream - 能够正确识别带有空格的用户名', async () => {
    await expect(acdream(null, ' ' + notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test acdream', async () => {
    const res = await acdream(null, username)
    checkRes(res)
  })

})

describe('dashiye', () => {

  test('test dashiye - 用户不存在时抛出异常', async () => {
    await expect(dashiye(null, notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test dashiye - 能够正确识别带有空格的用户名', async () => {
    await expect(dashiye(null, ' ' + notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test dashiye', async () => {
    const res = await dashiye(null, username)
    checkRes(res)
  })

})

describe('codeforces', () => {

  test('test codeforces - 用户不存在时抛出异常', async () => {
    await expect(codeforces(null, notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test codeforces - 能够正确识别带有空格的用户名', async () => {
    await expect(codeforces(null, ' ' + notExistUsername)).rejects.toThrow('handle: Field should contain only Latin letters, digits, underscore or dash characters')
  })

  test('test codeforces', async () => {
    const res = await codeforces(null, 'leoloveacm') // 没有找到好的测试多页返回的帐号，还是用这个测试单页吧
    checkRes(res)
  })

})

describe('uva', () => {

  test('test uva - 用户不存在时抛出异常', async () => {
    await expect(uva(null, notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test uva', async () => {
    const res = await uva(null, 'leoloveacm')
    checkRes(res)
  })

})

describe('fzu', () => {

  test('test fzu - 用户不存在时抛出异常', async () => {
    await expect(fzu(null, notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test fzu', async () => {
    const res = await fzu(null, username)
    checkRes(res)
  })

})

describe('spoj', () => {

  test('test spoj - 用户不存在时抛出异常', async () => {
    await expect(spoj(null, notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test spoj - 能够正确识别带有空格的用户名', async () => {
    await expect(spoj(null, ' ' + notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test spoj', async () => {
    const res = await spoj(null, username)
    checkRes(res)
  })

})

describe('timus', () => {

  test('test timus - 用户不存在时抛出异常', async () => {
    await expect(timus(null, notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test timus - 能够正确识别带有空格的用户名', async () => {
    await expect(timus(null, ' ' + notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test timus', async () => {
    const res = await timus(null, 'vjudge11')
    checkRes(res)
    expect(res.submissions).toBeGreaterThan(10)
  })

})

describe('sgu', () => {

  test('test sgu - 用户不存在时抛出异常', async () => {
    await expect(sgu(null, notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test sgu - 能够正确识别带有空格的用户名', async () => {
    await expect(sgu(null, ' ' + notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test sgu', async () => {
    const res = await sgu(null, username)
    checkRes(res)
  })

})

describe('leetcode_cn', () => {

  test('test leetcode_cn - 用户不存在时抛出异常', async () => {
    await expect(leetcode_cn(null, notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test leetcode_cn - 能够正确识别带有空格的用户名', async () => {
    await expect(leetcode_cn(null, ' ' + notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test leetcode_cn', async () => {
    const res = await leetcode_cn(null, 'wwwlsmcom')
    checkRes(res)
  })

})

describe('csu', () => {

  test('test csu - 用户不存在时抛出异常', async () => {
    await expect(csu(null, notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test csu - 能够正确识别带有空格的用户名', async () => {
    await expect(csu(null, ' ' + notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test csu', async () => {
    const res = await csu(null, username)
    checkRes(res)
  })

})

describe('vjudge', () => {

  let vjConfig

  beforeAll(async () => {
    const config = await ensureConfigAndRead()
    // 需要读取设置，因此没法在 ci 里面测试
    vjConfig = _.find(config.crawlers, item => item.name === 'vjudge')

    // 必须要保证配置文件正确才能进行此项测试
    expect(vjConfig).toBeTruthy()
    expect(vjConfig.crawler_login_user).toBeTruthy()
    expect(vjConfig.crawler_login_user).not.toBe('用户名')

    // console.log(vjConfig)
  })

  test('test vjudge - 用户不存在时抛出异常', async () => {
    await expect(vjudge(vjConfig, notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test vjudge - 能够正确识别带有空格的用户名', async () => {
    await expect(vjudge(vjConfig, ' ' + notExistUsername)).rejects.toThrow('用户不存在')
  })

  test('test vjudge', async () => {
    const res = await vjudge(vjConfig, 'leoloveacm')
    checkRes(res)
  })
})

function checkRes(res) {
  expect(typeof res.solved).toBe('number')
  expect(typeof res.submissions).toBe('number')

  expect(res.solved).toBeGreaterThan(0)
  expect(res.submissions).toBeGreaterThan(0)

  expect(res.submissions).toBeGreaterThan(res.solved)
}