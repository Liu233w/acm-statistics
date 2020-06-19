// block google analysis
module.exports = client =>
  client.mockAnyResponse({
    httpRequest: {
      headers: {
        host: ['www.googletagmanager.com'],
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
