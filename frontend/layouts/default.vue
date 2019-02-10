<template>
  <v-app>
    <v-navigation-drawer
      v-model="drawer"
      clipped
      fixed
      app
    >
      <v-list>
        <v-list-tile
          nuxt
          :to="item.to"
          :key="i"
          v-for="(item, i) in items"
          exact
        >
          <v-list-tile-action>
            <v-icon v-html="item.icon" />
          </v-list-tile-action>
          <v-list-tile-content>
            <v-list-tile-title v-text="item.title" />
          </v-list-tile-content>
        </v-list-tile>
      </v-list>
    </v-navigation-drawer>
    <v-toolbar fixed app clipped-left>
      <v-toolbar-side-icon @click="drawer = !drawer" />
      <v-toolbar-title v-text="title" />
      <v-spacer />
      <v-tooltip left>
        <v-btn icon slot="activator" href="https://github.com/Liu233w/acm-statistics" target="_blank">
          <v-icon medium>
            fa fa-github
          </v-icon>
        </v-btn>
        <span>Star me on Github!</span>
      </v-tooltip>
    </v-toolbar>
    <v-content>
      <v-container>
        <nuxt />
      </v-container>
    </v-content>
    <v-footer fixed app>
      <span>&copy; 2018 NWPU-ACM 技术组</span>
      <v-spacer />
      <span>陕ICP备17008184号</span>
    </v-footer>
  </v-app>
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
