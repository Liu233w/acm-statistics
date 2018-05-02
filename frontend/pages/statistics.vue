<template>
  <v-container grid-list-md>
    <v-layout row>
      <v-flex xs8>
        <v-text-field
          v-model="username"
          label="统一设置用户名"
          :disabled="isWorking"
          required
          @keyup.enter="runWorker"
        />
      </v-flex>
      <v-flex xs4>
        <v-btn class="primary" @click="runWorker" :disabled="isWorking">
          开始查询
        </v-btn>
        <v-tooltip bottom>
          <v-btn @click="saveUsername"
                 slot="activator"
                 :loading="savingUsername"
                 :disable="savingUsername"
                 :class="{ green: savingUsername }"
          >
            保存用户名
            <span slot="loader">保存成功</span>
          </v-btn>
          将用户名保存到本地，下次打开页面的时候会自动填写
          <br>
          （使用“开始查询”按钮也会保存用户名）
        </v-tooltip>
      </v-flex>
    </v-layout>
    <v-layout row v-if="submissionsNum">
      <v-flex>
        <p class="title text-xs-center"> {{ username }} 的总题量： {{ solvedNum }} / {{ submissionsNum }} </p>
      </v-flex>
    </v-layout>
    <v-layout row>
      <v-flex>
        <v-progress-linear color="primary"
                           :value="notWorkingRate"
                           :active="isWorking"
        />
      </v-flex>
    </v-layout>
    <v-layout row>
      <v-flex xs12 sm6 md4 lg3
              v-for="colIdx in columnCount" :key="colIdx">
        <v-layout column>
          <v-flex v-for="idx in workerLength"
                  v-if="(idx-1) % columnCount === colIdx - 1"
                  :key="idx">
            <crawler-card :index="idx-1"/>
          </v-flex>
        </v-layout>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
  import {mapGetters} from 'vuex'
  import _ from 'lodash'

  import CrawlerCard from '~/components/CrawlerCard'
  import Store from '~/store/-dynamic/statistics'

  export default {
    name: 'Statistics',
    components: {
      CrawlerCard,
    },
    created() {
      this.$store.registerModule('statistics', Store)
      this.$store.dispatch('statistics/initWorkers', this.$crawlers)
    },
    destroyed() {
      this.$store.unregisterModule('statistics')
    },
    mounted() {
      this.onResize()
      window.addEventListener('resize', this.onResize, {passive: true})
      this.loadUsername()
    },
    data() {
      return {
        // 一共有几列
        columnCount: 3,
        // 管理保存用户名按钮的动画
        savingUsername: false,
      }
    },
    computed: {
      ...mapGetters('statistics', [
        'solvedNum',
        'submissionsNum',
        'isWorking',
        'notWorkingRate',
      ]),
      username: {
        get() {
          return this.$store.state.statistics.mainUsername
        },
        set: _.debounce(function (username) {
          this.$store.dispatch('statistics/updateMainUsername', {username})
        }, 300),
      },
      workerLength() {
        return this.$store.state.statistics.workers.length
      },
    },
    methods: {
      runWorker() {
        this.saveUsername()

        this.$store.dispatch('statistics/startAll')
      },
      onResize() {
        const width = window.innerWidth
        if (width < 600) {
          this.columnCount = 1 // xs
        } else if (width < 960) {
          this.columnCount = 2 // sm
        } else if (width < 1264) {
          this.columnCount = 3 // md
        } else {
          this.columnCount = 4 // lg xl
        }
      },
      /**
       * 将用户名的情况存储进 localStorage 里面
       */
      saveUsername() {
        this.$store.dispatch('statistics/saveUsernames')

        this.savingUsername = true
        // 播放2秒的保存动画
        setTimeout(() => {
          this.savingUsername = false
        }, 2000)
      },
      /**
       * 从 localStorage 读取用户名情况，输入进 worker 中
       */
      loadUsername() {
        this.$store.dispatch('statistics/loadUsernames')
      },
    },
  }
</script>
