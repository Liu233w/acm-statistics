<template>
  <v-container
    grid-list-md
    v-resize="onResize"
  >
    <v-layout wrap>
      <v-flex
        xs12
        sm5
        md3
      >
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
      </v-flex>
      <v-flex
        xs12
        sm7
        md5
      >
        <v-btn
          class="primary"
          @click="runWorker"
          :disabled="isWorking"
        >
          query
        </v-btn>
        <v-tooltip bottom>
          <template #activator="{ on }">
            <v-btn
              class="error"
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
              :class="{ green: savingUsername }"
            >
              save username
              <template #loader>
                <span>
                  Success!
                </span>
              </template>
            </v-btn>
          </template>
          Save usernames to {{ $store.state.session.login ? 'server' : 'your browser' }} to restore them when opening the page.
          <br>
          (Using QUERY button can also save usernames.)
        </v-tooltip>
      </v-flex>
      <v-flex
        xs12
        md4
        v-show="submissionsNum"
      >
        <v-tooltip buttom>
          <template #activator="{ on }">
            <v-chip
              label
              color="grey lighten-3"
              class="elevation-2"
              v-on="on"
              @click.stop="openDialog"
            >
              <span class="title">
                SOLVED: {{ solvedNum }} / SUBMISSION: {{ submissionsNum }}
              </span>
            </v-chip>
          </template>
          Click to open summary panel after query finishing.
        </v-tooltip>
      </v-flex>
    </v-layout>
    <v-layout>
      <v-flex
        xs12
        sm12
        md6
        lg4
        xl3
        v-for="(column, idx) in workerLayout"
        :key="idx"
      >
        <v-layout column>
          <transition-group
            name="workers-column"
            @before-leave="itemBeforeLeaveTransition"
          >
            <div
              v-for="(item, itemIdx) in column"
              :key="item.key"
              :style="{'z-index': 1000 - itemIdx}"
              class="flex worker-item"
              :data-column-idx="idx"
              :data-column-item-idx="itemIdx"
            >
              <worker-card :index="item.index" />
            </div>
          </transition-group>
        </v-layout>
      </v-flex>
    </v-layout>
    <v-dialog
      v-model="dialog"
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
    >
      <v-card>
        <v-toolbar
          dark
          color="primary"
        >
          <v-btn
            icon
            dark
            @click="dialog = false"
          >
            <v-icon>mdi-close</v-icon>
          </v-btn>
          <v-toolbar-title>Summary</v-toolbar-title>
          <v-spacer />
          <v-toolbar-items>
            <v-tooltip>
              <template #activator="{ on }">
                <v-btn
                  dark
                  text
                  v-on="on"
                  @click="printPage"
                >
                  Print Page
                </v-btn>
              </template>
              Print current page (select "print to pdf" to save a pdf copy)
            </v-tooltip>
          </v-toolbar-items>
        </v-toolbar>
        <v-card-text
          v-if="!$store.state.session.login"
          class="text-center"
        >
          <p class="title text--primary mt-5">
            Please <nuxt-link to="/login">
              login
            </nuxt-link> to view your summary!
          </p>
        </v-card-text>
        <v-card-text
          v-else-if="summaryError"
          class="text-center"
        >
          <p class="title error--text mt-5">
            {{ summaryError }}
          </p>
        </v-card-text>
        <v-card-text
          v-else-if="summary == null"
          class="text-center"
        >
          <v-progress-circular
            :size="100"
            color="primary"
            indeterminate
            class="mt-10"
          />
        </v-card-text>
        <v-container v-else>
          <v-row justify="center">
            <v-col>
              <v-list>
                <v-list-item v-if="summary.mainUsername">
                  <v-list-item-content>
                    <v-list-item-title><strong>Main username:</strong> {{ summary.mainUsername }}</v-list-item-title>
                  </v-list-item-content>
                </v-list-item>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title><strong>SOLVED:</strong> {{ summary.solved }}</v-list-item-title>
                  </v-list-item-content>
                </v-list-item>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title><strong>SUBMISSION:</strong> {{ summary.submission }}</v-list-item-title>
                  </v-list-item-content>
                </v-list-item>
                <v-list-item>
                  <v-list-item-content>
                    <v-list-item-title><strong>Generated at</strong> {{ new Date(summary.generateTime) }}</v-list-item-title>
                  </v-list-item-content>
                </v-list-item>
              </v-list>
              <v-simple-table>
                <template v-slot:default>
                  <thead>
                    <tr>
                      <th
                        class="text-left"
                        scope="col"
                      >
                        Crawler
                      </th>
                      <th
                        class="text-left"
                        scope="col"
                      >
                        Username
                      </th>
                      <th
                        class="text-left"
                        scope="col"
                      >
                        Solved
                      </th>
                      <th
                        class="text-left"
                        scope="col"
                      >
                        Submission
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr
                      v-for="item in workerSummaryList"
                      :key="`${item.crawler}`"
                    >
                      <td scope="row">
                        {{ item.crawler }}
                      </td>
                      <td>{{ item.username }}</td>
                      <td>{{ item.solved }}</td>
                      <td>{{ item.submissions }}</td>
                    </tr>
                  </tbody>
                </template>
              </v-simple-table>
              <bar-chart
                :chart-data="chartData"
                style="height: 300px"
              />
              <v-list dense>
                <v-subheader
                  v-if="summary.summaryWarnings.length > 0"
                  class="red--text"
                >
                  WARNINGS
                </v-subheader>
                <v-list-item
                  v-for="item in summary.summaryWarnings"
                  :key="item"
                >
                  <v-list-item-content>
                    {{ $store.state.statistics.crawlers[item.crawlerName].title }}:
                    {{ item.content }}
                  </v-list-item-content>
                </v-list-item>
              </v-list>
            </v-col>
          </v-row>
        </v-container>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script>
import { mapGetters } from 'vuex'
import _ from 'lodash'

import WorkerCard from '~/components/WorkerCard'
import statisticsLayoutBuilder from '~/components/statisticsLayoutBuilder'
import Store from '~/store/-dynamic/statistics'
import { PROJECT_TITLE, WORKER_STATUS } from '~/components/consts'
import { getAbpErrorMessage } from '~/components/utils'

import BarChart from '~/components/BarChart'

export default {
  name: 'Statistics',
  components: {
    WorkerCard,
    BarChart,
  },
  head: {
    title: `AC Statistics - ${PROJECT_TITLE}`,
  },
  created() {
    this.$store.registerModule('statistics', Store, { preserveState: false })
  },
  destroyed() {
    this.$store.unregisterModule('statistics')
  },
  mounted() {
    this.onResize()
    this.loadUsername()
    this.$store.subscribeAction(action => {
      if (_.startsWith(action.type, 'statistics/')) {
        this.summary = null
        this.summaryError = null
      }
    })
  },
  data() {
    return {
      // 一共有几列
      columnCount: 3,
      // 管理保存用户名按钮的动画
      savingUsername: false,
      // summary dialog
      dialog: false,
      summary: null,
      summaryError: null,
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
    workerSummaryList() {
      // module not loaded
      if (!this.summary) {
        return []
      }

      const crawlers = this.$store.state.statistics.crawlers

      const res = []
      for (const summary of this.summary.queryCrawlerSummaries) {
        const usernames = _.map(summary.usernames, item => {
          if (item.fromCrawlerName) {
            return `[${item.username} in ${crawlers[item.fromCrawlerName].title}]`
          } else {
            return item.username
          }
        })
        const isVirtualJudge = crawlers[summary.crawlerName].virtual_judge
        res.push({
          crawler: crawlers[summary.crawlerName].title + (isVirtualJudge ? ' (Not Merged)' : ''),
          username: usernames.join(', '),
          solved: summary.solved,
          submissions: summary.submission,
        })
      }
      return res
    },
    chartData() {
      const solvedList = _.map(this.workerSummaryList, 'solved')
      const submissionsList = _.map(this.workerSummaryList, 'submissions')
      return {
        labels: _.map(this.workerSummaryList, 'crawler'),
        datasets: [
          {
            label: 'solved',
            data: solvedList,
            backgroundColor: '#6699ff',
          },
          {
            label: 'submissions',
            data: submissionsList,
            backgroundColor: '#3d3d5c',
          },
        ],
      }
    },
    updateDate() {
      if (this.notWorkingRate >= 100) {
        return new Date()
      } else {
        return null
      }
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
        this.columnCount = 1 // sm
      } else if (width < 1264) {
        this.columnCount = 2 // md
      } else if (width < 1904) {
        this.columnCount = 3 // lg
      } else {
        this.columnCount = 4 // xl
      }
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
      this.$store.dispatch('statistics/loadUsernames')
    },
    itemBeforeLeaveTransition(el) {
      const columnIdx = Number.parseInt(el.dataset.columnIdx, 10)
      const itemIdx = Number.parseInt(el.dataset.columnItemIdx, 10)

      // 假如是每列的最后一个worker，说明要移动到下一列，动画化向下淡出
      // 否则，说明是要删除本worker，向上淡出
      if (itemIdx < this.workerLayout[columnIdx].length - 1) {
        el.style.transform = 'translateY(-30px)'
      } else {
        el.style.transform = 'translateY(30px)'
      }
      // 最后一列单独判断，假如没有排满，即使是最后一个worker也向上淡出
      if (columnIdx == this.columnCount - 1 && this.workerLayout[columnIdx].length < this.maxItemPerColumn) {
        el.style.transform = 'translateY(-30px)'
      }
    },
    clearWorkers() {
      this.$store.dispatch('statistics/clearWorkers')
    },
    async openDialog() {
      if (this.notWorkingRate < 100) {
        return
      }

      this.dialog = true
      if (this.summary == null && this.summaryError == null) {
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
          const summaryResult = await this.$axios.$get('/api/services/app/QueryHistory/GetQuerySummary', {
            params: {
              queryHistoryId: saveResult.result.queryHistoryId,
            },
          })
          this.summary = summaryResult.result
        } catch (err) {
          this.summaryError = getAbpErrorMessage(err)
        }
      }
    },
    printPage() {
      window.print()
    },
  },
}
</script>

<style scoped>
.workers-column-move {
  transition: all 0.5s;
}

.workers-column-enter {
  opacity: 0;
  transform: translateY(-30px);
}

.workers-column-leave-to {
  opacity: 0;
  /*transform: translateY(30px);*/
}

.workers-column-leave-active {
  position: absolute;
}

.worker-item {
  transition: all 0.5s;
}
</style>
