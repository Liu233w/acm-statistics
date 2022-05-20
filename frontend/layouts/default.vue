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
            <v-icon>{{ item.icon }}</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title>{{ item.title }}</v-list-item-title>
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
      <v-toolbar-title>{{ title }}</v-toolbar-title>
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
          { icon: 'mdi-home', title: 'Homepage', to: '/' },
          { icon: 'mdi-code-tags', title: 'Statistics', to: '/statistics' },
          { icon: 'mdi-history', title: 'History', to: '/history', needLogin: true },
          { icon: 'mdi-cog', title: 'Settings', to: '/settings', needLogin: true },
          { icon: 'mdi-information', title: 'About', to: '/about' },
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
