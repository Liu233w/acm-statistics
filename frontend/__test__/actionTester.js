import _ from 'lodash'

export default class ActionTester {
  /**
   * 初始化
   * @param state {Object} mocked state
   * @param mutations {Object.<string, {Function}>} mocked mutations, 如果存在，提交一个 mutations 的时候会调用
   * @param getters {Object.<string, {Function|*}>} 如果为函数，得到的 getter 会使用此函数和 state 来计算出结果；否则直接以这个值作为结果
   */
  constructor(state = null, {mutations, getters} = {}) {
    this.commitHistory = []
    this.state = state
    this.mutations = mutations

    this.getters = {}
    if (getters) {
      for (let key in getters) {
        const val = getters[key]
        if (_.isFunction(val)) {
          Object.defineProperty(this.getters, key, {get: () => val(this.state)})
        } else {
          this.getters[key] = val
        }
      }
    }
  }

  getCommitHistory() {
    return this.commitHistory
  }

  getCommiter() {
    return (type, payload) => {
      this.commitHistory.push({
        type,
        payload: _.cloneDeep(payload),
      })
      if (this.state && this.mutations) {
        this.mutations[type](this.state, payload)
      }
    }
  }

  getGetters() {
    return this.getters
  }
}
