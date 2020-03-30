import Cookies from 'js-cookie'

export const state = () => ({
  login: false,
  username: '',
})

export const mutations = {
  setUser(state, { username }) {
    state.login = true
    state.username = username
  },
  removeUser(state) {
    state.login = false
    state.username = ''
  },
}

export const actions = {
  async refreshUser({ commit }) {
    const res = await this.$axios.$get('/api/services/app/Session/GetCurrentLoginInformations')

    if (res.result.user) {
      commit('setUser', { username: res.result.user.userName })
    } else {
      commit('removeUser')
    }
  },
  async nuxtServerInit({ dispatch }) {
    await dispatch('refreshUser')
  },
  async login({dispatch}, {username, password, remember}) {
    const res = await this.$axios.$post('/api/TokenAuth/Authenticate', {
      userNameOrEmailAddress: username,
      password: password,
      rememberClient: remember,
    })

    window.aaa = this
    Cookies.set('OAuthToken', res.result.accessToken)

    await dispatch('refreshUser')
  },
  async logout({commit}) {
    Cookies.remove('OAuthToken')
    commit('removeUser')
  },
}
