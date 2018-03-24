<template>
  <v-container grid-list-md>
    <v-layout row>
      <v-flex xs8>
        <v-text-field
          v-model="username"
          label="统一设置用户名"
          :disabled="working"
          required
          @keyup.enter="runWorker"
        />
      </v-flex>
      <v-flex xs4>
        <v-btn primary @click="runWorker" :disabled="working">
          开始查询
        </v-btn>
      </v-flex>
    </v-layout>
    <v-layout row v-if="allSubmissions">
      <v-flex>
        <p class="title text-xs-center"> {{ username }} 的总题量： {{ allSolved }} / {{ allSubmissions }} </p>
      </v-flex>
    </v-layout>
    <v-layout row>
      <v-flex>
        <v-progress-linear color="primary"
                           :value="notWorkeringRate"
                           :active="working"
        />
      </v-flex>
    </v-layout>
    <v-layout row wrap>
      <v-flex xs12 sm6 md4 xl3
              v-for="(item, name) in workers"
              :key="name">
        <crawler-worker
          :username="username"
          :worker-name="name"
          :solved.sync="item.solved"
          :submissions.sync="item.submissions"
          :status.sync="item.status"
          :func="item.func"
        />
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
  import _ from 'lodash'
  import {flow, filter, size} from 'lodash/fp'

  import CrawlerWorker from '~/components/CrawlerWorker'
  import {WORKER_STATUS} from '~/components/consts'

  export default {
    name: 'Statistics',
    components: {
      CrawlerWorker,
    },
    data() {
      return {
        username: '',
        workers: {},
      }
    },
    created() {
      // 由于 ssr，data是在服务器上运行的，因此必须在created里面初始化workers
      this.workers = _.mapValues(this.$crawlers, func => ({
        solved: 0,
        submissions: 0,
        status: WORKER_STATUS.WAITING,
        func: func,
      }))
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
      },
      notWorkeringRate() {
        // 返回一个0-100的数字，表示不在WORKING状态的Worker的数量
        const cnt = _.size(this.workers)
        const notWorking = flow(
          filter(item => item.status !== WORKER_STATUS.WORKING),
          size
        )(this.workers)
        return notWorking / cnt * 100
      },
    },
    methods: {
      runWorker() {
        _.forEach(this.workers, item => item.status = WORKER_STATUS.WORKING)
      },
    },
  }
</script>

<style scoped>

</style>
