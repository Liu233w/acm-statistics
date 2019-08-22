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
    <VAppBar fixed app clipped-left>
      <VAppBarNavIcon @click="drawer = !drawer" />
      <VAppBarTitle v-text="title" />
      <VSpacer />
      <VTooltip left>
        <VBtn icon slot="activator" href="https://github.com/Liu233w/acm-statistics" target="_blank">
          <VIcon medium>
            fab fa-github
          </VIcon>
        </VBtn>
        <span>Star me on Github!</span>
      </VTooltip>
    </VAppBar>
    <VContent>
      <VContainer>
        <Nuxt />
      </VContainer>
    </VContent>
    <VFooter fixed app>
      <span>&copy; 2018 - {{ buildYear }} NWPU-ACM 技术组</span>
      <VSpacer />
      <span>陕ICP备17008184号</span>
    </VFooter>
  </VApp>
</template>

<script>
  import {getDateFromTimestamp} from '~/components/utils'

  export default {
    data() {
      return {
        drawer: true,
        items: [
          {icon: 'home', title: '返回主页', to: '/'},
          {icon: 'code', title: 'OJ题量统计', to: '/statistics'},
          {icon: 'info', title: '关于', to: '/about'},
        ],
        title: 'NWPU-ACM 查询系统',
        buildYear: getDateFromTimestamp(this.$env.BUILD_TIME).getFullYear(),
      }
    },
  }
</script>
