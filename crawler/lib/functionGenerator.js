/*
用于生成爬虫函数，用在前端时，本模块只在编译期间运行，返回给用户已经注入好设置信息的爬虫函数
*/

const configReader = require('./configReader')

const fs = require('fs').promises
const _ = require('lodash')
const join = require('path').join

/**
 * 爬虫函数的返回类型
 * @typedef {Object} CrawlerReturns
 * @type {Object}
 * @property {Number} solved - 用户通过的题量
 * @property {Number} submissions - 用户的总提交量
 * @property {Array<String>|undefined|null} solvedList - 用户通过的题目列表
 */

/**
 * 为服务端返回的爬虫函数
 * @typedef {Function} ServerCrawlerFunction
 * @type {Function}
 * @param {String} username - 要爬取的用户名
 * @returns {Promise<CrawlerReturns>}
 */

/**
 * Wrap the result from crawler to make sure it has right format
 * @param {string} promiseExpression the string of express that evaluate to the promise
 * of the result
 * @returns {string} 
 */
exports.crawlerWrapper = promiseExpression => {
  return `${promiseExpression}
    .then(res => {
      ${checkNumberFormat('res.solved')}
      ${checkNumberFormat('res.submissions')}
      if (res.solvedList !== null && res.solvedList !== undefined 
        && !(res.solvedList instanceof Array)) {
        throw new Error('The crawler returned wrong format result. It can be a bug in crawler.')
      }
      return res
    })`

  function checkNumberFormat(field) {
    return `
      if (!Number.isInteger(${field}) || ${field} < 0) {
        throw new Error('The crawler returned wrong format result. It can be a bug in crawler.')
      }`
  }
}

/**
 * 为服务器端返回爬虫函数，会从config.yml读取信息，并返回一个对象
 *
 * @returns {Promise<Object.<string, ServerCrawlerFunction>>}
 */
exports.generateServerCrawlerFunctions = async () => {
  const config = await configReader.readCrawlerConfigs()

  const ret = {}
  for (let item of config) {
    if (!item.name) {
      continue
    }
    const crawlerConfig = {
      env: 'server',
    }
    _.assign(crawlerConfig, item)
    // eslint-disable-next-line no-unused-vars
    const crawlerFunc = require(`../crawlers/${item.name}.js`)
    ret[item.name] = eval(`username => ${exports.crawlerWrapper('crawlerFunc(crawlerConfig, username)')}`)
  }

  return ret
}

/**
 * 为客户端返回的爬虫函数的源代码字符串。
 * 如果爬虫是server_only的，将会返回一个带 axios 请求的函数，
 * 函数将会向服务器发起请求，让服务器进行爬取；
 * 如果爬虫不是server_only的，返回已经注入了设置信息的爬虫源代码。
 * 请注意：本函数返回的对象中不含有“函数”，只有“函数的源代码”。
 * 需要使用eval或者将源代码拼接进源码文件中使用。
 * @typedef {Function} ClientCrawlerFunction
 * @type {Function}
 * @param {String} username - 要爬取的用户名
 * @returns {Promise<CrawlerReturns>}
 */

/**
 * 返回给前端使用的爬虫函数，会从 config.yml 读取配置信息，装配配置信息并返回。
 * 设置了 server_only 的爬虫会返回一个 axios 的请求
 *
 * @returns {Promise<Object.<string, ClientCrawlerFunction>>}
 */
exports.generateBrowserCrawlerFunctions = async () => {

  // 生成从服务器端进行查询的代码
  const resolveServerQuery = (crawlerName) => _.trim(`
    new Promise((resolve, reject) => {
      superagent.get('/api/crawlers/${crawlerName}/'+username)
        .then(response => {
          // console.log(response)
          if (response.body.error) {
            reject(new Error(response.body.message))
          } else {
            resolve(response.body.data)
          }
        })
        .catch(err => {
          // console.error(err)
          if (err.response && err.response.body && err.response.body.message) {
            // 服务端的爬虫报的错
            reject(new Error(err.response.body.message))
          } else {
            //网络错误或其他错误
            reject(err)
          }
        })
    })
  `)

  const config = await configReader.readCrawlerConfigs()

  const ret = {}
  for (let item of config) {
    if (!item.name) {
      continue
    }
    if (item.server_only) {
      ret[item.name] = `
        (username) => {
          return ${resolveServerQuery(item.name)}
        }
      `
    } else {
      const crawlerFuncStr = await fs.readFile(join(__dirname, `../crawlers/${item.name}.js`), 'utf-8')
      const crawlerConfig = {
        env: 'browser',
      }
      _.assign(crawlerConfig, item)
      ret[item.name] = `
        (username) => {
          let module = {exports: {}}
          ;(function(module, exports) { ${crawlerFuncStr} })(module, module.exports)
          return ${exports.crawlerWrapper(`module.exports(${JSON.stringify(crawlerConfig)}, username)`)}
            .catch(err => {
              if (err.response || err.url) {
                // 有response字段说明这是由 superagent 抛出的异常
                // 有url字段（而没有 response字段）说明是cors的异常
                return ${resolveServerQuery(item.name)}
              } else {
                throw err
              }
            })
        }
      `
    }
  }
  return ret
}
