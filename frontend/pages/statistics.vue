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
        <v-btn class="primary" @click="runWorker" :disabled="working">
          开始查询
        </v-btn>
        <v-tooltip bottom>
          <v-btn @click="saveUsername"
                 slot="activator"
                 :loading="savingUsername"
                 :disable="savingUsername"
                 :class="{ green: savingUsername }"
          >
            保存用户名
            <span slot="loader">保存成功</span>
          </v-btn>
          将用户名保存到本地，下次打开页面的时候会自动填写
          <br>
          （使用“开始查询”按钮也会保存用户名）
        </v-tooltip>
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
            <crawler-card
              :username.sync="item.username"
              :worker-name="item.name"
              :solved.sync="item.solved"
              :submissions.sync="item.submissions"
              :status.sync="item.status"
              :error-message.sync="item.errorMessage"
            />
          </v-flex>
        </v-layout>
      </v-flex>
    </v-layout>
    <div :v-show="false">
      <crawler-worker v-for="item in workers"
                      :key="item.name"
                      :username="item.username"
                      :solved.sync="item.solved"
                      :submissions.sync="item.submissions"
                      :status.sync="item.status"
                      :error-message.sync="item.errorMessage"
                      :func="item.func"
      />
    </div>
  </v-container>
</template>

<script>
  import _ from 'lodash'

  import CrawlerWorker from '~/components/CrawlerWorker'
  import CrawlerCard from '~/components/CrawlerCard'
  import {WORKER_STATUS} from '~/components/consts'

  export default {
    name: 'Statistics',
    components: {
      CrawlerWorker,
      CrawlerCard,
    },
    mounted() {
      this.onResize()
      window.addEventListener('resize', this.onResize, {passive: true})
      this.loadUsername()
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
            username: '',
          })
        }, []),
        // 一共有几列
        columnCount: 3,
        // 管理保存用户名按钮的动画
        savingUsername: false,
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
        const notWorking = _.filter(this.workers, item => item.status !== WORKER_STATUS.WORKING).length
        return notWorking / cnt * 100
      },
    },
    methods: {
      runWorker() {
        this.saveUsername()
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
      /**
       * 将用户名的情况存储进 localStorage 里面
       */
      saveUsername() {
        const username = {
          main: this.username,
          subs: {},
        }
        for (let item of this.workers) {
          username.subs[item.name] = item.username
        }

        window.localStorage.setItem('username', JSON.stringify(username))

        this.savingUsername = true

        // 播放2秒的保存动画
        setTimeout(() => {
          this.savingUsername = false
        }, 2000)
      },
      /**
       * 从 localStorage 读取用户名情况，输入进 worker 中
       */
      loadUsername() {
        const username = JSON.parse(window.localStorage.getItem('username'))
        if (username) {
          this.username = username.main
          // 修改 this.username 会触发 watch 来修改其他用户名，所以这里必须放到下一个 tick 来修改
          this.$nextTick(() => {
            for (let item of this.workers) {
              if (username.subs[item.name]) {
                item.username = username.subs[item.name]
              }
            }
          })
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
