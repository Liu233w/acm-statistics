<template>
  <v-container
    grid-list-md
    v-resize="onResize"
  >
    <v-layout wrap>
      <v-text-field
        v-model="username"
        label="Set all usernames"
        :disabled="isWorking"
        required
        @keyup.enter="runWorker"
        :loading="isWorking"
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
            @click.stop="openDialog"
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
    </v-layout>
    <v-layout v-if="loading">
      <v-spacer />
      <v-progress-circular
        :size="100"
        color="primary"
        indeterminate
        class="mt-10"
      />
      <v-spacer />
    </v-layout>
    <v-layout
      v-else
      ref="layout"
      :style="{height: layoutHeight+'px'}"
    >
      <template v-for="column in workerLayout">
        <v-flex
          v-for="(item, i) in column"
          :key="item.key"
          :style="{
            width: columnWidth+'px',
            'z-index': (i===0||i===column.length-1 ? 2 : 1),
            transform: workerTransform[item.key] || '',
          }"
          class="worker"
        >
          <worker-card
            :index="item.index"
            :ref="'worker-'+item.key"
            @update-height="height=>onResizeWorker(height,item)"
          />
        </v-flex>
      </template>
    </v-layout>
  </v-container>
</template>

<script>
import { mapGetters, mapState } from 'vuex'
import _ from 'lodash'

import WorkerCard from '~/components/WorkerCard'
import statisticsLayoutBuilder from '~/components/statisticsLayoutBuilder'
import Store from '~/store/-dynamic/statistics'
import { PROJECT_TITLE, WORKER_STATUS } from '~/components/consts'
import { getAbpErrorMessage } from '~/components/utils'

export default {
  components: {
    WorkerCard,
  },
  inject: ['changeLayoutConfig'],
  head: {
    title: `Statistics - ${PROJECT_TITLE}`,
  },
  created() {
    this.$store.registerModule('statistics', Store, { preserveState: false })
  },
  destroyed() {
    if (this.unSubscribeFunc) {
      this.unSubscribeFunc()
    }
    this.$store.unregisterModule('statistics')
  },
  async mounted() {
    this.changeLayoutConfig({
      title: 'Statistics',
    })

    await this.loadUsername()
    this.loading = false

    this.unSubscribeFunc = this.$store.subscribeAction(action => {
      if (_.includes([
        'statistics/clearWorkers',
        'statistics/addWorkerForCrawler',
        'statistics/removeWorkerAtIndex',
      ], action.type)) {
        this.repositionWorkers()
      }
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
      unSubscribeFunc: null,
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
    ...mapState('session', [
      'login',
    ]),
    username: {
      get() {
        return this.$store.state.statistics.mainUsername
      },
      set: _.debounce(function (username) {
        this.$store.dispatch('statistics/updateMainUsername', { username })
      }, 300),
    },
    workerLayout() {
      const workers = this.$store.state.statistics.workers
      return statisticsLayoutBuilder(workers, this.columnCount)
    },
    maxItemPerColumn() {
      return Math.ceil(this.$store.state.statistics.workers.length / this.columnCount)
    },
    updateDate() {
      if (this.notWorkingRate >= 100) {
        return new Date()
      } else {
        return null
      }
    },
  },
  watch: {
    workerHeight: {
      handler() {
        this.repositionWorkers()
      },
      deep: true,
    },
  },
  methods: {
    updateLayoutSize() {
      if (this.$refs.layout) {
        this.columnWidth = this.$refs.layout.clientWidth / this.columnCount
      }
    },
    onResizeWorker(height, worker) {
      console.log(height)
      this.$set(this.workerHeight, worker.key, height)
    },
    async repositionWorkers() {
      await this.$nextTick()
      let maxHeight = 0

      for (const colIdx in this.workerLayout) {
        const col = this.workerLayout[colIdx]
        let offset = 0
        for (const worker of col) {
          const key = worker.key

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
        this.columnCount = 1 // sm
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
        new Promise(resolve => setTimeout(resolve, 2000)),
      ])
      // 至少等待2秒
      this.savingUsername = false
    },
    /**
     * 从 localStorage 读取用户名情况，输入进 worker 中
     */
    loadUsername() {
      return this.$store.dispatch('statistics/loadUsernames')
    },
    clearWorkers() {
      this.$store.dispatch('statistics/clearWorkers')
    },
    async openDialog() {
      if (this.notWorkingRate < 100 || !this.login) {
        return
      }

      const doneWorkers = _.filter(this.$store.state.statistics.workers,
        worker => worker.status === WORKER_STATUS.DONE)
      try {
        const saveResult = await this.$axios.$post('/api/services/app/QueryHistory/SaveOrReplaceQueryHistory', {
          mainUsername: this.username,
          queryWorkerHistories:
            _.map(doneWorkers, worker => {
              const crawler = this.$store.state.statistics.crawlers[worker.crawlerName]
              const hasErrorMessage = !_.isNil(worker.errorMessage) && worker.errorMessage !== ''
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
        })
        this.$router.push('/history/' + saveResult.result.queryHistoryId)
      } catch (err) {
        this.$store.commit('message/addError', getAbpErrorMessage(err))
      }
    },
  },
}
</script>

<style scoped>
.worker {
  position: absolute;
  transition: ease-in-out 300ms;
}
</style>
