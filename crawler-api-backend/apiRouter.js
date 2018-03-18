const Router = require('koa-router');
const _ = require('lodash')

let crawlers
require('../crawler/lib/configReader').generateServerCrawlerFunctions()
  .then(res => crawlers = res)

const router = new Router()

router.get('/api/crawlers/:type/:username', async (ctx, next) => {

  const ojFunc = crawlers[ctx.params.type]

  if (typeof ojFunc !== 'function') {
    throw new Error('不存在此OJ的爬虫')
  }

  ctx.rest(await ojFunc(ctx.params.username))

  await next()
})

router.get('/api/crawlers', async (ctx, next) => {
  ctx.rest(_.keys(crawlers))
  await next()
})

module.exports = router