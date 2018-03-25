const {Nuxt, Builder} = require('nuxt')
const {resolve} = require('path')
const cheerio = require('cheerio')

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

  expect($.html()).toMatchSnapshot()
}

const testPaths = [
]

for (let path of testPaths) {
  test(path, () => testPageByPath(path))
}

// Close the Nuxt server
afterAll('Closing server', () => {
  nuxt.close()
})
