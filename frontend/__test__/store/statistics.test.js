import StoreContextSimulator from '../StoreContextSimulator'
import { MUTATION_TYPES } from '../../store/-dynamic/statistics'

import { WORKER_STATUS } from '../../components/consts'

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
      cr1: username => Promise.resolve({ submissions: 10, solved: 5, solvedList: ['1001', '1002'] }),
      // eslint-disable-next-line no-unused-vars
      cr2: (username) => Promise.reject(new Error('The user does not exist')),
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
}, { virtual: true })

const store = require('../../store/-dynamic/statistics')

describe('state', () => {
  it('can generate state correctly', () => {
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
    it('can reset data when username changed', () => {
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

      store.mutations.updateUsername(state, { index: 0, username: 'user2' })

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

    it('can change username to empty string when it is set to null', () => {
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

      store.mutations.updateUsername(state, { index: 0, username: null })

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
    it('can update every workers\'s username', () => {
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

      store.mutations.updateMainUsername(state, { username: 'main2' })

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

    it('can update data correctly', () => {
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

    it('can set username to mainUsername when its data do not exist', () => {
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

    it('does not add redundant worker when certain data do not exist', () => {
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

    it('can generate worker correctly when certain username is empty string', () => {
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
    it('should reset tokenKey and status', () => {
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

      store.mutations.stopWorker(state, { index: 0 })

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

    it('should add worker to the end when other worker does not exist', () => {
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

      store.mutations.addWorkerForCrawler(state, { crawlerName: 'cr1' })

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

    it('should add new worker after existing one with same crawler', () => {
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

      store.mutations.addWorkerForCrawler(state, { crawlerName: 'cr1' })

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
    it('can reset workers', () => {
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

  describe('solvedNum', () => {

    it('can collect data of ordinary workers', () => {
      const state = {
        workers: [
          {
            solved: 1,
            solvedList: null,
            crawlerName: 'cr1',
          },
          {
            solved: 2,
            solvedList: null,
            crawlerName: 'cr1',
          },
          {
            solved: 4,
            solvedList: null,
            crawlerName: 'cr2',
          },
        ],
        crawlers: {
          cr1: {},
          cr2: {},
        },
      }
      const nullSolvedListWorkers = store.getters.nullSolvedListWorkers(state)

      const res = store.getters.solvedNum(state, { nullSolvedListWorkers })

      expect(res).toBe(7)
    })

    it('can collect data of workers with list', () => {
      const state = {
        workers: [
          {
            solved: 1,
            solvedList: ['1001'],
            crawlerName: 'cr1',
          },
          {
            solved: 2,
            solvedList: ['1001', '1002'],
            crawlerName: 'cr2',
          },
          {
            solved: 1,
            solvedList: ['1001'],
            crawlerName: 'cr2',
          },
        ],
        crawlers: {
          cr1: {},
          cr2: {},
        },
      }
      const nullSolvedListWorkers = store.getters.nullSolvedListWorkers(state)

      const res = store.getters.solvedNum(state, { nullSolvedListWorkers })

      expect(res).toBe(3)
    })

    it('can collect data of workers with mixed type', () => {
      const state = {
        workers: [
          {
            solved: 1,
            solvedList: ['1001'],
            crawlerName: 'cr1',
          },
          {
            solved: 4,
            solvedList: null,
            crawlerName: 'cr1',
          },
          {
            solved: 2,
            solvedList: ['1001', '1002'],
            crawlerName: 'cr2',
          },
        ],
        crawlers: {
          cr1: {},
          cr2: {},
        },
      }
      const nullSolvedListWorkers = store.getters.nullSolvedListWorkers(state)

      const res = store.getters.solvedNum(state, { nullSolvedListWorkers })

      expect(res).toBe(7)
    })

    it('can collect data of virtual_judge workers', () => {
      const state = {
        workers: [
          {
            solved: 1,
            solvedList: ['1001'],
            crawlerName: 'cr1',
          },
          {
            solved: 4,
            solvedList: null,
            crawlerName: 'cr1',
          },
          {
            solved: 2,
            solvedList: ['1001', '1002'],
            crawlerName: 'cr2',
          },
          {
            solved: 5,
            solvedList: ['cr1-1001', 'cr1-1002', 'cr2-1002', 'cr2-1003', 'NN-1001'],
            crawlerName: 'cr3',
          },
          { // redundant name
            solved: 1,
            solvedList: ['cr1-1001'],
            crawlerName: 'cr4',
          },
        ],
        crawlers: {
          cr1: {},
          cr2: {},
          cr3: {
            virtual_judge: true,
          },
          cr4: {
            virtual_judge: true,
          },
        },
      }
      const nullSolvedListWorkers = store.getters.nullSolvedListWorkers(state)

      const res = store.getters.solvedNum(state, { nullSolvedListWorkers })

      expect(res).toBe(10)
    })
  })

  describe('notWorkingRate', () => {

    it('can work correctly', () => {
      const state = {
        workers: [
          { status: 'DONE' },
          { status: 'WORKING' },
          { status: 'WAITING' },
          { status: 'DONE' },
        ],
      }

      expect(store.getters.notWorkingRate(state)).toBe(75)
    })

    it('can get 0', () => {
      const state = {
        workers: [
          { status: 'WORKING' },
          { status: 'WORKING' },
          { status: 'WORKING' },
          { status: 'WORKING' },
        ],
      }

      expect(store.getters.notWorkingRate(state)).toBe(0)
    })

    it('can get 100', () => {
      const state = {
        workers: [
          { status: 'DONE' },
          { status: 'DONE' },
          { status: 'DONE' },
          { status: 'DONE' },
        ],
      }

      expect(store.getters.notWorkingRate(state)).toBe(100)
    })

  })

  describe('workerNumberOfCrawler', () => {

    it('can get worker count of certain crawler', () => {

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
    it('can get worker index', () => {
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
    it('can get correct result', () => {

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

      const res = store.getters.nullSolvedListCrawlers(state, { nullSolvedListWorkers })

      expect(res).toMatchObject({
        cr1: 'cr1 title',
        cr3: 'cr3 title',
      })
    })
  })

  // TODO: test warnings, test submission numbers
  describe.skip('summaryForCrawler', () => {

    it('should skip workers with no solvedList', () => {
      const state = {
        workers: [
          {
            username: 'u1',
            solved: 1,
            solvedList: null,
            submissions: 3,
            crawlerName: 'cr1',
            status: WORKER_STATUS.DONE,
          },
          {
            username: 'u2',
            solved: 2,
            solvedList: null,
            submissions: 5,
            crawlerName: 'cr1',
            status: WORKER_STATUS.DONE,
          },
          {
            username: 'u3',
            solved: 4,
            solvedList: null,
            submissions: 10,
            crawlerName: 'cr2',
            status: WORKER_STATUS.DONE,
          },
        ],
        crawlers: {
          cr1: {
            title: 'Cr1',
          },
          cr2: {
            title: 'Cr2',
          },
        },
      }

      const res = store.getters.summaryForCrawler(state)

      expect(res).toMatchObject({
        cr1: {
          crawlerTitle: 'Cr1',
          usernames: new Set(),
          solvedSet: new Set(),
        },
        cr2: {
          crawlerTitle: 'Cr2',
          usernames: new Set(),
          solvedSet: new Set(),
        },
      })
    })

    it('can collect data of workers with list', () => {
      const state = {
        workers: [
          {
            username: 'u1',
            solved: 1,
            solvedList: ['1001'],
            crawlerName: 'cr1',
            status: WORKER_STATUS.DONE,
          },
          {
            username: 'u2',
            solved: 2,
            solvedList: ['1001', '1002'],
            crawlerName: 'cr2',
            status: WORKER_STATUS.DONE,
          },
          {
            username: 'u3',
            solved: 1,
            solvedList: ['1001'],
            crawlerName: 'cr2',
            status: WORKER_STATUS.DONE,
          },
        ],
        crawlers: {
          cr1: {
            title: 'Cr1',
          },
          cr2: {
            title: 'Cr2',
          },
        },
      }
      const res = store.getters.summaryForCrawler(state)

      expect(res).toMatchObject({
        cr1: {
          crawlerTitle: 'Cr1',
          usernames: new Set(['u1']),
          solvedSet: new Set(['1001']),
        },
        cr2: {
          crawlerTitle: 'Cr2',
          usernames: new Set(['u2', 'u3']),
          solvedSet: new Set(['1001', '1002']),
        },
      })
    })

    it('can collect data of workers with mixed type', () => {
      const state = {
        workers: [
          {
            username: 'u1',
            solved: 1,
            solvedList: ['1001'],
            crawlerName: 'cr1',
            status: WORKER_STATUS.DONE,
          },
          {
            username: 'u2',
            solved: 4,
            solvedList: null,
            crawlerName: 'cr1',
            status: WORKER_STATUS.DONE,
          },
          {
            username: 'u3',
            solved: 2,
            solvedList: ['1001', '1002'],
            crawlerName: 'cr2',
            status: WORKER_STATUS.DONE,
          },
        ],
        crawlers: {
          cr1: {
            title: 'Cr1',
          },
          cr2: {
            title: 'Cr2',
          },
        },
      }
      const res = store.getters.summaryForCrawler(state)

      expect(res).toMatchObject({
        cr1: {
          crawlerTitle: 'Cr1',
          usernames: new Set(['u1']),
          solvedSet: new Set(['1001']),
        },
        cr2: {
          crawlerTitle: 'Cr2',
          usernames: new Set(['u3']),
          solvedSet: new Set(['1001', '1002']),
        },
      })
    })

    it('can collect data of virtual_judge workers', () => {
      const state = {
        workers: [
          {
            username: 'u1',
            solved: 1,
            solvedList: ['1001'],
            crawlerName: 'cr1',
            status: WORKER_STATUS.DONE,
          },
          {
            username: 'u2',
            solved: 4,
            solvedList: null,
            crawlerName: 'cr1',
            status: WORKER_STATUS.DONE,
          },
          {
            username: 'u3',
            solved: 2,
            solvedList: ['1001', '1002'],
            crawlerName: 'cr2',
            status: WORKER_STATUS.DONE,
          },
          {
            username: 'u4',
            solved: 5,
            solvedList: ['cr1-1001', 'cr1-1002', 'cr2-1002', 'cr2-1003', 'NN-1001'],
            crawlerName: 'cr3',
            status: WORKER_STATUS.DONE,
          },
          { // redundant name
            username: 'u5',
            solved: 1,
            solvedList: ['cr1-1001'],
            crawlerName: 'cr4',
            status: WORKER_STATUS.DONE,
          },
        ],
        crawlers: {
          cr1: {
            title: 'Cr1',
          },
          cr2: {
            title: 'Cr2',
          },
          cr3: {
            title: 'Cr3',
            virtual_judge: true,
          },
          cr4: {
            title: 'Cr4',
            virtual_judge: true,
          },
        },
      }

      const res = store.getters.summaryForCrawler(state)

      expect(res).toMatchObject({
        cr1: {
          crawlerTitle: 'Cr1',
          usernames: new Set(['u1', '[u4 in Cr3]', '[u5 in Cr4]']),
          solvedSet: new Set(['1001', '1002']),
        },
        cr2: {
          crawlerTitle: 'Cr2',
          usernames: new Set(['u3', '[u4 in Cr3]']),
          solvedSet: new Set(['1001', '1002', '1003']),
        },
        cr3: {
          crawlerTitle: 'Cr3(Not Merged)',
          usernames: new Set(['u4']),
          solvedSet: new Set(['NN-1001']),
        },
        cr4: {
          crawlerTitle: 'Cr4(Not Merged)',
          usernames: new Set(['u5']),
          solvedSet: new Set(),
        },
      })
    })
  })
})

describe('helper functions', () => {

  describe('getUsernameObjectFromState', () => {

    it('can generate data correctly', () => {

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

    it('can generate data correctly with empty username', () => {

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
    it('can get correct result', () => {
      const res = store.addProblemPrefix(['1001', '1002'], 'A')
      expect(res).toMatchObject(['A-1001', 'A-1002'])
    })
  })

  describe('pushSet', () => {
    it('can get correct result', () => {
      const set = new Set([1, 2])
      store.pushSet(set, [2, 3])
      expect(set).toMatchObject(new Set([1, 2, 3]))
    })
  })
})

describe('actions', () => {

  describe('startOne', () => {

    it('can be executed correctly', async () => {
      const state = {
        crawlers: {
          cr1: {
            // mock at file beginning ({submissions: 10, solved: 5, solvedList: ['1001','1002']}),
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
      const actionTester = new StoreContextSimulator(state, { mutations: store.mutations })

      await store.actions.startOne({ state, commit: actionTester.getCommiter() }, { index: 0 })

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

    it('can work correctly when crawler does not return ac list', async () => {
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
      const actionTester = new StoreContextSimulator(state, { mutations: store.mutations })

      await store.actions.startOne({ state, commit: actionTester.getCommiter() }, { index: 0 })

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

    it('can set status when crawler throw error', async () => {
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
      const actionTester = new StoreContextSimulator(state, { mutations: store.mutations })

      await store.actions.startOne({ state, commit: actionTester.getCommiter() }, { index: 0 })

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
            errorMessage: 'The user does not exist',
            tokenKey: expect.any(Number),
            crawlerName: 'cr2',
            key: 0.6666666,
          },
        ],
      })
    })

    it('will not submit result when tokenKey has been changed', async () => {

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
      const actionTester = new StoreContextSimulator(state, { mutations: store.mutations })

      // Execute current frame
      const promise = store.actions.startOne({ state, commit: actionTester.getCommiter() }, { index: 0 })

      // tokenKey should be set before query
      expect(state.workers[0].tokenKey).toBeTruthy()

      // reset tokenKey
      state.workers[0].tokenKey = null

      // start query
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
            // state is not updated due to query cancellation
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

    it('does not query when username is empty', async () => {

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
      const actionTester = new StoreContextSimulator(state, { mutations: store.mutations })

      // Execute current frame
      await store.actions.startOne({ state, commit: actionTester.getCommiter() }, { index: 0 })

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

    it('can add worker correctly', () => {
      const state = {
        crawlers: {
          cr1: {
            name: 'cr1',
          },
        },
      }

      const actionTester = new StoreContextSimulator()

      store.actions.addWorkerForCrawler(
        { state, commit: actionTester.getCommiter() },
        { crawlerName: 'cr1' })

      const history = actionTester.getCommitHistory()
      expect(history).toHaveLength(1)

      expect(history[0]).toMatchObject({
        type: 'addWorkerForCrawler',
        payload: { crawlerName: 'cr1' },
      })
    })

    it('can throw when crawler does not exist', () => {
      const state = {
        crawlers: {},
      }

      const actionTester = new StoreContextSimulator()

      const action = () => {
        store.actions.addWorkerForCrawler(
          { state, commit: actionTester.getCommiter() },
          { crawlerName: 'cr1' })
      }

      expect(action).toThrow('Crawler does not exist')

      const history = actionTester.getCommitHistory()
      expect(history).toHaveLength(0)
    })
  })

  describe('removeWorkerAtIndex', () => {
    it('can remove worker', () => {
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
      const actionTester = new StoreContextSimulator(state, { mutations: store.mutations, getters: store.getters })

      store.actions.removeWorkerAtIndex(
        { state, commit: actionTester.getCommiter(), getters: actionTester.getGetters() },
        { index: 0 })

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
