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
    const res = await this.$axios.get('/api/services/app/Session/GetCurrentLoginInformations')

    if (res.result.user) {
      commit('setUser', { username: res.result.user.username })
    } else {
      commit('removeUser')
    }
  },
  async nuxtServerInit({ dispatch }) {
    await dispatch('refreshUser')
  },
  async login({dispatch, $cookie, $axios}, {username, password, remember}) {
    const res = await $axios.post('/api/TokenAuth/Authenticate', {
      userNameOrEmailAddress: username,
      password: password,
      rememberClient: remember,
    })

    $cookie.set('OAuthToken', res.data.result.accessToken)

    await dispatch('refreshUser')
  },
  async logout({commit, $cookie}) {
    $cookie.remove('OAuthToken')
    commit('removeUser')
  },
}
