<template>
  <VApp>
    <VNavigationDrawer
      v-model="drawer"
      clipped
      fixed
      app
    >
      <VList>
        <VListItem
          nuxt
          :to="item.to"
          :key="i"
          v-for="(item, i) in items"
          exact
        >
          <VListItemAction>
            <VIcon v-html="item.icon" />
          </VListItemAction>
          <VListItemContent>
            <VListItemTitle v-text="item.title" />
          </VListItemContent>
        </VListItem>
      </VList>
    </VNavigationDrawer>
    <VAppBar
      fixed
      app
      clipped-left
    >
      <VAppBarNavIcon @click="drawer = !drawer" />
      <VToolbarTitle v-text="title" />
      <VSpacer />
      <UserStatus />
      <VTooltip left>
        <template #activator="{ on }">
          <VBtn
            icon
            v-on="on"
            href="https://github.com/Liu233w/acm-statistics"
            target="_blank"
            rel="noopener noreferrer"
          >
            <VIcon medium>
              fab fa-github
            </VIcon>
          </VBtn>
        </template>
        <span>Star me on Github!</span>
      </VTooltip>
    </VAppBar>
    <VContent>
      <Nuxt />
    </VContent>
    <VFooter
      fixed
      app
    >
      <span class="body-2">&copy; 2018 - {{ buildYear }} NWPU-ACM 技术组</span>
      <VSpacer />
      <span class="body-2">陕ICP备17008184号</span>
    </VFooter>
  </VApp>
</template>

<script>
import _ from 'lodash'
import { getDateFromTimestamp } from '~/components/utils'
import UserStatus from '~/components/UserStatus'

export default {
  components: {
    UserStatus,
  },
  data() {
    return {
      drawer: true,
      title: 'NWPU-ACM 查询系统',
      buildYear: getDateFromTimestamp(this.$env.BUILD_TIME).getFullYear(),
    }
  },
  computed: {
    items() {
      return _.filter(
        [
          { icon: 'home', title: '返回主页', to: '/' },
          { icon: 'code', title: 'OJ题量统计', to: '/statistics' },
          ifTrue(this.$store.state.session.login, {
            icon: 'settings',
            title: '用户设置',
            to: '/settings',
          }),
          { icon: 'info', title: '关于', to: '/about' },
        ],
        item => item !== null,
      )
    },
  },
}

function ifTrue(condition, item) {
  if (condition) {
    return item
  } else {
    return null
  }
}
</script>
