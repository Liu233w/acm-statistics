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

  // 移除 id="input-XXXX" 和 for="input-XXXX" 属性
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

  // replace date and id in /history
  if (path === '/history') {
    $('tbody td.text-start:nth-child(3)').text('[[replaced date]]')
    $('body').html(_.replace(
      $('body').html(),
      new RegExp(`(?<=^|[^0-9])${historyId}(?=[^0-9]|$)`, 'g'),
      '[[replaced history id]]'))
    $('body').html(_.replace($('body'), /(?<=new Date\()\d+/g, '[[replaced date digit]]'))
  }

  // replace date and id in summary
  if (path.startsWith('/history/')) {
    $('div:contains("Generated at")')
      .contents()
      .end()
      .text('[[replaced date]]')
    $('body').html(_.replace(
      $('body').html(),
      new RegExp(`(?<=^|[^0-9])${historyId}(?=[^0-9]|$)`, 'g'),
      '[[replaced history id]]'))
    $('body').html(_.replace($('body'), /(?<=new Date\()\d+/g, '[[replaced date digit]]'))
  }

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

let historyId

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

  // add a record to history
  const postHistoryRes = await request.post(basePath + '/api/services/app/QueryHistory/SaveOrReplaceQueryHistory')
    .send({ 'mainUsername': '', 'queryWorkerHistories': [{ 'crawlerName': 'hdu', 'username': 'wwwlsmcom', 'errorMessage': null, 'submission': 72, 'solved': 34, 'solvedList': ['1000', '1114', '1166', '2000', '2001', '2002', '2003', '2004', '2005', '2006', '2007', '2008', '2009', '2010', '2011', '2012', '2013', '2014', '2015', '2016', '2017', '2018', '2019', '2020', '2021', '2022', '2024', '2025', '2026', '2027', '2070', '2191', '2602', '4825'], 'isVirtualJudge': false }, { 'crawlerName': 'leetcode_cn', 'username': 'wwwlsmcom', 'errorMessage': null, 'submission': 4, 'solved': 2, 'solvedList': null, 'isVirtualJudge': false }, { 'crawlerName': 'vjudge', 'username': 'wwwlsmcom', 'errorMessage': null, 'submission': 704, 'solved': 161, 'solvedList': ['poj-2394', 'poj-1011', 'poj-2251', 'poj-1426', 'poj-1330', 'hdu-4825', 'hdu-5090', 'hdu-5091', 'hdu-5095', 'hdu-5099', 'poj-3126', 'poj-1321', 'hdu-1879', 'hdu-1233', 'uva-10034', 'codeforces-415A', 'codeforces-369A', 'hdu-2222', 'poj-3630', 'hdu-1711', 'poj-3461', 'hdu-1878', 'poj-1094', 'uva-10033', 'uva-10020', 'hdu-1106', 'hdu-1166', 'hdu-1251', 'hdu-1506', 'poj-3061', 'poj-2823', 'hdu-2149', 'uva-10037', 'uva-10010', 'hdu-1527', 'hdu-3791', 'hdu-2176', 'hdu-1849', 'hdu-2177', 'timus-1009', 'hdu-1847', 'hdu-1846', 'poj-1988', 'poj-1611', 'poj-2955', 'poj-3280', 'hdu-5115', 'hdu-1003', 'hdu-1421', 'hdu-1087', 'hdu-1159', 'poj-2533', 'poj-1088', 'poj-1163', 'uva-442', 'hdu-2141', 'poj-1064', 'poj-3104', 'poj-2456', 'poj-2785', 'poj-1700', 'hdu-1114', 'hdu-2602', 'poj-2376', 'poj-1328', 'hdu-2037', 'codeforces-330B', 'timus-1792', 'hdu-1198', 'uva-10152', 'fzu-1327', 'poj-3009', 'poj-1564', 'poj-1562', 'poj-2488', 'poj-1111', 'poj-3984', 'poj-1915', 'poj-1979', 'poj-3278', 'codeforces-436B', 'codeforces-393A', 'codeforces-387A', 'hdu-2049', 'hdu-2045', 'hdu-2007', 'hdu-2054', 'hdu-2031', 'LightOJ-1303', 'hdu-1263', 'hdu-1004', 'hdu-1896', 'hdu-1873', 'poj-2259', 'uva-10935', 'poj-1363', 'codechef-DRAGNXOR', 'UVALive-4648', 'hdu-4006', 'uva-10305', 'hdu-2629', 'uva-548', 'hdu-5670', 'hdu-1221', 'hdu-5533', 'hdu-4472', 'hdu-5612', 'hdu-1045', 'uva-10115', 'hdu-1023', 'hdu-3823', 'hdu-2303', 'poj-1007', 'poj-2362', 'hdu-1240', 'poj-2243', 'poj-1019', 'hdu-1030', 'poj-1023', 'hdu-1015', 'hdu-4864', 'hdu-1016', 'poj-2503', 'poj-2051', 'poj-3190', 'poj-2782', 'poj-2393', 'poj-1456', 'hdu-1515', 'hdu-4310', 'hdu-1789', 'hdu-1022', 'hdu-1018', 'hdu-2057', 'poj-2833', 'hdu-2050', 'hdu-2051', 'hdu-1009', 'poj-2463', 'hdu-1075', 'poj-2196', 'hdu-1002', 'poj-2242', 'poj-3672', 'poj-1013', 'poj-1012', 'poj-1001', 'poj-2769', 'poj-3006', 'poj-2272', 'hdu-1013', 'hdu-1027', 'hdu-1029', 'hdu-1019', 'hdu-1031', 'hdu-1035', 'hdu-1228', 'hdu-1225', 'hdu-1033', 'hdu-1034', 'hdu-1032'], 'submissionsByCrawlerName': { 'poj': 325, 'hdu': 313, 'uva': 28, 'codeforces': 15, 'timus': 6, 'fzu': 5, 'LightOJ': 1, 'codechef': 5, 'UVALive': 1, 'dashiye': 5 }, 'isVirtualJudge': true }] })
    .set('Cookie', ['OAuthToken=' + authToken])

  historyId = postHistoryRes.body.result.queryHistoryId
})

const testPathsRequireLogin = [
  '/settings',
  '/history',
]

for (let path of testPathsRequireLogin) {
  test(path, () => testPageByPath(path, authToken))
}

test('/history/{historyId}', () => testPageByPath('/history/' + historyId, authToken))