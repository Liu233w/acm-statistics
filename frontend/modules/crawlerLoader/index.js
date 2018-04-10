const configReader = require('../../../crawler/lib/configReader')
const functionGenerator = require('../../../crawler/lib/functionGenerator')
const path = require('path')

module.exports = async function () {
  this.addPlugin({
    src: path.resolve(__dirname, './crawler.template.js'),
    options: {
      crawlers: await functionGenerator.generateBrowserCrawlerFunctions(),
      meta: await configReader.readMetaConfigs(),
    },
    // ssr: false
  })
  this.addPlugin({
    src: path.resolve(__dirname, './cors.js'),
    ssr: false,
  })
  this.addVendor(['axios', 'superagent', 'cheerio'])
}
