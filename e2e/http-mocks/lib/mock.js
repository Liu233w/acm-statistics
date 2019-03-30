const mockServerClient = require('mockserver-client').mockServerClient

const client = mockServerClient('mock-proxy', 1080)

/**
 * 将 client 传入 func，将结果 promise 输出
 * @param func 接收 mockServerClient, 返回 Promise
 */
module.exports = func => {
  ++promiseCount
  const promise = func(client)
  promise
    .then(() => {
      console.log('expectation created')
      tryExit()
    }, err => {
      console.error(err)
      tryExit()
    })
}

let promiseCount = 0

function tryExit() {
  if (--promiseCount === 0) {
    process.exit()
  }
}
