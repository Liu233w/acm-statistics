const koa = require('koa')

const restHelper = require('./utils/restHelper')
const logUtil = require('./utils/logUtil')

const app = new koa()
const apiRouter = require('./apiRouter')

const errHelper = async (ctx, next) => {
  try {
    await next()
  } catch (err) {
    ctx.error(err.message)
    // 重新抛出异常，让它被上面的 logUtil 捕捉到
    throw err
  }
}

const notFoundHelper = async (ctx, next) => {
  if (ctx.response.status === 404) {
    ctx.error('404 Not Found')
    ctx.response.status = 404
  }
  await next()
}

app
  .use(logUtil)
  .use(restHelper)
  .use(errHelper)
  .use(apiRouter.routes())
  .use(apiRouter.allowedMethods())
  .use(notFoundHelper)

module.exports = app
