const http = require('http')
const mock = require('./lib/mock')
const preActivation = require('./preActivation')

mock(client => client.reset())
  .then(() => console.log('All expectations reset'))
  .catch(console.error)

const mocks = {}
// 这个只会从 docker 运行，运行环境永远是一致的，就不管相对路径了。
require('fs').readdirSync('./mocks').forEach(file => {
  if (file.endsWith('.js')) {
    file = file.replace(/\.js$/, '')
    mocks[file] = require('./mocks/' + file)
  }
})

console.log('mock list', mocks)

// 激活预先的mock
preActivation(mocks)

http.createServer(async (req, res) => {

  try {
    const func = findInPath(mocks, req.url)
    if (!func) {
      throw new Error(`mock ${req.url} is not in the list`)
    }

    const response = await mock(func)
    console.log(response)

    res.writeHead(200, {'Content-Type': 'text/text'})
    res.write(`mock ${req.url} successfully created`)
    res.end()

  } catch (e) {
    console.error(e)

    res.writeHead(500, {'Content-Type': 'text/text'})
    res.write(`mock ${req.url} yield following error message: ${e.message}`)
    res.end()
  }
}).listen(80)

/**
 * 对于 /a/c/d 这样的 path，找到 obj 中的 obj.a.c.d，否则返回 null
 * @param obj
 * @param path {string} like /a/c/d
 * @return {Object|null}
 */
function findInPath(obj, path) {

  const findIn = path.substr(1).split('/')
  console.log('find in array', findIn)

  let current = obj

  for (let item of findIn) {
    if (!current[item]) return null
    current = current[item]
  }

  return current
}
