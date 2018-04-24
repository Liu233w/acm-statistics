const {readMetaConfigs} = require('./lib/configReader')
const {generateBrowserCrawlerFunctions, generateServerCrawlerFunctions} = require('./lib/functionGenerator')

module.exports = {
  readMetaConfigs,
  generateServerCrawlerFunctions,
  generateBrowserCrawlerFunctions,
}