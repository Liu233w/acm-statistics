const {readMetaConfigs, generateBrowserCrawlerFunctions} = require('crawler')
const path = require('path')
const fs = require('fs-extra')
const VirtualModulePlugin = require('webpack-virtual-modules')

async function buildSources() {
  const corsModule = await fs.readFile(path.join(__dirname, 'cors.js'), 'utf-8')
  let crawlersModule = `
  /* eslint-disable */
  ${corsModule}
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
    'dynamic/crawlers.js': await buildSources(),
  }))
}
