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
                 :disabled="worker.status === WORKER_STATUS.WORKING"
                 @click="startWorker"
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
    },
    computed: {
      worker() {
        return this.$store.state.statistics.workers[this.index]
      },
      crawlerTitle() {
        return this.$root.$crawlerMeta[this.worker.name].title
      },
      crawlerDescription() {
        return this.$root.$crawlerMeta[this.worker.name].description
      },
      crawlerUrl() {
        return this.$root.$crawlerMeta[this.worker.name].url
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
    },
  }
</script>
