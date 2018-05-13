<template>
  <v-card>
    <v-toolbar card dense class="blue-grey lighten-5">
      <v-toolbar-title>
        {{ crawlerTitle }}
      </v-toolbar-title>
      <v-spacer/>
      <v-toolbar-items>
        <v-tooltip bottom v-show="workerNum >= 2">
          <v-btn icon
                 slot="activator"
                 @click="removeWorker"
          >
            <v-icon>remove</v-icon>
          </v-btn>
          <span>
            移除此窗格
          </span>
        </v-tooltip>
        <v-tooltip bottom>
          <v-btn icon
                 slot="activator"
                 @click="addWorker"
          >
            <v-icon>add</v-icon>
          </v-btn>
          <span>
            添加一个此 OJ 的窗格
          </span>
        </v-tooltip>
        <v-tooltip bottom v-show="crawlerUrl">
          <v-btn icon
                 slot="activator"
                 @click="openOj"
          >
            <v-icon>link</v-icon>
          </v-btn>
          <span>
            转到此OJ
          </span>
        </v-tooltip>
        <transition name="fade">
          <v-tooltip bottom v-if="worker.status === WORKER_STATUS.WORKING">
            <v-btn icon
                   slot="activator"
                   @click="stopWorker"
            >
              <v-icon>stop</v-icon>
            </v-btn>
            <span>停止查询</span>
          </v-tooltip>
          <v-tooltip bottom v-else>
            <v-btn icon
                   slot="activator"
                   @click="startWorker"
            >
              <v-icon>refresh</v-icon>
            </v-btn>
            <span>重新爬取此处信息</span>
          </v-tooltip>
        </transition>
      </v-toolbar-items>
    </v-toolbar>
    <v-container>
      <v-layout row>
        <v-flex xs12>
          <span class="grey--text" v-show="crawlerDescription">{{ crawlerDescription }} </span>
        </v-flex>
      </v-layout>
      <v-layout row wrap>
        <v-flex xs12>
          <v-text-field
            v-model="username"
            label="Username"
            :disabled="worker.status === WORKER_STATUS.WORKING"
            required
            @keyup.enter="startWorker"
          />
        </v-flex>
      </v-layout>
      <v-layout row v-show="worker.status === WORKER_STATUS.DONE">
        <v-flex xs12 v-if="worker.errorMessage">
          <span class="red--text">{{ worker.errorMessage }}</span>
        </v-flex>
        <v-flex xs12 v-else>
          <span class="grey--text">SOLVED: </span> {{ worker.solved }}
          <br>
          <span class="grey--text">SUBMISSIONS: </span> {{ worker.submissions }}
        </v-flex>
      </v-layout>
      <v-layout row v-show="worker.status === WORKER_STATUS.WORKING">
        <v-spacer/>
        <v-flex xs2>
          <v-progress-circular indeterminate color="primary"/>
        </v-flex>
        <v-spacer/>
      </v-layout>
    </v-container>
  </v-card>
</template>

<script>
  import {WORKER_STATUS} from '~/components/consts'

  import {mapGetters} from 'vuex'

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
      }
    },
    methods: {
      openOj() {
        window.open(this.crawlerUrl)
      },
      startWorker() {
        this.$store.dispatch('statistics/startOne', {index: this.index})
      },
      stopWorker() {
        this.$store.dispatch('statistics/stopOne', {index: this.index})
      },
      addWorker() {
        this.$store.dispatch('statistics/addWorkerForCrawler', {crawlerName: this.crawlerName})
      },
      removeWorker() {
        this.$store.dispatch('statistics/removeWorkerAtIndex', {index: this.index})
      },
    },
    computed: {
      ...mapGetters('statistics', ['workerNumberOfCrawler']),
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
        set(username) {
          this.$store.dispatch('statistics/updateUsername', {
            index: this.index,
            username,
          })
        },
      },
      /* eslint-disable */
      workerNum() {
        //return this.workerNumberOfCrawler[this.crawlerName]
        return 1
      },
    },
  }
</script>

<style>
  .fade-enter-active, .fade-leave-active {
    transition: opacity .5s;
  }

  .fade-enter, .fade-leave-to /* .fade-leave-active below version 2.1.8 */
  {
    opacity: 0;
  }
</style>
