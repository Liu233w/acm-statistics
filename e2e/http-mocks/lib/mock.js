const mockServerClient = require('mockserver-client').mockServerClient

const client = mockServerClient('mock-proxy', 1080)

/**
 * 将 client 传入 func，将结果 promise 输出
 * @param func 接收 mockServerClient, 返回运行它之后的返回值
 * @return {Promise}
 */
module.exports = func => {
  const promise = func(client)
  return new Promise((resolve, reject) => promise.then(resolve, reject))
}
