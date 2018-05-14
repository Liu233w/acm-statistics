/* eslint-disable no-undef */

import statisticsLayoutBuilder from '~/components/statisticsLayoutBuilder'

describe('statisticsLayoutBuilder', () => {
  it('能够正确获得 worker 信息 1', () => {
    const result = statisticsLayoutBuilder([
      {crawlerName: 'c1'},
      {crawlerName: 'c1'},
      {crawlerName: 'c2'},
      {crawlerName: 'c3'},
      {crawlerName: 'c3'},
      {crawlerName: 'c3'},
      {crawlerName: 'c4'},
      {crawlerName: 'c5'},
    ], 3)

    expect(result).toMatchObject([
      [
        {
          index: 0,
          workerIdxOfCrawler: 1,
          key: 'c1:1',
        },
        {
          index: 1,
          workerIdxOfCrawler: 2,
          key: 'c1:2',
        },
        {
          index: 2,
          workerIdxOfCrawler: 1,
          key: 'c2:1',
        },
      ],
      [
        {
          index: 3,
          workerIdxOfCrawler: 1,
          key: 'c3:1',
        },
        {
          index: 4,
          workerIdxOfCrawler: 2,
          key: 'c3:2',
        },
        {
          index: 5,
          workerIdxOfCrawler: 3,
          key: 'c3:3',
        },
      ],
      [
        {
          index: 6,
          workerIdxOfCrawler: 1,
          key: 'c4:1',
        },
        {
          index: 7,
          workerIdxOfCrawler: 1,
          key: 'c5:1',
        },
      ],
    ])
  })

  it('能够正确获得 worker 信息 2', () => {
    const result = statisticsLayoutBuilder([
      {crawlerName: 'c1'},
      {crawlerName: 'c1'},
      {crawlerName: 'c2'},
      {crawlerName: 'c3'},
      {crawlerName: 'c3'},
      {crawlerName: 'c3'},
      {crawlerName: 'c4'},
      {crawlerName: 'c5'},
      {crawlerName: 'c6'},
      {crawlerName: 'c7'},
      {crawlerName: 'c8'},
      {crawlerName: 'c9'},
      {crawlerName: 'c10'},
    ], 4)

    expect(result).toMatchObject([
      [
        {
          index: 0,
          workerIdxOfCrawler: 1,
          key: 'c1:1',
        },
        {
          index: 1,
          workerIdxOfCrawler: 2,
          key: 'c1:2',
        },
        {
          index: 2,
          workerIdxOfCrawler: 1,
          key: 'c2:1',
        },
        {
          index: 3,
          workerIdxOfCrawler: 1,
          key: 'c3:1',
        },
      ],
      [
        {
          index: 4,
          workerIdxOfCrawler: 2,
          key: 'c3:2',
        },
        {
          index: 5,
          workerIdxOfCrawler: 3,
          key: 'c3:3',
        },
        {
          index: 6,
          workerIdxOfCrawler: 1,
          key: 'c4:1',
        },
        {
          index: 7,
          workerIdxOfCrawler: 1,
          key: 'c5:1',
        },
      ],
      [
        {
          index: 8,
          workerIdxOfCrawler: 1,
          key: 'c6:1',
        },
        {
          index: 9,
          workerIdxOfCrawler: 1,
          key: 'c7:1',
        },
        {
          index: 10,
          workerIdxOfCrawler: 1,
          key: 'c8:1',
        },
        {
          index: 11,
          workerIdxOfCrawler: 1,
          key: 'c9:1',
        },
      ],
      [
        {
          index: 12,
          workerIdxOfCrawler: 1,
          key: 'c10:1',
        },
      ],
    ])
  })

  it('能够正确获得 worker 信息 3', () => {
    const result = statisticsLayoutBuilder([
      {crawlerName: 'c1'},
      {crawlerName: 'c1'},
      {crawlerName: 'c2'},
      {crawlerName: 'c3'},
      {crawlerName: 'c3'},
      {crawlerName: 'c3'},
    ], 3)

    expect(result).toMatchObject([
      [
        {
          index: 0,
          workerIdxOfCrawler: 1,
          key: 'c1:1',
        },
        {
          index: 1,
          workerIdxOfCrawler: 2,
          key: 'c1:2',
        },
      ],
      [
        {
          index: 2,
          workerIdxOfCrawler: 1,
          key: 'c2:1',
        },
        {
          index: 3,
          workerIdxOfCrawler: 1,
          key: 'c3:1',
        },
      ],
      [
        {
          index: 4,
          workerIdxOfCrawler: 2,
          key: 'c3:2',
        },
        {
          index: 5,
          workerIdxOfCrawler: 3,
          key: 'c3:3',
        },
      ],
    ])
  })
})
