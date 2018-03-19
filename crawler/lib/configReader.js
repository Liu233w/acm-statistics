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
    const crawlerFunc = require(`../crawlers/${item.name}.js`)
    ret[item.name] = username => crawlerFunc(item, username)
  }

  return ret
}

