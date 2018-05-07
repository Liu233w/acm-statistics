/* eslint-disable no-undef */

jest.mock('~/dynamic/crawlers', () => function () {
  return {
    metas: {
      cr1: {
        title: 'crawler 1',
      },
    },
    crawlers: {
      // eslint-disable-next-line no-unused-vars
      cr1: username => ({submissions: 10, solved: 5}),
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
          func: expect.any(Function),
        },
      },
      workers: [{
        crawlerName: 'cr1',
        username: '',
        solved: 0,
        submissions: 0,
        errorMessage: '',
        tokenKey: null,
      }],
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
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr1',
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
            errorMessage: '',
            tokenKey: null,
            crawlerName: 'cr1',
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
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr1',
          },
          {
            username: 'user2',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr2',
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
            tokenKey: null,
            crawlerName: 'cr1',
          },
          {
            username: 'main2',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            tokenKey: null,
            crawlerName: 'cr2',
          },
        ],
      })
    })
  })

  describe('updateUsernamesFromObject', () => {

    // eslint-disable-next-line no-unused-vars
    const testFunc = username => ({submissions: 33, solved: 22})

    let startState

    beforeEach(() => {
      startState = {
        mainUsername: 'mainUser',
        crawlers: {
          cr1: {
            name: 'cr1',
            func: testFunc,
          },
          cr2: {
            name: 'cr2',
            func: testFunc,
          },
        },
        workers: [
          {
            username: 'user1',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr1',
          },
          {
            username: 'user2',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr1',
          },
          {
            username: 'user2',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr2',
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
            tokenKey: null,
            crawlerName: 'cr1',
          },
          {
            username: 'nu2',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            tokenKey: null,
            crawlerName: 'cr1',
          },
          {
            username: 'nu3',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            tokenKey: null,
            crawlerName: 'cr2',
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
            tokenKey: null,
            crawlerName: 'cr1',
          },
          {
            username: 'nu3',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            tokenKey: null,
            crawlerName: 'cr2',
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
            tokenKey: null,
            crawlerName: 'cr1',
          },
          {
            username: 'nu3',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            tokenKey: null,
            crawlerName: 'cr2',
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
            tokenKey: null,
            crawlerName: 'cr1',
          },
          {
            username: '',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            tokenKey: null,
            crawlerName: 'cr1',
          },
          {
            username: '',
            status: 'WAITING',
            submissions: 0,
            solved: 0,
            errorMessage: '',
            tokenKey: null,
            crawlerName: 'cr2',
          },
        ],
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
})

describe('helper functions', () => {

  describe('getUsernameObjectFromState', () => {

    // eslint-disable-next-line no-unused-vars
    const testFunc = username => ({submissions: 33, solved: 22})

    it('能够正确生成数据', () => {

      const state = {
        mainUsername: 'mainUser',
        crawlers: {
          cr1: {
            name: 'cr1',
            func: testFunc,
          },
          cr2: {
            name: 'cr2',
            func: testFunc,
          },
        },
        workers: [
          {
            username: 'user1',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr1',
          },
          {
            username: 'user2',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr1',
          },
          {
            username: 'user2',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr2',
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
            func: testFunc,
          },
          cr2: {
            name: 'cr2',
            func: testFunc,
          },
        },
        workers: [
          {
            username: 'user1',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr1',
          },
          {
            username: '',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr1',
          },
          {
            username: '',
            status: 'DONE',
            submissions: 33,
            solved: 22,
            errorMessage: '.....',
            tokenKey: 0.23333,
            crawlerName: 'cr2',
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
})
