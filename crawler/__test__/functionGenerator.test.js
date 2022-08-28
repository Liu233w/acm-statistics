const path = require('path')
const _ = require('lodash')

// used by eval()
// eslint-disable-next-line no-unused-vars
const superagent = require('superagent')

const nock = require('nock')

jest.mock('../lib/configReader')
jest.mock('fs')

const fs = require('fs')

describe('generateServerCrawlerFunctions', () => {

  let functions

  beforeAll(async () => {

    for (let item of ['crawler1', 'crawler2', 'crawler_for_server']) {
      // 因为 functionGenerator 里面 require 的时候带了 js 后缀，这里必须也加上后缀，否则会提示找不到模块
      jest.mock(`../crawlers/${item}.js`, () => async function (config, username) {
        return {
          config: config,
          username: username,
          solved: 0,
          submissions: username === 'wrong format' ? NaN : 0,
        }
      }, { virtual: true })
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

  it('should throw when crawler return wrong format results', async () => {
    await expect(functions.crawler1('wrong format'))
      .rejects.toThrow('The crawler returned wrong format result. It can be a bug in crawler.')
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
            solved: 0,
            submissions: username === 'wrong format' ? NaN : 0,
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

  it('should throw when crawler return wrong format results', async () => {
    await expect(functions.crawler1('wrong format'))
      .rejects.toThrow('The crawler returned wrong format result. It can be a bug in crawler.')
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

    it('不会打包服务器端的函数', () => {
      expect(functionStrs.crawler_for_server).not.toContain(serverFunctionContent)
    })

    it('生成的函数在服务器端正常时能够正确返回', async () => {

      const scope = nock('http://localhost')
        .get(crawlerbaseUrl + resolvedUser)
        .reply(200, {
          error: false,
          data: resolvedData,
        })

      const res = await functions.crawler_for_server(resolvedUser)
      expect(res).toMatchObject(resolvedData)

      scope.done()
    })

    it('生成的函数在服务器端返回错误时能够抛出异常，并包含正确的异常信息', async () => {

      const scope = nock('http://localhost')
        .get(crawlerbaseUrl + rejectedUser)
        .reply(200, {
          error: true,
          message: crawlerErrorMessage,
        })

      await expect(functions.crawler_for_server(rejectedUser))
        .rejects.toThrow(crawlerErrorMessage)

      scope.done()
    })

    it('生成的函数在网络错误时能够抛出异常，并且含有异常信息', async () => {
      const scope = nock('http://localhost')
        .get(crawlerbaseUrl + networkErrorUser)
        .replyWithError('something awful happened')

      await expect(functions.crawler_for_server(networkErrorUser))
        .rejects.toThrow(/.+/)

      scope.done()
    })
  })
})
