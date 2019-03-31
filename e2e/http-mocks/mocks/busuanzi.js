// 让 busuanzi 每次都返回一样的数据
module.exports = client =>
  client.mockWithCallback({
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
      body: `try{${req.queryStringParameters.jsonpCallback[0]}({"site_uv":3492,"page_pv":2770,"version":2.4,"site_pv":7164});}catch(e){}`,
    }
  })
