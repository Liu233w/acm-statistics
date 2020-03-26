/**
 * 方便写 rest API 的中间件
 */

module.exports = async function (ctx, next) {

  ctx.rest = function (data) {
    ctx.response.status = 200
    ctx.response.type = 'application/json'
    ctx.response.body = {
      error: false,
      data: data,
    }
  }

  ctx.error = function (message) {
    ctx.response.status = 400
    ctx.response.type = 'application/json'
    ctx.response.body = {
      error: true,
      message: message,
    }
  }

  await next()
}