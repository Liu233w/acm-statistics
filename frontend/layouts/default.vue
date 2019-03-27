<template>
  <VApp>
    <VNavigationDrawer
      v-model="drawer"
      clipped
      fixed
      app
    >
      <VList>
        <VListTile
          nuxt
          :to="item.to"
          :key="i"
          v-for="(item, i) in items"
          exact
        >
          <VListTileAction>
            <VIcon v-html="item.icon" />
          </VListTileAction>
          <VListTileContent>
            <VListTileTitle v-text="item.title" />
          </VListTileContent>
        </VListTile>
      </VList>
    </VNavigationDrawer>
    <VToolbar fixed app clipped-left>
      <VToolbarSideIcon @click="drawer = !drawer" />
      <VToolbarTitle v-text="title" />
      <VSpacer />
      <VTooltip left>
        <VBtn icon slot="activator" href="https://github.com/Liu233w/acm-statistics" target="_blank">
          <VIcon medium>
            fa fa-github
          </VIcon>
        </VBtn>
        <span>Star me on Github!</span>
      </VTooltip>
    </VToolbar>
    <VContent>
      <VContainer>
        <Nuxt />
      </VContainer>
    </VContent>
    <VFooter fixed app>
      <span>&copy; 2018 - {{ new Date($env.BUILD_TIME).getFullYear() }} NWPU-ACM 技术组</span>
      <VSpacer />
      <span>陕ICP备17008184号</span>
    </VFooter>
  </VApp>
</template>

<script>
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
      }
    },
  }
</script>
