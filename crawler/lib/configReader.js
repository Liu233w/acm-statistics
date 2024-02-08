/*
用于读取用户的爬虫设置，用在前端时，本模块只在编译期间运行，将会返回给用户设置信息
 */

const fs = require('fs')
const yml = require('js-yaml')
const join = require('path').join
const _ = require('lodash')

// require 的路径是相对源文件路径的，而 fs 模块的路径是相对于工作路径的，必须使用 __dirname 来转换
const configPath = join(__dirname, '../config.yml')

// 环境变量的前缀
const envConfigPrefix = 'ACM_STATISTICS_CRAWLER_ENV:'

/**
 * 从一个表示环境变量的键值对里面读取配置，将配置合并到 config 中。
 * 将使用 _.set 来合并数据
 *
 * 举例：对于环境变量 ACM_STATISTICS_CRAWLER_ENV:a:b:1:c = 12 和配置对象 {a:{b:[{}],d:333}}，
 * 结果为 {a:{b:[{},{c:12}],d:333}}
 *
 * 环境变量的值将使用 JSON.parse 来处理，因此可以使用任意 json 中存在的类型。
 * 如果需要传入字符串，需要使用类似于 ACM_STATISTICS_CRAWLER_ENV:a:b:1:c = "12"
 * 或者 ACM_STATISTICS_CRAWLER_ENV:a = "{asdf}" 这样的形式
 *
 * @param {object} config
 * @param {object.<string,string>} env
 */
exports.mergeConfigWithEnv = (config, env) => {

  _.forEach(env, (value, key) => {

    if (_.startsWith(key, envConfigPrefix)) {
      const keyStr = key.slice(envConfigPrefix.length)
      _.set(config, _.split(keyStr, ':'), JSON.parse(value))

    } else {
      // pass
    }
  })
}

/**
 * 从文件读取配置并和环境变量合并
 *
 * @return {Promise<object>}
 */
exports.readConfigs = () => {
  const config = yml.load(fs.readFileSync(configPath, 'utf-8'))
  exports.mergeConfigWithEnv(config, process.env)
  return config
}

/**
 * 获取爬虫的所有配置
 * @return {Promise<Array<Object>>}
 */
exports.readCrawlerConfigs = () => {

  const config = exports.readConfigs()

  return _.map(config.crawler_order, name =>
    _.assign({name: name}, config.crawlers[name]))
}

/**
 * 返回一个对象，其中key是爬虫名，value是一个Object，包含爬虫的元信息
 * @returns {Promise<Object.<String, Object>>}
 */
exports.readMetaConfigs = () => {
  const config = exports.readConfigs()

  let ret = {}
  for (let name of config.crawler_order) {
    ret[name] = config.crawlers[name].meta
  }
  return ret
}
