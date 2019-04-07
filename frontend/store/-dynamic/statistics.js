import {WORKER_STATUS} from '~/components/consts'
import getCrawlerData from '~/dynamic/crawlers'

import _ from 'lodash'

const crawlerFunctions = {}

function initCrawlers() {

  const data = getCrawlerData()

  const crawlers = {}
  _.forEach(data.metas, (val, key) => {
    crawlers[key] = val
    crawlers[key].name = key
    crawlerFunctions[key] = data.crawlers[key]
  })

  return crawlers
}

export function state() {

  const crawlers = initCrawlers()
  const workers = initWorkers(crawlers)

  return {
    workers,
    crawlers,
    mainUsername: '',
    // 多个worker有重复的ac时，移除掉重复的。
    checkDuplicateAc: true,
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
  addWorkerForCrawler: 'addWorkerForCrawler',
  removeWorkerAtIndex: 'removeWorkerAtIndex',
  clearWorkers: 'clearWorkers',
  setCheckDuplicateAc: 'setCheckDuplicateAc',
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
  [MUTATION_TYPES.setWorkerDone](state, {index, solved, submissions, solvedList}) {
    const worker = state.workers[index]

    worker.solved = solved
    worker.submissions = submissions
    if (_.isArray(solvedList)) {
      // 冻结列表，这样 vue 就不会给列表中的每个元素创建proxy了，可以显著提升性能
      // 来自 https://vuejs.org/v2/guide/instance.html#Data-and-Methods
      // 和 https://vuedose.tips/tips/improve-performance-on-large-lists-in-vue-js/
      worker.solvedList = Object.freeze(solvedList)
    } else {
      worker.solvedList = null
    }
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
  [MUTATION_TYPES.addWorkerForCrawler](state, {crawlerName}) {
    let insertIdx = _.findLastIndex(state.workers, _.matchesProperty('crawlerName', crawlerName))
    if (insertIdx === -1) {
      insertIdx = state.workers.length
    } else {
      // 插入到后方
      ++insertIdx
    }

    const worker = {
      crawlerName,
      username: '',
      status: WORKER_STATUS.WAITING,
    }
    resetWorker(worker)

    state.workers.splice(insertIdx, 0, worker)
  },
  [MUTATION_TYPES.removeWorkerAtIndex](state, {index}) {
    state.workers.splice(index, 1)
  },
  [MUTATION_TYPES.clearWorkers](state) {
    state.workers = initWorkers(state.crawlers)
    state.mainUsername = ''
  },
  [MUTATION_TYPES.setCheckDuplicateAc](state, {value}) {
    state.checkDuplicateAc = value
  },
}

export const getters = {
  /**
   * 没法得到 solvedList 的 worker
   * @param state
   * @return {Object[]}
   */
  nullSolvedListWorkers(state) {
    return _.filter(state.workers, item => item.solvedList === null)
  },
  /**
   * 没法得到 solvedList 的 worker 的爬虫名
   * @param state
   * @param nullSolvedListWorkers
   * @return {Object.<String, String>} key 是 crawler name， value 是 title
   */
  nullSolvedListCrawlers(state, {nullSolvedListWorkers}) {
    const res = {}
    for (let item of nullSolvedListWorkers) {
      res[item.crawlerName] = state.crawlers[item.crawlerName].title
    }
    return res
  },
  /**
   * 总体 solved 数量
   * @param state
   * @param nullSolvedListWorkers
   * @returns {number}
   */
  solvedNum(state, {nullSolvedListWorkers}) {

    if (state.checkDuplicateAc) {

      const nullSolvedListWorkerSolvedNum = _.sumBy(nullSolvedListWorkers, 'solved')

      const acSet = new Set()
      for (let worker of state.workers) {
        if (worker.solvedList) {
          if (state.crawlers[worker.crawlerName].virtual_judge) {
            pushSet(acSet, worker.solvedList)
          } else {
            pushSet(acSet, addProblemPrefix(worker.solvedList, worker.crawlerName))
          }
        }
      }

      return acSet.size + nullSolvedListWorkerSolvedNum

    } else {
      return _.sumBy(state.workers, 'solved')
    }
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
  /**
   * 给每个 worker 返回一个数字，表示它是相同 crawler 的第几个 worker，从 1 开始计数。
   * 相同 crawler 的 worker 必须在一起
   * @param state
   */
  workerIdxOfCrawler(state) {
    const ret = []
    let lastCrawlerName = null
    let num = 0
    _.forEach(state.workers, item => {
      if (item.crawlerName !== lastCrawlerName) {
        lastCrawlerName = item.crawlerName
        num = 0
      }
      ret.push(++num)
    })
    return ret
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

    const worker = state.workers[index]
    if (!worker.username) {
      return
    }

    const tokenKey = Math.random()
    commit(MUTATION_TYPES.startWorker, {index, tokenKey})

    try {
      const res = await crawlerFunctions[worker.crawlerName](worker.username)
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
  addWorkerForCrawler({state, commit}, {crawlerName}) {
    if (state.crawlers[crawlerName]) {
      commit(MUTATION_TYPES.addWorkerForCrawler, {crawlerName})
    } else {
      throw new Error('爬虫不存在')
    }
  },
  removeWorkerAtIndex({state, commit, getters}, {index}) {
    if (index < 0 || index >= state.workers.length) {
      throw new Error('该位置不存在')
    }

    const crawlerName = state.workers[index].crawlerName

    if (getters.workerNumberOfCrawler[crawlerName] <= 1) {
      throw new Error('不能移除最后一个 worker，您可以将用户名置为空以跳过查询')
    }

    commit(MUTATION_TYPES.removeWorkerAtIndex, {index})
  },
  clearWorkers({commit}) {
    commit(MUTATION_TYPES.clearWorkers)
  },
  setCheckDuplicateAc({commit}, {value}) {
    commit(MUTATION_TYPES.setCheckDuplicateAc, {value})
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
  worker.solvedList = []

  if (!worker.key) {
    worker.key = Math.random()
  }
}

function updateUsername(worker, username) {
  resetWorker(worker)
  if (!username) {
    // 假如 username 是 null 或 undefined，将其重设为 ''
    worker.username = ''
  } else {
    worker.username = username
  }
  worker.status = WORKER_STATUS.WAITING
}

function initWorkers(crawlers) {
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

  return workers
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

/**
 * 给题目列表里的每个题目加上前缀
 * @param {String[]} lst 题目列表
 * @param {String} prefix 前缀
 * @return {String[]}
 */
export function addProblemPrefix(lst, prefix) {
  return _.map(lst, item => `${prefix}-${item}`)
}

/**
 * 将 lst 的内容合并进 set 中
 * @param {Set<*>} set
 * @param {*[]} lst
 */
export function pushSet(set, lst) {
  for (let item of lst) {
    set.add(item)
  }
}
