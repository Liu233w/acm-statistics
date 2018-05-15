/* eslint-disable no-undef */

const {Nuxt, Builder} = require('nuxt')
const {resolve} = require('path')
const cheerio = require('cheerio')
const _ = require('lodash')

// mock 爬虫，防止修改 crawler 模块引起快照改变
jest.mock('../../modules/crawlerLoader')

// We keep a reference to Nuxt so we can close
// the server at the end of the test
let nuxt = null

// Init Nuxt.js
beforeAll(async () => {
  const rootDir = resolve(__dirname, '../..')
  const config = require(resolve(rootDir, 'nuxt.config.js'))
  config.rootDir = rootDir // project folder
  config.dev = false // production build
  nuxt = new Nuxt(config)
  await new Builder(nuxt).build()
}, 1200000) // 用两分钟的时间启动 nuxt

async function testPageByPath(path) {
  const context = {}
  const {html} = await nuxt.renderRoute(path, context)
  const $ = cheerio.load(html)

  $('link[href^="/_nuxt/"]').remove()
  $('script[src^="/_nuxt/"]').remove()

  // 移除 data-v- 开头的属性和 data-vue-ssr-id 属性
  $('*').each((i, el) => {
    $(el).removeAttr('data-vue-ssr-id')
    for (let key in $(el).attr()) {
      // eslint-disable-next-line lodash/prefer-lodash-method
      if (key.startsWith('data-v-')) {
        $(el).removeAttr(key)
      }
    }
  })

  // 移除随机数
  const storeEl = $(_.filter($('script'), el => /window\.__NUXT__/.test($(el).html())))
  storeEl.html(_.replace(storeEl.html(), /,"key":0\.\d*/g, ''))

  expect($.html()).toMatchSnapshot()
}

const testPaths = [
  '/',
  '/statistics',
]

for (let path of testPaths) {
  test(path, () => testPageByPath(path))
}

// Close the Nuxt server
afterAll('Closing server', () => {
  nuxt.close()
})
