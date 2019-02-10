<template>
  <v-container>
    <v-layout row>
      <v-flex xs12 sm6 offset-sm3>
        <v-card>
          <v-toolbar color="light-blue" dark>
            <v-toolbar-title>关于</v-toolbar-title>
            <v-spacer />
          </v-toolbar>
          <v-list>
            <v-list-tile>
              <v-list-tile-content>
                <v-list-tile-title>版本号</v-list-tile-title>
                <v-list-tile-sub-title>{{ $env.VERSION_NUM }}</v-list-tile-sub-title>
              </v-list-tile-content>
            </v-list-tile>

            <v-list-tile :href="`https://github.com/Liu233w/acm-statistics/tree/${$env.VERSION_NUM}`">
              <v-list-tile-content>
                <v-list-tile-title>当前版本源代码</v-list-tile-title>
                <v-list-tile-sub-title>
                  https://github.com/Liu233w/acm-statistics/tree/{{ $env.VERSION_NUM }}
                </v-list-tile-sub-title>
              </v-list-tile-content>
            </v-list-tile>

            <v-list-tile :href="`https://github.com/Liu233w/acm-statistics/commit/${$env.VERSION_NUM}`">
              <v-list-tile-content>
                <v-list-tile-title>最近更新</v-list-tile-title>
                <v-list-tile-sub-title>
                  https://github.com/Liu233w/acm-statistics/commit/{{ $env.VERSION_NUM }}
                </v-list-tile-sub-title>
              </v-list-tile-content>
            </v-list-tile>
          </v-list>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
  import {PROJECT_TITLE} from '~/components/consts'

  export default {
    head: {
      title: `关于 - ${PROJECT_TITLE}`,
    },
  }
</script>

<style scoped>

</style>
