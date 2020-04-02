export const state = () => ({
  sidebar: false,
})

export const mutations = {
  toggleSidebar (state) {
    state.sidebar = !state.sidebar
  },
}

export const actions = {
  async nuxtServerInit({ dispatch }) {
    await dispatch('session/refreshUser')
  },
}
