const Router = require('koa-router')
const _ = require('lodash')

const {readMetaConfigs, generateServerCrawlerFunctions} = require('crawler')
let crawlers
generateServerCrawlerFunctions()
  .then(res => crawlers = res)
let crawlerMeta
readMetaConfigs()
  .then(res => crawlerMeta = res)

const swagger = require('./swagger.json')

const router = new Router()

router.get('/api/crawlers/swagger.json', async (ctx) => {
  ctx.response.type = 'application/json'
  ctx.response.state = 200
  ctx.response.body = JSON.stringify(swagger)
})

router.get('/api/crawlers/:type/:username', async (ctx) => {

  const ojFunc = crawlers[ctx.params.type]

  if (!_.isFunction(ojFunc)) {
    throw new Error('Crawler of the oj does not exist')
  }

  ctx.rest(await ojFunc(ctx.params.username))
})

router.get('/api/crawlers', async (ctx) => {
  ctx.rest(_.mapValues(crawlers, (value, key) => crawlerMeta[key]))
})

module.exports = router