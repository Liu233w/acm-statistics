const fs = require('fs')
const join = require('path').join

module.exports = {
  poj: {
    backend_ok(client) {
      return client.mockAnyResponse({
        httpRequest: {
          path: '/userstatus',
          queryStringParameters: {
            user_id: ['vjudge5'],
          },
          headers: {
            host: ['poj.org.*'],
          },
        },
        httpResponse: {
          statusCode: 200,
          headers: {
            'Content-Type': ['text/html; charset=utf-8'],
          },
          body: pojResponse,
        },
      })
    },
    not_exist(client) {
      return client.mockAnyResponse({
        httpRequest: {
          path: '/userstatus',
          queryStringParameters: {
            user_id: ['Frkfe932fbcv09b'],
          },
          headers: {
            host: ['poj.org.*'],
          },
        },
        httpResponse: {
          statusCode: 200,
          headers: {
            'Content-Type': ['text/html; charset=utf-8'],
          },
          body: pojNotExistResponse,
        },
      })
    },
  },
  hdu: {
    ok(client) {
      return client.mockAnyResponse({
        httpRequest: {
          path: '/userstatus.php',
          queryStringParameters: {
            user: ['wwwlsmcom'],
          },
          headers: {
            host: ['acm.hdu.edu.cn'],
          },
        },
        httpResponse: {
          statusCode: 200,
          headers: {
            'Content-Type': ['text/html; charset=utf-8'],
          },
          body: hduOkResponse,
        },
      })
    },
  },
  leetcode: {
    ok(client) {
      return client.mockAnyResponse({
        httpRequest: {
          path: '/graphql',
          method: 'post',
          headers: {
            host: ['leetcode-cn.com'],
          },
        },
        httpResponse: {
          statusCode: 200,
          headers: {
            'Content-Type': ['application/json; charset=utf-8'],
          },
          body: leetcodeOkResponse,
        },
      })
    },
  },
}

/*
SOLVED: 1968
SUBMISSIONS: 277562
 */
const pojResponse = loadResponse('poj_ok.txt')
const pojNotExistResponse = loadResponse('poj_notExist.txt')

const hduOkResponse = loadResponse('hdu_ok.txt')
const leetcodeOkResponse = loadResponse('leetcode_ok.json')

function loadResponse(name) {
  return fs.readSync(join(__dirname , 'responses', name))
}