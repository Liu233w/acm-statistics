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

// redirect OHunt request (only work on crawler-api-backend)
const superagent = require('superagent')
const _ = require('lodash')

const getFunc = superagent.get
superagent.get = url => {
  if (_.startsWith(url, '/api/ohunt')) {
    return getFunc('http://ohunt' + url)
  } else {
    return getFunc(url)
  }
}
