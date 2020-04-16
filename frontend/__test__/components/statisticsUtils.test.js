import {warningHelper, mapVirtualJudgeProblemTitle} from '~/components/statisticsUtils'

describe('warningHelper', () => {
  it('does not generate warning when it is correct', () => {
    const worker = {
      username: 'user1',
      status: 'WORKING',
      submissions: 0,
      solved: 0,
      errorMessage: '.....',
      tokenKey: 0.23333,
      crawlerName: 'cr1',
      key: 0.66666666,
    }
    expect(warningHelper(worker, {}, {})).toMatchObject([])

    worker.username = ''
    expect(warningHelper(worker, {}, {})).toMatchObject([])
  })

  it('can generate warning with username that beginning with space', () => {
    const worker = {
      username: ' user1',
      status: 'WORKING',
      submissions: 0,
      solved: 0,
      errorMessage: '.....',
      tokenKey: 0.23333,
      crawlerName: 'cr1',
      key: 0.66666666,
    }

    expect(warningHelper(worker, {}, {})).toMatchObject([
      'Your username begins with a space.',
      'Your username includes space, which may not be supported by some crawlers.',
    ])
  })

  it('can generate warning that username contains space', () => {
    const worker = {
      username: 'user 1',
      status: 'WORKING',
      submissions: 0,
      solved: 0,
      errorMessage: '.....',
      tokenKey: 0.23333,
      crawlerName: 'cr1',
      key: 0.66666666,
    }

    expect(warningHelper(worker, {}, {})).toMatchObject([
      'Your username includes space, which may not be supported by some crawlers.',
    ])
  })

  it('can warn that there are collapses with ac list of vjudge and other workers', () => {

    const worker = {
      solvedList: [
        'A-001',
        'B-001',
        'B-002',
        'C-001',
      ],
      solved: 4,
      crawlerName: 'vjudge',
    }
    const crawlerMeta = {
      virtual_judge: true,
    }
    const getters = {
      nullSolvedListCrawlers: {
        'A': 'a title',
        'B': 'b title',
      },
    }

    const res = warningHelper(worker, crawlerMeta, getters)

    expect(res).toMatchObject([
      'Crawler a title did not return AC problem list, its result may overlap with this crawler\'s',
      'Crawler b title did not return AC problem list, its result may overlap with this crawler\'s',
    ])
  })

  it('can warn that multiple worker crawler can have redundant problems', () => {

    const worker = {
      solvedList: null,
      crawlerName: 'cr1',
    }
    const crawlerMeta = {}
    const getters = {
      nullSolvedListCrawlers: {
        'cr1': 'cr1 title',
      },
      workerNumberOfCrawler: {
        'cr1': 2,
      },
    }

    const res = warningHelper(worker, crawlerMeta, getters)

    expect(res).toMatchObject([
      'This crawler did not return AC problem list, so the same problem in different OJs can be recognized as different problems.',
    ])
  })

  it('can warn when solved and solvedList.length is not equal', () => {

    const worker = {
      solved: 2,
      solvedList: ['1001'],
    }

    const res = warningHelper(worker, {}, {})

    expect(res).toMatchObject([
      'The AC number of this crawler is 2, however, there are 1 problems in the AC list, which can be an error of the crawler.',
    ])
  })
})

describe('mapVirtualJudgeProblemTitle', () => {
  it('can work correctly', () => {

    const solvedList = [
      'aa-001',
      'b-002',
      'C-003',
    ]
    const crawlers = {
      aa: {
        title: 'AA',
      },
      b: {
        title: 'B',
      },
    }

    const res = mapVirtualJudgeProblemTitle(solvedList, crawlers)

    expect(res).toMatchObject([
      'AA-001',
      'B-002',
      'C-003',
    ])
  })
})
