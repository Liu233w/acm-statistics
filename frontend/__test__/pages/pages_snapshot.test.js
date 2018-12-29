/* eslint-disable no-undef */

const path = require('path')
const {spawn} = require('child_process')
const _ = require('lodash')
const superagent = require('superagent')
const cheerio = require('cheerio')

const basePath = 'http://localhost:3000'

let runProcess = null

beforeAll(async () => {

  console.log('start building nuxt...')
  await wrapSpawn('npm', ['run', 'build'])

  console.log('start running nuxt...')
  await wrapSpawn('npm', ['start'], (child, resolve) => {
    runProcess = child
    child.stdout.on('data', data => {
      // waiting for 'Listening on: http://localhost:3000'
      if (_.includes(data.toString(), basePath)) {
        resolve()
      }
    })
  })
}, 600000) // 最多等待 10 分钟

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
]

for (let path of testPaths) {
  test(path, () => testPageByPath(path))
}

let cleaningUp = false

// Close the Nuxt server
afterAll(async () => {
  cleaningUp = true
  await wrapSpawn('kill', ['-9', '' + runProcess.pid])
})

function wrapSpawn(cmd, args, callback) {
  return new Promise((resolve, reject) => {
    try {

      const child = spawn(cmd, args, {
        cwd: path.resolve(__dirname, '../..'),
        env: {
          E2E: 1,
          ...process.env,
        },
        shell: true,
      })
      child.on('error', reject)

      child.stdout.on('data', data => {
        console.log(data.toString())
      })
      child.stderr.on('data', data => {
        console.error(data.toString())
      })

      child.on('close', code => {
        if (code === 0) {
          resolve()
        } else {
          if (cleaningUp) {
            console.error(`process exited with code ${code}`)
            resolve()
          } else {
            reject(`process exited with code ${code}`)
          }
        }
      })

      if (callback) {
        callback(child, resolve, reject)
      }

    } catch (e) {
      reject(e)
    }
  })
}
