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
                           :value="notWorkingRate"
                           :active="working"
        />
      </v-flex>
    </v-layout>
    <v-layout row>
      <v-flex xs12 sm6 md4 lg3
              v-for="colIdx in columnCount" :key="colIdx">
        <v-layout column>
          <v-flex v-for="(item, idx) in workers"
                  v-if="idx % columnCount === colIdx - 1"
                  :key="item.name">
            <crawler-worker
              :username.sync="item.username"
              :worker-name="item.name"
              :solved.sync="item.solved"
              :submissions.sync="item.submissions"
              :status.sync="item.status"
              :func="item.func"
              :error-message.sync="item.errorMessage"
            />
          </v-flex>
        </v-layout>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
  import _ from 'lodash'

  import CrawlerWorker from '~/components/CrawlerWorker'
  import {WORKER_STATUS} from '~/components/consts'

  export default {
    name: 'Statistics',
    components: {
      CrawlerWorker,
    },
    mounted() {
      this.onResize()
      window.addEventListener('resize', this.onResize, {passive: true})
    },
    data() {
      return {
        username: '',
        workers: _.transform(this.$crawlers, (result, func, name) => {
          result.push({
            name: name,
            solved: 0,
            submissions: 0,
            status: WORKER_STATUS.WAITING,
            func: func,
            errorMessage: '',
            username: this.username,
          })
        }, []),
        // 一共有几列
        columnCount: 3,
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
      },
      notWorkingRate() {
        // 返回一个0-100的数字，表示不在WORKING状态的Worker的数量
        const cnt = this.workers.length
        const notWorking = _.filter(item => item.status !== WORKER_STATUS.WORKING, this.workers).length
        return notWorking / cnt * 100
      },
    },
    methods: {
      runWorker() {
        _.forEach(this.workers, item => item.status = WORKER_STATUS.WORKING)
      },
      onResize() {
        const width = window.innerWidth
        if (width < 600) {
          this.columnCount = 1 // xs
        } else if (width < 960) {
          this.columnCount = 2 // sm
        } else if (width < 1264) {
          this.columnCount = 3 // md
        } else {
          this.columnCount = 4 // lg xl
        }
      },
    },
    watch: {
      username(val) {
        _.forEach(this.workers, item => item.username = val)
      },
    },
  }
</script>

<style scoped>

</style>
