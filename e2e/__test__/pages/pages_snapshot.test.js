const _ = require('lodash')
const superagent = require('superagent')
const cheerio = require('cheerio')
const request = require('superagent')

const basePath = 'http://localhost:3000'

async function testPageByPath(path, authToken) {

  const url = basePath + path
  console.log(`request url at ${url}`)

  let request = superagent.get(url)
  if (authToken) {
    request = request.set('Cookie', ['OAuthToken=' + authToken])
  }

  const res = await request
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

  // remove id="input-XXXX" and for="input-XXXX"
  $('*').each((i, el) => {
    for (let key in $(el).attr()) {
      const value = $(el).attr(key)
      if (_.startsWith(value, 'input-') && _.includes(['for', 'id'], key)) {
        $(el).removeAttr(key)
      }
    }
  })

  // remove aria-owns="list-XXX"
  $('*').each((i, el) => {
    for (let key in $(el).attr()) {
      const value = $(el).attr(key)
      if (_.startsWith(value, 'list-') && key === 'aria-owns') {
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

  // remove name="radio-XXX"
  $('*').each((i, el) => {
    for (let key in $(el).attr()) {
      const value = $(el).attr(key)
      if (_.startsWith(value, 'radio-') && key === 'name') {
        $(el).removeAttr(key)
      }
    }
  })

  // remove aria-labelledby="input-XXX"
  $('*').each((i, el) => {
    for (let key in $(el).attr()) {
      const value = $(el).attr(key)
      if (_.startsWith(value, 'input-') && key === 'aria-labelledby') {
        $(el).removeAttr(key)
      }
    }
  })

  // remove data-fetch-key="data-v-..." and the value
  const dataVValues = []
  const dataFetchKeyKey = 'data-fetch-key'
  $('*').each((i, el) => {
    const value = $(el).attr(dataFetchKeyKey)
    if (value) {
      $(el).removeAttr(dataFetchKeyKey)
      dataVValues.push(value)
    }
  })

  let html = $(storeEl).html()
  dataVValues.forEach(v => {
    html = html.replace(`"${v}":`, '"FETCH_KEY":')
  })
  $(storeEl).html(html)

  expect($.html()).toMatchSnapshot()
}

const testPaths = [
  '/',
  '/statistics',
  '/about',
  '/login',
  '/register',
]

for (let path of testPaths) {
  test(path, async () => await testPageByPath(path))
}


let authToken

// eslint-disable-next-line no-undef
beforeAll(async () => {
  // do login
  const res = await superagent.post(basePath + '/api/TokenAuth/Authenticate')
    .send({
      userNameOrEmailAddress: 'admin',
      password: '123qwe',
      rememberClient: true,
    })

  authToken = res.body.result.accessToken
})

const testPathsRequireLogin = [
  '/settings',
]

for (let path of testPathsRequireLogin) {
  test(path, () => testPageByPath(path, authToken))
}

test('/history', async () => {
  await request.get('http://localhost:3000/mock-configurer/history-snapshot/history')
  await testPageByPath('/history/', authToken)
})

test('/history/{historyId}', async () => {
  await request.get('http://localhost:3000/mock-configurer/history-snapshot/summary')
  await testPageByPath('/history/1', authToken)
})