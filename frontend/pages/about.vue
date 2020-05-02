<template>
  <v-container>
    <v-layout>
      <v-flex
        xs12
        sm6
        offset-sm3
      >
        <v-card class="my-2">
          <v-app-bar
            color="light-blue"
            dark
          >
            <v-toolbar-title>关于我们</v-toolbar-title>
            <v-spacer />
          </v-app-bar>
          <v-list>
            <v-list-item>
              <v-list-item-content>
                <v-list-item-title>西北工业大学ACM协会技术组</v-list-item-title>
              </v-list-item-content>
            </v-list-item>
            <v-list-item>
              <v-list-item-content>
                <v-list-item-title>地址</v-list-item-title>
                <v-list-item-sub-title>陕西省西安市长安区西北工业大学长安校区计算机学院110室</v-list-item-sub-title>
              </v-list-item-content>
            </v-list-item>
            <v-list-item>
              <v-list-item-content>
                <v-list-item-title>ACM协会QQ群</v-list-item-title>
                <v-list-item-sub-title>198820787</v-list-item-sub-title>
              </v-list-item-content>
            </v-list-item>
          </v-list>
        </v-card>
        <v-card class="my-2">
          <v-app-bar
            color="light-blue"
            dark
          >
            <v-toolbar-title>系统信息</v-toolbar-title>
            <v-spacer />
          </v-app-bar>
          <v-list>
            <v-list-item>
              <v-list-item-content>
                <v-list-item-title>版本号</v-list-item-title>
                <v-list-item-sub-title>{{ $env.VERSION_NUM }}</v-list-item-sub-title>
              </v-list-item-content>
            </v-list-item>

            <v-list-item>
              <v-list-item-content>
                <v-list-item-title>构建时间</v-list-item-title>
                <v-list-item-sub-title>{{ buildTime }}</v-list-item-sub-title>
              </v-list-item-content>
            </v-list-item>

            <v-list-item :href="`https://github.com/Liu233w/acm-statistics/tree/${$env.VERSION_NUM}`">
              <v-list-item-content>
                <v-list-item-title>当前版本源代码</v-list-item-title>
                <v-list-item-sub-title>
                  https://github.com/Liu233w/acm-statistics/tree/{{ $env.VERSION_NUM }}
                </v-list-item-sub-title>
              </v-list-item-content>
            </v-list-item>

            <v-list-item :href="`https://github.com/Liu233w/acm-statistics/commits/${$env.VERSION_NUM}`">
              <v-list-item-content>
                <v-list-item-title>最近更新</v-list-item-title>
                <v-list-item-sub-title>
                  https://github.com/Liu233w/acm-statistics/commits/{{ $env.VERSION_NUM }}
                </v-list-item-sub-title>
              </v-list-item-content>
            </v-list-item>
          </v-list>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import { PROJECT_TITLE } from '~/components/consts'
import { getDateFromTimestamp } from '../components/utils'

export default {
  data() {
    return {
      buildTime: getDateFromTimestamp(this.$env.BUILD_TIME).toLocaleString(),
    }
  },
  head: {
    title: `关于 - ${PROJECT_TITLE}`,
  },
}
</script>
