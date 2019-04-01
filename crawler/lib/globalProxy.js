/*
根据环境变量设置 superagent 的 proxy
必须在所有的 superagent 之前引入
 */

if (process.env.http_proxy) {

  const superagent = require('superagent')
  const request = require('superagent-proxy')(superagent)

  const OrigRequest = request.Request
  superagent.Request = function RequestWithAgent(method, url) {
    console.log(`use stubbed ${method} proxy in url: ${url}`)
    const req = new OrigRequest(method, url)
    return req.proxy(process.env.http_proxy)
  }
}

