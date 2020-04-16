const koa = require('koa')
const Router = require('koa-router')
const bodyParser = require('koa-bodyparser')
const NodeCache = require('node-cache')
const { v4: uuid } = require('uuid')
let svgCaptcha = require('svg-captcha')

if (process.env.E2E) {
    // E2E 测试时把接口固定
    // eslint-disable-next-line jest/no-mocks-import
    svgCaptcha = require('../__mocks__/svg-captcha')
}

const restHelper = require('./restHelper')

const router = new Router()

const cache = new NodeCache()

const CAPTCHA_AVAILABLE_TIME = process.env.CAPTCHA_AVAILABLE_TIME
    ? process.env.CAPTCHA_AVAILABLE_TIME : 5 * 60 /* sec */

// 先把验证码存在项目中。如果遇到性能问题再上redis

router.post('/api/captcha-service/generate', async ctx => {
    const id = uuid()
    const captcha = svgCaptcha.create({
        size: 6,
        noise: 2,
    })

    cache.set(id, captcha.text, CAPTCHA_AVAILABLE_TIME)

    ctx.rest({
        id,
        captcha: captcha.data,
    })
})

router.post('/api/captcha-service/validate', async ctx => {
    const { id, text } = ctx.request.body

    const correct = cache.get(id)
    cache.del(id)

    if (!correct) {
        ctx.error('Please refresh captcha')
        return
    }

    if (correct.toLowerCase() !== text.toLowerCase()) {
        ctx.error('Incorrect captcha')
        return
    }

    ctx.rest(true)
})

const app = new koa()
    .use(bodyParser())
    .use(restHelper)
    .use(router.middleware())
    .use(router.allowedMethods)

// 将 req.ip 设置为 X-Forward-For 中的 ip
app.proxy = true

module.exports = app
