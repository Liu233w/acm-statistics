const Router = require('koa-router')
const _ = require('lodash')

const configReader = require('../crawler/lib/configReader')
let crawlers
configReader.generateServerCrawlerFunctions()
  .then(res => crawlers = res)
let crawlerMeta
configReader.readMetaConfigs()
  .then(res => crawlerMeta = res)

const swagger = require('./swagger.json')

const router = new Router()

router.get('/api/crawlers/swagger.json', async (ctx, next) => {
  ctx.response.type = 'application/json'
  ctx.response.state = 200
  ctx.response.body = JSON.stringify(swagger)

  await next()
})

router.get('/api/crawlers/:type/:username', async (ctx, next) => {

  const ojFunc = crawlers[ctx.params.type]

  if (!_.isFunction(ojFunc)) {
    throw new Error('不存在此OJ的爬虫')
  }

  ctx.rest(await ojFunc(ctx.params.username))

  await next()
})

router.get('/api/crawlers', async (ctx, next) => {
  ctx.rest(_.mapValues(crawlers, (value, key) => crawlerMeta[key]))
  await next()
})

module.exports = router