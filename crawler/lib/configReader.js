/*
用于读取用户的爬虫设置，用在前端时，本模块只在编译期间运行，将会返回给用户已经注入好设置信息的爬虫函数
 */

const fs = require('fs-extra')
const yml = require('js-yaml')
const join = require('path').join

// require 的路径是相对源文件路径的，而 fs 模块的路径是相对于工作路径的，必须使用 __dirname 来转换
const configPath = join(__dirname, '../config.yml')
const configTemplatePath = join(__dirname, '../config.template.yml')

/**
 * 从 config.yml 读取设置信息，并返回。如果文件不存在，将从 config.template.yml 中复制信息并生成此文件
 * @returns {Promise<Object>}
 */
exports.ensureConfigAndRead = async () => {
  const exists = await fs.pathExists(configPath)
  if (!exists) {
    await fs.copy(configTemplatePath, configPath)
  }

  return yml.safeLoad(await fs.readFile(configPath, 'utf-8'))
}

/**
 * 返回一个对象，其中key是爬虫名，value是一个Object，包含爬虫的元信息
 * @returns {Promise<{Object.<{String}, {Object}>}>}
 */
exports.readMetaConfigs = async () => {
  const config = await exports.ensureConfigAndRead()

  let ret = {}
  for (let item of config.crawlers) {
    ret[item.name] = item.meta
  }
  return ret
}

/**
 * 爬虫函数的返回类型
 * @typedef crawlerReturns
 * @type {Object}
 * @property {Number} solved - 用户通过的题量
 * @property {Number} submissions - 用户的总提交量
 */

/**
 * 为服务端返回的爬虫函数
 * @typedef serverCrawlerFunction
 * @type {Function}
 * @param {String} username - 要爬取的用户名
 * @returns {Promise<{crawlerReturns}>}
 */

/**
 * 为服务器端返回爬虫函数，会从config.yml读取信息，并返回一个对象
 *
 * @returns {Promise<{Object.<string, {serverCrawlerFunction}>}>}
 */
exports.generateServerCrawlerFunctions = async () => {
  const config = await exports.ensureConfigAndRead()

  const ret = {}
  for (let item of config.crawlers) {
    if (!item.name) {
      continue
    }
    const crawlerFunc = require(`../crawlers/${item.name}.js`)
    ret[item.name] = username => crawlerFunc(item, username)
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
 * @typedef clientCrawlerFunction
 * @type {String}
 */

/**
 * 返回给前端使用的爬虫函数，会从 config.yml 读取配置信息，装配配置信息并返回。
 * 设置了 server_only 的爬虫会返回一个 axios 的请求
 *
 * @returns {Promise<{Object.<string, {clientCrawlerFunction}>}>}
 */
exports.generateBrowserCrawlerFunctions = async () => {
  const config = await exports.ensureConfigAndRead()

  const ret = {}
  for (let item of config.crawlers) {
    if (!item.name) {
      continue
    }
    const crawlerFuncStr = await fs.readFile(join(__dirname, `../crawlers/${item.name}.js`), 'utf-8')
    if (item.server_only) {
      ret[item.name] = `
        (username) => {
          return new Promise((resolve, reject) => {
            axios.get('/api/crawlers/${item.name}/'+username)
              .then(response => {
                // console.log(response)
                if (response.data.error) {
                  reject(response.data.message)
                } else {
                  resolve(response.data.data)
                }
              })
              .catch(err => {
                // console.error(err)
                if (err.response && err.response.data.message) {
                  // 服务端的爬虫报的错
                  reject(new Error(err.response.data.message))
                } else {
                  //网络错误或其他错误
                  reject(new Error(err.message))
                }
              })
          })
        }
      `
    } else {
      ret[item.name] = `
        (username) => {
          let module = {exports: {}}
          ;(function(module, exports) { ${crawlerFuncStr} })(module, module.exports)
          return module.exports(${JSON.stringify(item)}, username)
        }
    `
    }
  }
  return ret
}
