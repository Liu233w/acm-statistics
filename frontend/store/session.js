export const state = () => ({
  login: false,
  username: '',
})

export const mutations={
  setUser(state, {username}) {
    state.login=true
    state.username = username
  },
  removeUser(state){
    state.login=false
    state.username=''
  },
}

export const actions = {
  async refreshUser({commit}) {
    const res = await this.$axios.get('/api/services/app/Session/GetCurrentLoginInformations')

    if (res.result.user) {
      commit('setUser', {username: res.result.user.username})
    } else {
      commit('removeUser')
    }
  },
  async nuxtServerInit ({ dispatch }) {
    await dispatch('refreshUser')
  },
}
