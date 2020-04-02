<template>
  <v-container grid-list-md v-resize="onResize">
    <v-layout wrap>
      <v-flex xs12 sm5 md3>
        <v-text-field
          v-model="username"
          label="统一设置用户名"
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
      <v-flex xs12 sm7 md5>
        <v-btn class="primary" @click="runWorker" :disabled="isWorking">
          开始查询
        </v-btn>
        <v-tooltip bottom>
          <template #activator="{ on }">
            <v-btn
              class="error"
              v-on="on"
              @click="clearWorkers"
            >
              重置查询
            </v-btn>
          </template>
          清空用户名，重置查询状态
        </v-tooltip>
        <v-tooltip bottom>
          <template #activator="{ on }">
            <v-btn @click="saveUsername"
                   v-on="on"
                   :loading="savingUsername"
                   :disable="savingUsername"
                   :class="{ green: savingUsername }"
            >
              保存用户名
              <template #loader>
                <span>
                  保存成功
                </span>
              </template>
            </v-btn>
          </template>
          将用户名保存到本地，下次打开页面的时候会自动填写
          <br>
          （使用“开始查询”按钮也会保存用户名）
        </v-tooltip>
      </v-flex>
      <v-flex xs12 md4 v-show="submissionsNum">
        <v-chip label color="grey lighten-3" class="elevation-2">
          <v-tooltip bottom>
            <template #activator="{ on }">
              <v-switch
                v-on="on"
                v-model="checkDuplicateAc"
              />
            </template>
            <span>统计AC数时，是否移除重复的题目</span>
          </v-tooltip>
          <span class="title">
            {{ summary }}
          </span>
        </v-chip>
      </v-flex>
    </v-layout>
    <v-layout>
      <v-flex xs12 sm12 md6 lg4 xl3
              v-for="(column, idx) in workerLayout" :key="idx"
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
              <worker-card
                :index="item.index"
              />
            </div>
          </transition-group>
        </v-layout>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
  import {mapGetters} from 'vuex'
  import _ from 'lodash'

  import WorkerCard from '~/components/WorkerCard'
  import statisticsLayoutBuilder from '~/components/statisticsLayoutBuilder'
  import Store from '~/store/-dynamic/statistics'
  import {PROJECT_TITLE} from '~/components/consts'

  export default {
    name: 'Statistics',
    components: {
      WorkerCard,
    },
    head: {
      title: `OJ 题量统计 - ${PROJECT_TITLE}`,
    },
    created() {
      this.$store.registerModule('statistics', Store, {preserveState: false})
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
      checkDuplicateAc: {
        get() {
          return this.$store.state.statistics.checkDuplicateAc
        },
        set(value) {
          this.$store.dispatch('statistics/setCheckDuplicateAc', {value})
        },
      },
      username: {
        get() {
          return this.$store.state.statistics.mainUsername
        },
        set: _.debounce(function (username) {
          this.$store.dispatch('statistics/updateMainUsername', {username})
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
        return `${this.username} 的总题量： ${this.solvedNum} / ${this.submissionsNum}`
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
      saveUsername() {
        this.$store.dispatch('statistics/saveUsernames')

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
