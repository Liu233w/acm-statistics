/*
根据环境变量设置 superagent 的 proxy
必须在所有的 superagent 之前引入
 */

if (process.env.http_proxy) {

  const superagent = require('superagent')
  const request = require('superagent-proxy')(superagent)
  const mock = {
    get(url) {
      console.log('use stubbed get proxy')
      return request.get(url).proxy(process.env.http_proxy)
    },
    post(url) {
      console.log('use stubbed post proxy')
      return request.post(url).proxy(process.env.http_proxy)
    },
    '@runtimeGlobal': true,
  }

  const proxyquire = require('proxyquire')

  const {join} = require('path')
  require('fs').readdirSync(join(__dirname, '../crawlers')).forEach(file => {
    if (file.endsWith('.js')) {
      file = file.replace(/\.js$/, '')
      console.log('stubbed on', file)
      proxyquire(`../crawlers/${file}`, {superagent: mock})
    }
  })
}

