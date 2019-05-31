// 屏蔽对腾讯分析的请求，防止分析不准确
module.exports = client =>
  client.mockAnyResponse({
    httpRequest: {
      headers: {
        host: ['tajs.qq.com'],
      },
    },
    httpResponse: {
      statusCode: 404,
    },
    timeToLive:{
      unlimited: true,
    },
    times: {
      unlimited: true,
    },
  })
