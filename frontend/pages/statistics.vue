<template>
  <v-container
    grid-list-md
    v-resize="onResize"
  >
    <v-row
      wrap
      no-gutters
    >
      <v-col
        :md="3"
        class="pt-0 pb-0"
      >
        <v-text-field
          v-model="username"
          label="Set all usernames"
          :disabled="isWorking"
          required
          @keyup.enter="runWorker"
          :loading="isWorking"
          style="min-width: 240px"
        >
          <template #progress>
            <v-progress-linear
              color="primary"
              :value="notWorkingRate"
              :height="3"
              absolute
            />
          </template>
        </v-text-field>
      </v-col>
      <v-btn
        class="ma-1 primary"
        @click="runWorker"
        :disabled="isWorking"
      >
        query
      </v-btn>
      <v-tooltip bottom>
        <template #activator="{ on }">
          <v-btn
            class="ma-1 error"
            v-on="on"
            @click="clearWorkers"
          >
            reset
          </v-btn>
        </template>
        Clear usernames and reset all worker status
      </v-tooltip>
      <v-tooltip bottom>
        <template #activator="{ on }">
          <v-btn
            class="ma-1"
            v-on="on"
            @click="loadUsername"
            :disabled="loading"
          >
            reload
          </v-btn>
        </template>
        Load saved usernames from {{ login ? 'server' : 'your browser' }}
      </v-tooltip>
      <v-tooltip bottom>
        <template #activator="{ on }">
          <v-btn
            @click="saveUsername"
            v-on="on"
            :loading="savingUsername"
            :disable="savingUsername"
            :class="{ green: savingUsername, 'ma-1': true }"
          >
            save username
            <template #loader>
              <span>
                Success!
              </span>
            </template>
          </v-btn>
        </template>
        Save usernames to {{ login ? 'server' : 'your browser' }} to restore them when opening the page.
        <br>
        (Using QUERY button can also save usernames.)
      </v-tooltip>
      <v-spacer />
      <v-tooltip
        v-if="submissionsNum"
        buttom
      >
        <template #activator="{ on }">
          <v-chip
            label
            color="grey lighten-3"
            class="ma-1 elevation-2"
            v-on="on"
            @click.stop="openSummary"
          >
            <v-icon
              v-if="notWorkingRate >= 100"
              class="mr-1"
            >
              mdi-file-chart
            </v-icon>
            <span class="title">
              SOLVED: {{ solvedNum }} / SUBMISSION: {{ submissionsNum }}
            </span>
          </v-chip>
        </template>
        <template v-if="login">
          Click to view summary after query finishing.
        </template>
        <template v-else>
          Login to view summary.
        </template>
      </v-tooltip>
    </v-row>
    <v-row>
      <advertisement id="135755353" />
    </v-row>
    <v-row v-if="loading">
      <v-spacer />
      <v-progress-circular
        :size="100"
        color="primary"
        indeterminate
        class="mt-10"
      />
      <v-spacer />
    </v-row>
    <v-row
      v-else
      ref="layout"
      :style="{height: layoutHeight+'px'}"
    >
      <div
        v-for="(item,i) in workers"
        :key="item.key"
        :style="{
          width: columnWidth+'px',
          transform: workerTransform[item.key] || '',
        }"
        class="worker"
      >
        <worker-card
          :index="i"
          :ref="'worker-'+item.key"
          @update-height="height=>onResizeWorker(height,item)"
        />
      </div>
    </v-row>
  </v-container>
</template>

<script>
import { mapGetters, mapState } from 'vuex'
import _ from 'lodash'

import WorkerCard from '~/components/WorkerCard'
import statisticsLayoutBuilder from '~/components/statisticsLayoutBuilder'
import Store from '~/store/-dynamic/statistics'
import { PROJECT_TITLE, WORKER_STATUS } from '~/components/consts'
import { getAbpErrorMessage, delay } from '~/components/utils'
import Advertisement from '~/components/Advertisement.vue'

// keep it when url change
let globalLastSavedQueryId = null

export default {
  components: {
    WorkerCard,
    Advertisement,
  },
  inject: ['changeLayoutConfig'],
  head: {
    title: `Statistics - ${PROJECT_TITLE}`,
  },
  fetch() {
    // free plan of heroku service sleeps when it is inactive for 30 minutes
    // request cors-proxy to wake it up
    this.$axios.get('https://acm-statistics-cors.herokuapp.com')
  },
  async created() {
    if (!this.$store.hasModule('statistics')) {
      this.$store.registerModule('statistics', Store, { preserveState: false })
      await this.loadUsername()
    } else {
      this.loading = false
    }

    if (globalLastSavedQueryId) {
      this.lastSavedQueryId = globalLastSavedQueryId
    }

    // registered module should not use watch in component
    // use the watch method below instead
    this.watchFunc = this.$store.watch(
      () => _.map(this.$store.state.statistics.workers, 'status'),
      async () => {
        if (
          this.$store.state.session.settings['App.AutoSaveHistory'] === 'true'
        ) {
          if (this.isWorking) {
            this.lastSavedQueryId = null
          } else {
            this.lastSavedQueryId = await this.saveHistory()
          }
        }
      },
    )
  },
  destroyed() {
    if (this.watchFunc) {
      this.watchFunc()
    }
  },
  async mounted() {
    this.changeLayoutConfig({
      title: 'Statistics',
    })

    this.onResize()
  },
  data() {
    return {
      // 一共有几列
      columnCount: 3,
      columnWidth: 0,
      // 管理保存用户名按钮的动画
      savingUsername: false,
      loading: true,
      layoutHeight: 0,
      workerTransform: {},
      workerHeight: {},
      lastSavedQueryId: null,
      watchFunc: null,
    }
  },
  computed: {
    ...mapGetters('statistics', [
      'solvedNum',
      'submissionsNum',
      'isWorking',
      'notWorkingRate',
      'workerIdxOfCrawler',
    ]),
    ...mapState('session', ['login']),
    username: {
      get() {
        return this.$store.state.statistics.mainUsername
      },
      set: _.debounce(function (username) {
        this.$store.dispatch('statistics/updateMainUsername', { username })
      }, 300),
    },
    workers() {
      return this.$store.state.statistics.workers
    },
  },
  watch: {
    workerHeight: {
      handler() {
        this.repositionWorkers()
      },
      deep: true,
    },
    lastSavedQueryId(val) {
      globalLastSavedQueryId = val
    },
  },
  methods: {
    async updateLayoutSize() {
      while (!this.$refs.layout) {
        await delay(50)
      }
      this.columnWidth = this.$refs.layout.clientWidth / this.columnCount
    },
    onResizeWorker(height, worker) {
      this.$set(this.workerHeight, worker.key, height)
    },
    async repositionWorkers() {
      await this.$nextTick()

      let maxHeight = 0
      const layout = statisticsLayoutBuilder(this.workers, this.columnCount)

      for (const colIdx in layout) {
        const col = layout[colIdx]
        let offset = 0
        for (const workerIdx of col) {
          const key = this.workers[workerIdx].key

          const x = colIdx * this.columnWidth
          const y = offset
          this.$set(this.workerTransform, key, `translate(${x}px,${y}px)`)

          offset += (this.workerHeight[key] || 0) + 8
        }
        maxHeight = Math.max(maxHeight, offset)
      }

      this.layoutHeight = maxHeight
    },
    runWorker() {
      this.saveUsername()

      this.$store.dispatch('statistics/startAll')
    },
    async onResize() {
      const width = window.innerWidth
      if (width < 600) {
        this.columnCount = 1 // xs
      } else if (width < 960) {
        this.columnCount = 2 // sm
      } else if (width < 1264) {
        this.columnCount = 2 // md
      } else if (width < 1904) {
        this.columnCount = 3 // lg
      } else {
        this.columnCount = 4 // xl
      }
      await this.$nextTick()
      this.updateLayoutSize()
      this.repositionWorkers()
    },
    /**
     * 将用户名的情况存储进 localStorage 里面
     */
    async saveUsername() {
      this.savingUsername = true
      // 至少等待2秒
      await Promise.all([
        this.$store.dispatch('statistics/saveUsernames'),
        new Promise((resolve) => setTimeout(resolve, 2000)),
      ])
      // 至少等待2秒
      this.savingUsername = false
    },
    /**
     * 从 localStorage 读取用户名情况，输入进 worker 中
     */
    async loadUsername() {
      this.loading = true
      await this.$store.dispatch('statistics/loadUsernames')
      this.loading = false
    },
    clearWorkers() {
      this.$store.dispatch('statistics/clearWorkers')
    },
    async saveHistory() {
      const doneWorkers = _.filter(
        this.workers,
        (worker) => worker.status === WORKER_STATUS.DONE,
      )
      if (doneWorkers.length === 0) {
        return null
      }

      try {
        const saveResult = await this.$axios.$post(
          '/api/services/app/QueryHistory/SaveOrReplaceQueryHistory',
          {
            mainUsername: this.username,
            queryWorkerHistories: _.map(doneWorkers, (worker) => {
              const crawler = this.$store.state.statistics.crawlers[
                worker.crawlerName
              ]
              const hasErrorMessage =
                !_.isNil(worker.errorMessage) && worker.errorMessage !== ''
              const history = {
                crawlerName: worker.crawlerName,
                username: worker.username,
                errorMessage: hasErrorMessage ? worker.errorMessage : null,
                submission: worker.submissions,
                solved: worker.solved,
                solvedList: hasErrorMessage ? null : worker.solvedList,
                submissionsByCrawlerName: worker.submissionsByCrawlerName,
                isVirtualJudge: crawler.virtual_judge || false,
              }
              return history
            }),
          },
        )
        return saveResult.result.queryHistoryId
      } catch (err) {
        this.$store.commit('message/addError', getAbpErrorMessage(err))
      }
    },
    async openSummary() {
      if (this.notWorkingRate < 100 || !this.login) {
        return
      }

      if (!this.lastSavedQueryId) {
        this.lastSavedQueryId = await this.saveHistory()
      }
      this.$router.push('/history/' + this.lastSavedQueryId)
    },
  },
}
</script>

<style scoped>
.worker {
  position: absolute;
  transition: ease-in-out 300ms;
  padding-left: 4px;
  padding-right: 4px;
}
</style>
