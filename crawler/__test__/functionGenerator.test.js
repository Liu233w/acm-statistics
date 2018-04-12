/* eslint-disable no-undef */

const path = require('path')
const _ = require('lodash')

const axios = require('axios')
const MockAdapter = require('axios-mock-adapter')
const axiosMock = new MockAdapter(axios, {delayResponse: 200})

jest.mock('../lib/configReader')
// fs-extra 引用了 fs，只要 mock fs 模块，fs-extra就不会使用fs对文件系统进行io了
jest.mock('fs')

const fs = require('fs-extra')

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

describe('generateBrowserCrawlerFunctions', () => {

  let functions
  let functionStrs
  const serverFunctionContent = 'This line should not exist in generatedFunctions'

  beforeAll(async () => {
    function ensureDirectoryExistence(filePath) {
      var dirname = path.dirname(filePath)
      if (fs.existsSync(dirname)) {
        return true
      }
      ensureDirectoryExistence(dirname)
      fs.mkdirSync(dirname)
    }

    function writeCrawlerMockFile(fileName, content) {
      const filePath = path.join(__dirname, `../crawlers/${fileName}.js`)
      ensureDirectoryExistence(filePath)
      fs.writeFileSync(filePath, content)
    }

    for (let item of ['crawler1', 'crawler2']) {
      writeCrawlerMockFile(item,
        `module.exports = async function (config, username) {
          return {
            config: config,
            username: username,
          }
        }`)
    }

    writeCrawlerMockFile('crawler_for_server', serverFunctionContent)

    const functionGenerator = require('../lib/functionGenerator')

    functionStrs = await functionGenerator.generateBrowserCrawlerFunctions()
    functions = _.mapValues(functionStrs, str => eval(str))
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

  describe('在浏览器和服务器同时使用的配置下', () => {

    it('能让函数取到设置信息', async () => {
      const ret = await functions.crawler1('username1')
      expect(ret.config.meta.title).toBe('Crawler1')
    })

    it('能让函数取到环境信息 - browser', async () => {
      const ret = await functions.crawler1('username1')
      expect(ret.config.env).toBe('browser')
    })

  })

  describe('在只运行在服务器上的配置下', () => {

    const crawlerbaseUrl = '/api/crawlers/crawler_for_server/'
    const resolvedUser = 'resolvedUser'
    const rejectedUser = 'rejectedUser'
    const networkErrorUser = 'networkErrorUser'
    const crawlerErrorMessage = 'User not found'
    const resolvedData = {
      solved: 101,
      submissions: 230,
    }

    beforeAll(() => {
      axiosMock.onGet(crawlerbaseUrl + resolvedUser)
        .replyOnce(200, {
          error: false,
          data: resolvedData,
        })
      axiosMock.onGet(crawlerbaseUrl + rejectedUser)
        .replyOnce(400, {
          error: true,
          message: crawlerErrorMessage,
        })
      axiosMock.onGet(crawlerbaseUrl + networkErrorUser)
        .networkError()
    })

    it('不会打包服务器端的函数', () => {
      expect(functionStrs.crawler_for_server).not.toContain(serverFunctionContent)
    })

    it('生成的函数在服务器端正常时能够正确返回', async () => {
      const res = await functions.crawler_for_server(resolvedUser)
      expect(res).toMatchObject(resolvedData)
    })

    it('生成的函数在服务器端返回错误时能够抛出异常，并包含正确的异常信息', async () => {
      const promise = expect(functions.crawler_for_server(rejectedUser))
      await promise.rejects.toThrow(Error)
      await promise.rejects.toThrow(crawlerErrorMessage)
      // 等到下方的测试用例失败时，可以直接使用第二条语句。
    })

    it('reject a string won\'t pass', async () => {
      // 按理来说这个用例应该失败的，否则有些情况测不出来。参考：
      // https://github.com/Liu233w/acm-statistics/issues/29
      expect(() => {
        throw 'aaa'
      }).toThrow('aaa')
      await expect(Promise.reject('aaa')).rejects.toThrow('aaa')
    })

    it('生成的函数在网络错误时能够抛出异常，并且含有异常信息', async () => {
      await expect(functions.crawler_for_server(networkErrorUser))
        .rejects.toThrow(/.+/)
    })
  })
})