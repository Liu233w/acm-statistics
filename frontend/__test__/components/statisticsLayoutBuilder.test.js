import statisticsLayoutBuilder from '~/components/statisticsLayoutBuilder'

describe('statisticsLayoutBuilder', () => {
  it('can get worker information 1', () => {
    const result = statisticsLayoutBuilder([
      { crawlerName: 'c1', key: 0.1 },
      { crawlerName: 'c1', key: 0.2 },
      { crawlerName: 'c2', key: 0.3 },
      { crawlerName: 'c3', key: 0.4 },
      { crawlerName: 'c3', key: 0.5 },
      { crawlerName: 'c3', key: 0.6 },
      { crawlerName: 'c4', key: 0.7 },
      { crawlerName: 'c5', key: 0.8 },
    ], 3)

    expect(result).toMatchObject([
      [0, 1, 2],
      [3, 4, 5],
      [6, 7],
    ])
  })

  it('can get worker information 2', () => {
    const result = statisticsLayoutBuilder([
      { crawlerName: 'c1', key: 0.01 },
      { crawlerName: 'c1', key: 0.02 },
      { crawlerName: 'c2', key: 0.03 },
      { crawlerName: 'c3', key: 0.04 },
      { crawlerName: 'c3', key: 0.05 },
      { crawlerName: 'c3', key: 0.06 },
      { crawlerName: 'c4', key: 0.07 },
      { crawlerName: 'c5', key: 0.08 },
      { crawlerName: 'c6', key: 0.09 },
      { crawlerName: 'c7', key: 0.10 },
      { crawlerName: 'c8', key: 0.11 },
      { crawlerName: 'c9', key: 0.12 },
      { crawlerName: 'c10', key: 0.13 },
    ], 4)

    expect(result).toMatchObject([
      [0, 1, 2, 3],
      [4, 5, 6, 7],
      [8, 9, 10, 11],
      [12],
    ])
  })

  it('can get worker information 3', () => {
    const result = statisticsLayoutBuilder([
      { crawlerName: 'c1', key: 0.1 },
      { crawlerName: 'c1', key: 0.1 },
      { crawlerName: 'c2', key: 0.1 },
      { crawlerName: 'c3', key: 0.1 },
      { crawlerName: 'c3', key: 0.1 },
      { crawlerName: 'c3', key: 0.1 },
    ], 3)

    expect(result).toMatchObject([
      [0, 1],
      [2, 3],
      [4, 5],
    ])
  })
})
