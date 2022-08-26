const http = require('http')
const { join } = require('path')
const wait = require('wait-on')
const mock = require('./lib/mock')

const mocks = {}
require('fs').readdirSync(join(__dirname, 'mocks')).forEach(file => {
  if (file.endsWith('.js')) {
    file = file.replace(/\.js$/, '')
    mocks[file] = require('./mocks/' + file)
  }
})

console.log('mock list', mocks)

const server = http.createServer(async (req, res) => {

  try {
    const func = findInPath(mocks, req.url)
    if (!func) {
      throw new Error(`mock ${req.url} is not in the list`)
    }

    const response = await mock(func)
    console.log(response)

    res.writeHead(200, { 'Content-Type': 'text/text' })
    res.write(`mock ${req.url} successfully created`)
    res.end()

  } catch (e) {
    console.error(e)

    res.writeHead(500, { 'Content-Type': 'text/text' })
    res.write(`mock ${req.url} yield following error message: ${e.message}`)
    res.end()
  }
})

/**
 * 对于 /a/c/d 这样的 path，找到 obj 中的 obj.a.c.d，否则返回 null
 * @param obj
 * @param path {string} like /a/c/d
 * @return {Object|null}
 */
function findInPath(obj, path) {

  const findIn = path.substr(1).split('/')
  if (findIn[0] === 'mock-configurer') {
    findIn.shift()
  }
  console.log('find in array', findIn)

  let current = obj

  for (let item of findIn) {
    if (!current[item]) return null
    current = current[item]
  }

  return current
}

console.log('waiting for proxy server to be available...')

wait({
  resources: [
    'tcp:mock-proxy:1080',
  ],
  timeout: 60000,
}).then(() => {

  console.log('proxy server started, now starting configurer server')

  mock(client => client.reset())
    .then(() => console.log('All expectations reset'))
    .catch(console.error)

  server.listen(80)
})
