const {readMetaConfigs, generateBrowserCrawlerFunctions} = require('crawler')
const path = require('path')
const fs = require('fs-extra')
const VirtualModulePlugin = require('webpack-virtual-modules')

function buildSources() {
  const corsModule = fs.readFileSync(path.join(__dirname, 'cors.js'), 'utf-8')
  let crawlersModule = `
  /* eslint-disable */
  ${corsModule}
  export default () => {
    const metas = ${JSON.stringify(readMetaConfigs())};
    const crawlers = {
  `
  const functions = generateBrowserCrawlerFunctions()
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

module.exports = function () {
  this.options.build.plugins.push(new VirtualModulePlugin({
    'dynamic/crawlers.js': buildSources(),
  }))
}
