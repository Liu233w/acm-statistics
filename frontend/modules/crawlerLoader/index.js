let crawlerModule
if (process.env.E2E) {
  console.log('running project in e2e test, will mock crawlerModule')
  crawlerModule = require('../../__test__/e2eMocks/crawler.js')
} else {
  crawlerModule = require('crawler')
}

const {readMetaConfigs, generateBrowserCrawlerFunctions} = crawlerModule
const path = require('path')
const fs = require('fs-extra')
const VirtualModulePlugin = require('virtual-module-webpack-plugin')

async function buildSources() {
  const corsModule = await fs.readFile(path.join(__dirname, 'cors.js'), 'utf-8')
  const mockedRequire = await fs.readFile(path.join(__dirname, 'mockRequire.js'), 'utf-8')
  let crawlersModule = `
  /* eslint-disable */
  import axios from 'axios'
  import superagent from 'superagent'
  import jquery from 'jquery'
  ${corsModule}
  ${mockedRequire}
  export default () => {
    const metas = ${JSON.stringify(await readMetaConfigs())};
    const crawlers = {
  `
  const functions = await generateBrowserCrawlerFunctions()
  for (let key in functions) {
    crawlersModule += `${key}: ${functions[key]},\n`
  }
  crawlersModule += `
    }
    return {
      metas,
      crawlers,
    }
  }`

  return crawlersModule
}

module.exports = async function () {
  this.options.build.plugins.push(new VirtualModulePlugin({
    moduleName: 'dynamic/crawlers.js',
    contents: await buildSources(),
  }))
}
