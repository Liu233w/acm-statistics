const mock = require('./lib/mock')

// 让 busuanzi 每次都返回一样的数据
mock(client => client.mockWithCallback({
    path: '/busuanzi',
    queryStringParameters: {
      jsonpCallback: ['.*'],
    },
    headers: {
      host: ['busuanzi.ibruce.info'],
    },
  }, function (req) {
    return {
      statusCode: 200,
      // 那个网站就是这么返回的
      'content-type': 'application/json',
      body: `try{${req.queryStringParameters.jsonpCallback[0]}({"site_uv":3492,"page_pv":2770,"version":2.4,"site_pv":7164});}catch(e){}`,
    }
  }),
)
