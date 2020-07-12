import Cookies from 'js-cookie'
import _ from 'lodash'

export const state = () => ({
  login: false,
  username: '',
  settings: {},
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
  updateSettings(state, newSettings) {
    state.settings = _.assign(state.settings, newSettings)
  },
  resetSettings(state) {
    state.settings = {}
  },
}

export const actions = {
  async refreshUser({ commit, dispatch }) {
    try {
      const res = await this.$axios.$get('/api/services/app/Session/GetCurrentLoginInformations')

      if (res.result.user) {
        commit('setUser', { username: res.result.user.userName })
        await dispatch('refreshSettings')
      } else {
        commit('removeUser')
        commit('resetSettings')
      }
    } catch (err) {
      Cookies.remove('OAuthToken')
    }
  },
  async login({ dispatch }, { username, password, remember }) {
    const res = await this.$axios.$post('/api/TokenAuth/Authenticate', {
      userNameOrEmailAddress: username,
      password: password,
      rememberClient: remember,
    })

    const config = {}
    if (remember) {
      config.expires = 30
    }
    Cookies.set('OAuthToken', res.result.accessToken, config)

    await dispatch('refreshUser')
    await dispatch('refreshSettings')
  },
  async logout({ commit }) {
    Cookies.remove('OAuthToken')
    commit('removeUser')
    commit('resetSettings')
  },
  async refreshSettings({ commit }) {
    const res = await this.$axios.$get('/api/services/app/UserConfig/GetUserSettings')
    commit('updateSettings', res.result.values)
  },
}
