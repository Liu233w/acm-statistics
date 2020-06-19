// block google ads
module.exports = client =>
  client.mockAnyResponse({
    httpRequest: {
      headers: {
        host: ['pagead2.googlesyndication.com'],
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
