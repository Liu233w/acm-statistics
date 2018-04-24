const {readMetaConfigs, generateBrowserCrawlerFunctions} = require('crawler')
const path = require('path')

module.exports = async function () {
  this.addPlugin({
    src: path.resolve(__dirname, './crawler.template.js'),
    options: {
      crawlers: await generateBrowserCrawlerFunctions(),
      meta: await readMetaConfigs(),
    },
    // ssr: false
  })
  this.addPlugin({
    src: path.resolve(__dirname, './cors.js'),
    ssr: false,
  })
  this.addVendor(['axios', 'superagent', 'cheerio'])
}
