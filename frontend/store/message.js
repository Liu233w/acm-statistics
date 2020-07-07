import _ from 'lodash'

export const namespaced = true

export const state = () => ({
  list: [],
})

function addMessage(state, message, type) {
  state.list.push({
    id: Math.random(),
    message,
    type,
  })
}

export const mutations = {
  add(state, param) {
    if (_.isString(param)) {
      addMessage(state, param, 'info')
    } else {
      const { message, type } = param
      addMessage(state, message, type)
    }
  },
  pop(state) {
    state.list.pop()
  },
  addError(state, message) {
    addMessage(state, message, 'error')
  },
  addSuccess(state, message) {
    addMessage(state, message, 'success')
  },
  remove(state, index) {
    state.list.splice(index, 1)
  },
  clear(state) {
    state.list = []
  },
}

export const getters = {
  top(state) {
    if (state.list.length > 0) {
      return state.list[state.list.length - 1]
    } else {
      return {}
    }
  },
  show(state) {
    return state.list.length > 0
  },
}
