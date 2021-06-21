/**
 * @jest-environment jsdom
 */

import { shallowMount, createLocalVue, mount } from '@vue/test-utils'
import Vuex from 'vuex'
import Vuetify from 'vuetify'
import Vue from 'vue'

Vue.use(Vuetify)

const localVue = createLocalVue()
localVue.use(Vuex)

jest.mock('~/dynamic/crawlers', () => () => ({
  metas: {
    testCrawler: {
      title: 'crawler title',
      description: 'crawler description',
      url: 'http://crawler.url',
    },
  },
  crawlers: {
    testCrawler: () => ({ submissions: 0, pass: 0 }),
  },
}), { virtual: true })

import { WORKER_STATUS } from '~/components/consts'
import WorkerCard from '~/components/WorkerCard.vue'
import StatisticsStore from '~/store/-dynamic/statistics'

describe('WorkerCard', () => {

  let vuetify

  beforeEach(() => {
    vuetify = new Vuetify()
  })

  it('can be mounted correctly', () => {
    const wrapper = shallowMount(WorkerCard, {
      store: new Vuex.Store({
        modules: {
          statistics: StatisticsStore,
        },
      }),
      localVue,
      vuetify,
      propsData: {
        index: 0,
      },
    })
    expect(wrapper.vm).toBeTruthy()
  })

  it('can be rendered correctly', () => {
    const wrapper = mount(WorkerCard, {
      store: new Vuex.Store({
        modules: {
          statistics: StatisticsStore,
        },
      }),
      localVue,
      vuetify,
      propsData: {
        index: 0,
      },
    })
    expect(wrapper.html()).toMatchSnapshot()
  })

  it('can render username correctly', () => {

    const wrapper = mount(WorkerCard, {
      store: new Vuex.Store({
        modules: {
          statistics: {
            state: {
              workers: [{
                crawlerName: 'testCrawler',
                username: 'test_crawler_username',
                status: WORKER_STATUS.WAITING,
                solved: 0,
                submissions: 0,
                errorMessage: '',
                tokenKey: null,
              }],
              crawlers: {
                'testCrawler': {
                  name: 'testCrawler',
                  title: 'crawler title',
                  description: 'crawler description',
                  url: 'http://crawler.url',
                },
              },
            },
            getters: StatisticsStore.getters,
            namespaced: true,
          },
        },
      }),
      localVue,
      vuetify,
      propsData: {
        index: 0,
      },
    })

    expect(wrapper.vm.username).toBe('test_crawler_username')
    expect(wrapper.html()).toMatchSnapshot()
  })

  it('can render warning correctly', () => {

    const wrapper = mount(WorkerCard, {
      store: new Vuex.Store({
        modules: {
          statistics: {
            state: {
              workers: [{
                crawlerName: 'testCrawler',
                username: ' username with space',
                status: WORKER_STATUS.WAITING,
                solved: 0,
                submissions: 0,
                errorMessage: '',
                tokenKey: null,
              }],
              crawlers: {
                'testCrawler': {
                  name: 'testCrawler',
                  title: 'crawler title',
                  description: 'crawler description',
                  url: 'http://crawler.url',
                },
              },
            },
            getters: StatisticsStore.getters,
            namespaced: true,
          },
        },
      }),
      localVue,
      vuetify,
      propsData: {
        index: 0,
      },
    })

    expect(wrapper.html()).toMatchSnapshot()
  })

  it('can render result correctly', () => {

    const wrapper = mount(WorkerCard, {
      store: new Vuex.Store({
        modules: {
          statistics: {
            state: {
              workers: [{
                crawlerName: 'testCrawler',
                username: 'test_crawler_username',
                status: WORKER_STATUS.WAITING,
                solved: 10,
                submissions: 20,
                errorMessage: '',
                tokenKey: null,
              }],
              crawlers: {
                'testCrawler': {
                  name: 'testCrawler',
                  title: 'crawler title',
                  description: 'crawler description',
                  url: 'http://crawler.url',
                },
              },
            },
            getters: StatisticsStore.getters,
            namespaced: true,
          },
        },
      }),
      localVue,
      vuetify,
      propsData: {
        index: 0,
      },
    })

    expect(wrapper.html()).toMatchSnapshot()
  })

  it('can render error correctly', () => {

    const wrapper = mount(WorkerCard, {
      store: new Vuex.Store({
        modules: {
          statistics: {
            state: {
              workers: [{
                crawlerName: 'testCrawler',
                username: 'test_crawler_username',
                status: WORKER_STATUS.WAITING,
                solved: 0,
                submissions: 0,
                errorMessage: 'a error is occurred',
                tokenKey: null,
              }],
              crawlers: {
                'testCrawler': {
                  name: 'testCrawler',
                  title: 'crawler title',
                  description: 'crawler description',
                  url: 'http://crawler.url',
                },
              },
            },
            getters: StatisticsStore.getters,
            namespaced: true,
          },
        },
      }),
      localVue,
      vuetify,
      propsData: {
        index: 0,
      },
    })

    expect(wrapper.html()).toMatchSnapshot()
  })
})
