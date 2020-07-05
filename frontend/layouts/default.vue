<template>
  <v-app>
    <v-navigation-drawer
      v-model="drawer"
      clipped
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
      fixed
      app
      clipped-left
    >
      <v-app-bar-nav-icon @click="drawer = !drawer" />
      <v-toolbar-title v-text="title" />
      <v-spacer />
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
      <span class="body-2">&copy; 2018 - {{ buildYear }} NWPU-ACM 技术组</span>
      <v-spacer />
      <span class="body-2">陕ICP备17008184号</span>
    </v-footer>
  </v-app>
</template>

<script>
import _ from 'lodash'
import { getDateFromTimestamp } from '~/components/utils'
import UserStatus from '~/components/UserStatus'
import GithubButton from '~/components/GithubButton'

export default {
  components: {
    UserStatus,
    GithubButton,
  },
  data() {
    return {
      drawer: null,
      title: 'NWPU-ACM 查询系统',
      buildYear: getDateFromTimestamp(this.$env.BUILD_TIME).getFullYear(),
    }
  },
  computed: {
    items() {
      const logined = this.$store.state.session.login
      return _.filter(
        [
          { icon: 'home', title: 'Homepage', to: '/' },
          { icon: 'code', title: 'AC Statistics', to: '/statistics' },
          { icon: 'history', title: 'History', to: '/history', needLogin: true },
          { icon: 'settings', title: 'Settings', to: '/settings', needLogin: true },
          { icon: 'info', title: 'About', to: '/about' },
        ],
        item => !item.needLogin || logined,
      )
    },
  },
}
</script>
