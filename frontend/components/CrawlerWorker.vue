<template>
  <v-card>
    <v-toolbar card dense class="blue-grey lighten-5">
      <v-toolbar-title>
        {{ crawlerTitle }}
      </v-toolbar-title>
      <v-spacer/>
      <v-toolbar-items>
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
        <v-tooltip bottom>
          <v-btn icon
                 slot="activator"
                 :disabled="status === WORKER_STATUS.WORKING"
                 @click="$emit('update:status', WORKER_STATUS.WORKING)"
          >
            <v-icon>refresh</v-icon>
          </v-btn>
          <span>重新爬取此处信息</span>
        </v-tooltip>
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
            :value="username"
            @input="updateUsername"
            label="Username"
            :disabled="status === WORKER_STATUS.WORKING"
            required
            @keyup.enter="$emit('update:status', WORKER_STATUS.WORKING)"
          />
        </v-flex>
      </v-layout>
      <v-layout row v-show="status === WORKER_STATUS.DONE">
        <v-flex xs12 v-if="errorMessage">
          <span class="red--text">{{ errorMessage }}</span>
        </v-flex>
        <v-flex xs12 v-else>
          <span class="grey--text">SOLVED: </span> {{ solved }}
          <br>
          <span class="grey--text">SUBMISSIONS: </span> {{ submissions }}
        </v-flex>
      </v-layout>
      <v-layout row v-show="status === WORKER_STATUS.WORKING">
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
  import {WORKER_STATUS} from './consts'

  export default {
    name: 'CrawlerWorker',
    props: {
      status: {
        type: String,
        default: WORKER_STATUS.WAITING,
      },
      username: {
        type: String,
        required: true,
      },
      solved: {
        type: Number,
        default: 0,
      },
      submissions: {
        type: Number,
        default: 0,
      },
      workerName: {
        type: String,
        required: true,
      },
      func: {
        type: Function,
        required: true,
      },
      errorMessage: {
        type: String,
        default: '',
      },
    },
    data() {
      return {
        WORKER_STATUS: WORKER_STATUS,
      }
    },
    watch: {
      status: async function (val) {
        if (val === WORKER_STATUS.WORKING) {
          this.resetRes()
          // 启动爬虫
          try {
            const res = await this.func(this.username)
            this.$emit('update:status', WORKER_STATUS.DONE)
            this.$emit('update:solved', res.solved)
            this.$emit('update:submissions', res.submissions)
          } catch (err) {
            this.$emit('update:status', WORKER_STATUS.DONE)
            this.$emit('update:errorMessage', err.message)
          }
        }
      },
      username: function () {
        this.$emit('update:status', WORKER_STATUS.WAITING)
        this.resetRes()
      },
    },
    methods: {
      /**
       * 重设当前计数器和错误信息，不重设状态信息
       */
      resetRes() {
        this.$emit('update:solved', 0)
        this.$emit('update:submissions', 0)
        this.$emit('update:errorMessage', '')
      },
      openOj() {
        window.open(this.crawlerUrl)
      },
      updateUsername(val) {
        this.$emit('update:username', val)
      },
    },
    computed: {
      crawlerTitle() {
        return this.$crawlerMeta[this.workerName].title
      },
      crawlerDescription() {
        return this.$crawlerMeta[this.workerName].description
      },
      crawlerUrl() {
        return this.$crawlerMeta[this.workerName].url
      },
    },
  }
</script>

<style scoped>

</style>
