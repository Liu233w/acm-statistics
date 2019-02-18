import StoreContextSimulator from '../StoreContextSimulator'
import {MUTATION_TYPES} from '../../store/-dynamic/statistics'

jest.mock('~/dynamic/crawlers', () => function () {
  return {
    metas: {
      cr1: {
        title: 'crawler 1',
      },
      cr2: {
        title: 'crawler 2, should throw exception',
      },
      cr3: {
        title: 'crawler 3, return at next tick',
      },
    },
    crawlers: {
      // eslint-disable-next-line no-unused-vars
      cr1: username => Promise.resolve({submissions: 10, solved: 5, solvedList: ['1001', '1002']}),
      // eslint-disable-next-line no-unused-vars
      cr2: (username) => Promise.reject(new Error('用户不存在')),
      // eslint-disable-next-line no-unused-vars
      cr3: (username) => {
        return new Promise(resolve =>
          setImmediate(() => resolve({
            submissions: 10,
            solved: 1,
            solvedList: null,
          })))
      },
    },
  }
}, {virtual: true})

const store = require('../../store/-dynamic/statistics')

describe('state', () => {
  it('能够正确生成 state', () => {
    const state = store.state()
    expect(state).toMatchObject({
      mainUsername: '',
      crawlers: {
        cr1: {
          name: 'cr1',
          title: 'crawler 1',
        },
        cr2: {
          title: 'crawler 2, should throw exception',
        },
        cr3: {
          title: 'crawler 3, return at next tick',
        },
      },
      workers: [
        {
          crawlerName: 'cr1',
          username: '',
          solved: 0,
          submissions: 0,
          solvedList: [],
          errorMessage: '',
          tokenKey: null,
          key: expect.any(Number),
        },
        {
          crawlerName: 'cr2',
          username: '',
          solved: 0,
          submissions: 0,
          solvedList: [],
          errorMessage: '',
          tokenKey: null,
          key: expect.any(Number),
        },
        {
          crawlerName: 'cr3',
          username: '',
          solved: 0,
          submissions: 0,
          solvedList: [],
          errorMessage: '',
          tokenKey: null,
          key: expect.any(Number),
        },
      ],
    })
  })
})

describe('mutations', () => {

  describe('updateUsername', () => {
    it('在更改用户名时能够重设其他信息', () => {
      const state = {
        workers: [
          {
            username: 'user1',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            solvedList: ['1', '2'],
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr1',
            key: 0.6666666,
          },
        ],
      }

      store.mutations.updateUsername(state, {index: 0, username: 'user2'})

      expect(state).toMatchObject({
        workers: [
          {
            username: 'user2',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            solvedList: [],
            errorMessage: '',
            tokenKey: null,
            crawlerName: 'cr1',
            key: 0.6666666,
          },
        ],
      })
    })

    it('在用户名为 null 时能够重设为空字符串', () => {
      const state = {
        workers: [
          {
            username: 'user1',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr1',
            key: 0.6666666,
          },
        ],
      }

      store.mutations.updateUsername(state, {index: 0, username: null})

      expect(state).toMatchObject({
        workers: [
          {
            username: '',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            solvedList: [],
            errorMessage: '',
            tokenKey: null,
            crawlerName: 'cr1',
            key: 0.6666666,
          },
        ],
      })
    })
  })

  describe('updateMainUsername', () => {
    it('能够同时每个worker的用户名', () => {
      const state = {
        mainUsername: 'mainUser',
        workers: [
          {
            username: 'user1',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            solvedList: ['1', '2'],
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr1',
            key: 0.6666666,
          },
          {
            username: 'user2',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            solvedList: ['1', '2'],
            tokenKey: 0.23333,
            crawlerName: 'cr2',
            key: 0.7777777,
          },
        ],
      }

      store.mutations.updateMainUsername(state, {username: 'main2'})

      expect(state).toMatchObject({
        mainUsername: 'main2',
        workers: [
          {
            username: 'main2',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            solvedList: [],
            tokenKey: null,
            crawlerName: 'cr1',
            key: 0.6666666,
          },
          {
            username: 'main2',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            solvedList: [],
            tokenKey: null,
            crawlerName: 'cr2',
            key: 0.7777777,
          },
        ],
      })
    })
  })

  describe('updateUsernamesFromObject', () => {

    let startState

    beforeEach(() => {
      startState = {
        mainUsername: 'mainUser',
        crawlers: {
          cr1: {
            name: 'cr1',
          },
          cr2: {
            name: 'cr2',
          },
        },
        workers: [
          {
            username: 'user1',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            solvedList: ['1', '2'],
            tokenKey: 0.23333,
            crawlerName: 'cr1',
            key: 0.6666666,
          },
          {
            username: 'user2',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            solvedList: ['1', '2'],
            tokenKey: 0.23333,
            crawlerName: 'cr1',
            key: 0.6666666,
          },
          {
            username: 'user2',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            solvedList: ['1', '2'],
            tokenKey: 0.23333,
            crawlerName: 'cr2',
            key: 0.6666666,
          },
        ],
      }
    })

    it('能够正确更新数据', () => {
      store.mutations.updateUsernamesFromObject(startState, {
        main: 'main2',
        subs: {
          cr1: ['nu1', 'nu2'],
          cr2: ['nu3'],
        },
      })

      expect(startState).toMatchObject({
        mainUsername: 'main2',
        workers: [
          {
            username: 'nu1',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            solvedList: [],
            tokenKey: null,
            crawlerName: 'cr1',
            key: expect.any(Number),
          },
          {
            username: 'nu2',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            solvedList: [],
            tokenKey: null,
            crawlerName: 'cr1',
            key: expect.any(Number),
          },
          {
            username: 'nu3',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            solvedList: [],
            tokenKey: null,
            crawlerName: 'cr2',
            key: expect.any(Number),
          },
        ],
      })
    })

    it('在某个 worker 不存在时能够将多出来的设置为 mainUsername', () => {
      store.mutations.updateUsernamesFromObject(startState, {
        main: 'main2',
        subs: {
          cr2: ['nu3'],
        },
      })

      expect(startState).toMatchObject({
        mainUsername: 'main2',
        workers: [
          {
            username: 'main2',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            solvedList: [],
            tokenKey: null,
            crawlerName: 'cr1',
            key: expect.any(Number),
          },
          {
            username: 'nu3',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            solvedList: [],
            tokenKey: null,
            crawlerName: 'cr2',
            key: expect.any(Number),
          },
        ],
      })
    })

    it('在某个 crawler 不存在时不会添加多余的worker', () => {
      store.mutations.updateUsernamesFromObject(startState, {
        main: 'main2',
        subs: {
          cr1: ['nu1'],
          cr2: ['nu3'],
          cr3: ['nnnn'],
        },
      })

      expect(startState).toMatchObject({
        mainUsername: 'main2',
        workers: [
          {
            username: 'nu1',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            solvedList: [],
            tokenKey: null,
            crawlerName: 'cr1',
            key: expect.any(Number),
          },
          {
            username: 'nu3',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            solvedList: [],
            tokenKey: null,
            crawlerName: 'cr2',
            key: expect.any(Number),
          },
        ],
      })
    })

    it('在某些用户名为空时能够正确生成 worker', () => {
      store.mutations.updateUsernamesFromObject(startState, {
        main: 'main2',
        subs: {
          cr1: ['nu1', ''],
          cr2: [''],
        },
      })

      expect(startState).toMatchObject({
        mainUsername: 'main2',
        workers: [
          {
            username: 'nu1',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            solvedList: [],
            tokenKey: null,
            crawlerName: 'cr1',
            key: expect.any(Number),
          },
          {
            username: '',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            solvedList: [],
            tokenKey: null,
            crawlerName: 'cr1',
            key: expect.any(Number),
          },
          {
            username: '',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            solvedList: [],
            tokenKey: null,
            crawlerName: 'cr2',
            key: expect.any(Number),
          },
        ],
      })
    })
  })

  describe('stopWorker', () => {
    it('应该重设tokenKey和status', () => {
      const state = {
        crawlers: {
          cr1: {
            name: 'cr1',
          },
        },
        workers: [
          {
            username: 'user1',
            status: 'WORKING',
            submissions: 0,
            solved: 0,
            solvedList: [],
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr1',
            key: 0.66666666,
          },
        ],
      }

      store.mutations.stopWorker(state, {index: 0})

      expect(state).toMatchObject({
        crawlers: {
          cr1: {
            name: 'cr1',
          },
        },
        workers: [
          {
            username: 'user1',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            solvedList: [],
            errorMessage: '.....',
            tokenKey: null,
            crawlerName: 'cr1',
            key: 0.66666666,
          },
        ],
      })
    })
  })

  describe('addWorkerForCrawler', () => {

    it('在其他的 worker 不存在时能够将 worker 添加到末尾', () => {
      const state = {
        crawlers: {
          cr1: {
            name: 'cr1',
          },
          cr2: {
            name: 'cr2',
          },
        },
        workers: [
          {
            username: 'user1',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            solvedList: ['1', '2'],
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr2',
            key: 0.66666666,
          },
        ],
      }

      store.mutations.addWorkerForCrawler(state, {crawlerName: 'cr1'})

      expect(state).toMatchObject({
        crawlers: {
          cr1: {
            name: 'cr1',
          },
          cr2: {
            name: 'cr2',
          },
        },
        workers: [
          {
            username: 'user1',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            solvedList: ['1', '2'],
            tokenKey: 0.23333,
            crawlerName: 'cr2',
            key: 0.66666666,
          },
          {
            username: '',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            solvedList: [],
            errorMessage: '',
            tokenKey: null,
            crawlerName: 'cr1',
            key: expect.any(Number),
          },
        ],
      })
    })

    it('在其他的 worker 存在时能够将 worker 添加到其后面', () => {
      const state = {
        crawlers: {
          cr1: {
            name: 'cr1',
          },
          cr2: {
            name: 'cr2',
          },
        },
        workers: [
          {
            username: 'user0',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            solvedList: ['1', '2'],
            tokenKey: 0.23333,
            crawlerName: 'cr1',
            key: 0.6666666,
          },
          {
            username: 'user1',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            solvedList: ['1', '2'],
            tokenKey: 0.23333,
            crawlerName: 'cr2',
            key: 0.6666666,
          },
        ],
      }

      store.mutations.addWorkerForCrawler(state, {crawlerName: 'cr1'})

      expect(state).toMatchObject({
        crawlers: {
          cr1: {
            name: 'cr1',
          },
          cr2: {
            name: 'cr2',
          },
        },
        workers: [
          {
            username: 'user0',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            solvedList: ['1', '2'],
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr1',
            key: 0.6666666,
          },
          {
            username: '',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            solvedList: [],
            tokenKey: null,
            crawlerName: 'cr1',
            key: expect.any(Number),
          },
          {
            username: 'user1',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            solvedList: ['1', '2'],
            tokenKey: 0.23333,
            crawlerName: 'cr2',
            key: 0.6666666,
          },
        ],
      })
    })

  })

  describe('clearWorkers', () => {
    it('能够重设 workers', () => {
      const state = {
        mainUsername: 'wwwwww',
        crawlers: {
          cr1: {
            name: 'cr1',
            title: 'crawler 1',
          },
        },
        workers: [
          {
            crawlerName: 'cr1',
            username: 'asdfg',
            solved: 0,
            submissions: 0,
            errorMessage: '',
            solvedList: [],
            tokenKey: 0.123,
            key: 0.23333,
          },
          {
            crawlerName: 'cr1',
            username: 'qwert',
            solved: 0,
            submissions: 0,
            errorMessage: '',
            solvedList: [],
            tokenKey: 0.456,
            key: 0.666,
          },
        ],
      }

      store.mutations[MUTATION_TYPES.clearWorkers](state)
      expect(state).toMatchObject({
        mainUsername: '',
        crawlers: {
          cr1: {
            name: 'cr1',
            title: 'crawler 1',
          },
        },
        workers: [{
          crawlerName: 'cr1',
          username: '',
          solved: 0,
          submissions: 0,
          solvedList: [],
          errorMessage: '',
          tokenKey: null,
          key: expect.any(Number),
        }],
      })
    })
  })
})

describe('getters', () => {

  describe('notWorkingRate', () => {

    it('能够正常运作', () => {
      const state = {
        workers: [
          {status: 'DONE'},
          {status: 'WORKING'},
          {status: 'WAITING'},
          {status: 'DONE'},
        ],
      }

      expect(store.getters.notWorkingRate(state)).toBe(75)
    })

    it('能够得到 0', () => {
      const state = {
        workers: [
          {status: 'WORKING'},
          {status: 'WORKING'},
          {status: 'WORKING'},
          {status: 'WORKING'},
        ],
      }

      expect(store.getters.notWorkingRate(state)).toBe(0)
    })

    it('能够得到 100', () => {
      const state = {
        workers: [
          {status: 'DONE'},
          {status: 'DONE'},
          {status: 'DONE'},
          {status: 'DONE'},
        ],
      }

      expect(store.getters.notWorkingRate(state)).toBe(100)
    })

  })

  describe('workerNumberOfCrawler', () => {

    it('能够获取爬虫的worker数量', () => {

      const state = {
        workers: [
          {
            crawlerName: 'cr1',
          },
          {
            crawlerName: 'cr1',
          },
          {
            crawlerName: 'cr2',
          },
          {
            crawlerName: 'cr3',
          },
        ],
      }

      expect(store.getters.workerNumberOfCrawler(state)).toMatchObject({
        cr1: 2,
        cr2: 1,
        cr3: 1,
      })

    })
  })

  describe('workerIdxOfCrawler', () => {
    it('能够获取到 worker 的序号', () => {
      const state = {
        workers: [
          {
            crawlerName: 'cr1',
          },
          {
            crawlerName: 'cr1',
          },
          {
            crawlerName: 'cr2',
          },
          {
            crawlerName: 'cr3',
          },
        ],
      }

      expect(store.getters.workerIdxOfCrawler(state)).toMatchObject([
        1,
        2,
        1,
        1,
      ])
    })
  })

  describe('nullSolvedListCrawlers', () => {
    it('能够得到正确的结果', () => {

      const state = {
        crawlers: {
          cr1: {
            title: 'cr1 title',
          },
          cr2: {
            title: 'cr2 title',
          },
          cr3: {
            title: 'cr3 title',
          },
        },
        workers: [
          {
            solvedList: null,
            crawlerName: 'cr1',
          },
          {
            solvedList: ['1', '2'],
            crawlerName: 'cr2',
          },
          {
            solvedList: null,
            crawlerName: 'cr3',
          },
        ],
      }
      const nullSolvedListWorkers = store.getters.nullSolvedListWorkers(state)

      const res = store.getters.nullSolvedListCrawlers(state, {nullSolvedListWorkers})

      expect(res).toMatchObject({
        cr1: 'cr1 title',
        cr3: 'cr3 title',
      })
    })
  })
})

describe('helper functions', () => {

  describe('getUsernameObjectFromState', () => {

    it('能够正确生成数据', () => {

      const state = {
        mainUsername: 'mainUser',
        crawlers: {
          cr1: {
            name: 'cr1',
          },
          cr2: {
            name: 'cr2',
          },
        },
        workers: [
          {
            username: 'user1',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            solvedList: ['1', '2'],
            tokenKey: 0.23333,
            crawlerName: 'cr1',
            key: 0.6666666,
          },
          {
            username: 'user2',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            solvedList: ['1', '2'],
            tokenKey: 0.23333,
            crawlerName: 'cr1',
            key: 0.6666666,
          },
          {
            username: 'user2',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            solvedList: ['1', '2'],
            tokenKey: 0.23333,
            crawlerName: 'cr2',
            key: 0.6666666,
          },
        ],
      }

      const result = store.getUsernameObjectFromState(state)
      expect(result).toMatchObject({
        main: 'mainUser',
        subs: {
          cr1: ['user1', 'user2'],
          cr2: ['user2'],
        },
      })
    })

    it('在某些 username 为空时能够正确生成', () => {

      const state = {
        mainUsername: 'mainUser',
        crawlers: {
          cr1: {
            name: 'cr1',
          },
          cr2: {
            name: 'cr2',
          },
        },
        workers: [
          {
            username: 'user1',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            solvedList: ['1', '2'],
            tokenKey: 0.23333,
            crawlerName: 'cr1',
            key: 0.6666666,
          },
          {
            username: '',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            solvedList: ['1', '2'],
            tokenKey: 0.23333,
            crawlerName: 'cr1',
            key: 0.6666666,
          },
          {
            username: '',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            solvedList: ['1', '2'],
            tokenKey: 0.23333,
            crawlerName: 'cr2',
            key: 0.6666666,
          },
        ],
      }

      const result = store.getUsernameObjectFromState(state)
      expect(result).toMatchObject({
        main: 'mainUser',
        subs: {
          cr1: ['user1', ''],
          cr2: [''],
        },
      })
    })

  })

  describe('addProblemPrefix', () => {
    it('能够得到正确的结果', () => {
      const res = store.addProblemPrefix(['1001', '1002'], 'A')
      expect(res).toMatchObject(['A-1001', 'A-1002'])
    })
  })

  describe('pushSet', () => {
    it('能够得到正确的结果', () => {
      const set = new Set([1, 2])
      store.pushSet(set, [2, 3])
      expect(set).toMatchObject(new Set([1, 2, 3]))
    })
  })
})

describe('actions', () => {

  describe('startOne', () => {

    it('能够正常运行', async () => {
      const state = {
        crawlers: {
          cr1: {
            // mock 在文件开头 ({submissions: 10, solved: 5, solvedList: ['1001','1002']}),
            name: 'cr1',
          },
        },
        workers: [
          {
            username: 'user1',
            status: 'DONE',
            submissions: 0,
            solved: 0,
            errorMessage: '.....',
            solvedList: [],
            tokenKey: 0.23333,
            crawlerName: 'cr1',
            key: 0.6666666,
          },
        ],
      }
      const actionTester = new StoreContextSimulator(state, {mutations: store.mutations})

      await store.actions.startOne({state, commit: actionTester.getCommiter()}, {index: 0})

      const history = actionTester.getCommitHistory()
      expect(history).toHaveLength(2)

      expect(state).toMatchObject({
        crawlers: {
          cr1: {
            name: 'cr1',
          },
        },
        workers: [
          {
            username: 'user1',
            status: 'DONE',
            submissions: 10,
            solved: 5,
            solvedList: ['1001', '1002'],
            errorMessage: '',
            tokenKey: expect.any(Number),
            crawlerName: 'cr1',
            key: 0.6666666,
          },
        ],
      })
    })

    it('在爬虫没有返回题目列表时能够正常运行', async () => {
      const state = {
        crawlers: {
          cr3: {
            name: 'cr3',
          },
        },
        workers: [
          {
            username: 'user1',
            status: 'DONE',
            submissions: 0,
            solved: 0,
            errorMessage: '.....',
            solvedList: [],
            tokenKey: 0.23333,
            crawlerName: 'cr3',
            key: 0.6666666,
          },
        ],
      }
      const actionTester = new StoreContextSimulator(state, {mutations: store.mutations})

      await store.actions.startOne({state, commit: actionTester.getCommiter()}, {index: 0})

      const history = actionTester.getCommitHistory()
      expect(history).toHaveLength(2)

      expect(state).toMatchObject({
        crawlers: {
          cr3: {
            name: 'cr3',
          },
        },
        workers: [
          {
            username: 'user1',
            status: 'DONE',
            submissions: 10,
            solved: 1,
            solvedList: null,
            errorMessage: '',
            tokenKey: expect.any(Number),
            crawlerName: 'cr3',
            key: 0.6666666,
          },
        ],
      })
    })

    it('在爬虫抛出异常时能正确设置状态', async () => {
      const state = {
        crawlers: {
          cr2: {
            name: 'cr2',
          },
        },
        workers: [
          {
            username: 'user1',
            status: 'DONE',
            submissions: 0,
            solved: 0,
            solvedList: [],
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr2',
            key: 0.6666666,
          },
        ],
      }
      const actionTester = new StoreContextSimulator(state, {mutations: store.mutations})

      await store.actions.startOne({state, commit: actionTester.getCommiter()}, {index: 0})

      const history = actionTester.getCommitHistory()
      expect(history).toHaveLength(2)

      expect(state).toMatchObject({
        crawlers: {
          cr2: {
            name: 'cr2',
          },
        },
        workers: [
          {
            username: 'user1',
            status: 'DONE',
            submissions: 0,
            solved: 0,
            solvedList: [],
            errorMessage: '用户不存在',
            tokenKey: expect.any(Number),
            crawlerName: 'cr2',
            key: 0.6666666,
          },
        ],
      })
    })

    it('在 tokenKey 改变时不会提交结果', async () => {

      const state = {
        crawlers: {
          cr3: {
            name: 'cr3',
          },
        },
        workers: [
          {
            username: 'user1',
            status: 'DONE',
            submissions: 0,
            solved: 0,
            solvedList: [],
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr3',
            key: 0.6666666,
          },
        ],
      }
      const actionTester = new StoreContextSimulator(state, {mutations: store.mutations})

      // 执行当前帧
      const promise = store.actions.startOne({state, commit: actionTester.getCommiter()}, {index: 0})

      // 在开始查询之前tokenKey 被设置
      expect(state.workers[0].tokenKey).toBeTruthy()

      // 重设 tokenKey
      state.workers[0].tokenKey = null

      // 开始执行查询
      await promise

      const history = actionTester.getCommitHistory()
      expect(history).toHaveLength(1)

      expect(state).toMatchObject({
        crawlers: {
          cr3: {
            name: 'cr3',
          },
        },
        workers: [
          {
            username: 'user1',
            // 禁止执行了，不会更新 state
            status: 'WORKING',
            submissions: 0,
            solved: 0,
            solvedList: [],
            errorMessage: '',
            tokenKey: null,
            crawlerName: 'cr3',
            key: 0.6666666,
          },
        ],
      })

    })

    it('在用户名为空时不进行查询', async () => {

      const state = {
        crawlers: {
          cr1: {
            name: 'cr2',
          },
        },
        workers: [
          {
            username: '',
            status: 'DONE',
            submissions: 0,
            solved: 0,
            solvedList: [],
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr2',
            key: 0.6666666,
          },
        ],
      }
      const actionTester = new StoreContextSimulator(state, {mutations: store.mutations})

      // 执行当前帧
      await store.actions.startOne({state, commit: actionTester.getCommiter()}, {index: 0})

      const history = actionTester.getCommitHistory()
      expect(history).toHaveLength(0)

      expect(state).toMatchObject({
        crawlers: {
          cr1: {
            name: 'cr2',
          },
        },
        workers: [
          {
            username: '',
            status: 'DONE',
            submissions: 0,
            solved: 0,
            solvedList: [],
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr2',
            key: 0.6666666,
          },
        ],
      })
    })
  })

  describe('addWorkerForCrawler', () => {

    it('能够正确添加 Worker', () => {
      const state = {
        crawlers: {
          cr1: {
            name: 'cr1',
          },
        },
      }

      const actionTester = new StoreContextSimulator()

      store.actions.addWorkerForCrawler(
        {state, commit: actionTester.getCommiter()},
        {crawlerName: 'cr1'})

      const history = actionTester.getCommitHistory()
      expect(history).toHaveLength(1)

      expect(history[0]).toMatchObject({
        type: 'addWorkerForCrawler',
        payload: {crawlerName: 'cr1'},
      })
    })

    it('在 crawler 不存在时能够抛出异常', () => {
      const state = {
        crawlers: {},
      }

      const actionTester = new StoreContextSimulator()

      const action = () => {
        store.actions.addWorkerForCrawler(
          {state, commit: actionTester.getCommiter()},
          {crawlerName: 'cr1'})
      }

      expect(action).toThrow('爬虫不存在')

      const history = actionTester.getCommitHistory()
      expect(history).toHaveLength(0)
    })
  })

  describe('removeWorkerAtIndex', () => {
    it('能够移除 worker', () => {
      const state = {
        workers: [
          {
            username: 'user1',
            crawlerName: 'cr1',
          },
          {
            username: 'user2',
            crawlerName: 'cr1',
          },
        ],
      }
      const actionTester = new StoreContextSimulator(state, {mutations: store.mutations, getters: store.getters})

      store.actions.removeWorkerAtIndex(
        {state, commit: actionTester.getCommiter(), getters: actionTester.getGetters()},
        {index: 0})

      expect(actionTester.getCommitHistory()).toHaveLength(1)
      expect(state).toMatchObject({
        workers: [
          {
            username: 'user2',
            crawlerName: 'cr1',
          },
        ],
      })
    })
  })

})
