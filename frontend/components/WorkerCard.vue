<template>
  <v-card>
    <v-app-bar
      flat
      dense
      elevation="0"
      class="blue-grey lighten-5"
    >
      <v-toolbar-title :title="crawlerTitle">
        {{ crawlerTitle }}
      </v-toolbar-title>
      <v-spacer />
      <v-toolbar-items>
        <v-tooltip
          bottom
          v-if="workerNum >= 2"
        >
          <template #activator="{ on }">
            <v-btn
              icon
              v-on="on"
              @click="removeWorker"
            >
              <v-icon>mdi-delete-outline</v-icon>
            </v-btn>
          </template>
          <span>
            Remove this worker
          </span>
        </v-tooltip>
        <v-tooltip
          bottom
          v-if="myWorkerIdxOfCrawler == workerNum"
        >
          <template #activator="{ on }">
            <v-btn
              icon
              v-on="on"
              @click="addWorker"
            >
              <v-icon>mdi-plus-circle-outline</v-icon>
            </v-btn>
          </template>
          <span>
            Add a new worker for the crawler
          </span>
        </v-tooltip>
        <v-tooltip
          bottom
          v-if="crawlerUrl"
        >
          <template #activator="{ on }">
            <v-btn
              icon
              v-on="on"
              @click="openOj"
            >
              <v-icon>mdi-open-in-new</v-icon>
            </v-btn>
          </template>
          <span>
            Enter OJ website
          </span>
        </v-tooltip>
        <v-fade-transition>
          <v-tooltip
            bottom
            v-if="worker.status === WORKER_STATUS.WORKING"
          >
            <template #activator="{ on }">
              <v-btn
                icon
                v-on="on"
                @click="stopWorker"
              >
                <v-icon>mdi-stop</v-icon>
              </v-btn>
            </template>
            <span>Cancel query</span>
          </v-tooltip>
          <v-tooltip
            bottom
            v-else
          >
            <template #activator="{ on }">
              <v-btn
                icon
                v-on="on"
                @click="startWorker"
              >
                <v-icon>mdi-refresh</v-icon>
              </v-btn>
            </template>
            <span>Re-query</span>
          </v-tooltip>
        </v-fade-transition>
      </v-toolbar-items>
    </v-app-bar>
    <v-container>
      <v-layout>
        <v-flex xs12>
          <span
            class="grey--text"
            v-if="crawlerDescription"
          >
            {{ crawlerDescription }}
          </span>
        </v-flex>
      </v-layout>
      <v-layout wrap>
        <v-flex xs12>
          <v-text-field
            v-model="username"
            label="Username"
            :disabled="worker.status === WORKER_STATUS.WORKING"
            required
            @keyup.enter="startWorker"
            :loading="worker.status === WORKER_STATUS.WORKING"
            clearable
          />
        </v-flex>
      </v-layout>
      <template v-if="warnings">
        <v-layout
          xs12
          v-for="item in warnings"
          :key="item"
        >
          <v-flex
            align-self-start
            shrink
          >
            <v-icon color="orange darken-2">
              mdi-alert
            </v-icon>
          </v-flex>
          <v-flex align-self-start>
            <span>{{ item }}</span>
          </v-flex>
        </v-layout>
      </template>
      <template v-if="worker.status === WORKER_STATUS.DONE">
        <v-layout
          xs12
          v-if="worker.errorMessage"
        >
          <v-flex
            align-self-start
            shrink
          >
            <v-icon color="red">
              mdi-alert-circle
            </v-icon>
          </v-flex>
          <v-flex align-self-start>
            <span>{{ worker.errorMessage }}</span>
          </v-flex>
        </v-layout>
        <v-layout
          column
          xs12
          v-else
        >
          <v-flex>
            <span class="grey--text text-body-2">
              SOLVED:
            </span>
            <template v-if="solvedListStatus === 'none'">
              {{ worker.solved }}
            </template>
            <v-tooltip
              bottom
              v-else
            >
              <template #activator="tip">
                <a
                  v-on="tip.on"
                  @click="solvedListDialog = true"
                >
                  {{ worker.solved }}
                </a>
              </template>
              <span>Open AC problem list</span>
            </v-tooltip>
          </v-flex>
          <v-flex>
            <span class="grey--text text-body-2">
              SUBMISSIONS:
            </span>
            {{ worker.submissions }}
          </v-flex>
        </v-layout>
      </template>
    </v-container>

    <v-dialog
      v-model="solvedListDialog"
      max-width="500"
      scrollable
    >
      <v-card
        xs12
        md8
        lg6
      >
        <v-card-title>
          <span class="text-h5">
            AC problem list of {{ worker.username }} in {{ crawler.title }}
          </span>
        </v-card-title>
        <v-card-text v-if="solvedListStatus === 'empty'">
          There are no AC problems.
        </v-card-text>
        <v-card-text v-else-if="solvedListStatus === 'none'">
          AC problem list is not supported by the crawler.
        </v-card-text>
        <v-card-text v-else-if="solvedListDialog">
          <v-chip
            v-for="item in prettifiedSolvedList"
            :key="item"
            class="ma-1"
          >
            {{ item }}
          </v-chip>
        </v-card-text>
        <v-divider />
        <v-card-actions>
          <v-btn
            color="blue darken-1"
            text
            @click="solvedListDialog = false"
          >
            Close
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-card>
</template>

<script>
import { mapGetters } from 'vuex'
import _ from 'lodash'

import { WORKER_STATUS } from '~/components/consts'
import { warningHelper, mapVirtualJudgeProblemTitle } from '~/components/statisticsUtils'
import { delay } from '~/components/utils'

export default {
  name: 'CrawlerCard',
  props: {
    index: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      WORKER_STATUS: WORKER_STATUS,
      solvedListDialog: false,
      previousHeight: null,
    }
  },
  async mounted() {
    // don't know why, but when init, the offsetHeight may be larger
    // than real height
    do {
      await delay(100)
    } while (this.$el.offsetHeight >= 500)
    this.updateHeight()

    // force update height to report the real height
    await delay(1000)
    this.updateHeight()
  },
  destroyed() {
    this.$emit('update-height', 0)
  },
  methods: {
    openOj() {
      window.open(this.crawlerUrl)
    },
    startWorker() {
      this.$store.dispatch('statistics/startOne', { index: this.index })
    },
    stopWorker() {
      this.$store.dispatch('statistics/stopOne', { index: this.index })
    },
    addWorker() {
      this.$store.dispatch('statistics/addWorkerForCrawler', { crawlerName: this.crawlerName })
    },
    removeWorker() {
      this.$store.dispatch('statistics/removeWorkerAtIndex', { index: this.index })
    },
    async updateHeight() {
      await this.$nextTick()
      const currentHeight = this.$el.offsetHeight
      if (currentHeight !== this.previousHeight) {
        this.$emit('update-height', currentHeight)
        this.previousHeight = currentHeight
      }
    },
  },
  watch: {
    warnings() {
      this.updateHeight()
    },
    workerStatus() {
      this.updateHeight()
    },
  },
  computed: {
    ...mapGetters('statistics', [
      'workerNumberOfCrawler',
      'workerIdxOfCrawler',
      'nullSolvedListCrawlers',
    ]),
    worker() {
      return this.$store.state.statistics.workers[this.index]
    },
    crawlerName() {
      return this.worker.crawlerName
    },
    crawler() {
      return this.$store.state.statistics.crawlers[this.crawlerName]
    },
    crawlerTitle() {
      return this.crawler.title
    },
    crawlerDescription() {
      return this.crawler.description
    },
    crawlerUrl() {
      return this.crawler.url
    },
    username: {
      get() {
        return this.worker.username
      },
      set: _.debounce(function(username) {
        this.$store.dispatch('statistics/updateUsername', {
          index: this.index,
          username,
        })
      }, 300),
    },
    workerNum() {
      return this.workerNumberOfCrawler[this.crawlerName]
    },
    myWorkerIdxOfCrawler() {
      return this.workerIdxOfCrawler[this.index]
    },
    warnings() {
      return warningHelper(this.worker, this.crawler, {
        nullSolvedListCrawlers: this.nullSolvedListCrawlers,
        workerNumberOfCrawler: this.workerNumberOfCrawler,
      })
    },
    solvedListStatus() {
      if (_.isArray(this.worker.solvedList)) {
        if (this.worker.solvedList.length > 0) {
          return 'list'
        } else {
          return 'empty'
        }
      } else {
        return 'none'
      }
    },
    prettifiedSolvedList() {

      if (this.worker.solvedList == null) {
        return null
      }

      let res
      if (this.crawler.virtual_judge) {
        res = mapVirtualJudgeProblemTitle(
          this.worker.solvedList,
          this.$store.state.statistics.crawlers)
      } else {
        res = _.map(this.worker.solvedList, item => `${this.crawler.title}-${item}`)
      }

      // Freeze the list to improve performance, because vue will not create proxy
      // for each element
      // see https://vuejs.org/v2/guide/instance.html#Data-and-Methods
      // and https://vuedose.tips/tips/improve-performance-on-large-lists-in-vue-js/
      return Object.freeze(res)
    },
    // only used in watch
    workerStatus() {
      return this.worker.status
    },
  },
}
</script>
