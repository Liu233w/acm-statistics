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
                <v-list-item-subtitle>Shumin Liu and Contributors</v-list-item-subtitle>
              </v-list-item-content>
              <v-list-item-action>
                <v-icon class="grey--text">
                  mdi-open-in-new
                </v-icon>
              </v-list-item-action>
            </v-list-item>
            <v-list-item href="mailto:support@ojhunt.com">
              <v-list-item-action>
                <v-icon class="blue--text text--lighten-2">
                  mdi-email
                </v-icon>
              </v-list-item-action>
              <v-list-item-content>
                <v-list-item-subtitle>support@ojhunt.com</v-list-item-subtitle>
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
                <v-list-item-subtitle>https://github.com/Liu233w/acm-statistics</v-list-item-subtitle>
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
                  fab fa-telegram
                </v-icon>
              </v-list-item-action>
              <v-list-item-content>
                <v-list-item-subtitle>Telegram: https://t.me/ojhunt</v-list-item-subtitle>
              </v-list-item-content>
              <v-list-item-action>
                <v-icon class="grey--text">
                  mdi-open-in-new
                </v-icon>
              </v-list-item-action>
            </v-list-item>
            <v-list-item
              href="https://discord.gg/cQemBNNr"
              target="_blank"
              rel="noopener noreferrer"
            >
              <v-list-item-action>
                <v-icon class="blue--text text--lighten-2">
                  mdi-discord
                </v-icon>
              </v-list-item-action>
              <v-list-item-content>
                <v-list-item-subtitle>Discord: OJ Tracker Development</v-list-item-subtitle>
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
                <v-list-item-subtitle>API Document</v-list-item-subtitle>
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
                  mdi-map-marker
                </v-icon>
              </v-list-item-action>
              <v-list-item-content>
                <v-list-item-subtitle>陕西省西安市长安区西北工业大学长安校区计算机学院110室</v-list-item-subtitle>
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
                <v-list-item-subtitle>198820787</v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
            <v-list-item @click="dialog = !dialog">
              <v-list-item-action>
                <v-icon class="blue--text text--lighten-2">
                  mdi-wechat
                </v-icon>
              </v-list-item-action>
              <v-list-item-content>
                <v-list-item-subtitle>微信公众号</v-list-item-subtitle>
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
                <v-list-item-subtitle>{{ $env.VERSION_NUM }}</v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>

            <v-list-item>
              <v-list-item-content>
                <v-list-item-title>Build time</v-list-item-title>
                <v-list-item-subtitle>{{ buildTime }}</v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>

            <v-list-item :href="`https://github.com/Liu233w/acm-statistics/tree/${$env.VERSION_NUM}`">
              <v-list-item-content>
                <v-list-item-title>Source code of current version</v-list-item-title>
                <v-list-item-subtitle>
                  https://github.com/Liu233w/acm-statistics/tree/{{ $env.VERSION_NUM }}
                </v-list-item-subtitle>
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
                <v-list-item-subtitle>
                  https://github.com/Liu233w/acm-statistics/commits/{{ $env.VERSION_NUM }}
                </v-list-item-subtitle>
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
