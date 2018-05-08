import {WORKER_STATUS} from '~/components/consts'
import getCrawlerData from '~/dynamic/crawlers'

import _ from 'lodash'

export function state() {
  const data = getCrawlerData()

  const crawlers = {}
  _.forEach(data.metas, (val, key) => {
    crawlers[key] = val
    crawlers[key].name = key
    crawlers[key].func = data.crawlers[key]
  })

  const workers = []
  _.forEach(crawlers, val => {

    const worker = {
      crawlerName: val.name,
      username: '',
      status: WORKER_STATUS.WAITING,
    }
    resetWorker(worker)
    workers.push(worker)
  })

  return {
    workers,
    crawlers,
    mainUsername: '',
  }
}

export const MUTATION_TYPES = {
  updateUsername: 'updateUsername',
  updateMainUsername: 'updateMainUsername',
  updateUsernamesFromObject: 'updateUsernamesFromObject',
  setWorkerDone: 'setWorkerDone',
  setWorkerError: 'setWorkerError',
  startWorker: 'startWorker',
  stopWorker: 'stopWorker',
}

export const mutations = {
  [MUTATION_TYPES.updateUsername](state, {index, username}) {
    updateUsername(state.workers[index], username)
  },
  [MUTATION_TYPES.updateMainUsername](state, {username}) {
    state.mainUsername = username
    _.forEach(state.workers, (worker) => updateUsername(worker, username))
  },
  /**
   * 从 payload 读取用户名，如果某爬虫在 state.crawlers 中存在，而在subs中不存在，
   * 则创建一个用户名为 mainUsername 的 worker。
   * @param state
   * @param main
   * @param subs
   */
  [MUTATION_TYPES.updateUsernamesFromObject](state, {main, subs}) {
    state.mainUsername = main

    /* subs: {
     *   crawlerName: [ 'username1', 'username2' ],
     * }
     */
    // 从 crawler 重新生成 worker
    state.workers = []

    _.forEach(state.crawlers, item => {
      if (subs[item.name]) {
        _.forEach(subs[item.name], username => {

          const worker = {
            crawlerName: item.name,
            username: username,
            status: WORKER_STATUS.WAITING,
          }
          resetWorker(worker)

          state.workers.push(worker)
        })
      } else {
        const worker = {
          crawlerName: item.name,
          username: main,
          status: WORKER_STATUS.WAITING,
        }
        resetWorker(worker)

        state.workers.push(worker)
      }
    })
  },
  [MUTATION_TYPES.setWorkerDone](state, {index, solved, submissions}) {
    const worker = state.workers[index]

    worker.solved = solved
    worker.submissions = submissions
    worker.status = WORKER_STATUS.DONE
  },
  [MUTATION_TYPES.setWorkerError](state, {index, errorMessage}) {
    const worker = state.workers[index]

    worker.errorMessage = errorMessage
    worker.status = WORKER_STATUS.DONE
  },
  /**
   * 更新状态，准备启动 worker
   */
  [MUTATION_TYPES.startWorker](state, {index, tokenKey}) {
    const worker = state.workers[index]

    resetWorker(worker)

    worker.status = WORKER_STATUS.WORKING
    worker.tokenKey = tokenKey
  },
  [MUTATION_TYPES.stopWorker](state, {index}) {
    const worker = state.workers[index]

    worker.status = WORKER_STATUS.WAITING
    worker.tokenKey = null
  },
}

export const getters = {
  /**
   * 总体 solved 数量
   * @param state
   * @returns {number}
   */
  solvedNum(state) {
    return _.reduce(state.workers, (sum, val) => sum + val.solved, 0)
  },
  /**
   * 总体 submissions 数量
   * @param state
   * @returns {number}
   */
  submissionsNum(state) {
    return _.reduce(state.workers, (sum, val) => sum + val.submissions, 0)
  },
  /**
   * 是否还有worker正在工作
   * @param state
   * @returns {boolean}
   */
  isWorking(state) {
    return _.some(state.workers, item => item.status === WORKER_STATUS.WORKING)
  },
  /**
   * 返回一个0-100的数字，表示不在WORKING状态的Worker的比例
   * @param state
   * @returns {number}
   */
  notWorkingRate(state) {
    const cnt = state.workers.length
    const notWorking = _.filter(state.workers, item => item.status !== WORKER_STATUS.WORKING).length
    return notWorking / cnt * 100
  },
  /**
   * 每个 crawler 有多少 worker
   * @param state
   * @return {Object.<string, number>}
   */
  workerNumberOfCrawler(state) {
    return _.countBy(state.workers, 'crawlerName')
  },
}

export const actions = {
  loadUsernames({commit}) {
    const username = JSON.parse(window.localStorage.getItem('username-v2'))
    if (username) {
      commit(MUTATION_TYPES.updateUsernamesFromObject, username)
    }
  },
  saveUsernames({state}) {
    const username = getUsernameObjectFromState(state)
    window.localStorage.setItem('username-v2', JSON.stringify(username))
  },
  updateUsername({commit}, {index, username}) {
    commit(MUTATION_TYPES.updateUsername, {index, username})
  },
  updateMainUsername({commit}, {username}) {
    commit(MUTATION_TYPES.updateMainUsername, {username})
  },
  /**
   * 启动一个 worker
   */
  async startOne({state, commit}, {index}) {
    const tokenKey = Math.random()
    commit(MUTATION_TYPES.startWorker, {index, tokenKey})

    const worker = state.workers[index]
    try {
      const res = await state.crawlers[worker.crawlerName].func(worker.username)
      if (state.workers[index].tokenKey !== tokenKey) {
        console.log('done but stopped')
        return
      }
      commit(MUTATION_TYPES.setWorkerDone, {index, ...res})
    } catch (err) {
      if (state.workers[index].tokenKey !== tokenKey) {
        console.log('done but stopped')
        return
      }
      commit(MUTATION_TYPES.setWorkerError, {index, errorMessage: err.message})
    }
  },
  startAll({state, dispatch}) {
    return Promise.all(_.map(
      _.range(state.workers.length),
      index => dispatch('startOne', {index})))
  },
  stopOne({commit}, {index}) {
    commit(MUTATION_TYPES.stopWorker, {index})
  },
}

export const namespaced = true

export default {
  namespaced,
  state,
  mutations,
  getters,
  actions,
}

/**
 * 清空某个worker的数据，清除 solved, submissions, errorMessage 和 tokenKey，不会重设状态
 */
function resetWorker(worker) {
  worker.solved = 0
  worker.submissions = 0
  worker.errorMessage = ''
  worker.tokenKey = null
}

function updateUsername(worker, username) {
  resetWorker(worker)
  worker.username = username
  worker.status = WORKER_STATUS.WAITING
}

export function getUsernameObjectFromState(state) {
  const username = {
    main: state.mainUsername,
    subs: {},
  }

  for (let item of state.workers) {
    if (username.subs[item.crawlerName]) {
      username.subs[item.crawlerName].push(item.username)
    } else {
      username.subs[item.crawlerName] = [item.username]
    }
  }

  return username
}
