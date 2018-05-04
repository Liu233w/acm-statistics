import {WORKER_STATUS} from '~/components/consts'

import _ from 'lodash'

export function state() {
  return {
    workers: [],
    mainUsername: '',
  }
}

export const mutations = {
  addWorker(state, {name, func, username}) {
    state.workers.push({
      name: name,
      solved: 0,
      submissions: 0,
      status: WORKER_STATUS.WAITING,
      func: func,
      errorMessage: '',
      username: username,
    })
  },
  setUsername(state, {index, username}) {
    state.workers[index].username = username
  },
  setMainUsername(state, {username}) {
    state.mainUsername = username
  },
  setResult(state, {index, solved, submissions}) {
    const worker = state.workers[index]

    worker.solved = solved
    worker.submissions = submissions
  },
  setError(state, {index, errorMessage}) {
    state.workers[index].errorMessage = errorMessage
  },
  /**
   * 清空某个worker的数据，清除 solved, submissions 和 errorMessage，不会重设状态
   */
  resetData(state, {index}) {
    const worker = state.workers[index]

    worker.solved = 0
    worker.submissions = 0
    worker.errorMessage = ''
  },
  setToWorking(state, {index}) {
    state.workers[index].status = WORKER_STATUS.WORKING
  },
  setToWaiting(state, {index}) {
    state.workers[index].status = WORKER_STATUS.WAITING
  },
  setToDone(state, {index}) {
    state.workers[index].status = WORKER_STATUS.DONE
  },
  /**
   * 用来查询 worker 的状态
   * @param state
   * @param index
   * @param key
   */
  setWorkerTokenKey(state, {index, tokenKey}) {
    state.workers[index].tokenKey = tokenKey
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
}

export const actions = {
  /**
   * 初始化worker
   * @param commit
   * @param crawlerFuncs {Object.<string, {Function}>}
   */
  initWorkers({commit}, crawlerFuncs) {
    for (let key in crawlerFuncs) {
      commit('addWorker', {
        name: key,
        func: crawlerFuncs[key],
        username: '',
      })
    }
  },
  loadUsernames({state, commit, dispatch}) {
    const username = JSON.parse(window.localStorage.getItem('username'))
    if (username) {
      commit('setMainUsername', {username: username.main})

      _.forEach(state.workers, (item, index) => {
        if (username.subs[item.name]) {
          dispatch('updateUsername', {
            index,
            username: username.subs[item.name],
          })
        }
      })
    }
  },
  saveUsernames({state}) {
    const username = {
      main: state.mainUsername,
      subs: {},
    }
    for (let item of state.workers) {
      username.subs[item.name] = item.username
    }

    window.localStorage.setItem('username', JSON.stringify(username))
  },
  updateUsername({commit}, {index, username}) {
    commit('setToWaiting', {index})
    commit('setUsername', {index, username})
    commit('resetData', {index})
  },
  updateMainUsername({state, commit, dispatch}, {username}) {
    commit('setMainUsername', {username})
    for (let index in state.workers) {
      dispatch('updateUsername', {index, username})
    }
  },
  /**
   * 启动一个 worker
   */
  async startOne({state, commit}, {index}) {
    commit('resetData', {index})
    commit('setToWorking', {index})

    const tokenKey = Math.random()
    commit('setWorkerTokenKey', {index, tokenKey})

    const worker = state.workers[index]
    try {
      const res = await worker.func(worker.username)
      if (state.workers[index].tokenKey !== tokenKey) {
        return
      }
      commit('setResult', {index, ...res})
    } catch (err) {
      if (state.workers[index].tokenKey !== tokenKey) {
        return
      }
      commit('setError', {index, errorMessage: err.message})
    }
    commit('setToDone', {index})
  },
  startAll({state, dispatch}) {
    return Promise.all(_.map(
      _.range(state.workers.length),
      index => dispatch('startOne', {index})))
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
