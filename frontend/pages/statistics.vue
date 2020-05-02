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
        <v-dialog
          v-model="dialog"
          fullscreen
          hide-overlay
          transition="dialog-bottom-transition"
        >
          <template v-slot:activator="{ on: { click } }">
            <v-tooltip buttom>
              <template #activator="{ on }">
                <v-chip
                  label
                  color="grey lighten-3"
                  class="elevation-2"
                  v-on="on"
                  @click="click"
                >
                  <span class="title">
                    {{ notWorkingRate >= 100 ? 'SUMMARY' : summary }}
                  </span>
                </v-chip>
              </template>
              Click to open summary panel
            </v-tooltip>
          </template>
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
            <v-container>
              <v-row justify="center">
                <v-col>
                  <v-list>
                    <v-list-item v-if="username">
                      <v-list-item-content>
                        <v-list-item-title><strong>Main username:</strong> {{ username }}</v-list-item-title>
                      </v-list-item-content>
                    </v-list-item>
                    <v-list-item>
                      <v-list-item-content>
                        <v-list-item-title>{{ summary }}</v-list-item-title>
                      </v-list-item-content>
                    </v-list-item>
                    <v-list-item>
                      <v-list-item-content>
                        <v-list-item-title><strong>Generated at</strong> {{ updateDate }}</v-list-item-title>
                      </v-list-item-content>
                    </v-list-item>
                  </v-list>
                  <v-simple-table>
                    <template v-slot:default>
                      <thead>
                        <tr>
                          <th class="text-left" scope="col">
                            Crawler
                          </th>
                          <th class="text-left" scope="col">
                            Username
                          </th>
                          <th class="text-left" scope="col">
                            Solved
                          </th>
                          <th class="text-left" scope="col">
                            Submission
                          </th>
                        </tr>
                      </thead>
                      <tbody>
                        <tr
                          v-for="item in workerSummaryList"
                          :key="`${item.crawler}`"
                        >
                          <td scope="row">{{ item.crawler }}</td>
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
                      v-if="summaryForCrawler.warnings.length > 0"
                      class="red--text"
                    >
                      WARNINGS
                    </v-subheader>
                    <v-list-item
                      v-for="item in summaryForCrawler.warnings"
                      :key="item"
                    >
                      <v-list-item-content>
                        {{ item }}
                      </v-list-item-content>
                    </v-list-item>
                  </v-list>
                </v-col>
              </v-row>
            </v-container>
          </v-card>
        </v-dialog>
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
  </v-container>
</template>

<script>
import { mapGetters } from 'vuex'
import _ from 'lodash'

import WorkerCard from '~/components/WorkerCard'
import statisticsLayoutBuilder from '~/components/statisticsLayoutBuilder'
import Store from '~/store/-dynamic/statistics'
import { PROJECT_TITLE } from '~/components/consts'

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
  },
  data() {
    return {
      // 一共有几列
      columnCount: 3,
      // 管理保存用户名按钮的动画
      savingUsername: false,
      // summary dialog
      dialog: false,
    }
  },
  computed: {
    ...mapGetters('statistics', [
      'solvedNum',
      'submissionsNum',
      'isWorking',
      'notWorkingRate',
      'workerIdxOfCrawler',
      'summaryForCrawler',
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
    summary() {
      return `SOLVED: ${this.solvedNum} / SUBMISSION: ${this.submissionsNum}`
    },
    workerSummaryList() {
      // module not loaded
      if (!this.summaryForCrawler) {
        return []
      }

      const res = []
      for (const crawlerName in this.summaryForCrawler.summaries) {
        const summary = this.summaryForCrawler.summaries[crawlerName]
        if (summary.usernames.size > 0) {
          res.push({
            crawler: summary.crawlerTitle,
            username: [...summary.usernames].join(', '),
            solved: summary.solvedSet.size,
            submissions: summary.submissions,
          })
        }
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
