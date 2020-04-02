const { exec } = require('child_process')
const _ = require('lodash')
const superagent = require('superagent')
const cheerio = require('cheerio')
const psTree = require('ps-tree')

const basePath = 'http://localhost:3000'

let childProcess = null

let runningOut = ''
let runningError = ''

beforeAll(done => {

  const execOption = {
    env: {
      E2E: 1,
      ...process.env,
    },
    cwd: `${__dirname}/../..`,
  }

  console.log('start building nuxt...')
  exec('npm run build', execOption, (err, stdout, stderr) => {
    if (err) {
      console.error('build process fail to start', err)
    }
    if (stdout) {
      console.log('build process out', stdout)
    }
    if (stderr) {
      console.error('build process error', stderr)
    }

    console.log('start running nuxt...')
    childProcess = exec('npm start', execOption)

    childProcess.stdout.on('data', out => {
      runningOut += out
      runningOut += '\n====================================\n'
    })
    childProcess.stderr.on('data', out => {
      runningError += out
      runningError += '\n====================================\n'
    })

    setTimeout(done, 10000)
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
  '/about',
  '/login',
  '/register',
  '/settings',
]

for (let path of testPaths) {
  test(path, async () => await testPageByPath(path))
}

afterAll(done => {
  if (!childProcess) {
    return
  }

  console.log(`trying to kill process ${childProcess.pid}`)
  if (/^win/.test(process.platform)) {
    exec(`taskkill /PID ${childProcess.pid} /T /F`, afterCallback)
  } else {
    treeKill(childProcess.pid, null, afterCallback)
  }

  function afterCallback() {
    if (runningOut) {
      console.log(`running process out:\n${runningOut}`)
    }
    if (runningError) {
      console.log(`running process error:\n${runningError}`)
    }
    done()
  }
})

// from http://krasimirtsonev.com/blog/article/Nodejs-managing-child-processes-starting-stopping-exec-spawn
function treeKill(pid, signal, callback) {
  signal = signal || 'SIGKILL'
  psTree(pid, (err, children) => {
    try {
      if (err) throw err
      process.kill(pid, signal)
      _.forEach(children, p => {
        process.kill(p.PID, signal)
      })
      callback && callback()
    } catch (e) {
      callback && callback(e)
    }
  })
}
