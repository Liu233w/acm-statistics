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
            <v-toolbar-title>About Us</v-toolbar-title>
            <v-spacer />
          </v-app-bar>
          <v-list>
            <v-list-item
              href="https://github.com/Liu233w/acm-statistics/blob/master/README.md#contributors-"
              target="_blank"
              rel="noopener noreferrer"
            >
              <v-list-item-content>
                <v-list-item-title>Developed By</v-list-item-title>
                <v-list-item-sub-title>Shumin Liu and Contributors</v-list-item-sub-title>
              </v-list-item-content>
              <v-list-item-action>
                <v-icon class="grey--text">
                  mdi-open-in-new
                </v-icon>
              </v-list-item-action>
            </v-list-item>
            <v-list-item href="mailto:mail@ojhunt.com">
              <v-list-item-action>
                <v-icon class="blue--text text--lighten-2">
                  email
                </v-icon>
              </v-list-item-action>
              <v-list-item-content>
                <v-list-item-sub-title>mail@ojhunt.com</v-list-item-sub-title>
              </v-list-item-content>
            </v-list-item>
            <v-list-item
              href="https://github.com/Liu233w/acm-statistics"
              target="_blank"
              rel="noopener noreferrer"
            >
              <v-list-item-action>
                <v-icon class="blue--text text--lighten-2">
                  mdi-github
                </v-icon>
              </v-list-item-action>
              <v-list-item-content>
                <v-list-item-sub-title>https://github.com/Liu233w/acm-statistics</v-list-item-sub-title>
              </v-list-item-content>
              <v-list-item-action>
                <v-icon class="grey--text">
                  mdi-open-in-new
                </v-icon>
              </v-list-item-action>
            </v-list-item>
            <v-list-item
              href="https://t.me/ojhunt"
              target="_blank"
              rel="noopener noreferrer"
            >
              <v-list-item-action>
                <v-icon class="blue--text text--lighten-2">
                  mdi-telegram
                </v-icon>
              </v-list-item-action>
              <v-list-item-content>
                <v-list-item-sub-title>Telegram: https://t.me/ojhunt</v-list-item-sub-title>
              </v-list-item-content>
              <v-list-item-action>
                <v-icon class="grey--text">
                  mdi-open-in-new
                </v-icon>
              </v-list-item-action>
            </v-list-item>
            <v-list-item
              href="https://join.slack.com/t/acm-statistics/shared_invite/zt-geel58eq-MjD3tBukUAC4ZSCS4DoLTw"
              target="_blank"
              rel="noopener noreferrer"
            >
              <v-list-item-action>
                <v-icon class="blue--text text--lighten-2">
                  mdi-slack
                </v-icon>
              </v-list-item-action>
              <v-list-item-content>
                <v-list-item-sub-title>Slack: ACM Statistics Development</v-list-item-sub-title>
              </v-list-item-content>
              <v-list-item-action>
                <v-icon class="grey--text">
                  mdi-open-in-new
                </v-icon>
              </v-list-item-action>
            </v-list-item>
          </v-list>
        </v-card>
        <v-card class="my-2">
          <v-app-bar
            color="light-blue"
            dark
          >
            <v-toolbar-title>Development</v-toolbar-title>
            <v-spacer />
          </v-app-bar>
          <v-list>
            <v-list-item
              href="/swagger"
              target="_blank"
              rel="noopener noreferrer"
            >
              <v-list-item-action>
                <v-icon class="blue--text text--lighten-2">
                  mdi-api
                </v-icon>
              </v-list-item-action>
              <v-list-item-content>
                <v-list-item-sub-title>API Document</v-list-item-sub-title>
              </v-list-item-content>
              <v-list-item-action>
                <v-icon class="grey--text">
                  mdi-open-in-new
                </v-icon>
              </v-list-item-action>
            </v-list-item>
          </v-list>
        </v-card>
        <v-card class="my-2">
          <v-app-bar
            color="light-blue"
            dark
          >
            <v-toolbar-title>Special Thanks To</v-toolbar-title>
            <v-spacer />
          </v-app-bar>
          <v-list>
            <v-list-item>
              <v-list-item-content>
                <v-list-item-title>西北工业大学ACM协会技术组</v-list-item-title>
              </v-list-item-content>
            </v-list-item>
            <v-list-item>
              <v-list-item-action>
                <v-icon class="blue--text text--lighten-2">
                  place
                </v-icon>
              </v-list-item-action>
              <v-list-item-content>
                <v-list-item-sub-title>陕西省西安市长安区西北工业大学长安校区计算机学院110室</v-list-item-sub-title>
              </v-list-item-content>
            </v-list-item>
            <v-list-item
              href="https://jq.qq.com/?_wv=1027&k=5qgzfQc"
              target="_blank"
              rel="noopener noreferrer"
            >
              <v-list-item-action>
                <v-icon class="blue--text text--lighten-2">
                  mdi-qqchat
                </v-icon>
              </v-list-item-action>
              <v-list-item-content>
                <v-list-item-sub-title>198820787</v-list-item-sub-title>
              </v-list-item-content>
            </v-list-item>
            <v-list-item @click="dialog = !dialog">
              <v-list-item-action>
                <v-icon class="blue--text text--lighten-2">
                  mdi-wechat
                </v-icon>
              </v-list-item-action>
              <v-list-item-content>
                <v-list-item-title>微信公众号</v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </v-list>
        </v-card>
        <v-card class="my-2">
          <v-app-bar
            color="light-blue"
            dark
          >
            <v-toolbar-title>System Info</v-toolbar-title>
            <v-spacer />
          </v-app-bar>
          <v-list>
            <v-list-item>
              <v-list-item-content>
                <v-list-item-title>Version</v-list-item-title>
                <v-list-item-sub-title>{{ $env.VERSION_NUM }}</v-list-item-sub-title>
              </v-list-item-content>
            </v-list-item>

            <v-list-item>
              <v-list-item-content>
                <v-list-item-title>Build time</v-list-item-title>
                <v-list-item-sub-title>{{ buildTime }}</v-list-item-sub-title>
              </v-list-item-content>
            </v-list-item>

            <v-list-item :href="`https://github.com/Liu233w/acm-statistics/tree/${$env.VERSION_NUM}`">
              <v-list-item-content>
                <v-list-item-title>Source code of current version</v-list-item-title>
                <v-list-item-sub-title>
                  https://github.com/Liu233w/acm-statistics/tree/{{ $env.VERSION_NUM }}
                </v-list-item-sub-title>
              </v-list-item-content>
              <v-list-item-action>
                <v-icon class="grey--text">
                  mdi-open-in-new
                </v-icon>
              </v-list-item-action>
            </v-list-item>

            <v-list-item :href="`https://github.com/Liu233w/acm-statistics/commits/${$env.VERSION_NUM}`">
              <v-list-item-content>
                <v-list-item-title>Recent updates</v-list-item-title>
                <v-list-item-sub-title>
                  https://github.com/Liu233w/acm-statistics/commits/{{ $env.VERSION_NUM }}
                </v-list-item-sub-title>
              </v-list-item-content>
              <v-list-item-action>
                <v-icon class="grey--text">
                  mdi-open-in-new
                </v-icon>
              </v-list-item-action>
            </v-list-item>
          </v-list>
        </v-card>
      </v-flex>
    </v-layout>
    <v-dialog
      v-model="dialog"
      max-width="500px"
    >
      <v-card>
        <v-card-title>
          <span>微信公众号</span>
        </v-card-title>
        <v-card-text>
          <img
            :src="require('~/assets/img/wechat.jpg')"
            alt="公众号二维码"
            style="height: 300px"
          >
        </v-card-text>
        <v-card-actions>
          <v-btn
            color="primary"
            text
            @click.stop="dialog=false"
          >
            close
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script>
import { PROJECT_TITLE } from '~/components/consts'
import { getDateFromTimestamp } from '../components/utils'

export default {
  inject: ['changeLayoutConfig'],
  data() {
    return {
      buildTime: getDateFromTimestamp(this.$env.BUILD_TIME).toLocaleString(),
      dialog: false,
    }
  },
  head: {
    title: `About - ${PROJECT_TITLE}`,
  },
  mounted() {
    this.changeLayoutConfig({
      title: 'About',
    })
  },
}
</script>
