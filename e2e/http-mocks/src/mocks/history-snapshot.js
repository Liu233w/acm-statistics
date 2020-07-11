module.exports = {
  history(client) {
    return client.mockAnyResponse({
      httpRequest: {
        path: '/api/services/app/QueryHistory/GetQueryHistoriesAndSummaries',
        headers: {
          host: ['reverse-proxy'],
        },
      },
      httpResponse: {
        statusCode: 200,
        headers: {
          'Content-Type': ['application/json; charset=utf-8'],
        },
        body: JSON.stringify(historyList),
      },
    })
  },
  summary(client) {
    return client.mockAnyResponse({
      httpRequest: {
        path: '/api/services/app/QueryHistory/GetQuerySummary',
        queryStringParameters: {
          queryHistoryId: ['1'],
        },
        headers: {
          host: ['reverse-proxy'],
        },
      },
      httpResponse: {
        statusCode: 200,
        headers: {
          'Content-Type': ['application/json; charset=utf-8'],
        },
        body: JSON.stringify(summaryResponse),
      },
    })
  },
}

const historyList = {
  'result': {
    'totalCount': 60,
    'items': [{
      'historyId': 10,
      'summaryId': 10,
      'creationTime': '2020-04-10T08:00:00Z',
      'submission': 100,
      'solved': 10,
    },
    {
      'historyId': 9,
      'summaryId': 9,
      'creationTime': '2020-04-09T08:00:00Z',
      'submission': 90,
      'solved': 9,
    },
    {
      'historyId': 8,
      'summaryId': 8,
      'creationTime': '2020-04-08T08:00:00Z',
      'submission': 80,
      'solved': 8,
    },
    {
      'historyId': 7,
      'summaryId': 7,
      'creationTime': '2020-04-07T08:00:00Z',
      'submission': 70,
      'solved': 7,
    },
    {
      'historyId': 6,
      'summaryId': 6,
      'creationTime': '2020-04-06T08:00:00Z',
      'submission': 60,
      'solved': 6,
    },
    {
      'historyId': 5,
      'summaryId': 5,
      'creationTime': '2020-04-05T08:00:00Z',
      'submission': 50,
      'solved': 5,
    },
    {
      'historyId': 4,
      'summaryId': 4,
      'creationTime': '2020-04-04T08:00:00Z',
      'submission': 40,
      'solved': 4,
    },
    {
      'historyId': 3,
      'summaryId': 3,
      'creationTime': '2020-04-03T08:00:00Z',
      'submission': 30,
      'solved': 3,
    },
    {
      'historyId': 2,
      'summaryId': 2,
      'creationTime': '2020-04-02T08:00:00Z',
      'submission': 20,
      'solved': 2,
    },
    {
      'historyId': 1,
      'summaryId': 1,
      'creationTime': '2020-04-01T08:00:00Z',
      'submission': 10,
      'solved': 1,
    },
    ],
  },
  'targetUrl': null,
  'success': true,
  'error': null,
  'unAuthorizedRequest': false,
  '__abp': true,
}

const summaryResponse = {
  'result': {
    'queryHistoryId': 1,
    'generateTime': '2020-07-11T04:15:22.010446Z',
    'mainUsername': 'wwwlsmcom',
    'queryCrawlerSummaries': [{
      'crawlerName': 'vjudge',
      'submission': 1,
      'solved': 1,
      'usernames': [{
        'fromCrawlerName': null,
        'username': 'wwwlsmcom',
      }],
      'isVirtualJudge': true,
    }, {
      'crawlerName': 'codechef',
      'submission': 5,
      'solved': 1,
      'usernames': [{
        'fromCrawlerName': 'vjudge',
        'username': 'wwwlsmcom',
      }],
      'isVirtualJudge': false,
    }, {
      'crawlerName': 'leetcode_cn',
      'submission': 9,
      'solved': 3,
      'usernames': [{
        'fromCrawlerName': null,
        'username': 'wwwlsmcom',
      }],
      'isVirtualJudge': false,
    }, {
      'crawlerName': 'timus',
      'submission': 6,
      'solved': 2,
      'usernames': [{
        'fromCrawlerName': 'vjudge',
        'username': 'wwwlsmcom',
      }],
      'isVirtualJudge': false,
    }, {
      'crawlerName': 'fzu',
      'submission': 5,
      'solved': 1,
      'usernames': [{
        'fromCrawlerName': 'vjudge',
        'username': 'wwwlsmcom',
      }],
      'isVirtualJudge': false,
    }, {
      'crawlerName': 'uvalive',
      'submission': 1,
      'solved': 1,
      'usernames': [{
        'fromCrawlerName': 'vjudge',
        'username': 'wwwlsmcom',
      }],
      'isVirtualJudge': false,
    }, {
      'crawlerName': 'uva',
      'submission': 28,
      'solved': 11,
      'usernames': [{
        'fromCrawlerName': 'vjudge',
        'username': 'wwwlsmcom',
      }],
      'isVirtualJudge': false,
    }, {
      'crawlerName': 'codeforces',
      'submission': 15,
      'solved': 6,
      'usernames': [{
        'fromCrawlerName': 'vjudge',
        'username': 'wwwlsmcom',
      }],
      'isVirtualJudge': false,
    }, {
      'crawlerName': 'dashiye',
      'submission': 5,
      'solved': 0,
      'usernames': [{
        'fromCrawlerName': 'vjudge',
        'username': 'wwwlsmcom',
      }],
      'isVirtualJudge': false,
    }, {
      'crawlerName': 'hdu',
      'submission': 385,
      'solved': 108,
      'usernames': [{
        'fromCrawlerName': 'vjudge',
        'username': 'wwwlsmcom',
      }, {
        'fromCrawlerName': null,
        'username': 'wwwlsmcom',
      }],
      'isVirtualJudge': false,
    }, {
      'crawlerName': 'poj',
      'submission': 325,
      'solved': 59,
      'usernames': [{
        'fromCrawlerName': 'vjudge',
        'username': 'wwwlsmcom',
      }],
      'isVirtualJudge': false,
    }],
    'summaryWarnings': [{
      'crawlerName': 'leetcode_cn',
      'content': 'This crawler does not have a solved list and its result will be directly added to summary.',
    }],
    'submission': 785,
    'solved': 193,
  },
  'targetUrl': null,
  'success': true,
  'error': null,
  'unAuthorizedRequest': false,
  '__abp': true,
}