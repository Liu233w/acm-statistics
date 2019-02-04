/*
用于读取用户的爬虫设置，用在前端时，本模块只在编译期间运行，将会返回给用户设置信息
 */

const fs = require('fs-extra')
const yml = require('js-yaml')
const join = require('path').join
const _ = require('lodash')

// require 的路径是相对源文件路径的，而 fs 模块的路径是相对于工作路径的，必须使用 __dirname 来转换
const configPath = join(__dirname, '../config.yml')

// 环境变量的前缀
const envConfigPrefix = 'ACM_STATISTICS_CRAWLER_ENV:'
const envConfigObjectPrefix = 'ACM_STATISTICS_CRAWLER_ENV_OBJECT:'

/**
 * 从一个表示环境变量的键值对里面读取配置，将配置合并到 config 中。
 * 将使用 _.set 来合并数据
 *
 * 举例：对于环境变量 ACM_STATISTICS_CRAWLER_ENV:a:b:1:c = 12 和配置对象 {a:{b:[{}],d:333}}，
 * 结果为 {a:{b:[{},{c:12}],d:333}}
 *
 * 如果环境变量的前缀为 ACM_STATISTICS_CRAWLER_ENV_OBJECT: ，则将值作为json对象直接覆盖对应的路径值
 *
 * @param {object} config
 * @param {object.<string,string>} env
 */
exports.mergeConfigWithEnv = (config, env) => {

  _.forEach(env, (value, key) => {

    if (_.startsWith(key, envConfigPrefix)) {
      const keyStr = key.slice(envConfigPrefix.length)
      _.set(config, _.split(keyStr, ':'), value)

    } else if (_.startsWith(key, envConfigObjectPrefix)) {
      const keyStr = key.slice(envConfigObjectPrefix.length)
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
exports.readConfigs = async () => {
  const config = yml.safeLoad(await fs.readFile(configPath, 'utf-8'))
  exports.mergeConfigWithEnv(config, process.env)
  return config
}

/**
 * 返回一个对象，其中key是爬虫名，value是一个Object，包含爬虫的元信息
 * @returns {Promise<Object.<String, Object>>}
 */
exports.readMetaConfigs = async () => {
  const config = await exports.readConfigs()

  let ret = {}
  for (let item of config.crawlers) {
    ret[item.name] = item.meta
  }
  return ret
}
