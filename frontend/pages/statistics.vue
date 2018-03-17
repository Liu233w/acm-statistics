<template>
  <v-container grid-list-md>
    <v-layout row>
      <v-flex xs8>
        <v-form>
          <v-text-field
            v-model="username"
            label="统一设置用户名"
            :disabled="working"
            required
          ></v-text-field>
        </v-form>
      </v-flex>
      <v-flex xs4>
        <v-btn primary @click="runWorker" :disabled="working">
          开始查询
        </v-btn>
      </v-flex>
    </v-layout>
    <v-layout row v-if="allSubmissions">
      <v-flex>
        <p class="title text-xs-center"> {{username}} 的总题量： {{allSolved}} / {{allSubmissions}} </p>
      </v-flex>
    </v-layout>
    <v-layout row wrap>
      <v-flex xs12 sm6 md4 xl3
              v-for="(item, name) in workers"
              :key="name">
        <crawler-worker
          :username="username"
          :workerName="name"
          :solved.sync="item.solved"
          :submissions.sync="item.submissions"
          :status.sync="item.status"
        ></crawler-worker>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
  import CrawlerWorker from '~/components/CrawlerWorker'
  import {WORKER_STATUS} from '~/components/consts'

  export default {
    name: "statistics",
    components: {
      CrawlerWorker
    },
    data() {
      return {
        username: '',
        workers: {
          'OJ1': {
            solved: 0,
            submissions: 0,
            status: WORKER_STATUS.WAITING
          },
          'OJ2': {
            solved: 0,
            submissions: 0,
            status: WORKER_STATUS.WAITING
          },
          'OJ3': {
            solved: 0,
            submissions: 0,
            status: WORKER_STATUS.WAITING
          },
          'OJ4': {
            solved: 0,
            submissions: 0,
            status: WORKER_STATUS.WAITING
          },
          'OJ5': {
            solved: 0,
            submissions: 0,
            status: WORKER_STATUS.WAITING
          },
          'OJ6': {
            solved: 0,
            submissions: 0,
            status: WORKER_STATUS.WAITING
          },
          'OJ7': {
            solved: 0,
            submissions: 0,
            status: WORKER_STATUS.WAITING
          },
        }
      }
    },
    computed: {
      allSolved() {
        return _.reduce(this.workers, (sum, val) => sum + val.solved, 0)
      },
      allSubmissions() {
        return _.reduce(this.workers, (sum, val) => sum + val.submissions, 0)
      },
      working() {
        // 是否还有worker正在工作
        return _.some(this.workers, item => item.status === WORKER_STATUS.WORKING)
      }
    },
    methods: {
      runWorker() {
        _.forEach(this.workers, item => item.status = WORKER_STATUS.WORKING)
      }
    }
  }
</script>

<style scoped>

</style>
