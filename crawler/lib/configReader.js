/*
用于读取用户的爬虫设置，用在前端时，本模块只在编译期间运行，将会返回给用户设置信息
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
