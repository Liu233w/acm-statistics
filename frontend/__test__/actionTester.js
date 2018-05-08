import _ from 'lodash'

export default class ActionTester {
  constructor(state = null, mutations = null) {
    this.commitHistory = []
    this.state = state
    this.mutations = mutations
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
      this.mutations[type](this.state, payload)
    }
  }
}
