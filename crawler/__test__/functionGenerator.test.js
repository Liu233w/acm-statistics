/* eslint-disable no-undef */

jest.mock('../lib/configReader')

describe('generateServerCrawlerFunctions', () => {

  let functions

  beforeAll(async () => {

    for (let item of ['crawler1', 'crawler2', 'crawler_for_server']) {
      // 因为 functionGenerator 里面 require 的时候带了 js 后缀，这里必须也加上后缀，否则会提示找不到模块
      jest.mock(`../crawlers/${item}.js`, () => async function (config, username) {
        return {
          config: config,
          username: username,
        }
      }, {virtual: true})
    }

    const functionGenerator = require('../lib/functionGenerator')

    functions = await functionGenerator.generateServerCrawlerFunctions()
  })

  it('能够获取到函数', () => {
    expect(functions).toEqual(expect.objectContaining({
      crawler1: expect.any(Function),
      crawler2: expect.any(Function),
      crawler_for_server: expect.any(Function),
    }))
  })

  it('能将用户名传入函数', async () => {
    const ret = await functions.crawler1('username1')
    expect(ret.username).toBe('username1')
  })

  it('能让函数取到设置信息', async () => {
    const ret = await functions.crawler1('username1')
    expect(ret.config.meta.title).toBe('Crawler1')
  })

  it('能让函数取到环境信息 - server', async () => {
    const ret = await functions.crawler1('username1')
    expect(ret.config.env).toBe('server')
  })
})