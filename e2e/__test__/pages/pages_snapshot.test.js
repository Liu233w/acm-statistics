const _ = require('lodash')
const superagent = require('superagent')
const cheerio = require('cheerio')

const basePath = 'http://localhost:3000'

async function testPageByPath(path) {

  const url = basePath + path
  console.log(`request url at ${url}`)

  const res = await superagent.get(url)
  if (!res.ok) {
    console.log(`path ${path} does not have a 200 response, the response: `, res)
    throw Error(`path ${path} does not have a 200 response`)
  }
  const $ = cheerio.load(res.text)

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

  // 移除 id="input-XXXX" 和 for="input-XXXX" 属性
  $('*').each((i, el) => {
    for (let key in $(el).attr()) {
      const value = $(el).attr(key)
      if (_.startsWith(value, 'input-') && _.includes(['for', 'id'], key)) {
        $(el).removeAttr(key)
      }
    }
  })

  // 将 css 中的id属性去掉
  $('style').each((i, el) => {
    $(el).html(_.replace($(el).html(), /\[data-v-.*?\]/g, ''))
  })

  // 移除随机数
  const storeEl = $(_.filter($('script'), el => /window\.__NUXT__/.test($(el).html())))
  storeEl.html(_.replace(storeEl.html(), /,key:\.\d*/g, ''))

  expect($.html()).toMatchSnapshot()
}

const testPaths = [
  '/',
  '/statistics',
  '/about',
  '/login',
  '/register',
  '/settings',
]

for (let path of testPaths) {
  test(path, async () => await testPageByPath(path))
}
