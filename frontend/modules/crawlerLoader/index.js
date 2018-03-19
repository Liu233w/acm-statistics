const configReader = require('../../../crawler/lib/configReader')
const path = require('path')

module.exports = async function (moduleOptions) {
  this.addPlugin({
    src: path.resolve(__dirname, './crawler.template.js'),
    options: await configReader.generateBrowserCrawlerFunctions(),
    // ssr: false
  })
  this.addPlugin({
    src: path.resolve(__dirname, './cors.js'),
    ssr: false
  })
  this.addVendor(['axios', 'superagent', 'cheerio'])
}
