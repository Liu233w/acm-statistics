import { createLocalVue, mount } from '@vue/test-utils'
import Vuex from 'vuex'
import Vuetify from 'vuetify'

const localVue = createLocalVue()
localVue.use(Vuex)

import MessagePanel from '~/components/MessagePanel.vue'
import * as MessageStore from '~/store/message'

describe('MessagePanel', () => {

  let wrapper
  let store

  beforeEach(() => {
    const vuetify = new Vuetify()
    store = new Vuex.Store({
      modules: {
        message: MessageStore,
      },
    })
    wrapper = mount(MessagePanel, {
      store,
      localVue,
      vuetify,
    })
  })

  it('can render correctly', async () => {
    expect(wrapper.vm).toBeTruthy()
    await wrapper.vm.$nextTick()
    expect(wrapper.html()).toMatchSnapshot()
  })

  it('can render when there are 1 element', async () => {
    store.commit('message/add', 'Info')
    await wrapper.vm.$nextTick()
    expect(store.state.message.list).toHaveLength(1)
    expect(wrapper.html()).toMatchSnapshot()
  })

  it('can render when there are 2 elements', async () => {
    store.commit('message/add', {
      message: 'Info',
      type: 'info',
    })
    store.commit('message/add', {
      message: 'Error',
      type: 'error',
    })
    await wrapper.vm.$nextTick()
    expect(store.state.message.list).toHaveLength(2)
    expect(wrapper.html()).toMatchSnapshot()
  })
})
