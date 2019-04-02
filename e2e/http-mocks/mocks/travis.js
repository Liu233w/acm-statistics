module.exports = {
  mock_font_awesome(client) {
    return client.mockAnyResponse({
      httpRequest: {
        path: '/font-awesome',
        headers: {
          host: ['cdn.bootcss.com'],
        },
      },
      httpForward: {
        'host': 'stackpath.bootstrapcdn.com',
        'port': 80,
        'scheme': 'HTTP',
      },
      times: {
        unlimited: true,
      },
    })
  },
  mock_font_cdn(client) {
    return client.mockAnyResponse({
      httpRequest: {
        headers: {
          host: ['fonts.loli.net'],
        },
      },
      httpForward: {
        'host': 'fonts.googleapis.com',
        'port': 80,
        'scheme': 'HTTP',
      },
      times: {
        unlimited: true,
      },
    })
  },
}