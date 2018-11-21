const {readMetaConfigs, generateBrowserCrawlerFunctions} = require('crawler')
const path = require('path')
const fs = require('fs-extra')
const VirtualModulePlugin = require('virtual-module-webpack-plugin')

async function buildSources() {
  const corsModule = await fs.readFile(path.join(__dirname, 'cors.js'), 'utf-8')
  let crawlersModule = `
  /* eslint-disable */
  ${corsModule}
  import axios from 'axios'
  export default () => {
    const metas = ${JSON.stringify(await readMetaConfigs())};
    const crawlers = {
  `
  const functions = await generateBrowserCrawlerFunctions()
  for(let key in functions) {
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
