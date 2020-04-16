import statisticsLayoutBuilder from '~/components/statisticsLayoutBuilder'

describe('statisticsLayoutBuilder', () => {
  it('can get worker information 1', () => {
    const result = statisticsLayoutBuilder([
      {crawlerName: 'c1', key: 0.1},
      {crawlerName: 'c1', key: 0.2},
      {crawlerName: 'c2', key: 0.3},
      {crawlerName: 'c3', key: 0.4},
      {crawlerName: 'c3', key: 0.5},
      {crawlerName: 'c3', key: 0.6},
      {crawlerName: 'c4', key: 0.7},
      {crawlerName: 'c5', key: 0.8},
    ], 3)

    expect(result).toMatchObject([
      [
        {
          index: 0,
          workerIdxOfCrawler: 1,
          crawlerName: 'c1',
          key: 0.1,
        },
        {
          index: 1,
          workerIdxOfCrawler: 2,
          crawlerName: 'c1',
          key: 0.2,
        },
        {
          index: 2,
          workerIdxOfCrawler: 1,
          crawlerName: 'c2',
          key: 0.3,
        },
      ],
      [
        {
          index: 3,
          workerIdxOfCrawler: 1,
          crawlerName: 'c3',
          key: 0.4,
        },
        {
          index: 4,
          workerIdxOfCrawler: 2,
          crawlerName: 'c3',
          key: 0.5,
        },
        {
          index: 5,
          workerIdxOfCrawler: 3,
          crawlerName: 'c3',
          key: 0.6,
        },
      ],
      [
        {
          index: 6,
          workerIdxOfCrawler: 1,
          crawlerName: 'c4',
          key: 0.7,
        },
        {
          index: 7,
          workerIdxOfCrawler: 1,
          crawlerName: 'c5',
          key: 0.8,
        },
      ],
    ])
  })

  it('can get worker information 2', () => {
    const result = statisticsLayoutBuilder([
      {crawlerName: 'c1', key: 0.01},
      {crawlerName: 'c1', key: 0.02},
      {crawlerName: 'c2', key: 0.03},
      {crawlerName: 'c3', key: 0.04},
      {crawlerName: 'c3', key: 0.05},
      {crawlerName: 'c3', key: 0.06},
      {crawlerName: 'c4', key: 0.07},
      {crawlerName: 'c5', key: 0.08},
      {crawlerName: 'c6', key: 0.09},
      {crawlerName: 'c7', key: 0.10},
      {crawlerName: 'c8', key: 0.11},
      {crawlerName: 'c9', key: 0.12},
      {crawlerName: 'c10', key: 0.13},
    ], 4)

    expect(result).toMatchObject([
      [
        {
          index: 0,
          workerIdxOfCrawler: 1,
          crawlerName: 'c1',
          key: expect.any(Number),
        },
        {
          index: 1,
          workerIdxOfCrawler: 2,
          crawlerName: 'c1',
          key: expect.any(Number),
        },
        {
          index: 2,
          workerIdxOfCrawler: 1,
          crawlerName: 'c2',
          key: expect.any(Number),
        },
        {
          index: 3,
          workerIdxOfCrawler: 1,
          crawlerName: 'c3',
          key: expect.any(Number),
        },
      ],
      [
        {
          index: 4,
          workerIdxOfCrawler: 2,
          crawlerName: 'c3',
          key: expect.any(Number),
        },
        {
          index: 5,
          workerIdxOfCrawler: 3,
          crawlerName: 'c3',
          key: expect.any(Number),
        },
        {
          index: 6,
          workerIdxOfCrawler: 1,
          crawlerName: 'c4',
          key: expect.any(Number),
        },
        {
          index: 7,
          workerIdxOfCrawler: 1,
          crawlerName: 'c5',
          key: expect.any(Number),
        },
      ],
      [
        {
          index: 8,
          workerIdxOfCrawler: 1,
          crawlerName: 'c6',
          key: expect.any(Number),
        },
        {
          index: 9,
          workerIdxOfCrawler: 1,
          crawlerName: 'c7',
          key: expect.any(Number),
        },
        {
          index: 10,
          workerIdxOfCrawler: 1,
          crawlerName: 'c8',
          key: expect.any(Number),
        },
        {
          index: 11,
          workerIdxOfCrawler: 1,
          crawlerName: 'c9',
          key: expect.any(Number),
        },
      ],
      [
        {
          index: 12,
          workerIdxOfCrawler: 1,
          crawlerName: 'c10',
          key: expect.any(Number),
        },
      ],
    ])
  })

  it('can get worker information 3', () => {
    const result = statisticsLayoutBuilder([
      {crawlerName: 'c1', key: 0.1},
      {crawlerName: 'c1', key: 0.1},
      {crawlerName: 'c2', key: 0.1},
      {crawlerName: 'c3', key: 0.1},
      {crawlerName: 'c3', key: 0.1},
      {crawlerName: 'c3', key: 0.1},
    ], 3)

    expect(result).toMatchObject([
      [
        {
          index: 0,
          workerIdxOfCrawler: 1,
          crawlerName: 'c1',
          key: expect.any(Number),
        },
        {
          index: 1,
          workerIdxOfCrawler: 2,
          crawlerName: 'c1',
          key: expect.any(Number),
        },
      ],
      [
        {
          index: 2,
          workerIdxOfCrawler: 1,
          crawlerName: 'c2',
          key: expect.any(Number),
        },
        {
          index: 3,
          workerIdxOfCrawler: 1,
          crawlerName: 'c3',
          key: expect.any(Number),
        },
      ],
      [
        {
          index: 4,
          workerIdxOfCrawler: 2,
          crawlerName: 'c3',
          key: expect.any(Number),
        },
        {
          index: 5,
          workerIdxOfCrawler: 3,
          crawlerName: 'c3',
          key: expect.any(Number),
        },
      ],
    ])
  })
})
