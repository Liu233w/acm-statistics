const koa = require('koa')

const restHelper = require('./utils/restHelper')
const logUtil = require('./utils/logUtil')
const rateLimiter = require('./utils/rateLimit')

const app = new koa()
const apiRouter = require('./apiRouter')

const errHelper = async (ctx, next) => {
  try {
    await next()
  } catch (err) {
    ctx.error(err.message)
    // re-throw the exception to let it be caught by logUtil
    throw err
  }
}

const notFoundHelper = async (ctx, next) => {
  await next()
  if (ctx.response.status === 404) {
    ctx.error('404 Not Found')
    ctx.response.status = 404
  }
}

app
  .use(rateLimiter)
  .use(logUtil)
  .use(restHelper)
  .use(notFoundHelper)
  .use(errHelper)
  .use(apiRouter.routes())
  .use(apiRouter.allowedMethods())

module.exports = app
