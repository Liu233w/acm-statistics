<template>
  <v-app>
    <v-navigation-drawer
      v-model="drawer"
      fixed
      app
    >
      <v-list>
        <v-list-item
          nuxt
          :to="item.to"
          :key="i"
          v-for="(item, i) in items"
          exact
        >
          <v-list-item-action>
            <v-icon v-html="item.icon" />
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title v-text="item.title" />
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>
    <v-app-bar
      app
      absolute
    >
      <v-app-bar-nav-icon @click="drawer = !drawer" />
      <v-slide-x-transition>
        <div v-if="topBarBeforeTitle">
          <component :is="topBarBeforeTitle" />
        </div>
      </v-slide-x-transition>
      <v-toolbar-title v-text="title" />
      <v-slide-x-transition>
        <v-divider
          v-if="topBarAfterTitle"
          inset
          vertical
        />
      </v-slide-x-transition>
      <v-slide-x-transition>
        <div v-if="topBarAfterTitle">
          <component :is="topBarAfterTitle" />
        </div>
      </v-slide-x-transition>
      <v-spacer />
      <v-slide-x-transition>
        <div v-if="topBarBeforeUserName">
          <component :is="topBarBeforeUserName" />
        </div>
      </v-slide-x-transition>
      <v-slide-x-transition>
        <v-divider
          v-if="topBarBeforeUserName"
          inset
          vertical
        />
      </v-slide-x-transition>
      <user-status />
      <github-button />
    </v-app-bar>
    <v-content>
      <nuxt />
    </v-content>
    <v-footer
      app
      inset
      absolute
    >
      <span class="text-body-2">&copy; 2018 - {{ buildYear }} Shumin Liu and Contributors</span>
    </v-footer>
    <message-panel />
  </v-app>
</template>

<script>
import _ from 'lodash'

import { getDateFromTimestamp } from '~/components/utils'
import UserStatus from '~/components/UserStatus'
import GithubButton from '~/components/GithubButton'
import { PROJECT_TITLE } from '~/components/consts'
import MessagePanel from '~/components/MessagePanel'

export default {
  components: {
    UserStatus,
    GithubButton,
    MessagePanel,
  },
  provide() {
    return {
      changeLayoutConfig: this.changeLayoutConfig,
    }
  },
  data() {
    return {
      drawer: null,
      title: PROJECT_TITLE,
      buildYear: getDateFromTimestamp(this.$env.BUILD_TIME).getFullYear(),
      topBarBeforeTitle: null,
      topBarAfterTitle: null,
      topBarBeforeUserName: null,
    }
  },
  watch: {
    $route() {
      this.changeLayoutConfig()
    },
  },
  computed: {
    items() {
      const logined = this.$store.state.session.login
      return _.filter(
        [
          { icon: 'home', title: 'Homepage', to: '/' },
          { icon: 'code', title: 'Statistics', to: '/statistics' },
          { icon: 'history', title: 'History', to: '/history', needLogin: true },
          { icon: 'settings', title: 'Settings', to: '/settings', needLogin: true },
          { icon: 'info', title: 'About', to: '/about' },
        ],
        item => !item.needLogin || logined,
      )
    },
  },
  methods: {
    changeLayoutConfig(config = {}) {
      this.title = config.title || PROJECT_TITLE
      this.topBarBeforeTitle = config.topBarBeforeTitle || null
      this.topBarAfterTitle = config.topBarAfterTitle || null
      this.topBarBeforeUserName = config.topBarBeforeUserName || null
    },
  },
}
</script>
