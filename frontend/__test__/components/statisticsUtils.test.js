import {warningHelper, mapVirtualJudgeProblemTitle} from '~/components/statisticsUtils'

describe('warningHelper', () => {
  it('在正确时不会生成警告', () => {
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

  it('能够生成用户名以空格开头的警告', () => {
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
      '用户名以空格开头',
      '用户名含有空格，部分爬虫可能不支持',
    ])
  })

  it('能够生成用户名带有空格的警告', () => {
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
      '用户名含有空格，部分爬虫可能不支持',
    ])
  })

  it('能够警告题目列表可能有重复', () => {

    const worker = {
      solvedList: [
        'A-001',
        'B-001',
        'C-001',
      ],
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
      '爬虫 a title 无法返回题目列表，因此它的结果和本爬虫的结果可能会有重复',
      '爬虫 b title 无法返回题目列表，因此它的结果和本爬虫的结果可能会有重复',
    ])
  })
})

describe('mapVirtualJudgeProblemTitle', () => {
  it('能够正常运行', () => {

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
