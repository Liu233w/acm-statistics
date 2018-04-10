exports.ensureConfigAndRead = async () => {
  return {
    crawlers: [
      {
        name: 'crawler1',
        meta: {
          title: 'Crawler1',
          description: 'Description1',
          url: 'http://www.c1.com',
        },
      },
      {
        name: 'crawler2',
        meta: {
          title: 'Crawler2',
        },
        custom_data: 'CustomData',
      },
      {
        name: 'crawler_for_server',
        meta: {
          title: 'CrawlerForServer',
        },
        server_only: true,
      },
    ],
  }
}

exports.readMetaConfigs = async () => {
  return {
    crawler1: {
      title: 'Crawler1',
      description: 'Description1',
      url: 'http://www.c1.com',
    },
    crawler2: {
      title: 'Crawler2',
    },
    crawler_for_server: {
      title: 'CrawlerForServer',
    },
  }
}