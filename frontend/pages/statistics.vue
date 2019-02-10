<template>
  <VContainer grid-list-md>
    <VLayout row wrap>
      <VFlex xs12 sm5 md3>
        <VTextField
          v-model="username"
          label="统一设置用户名"
          :disabled="isWorking"
          required
          @keyup.enter="runWorker"
          :loading="isWorking"
        >
          <VProgressLinear
            slot="progress"
            color="primary"
            :value="notWorkingRate"
            :height="3"
          />
        </VTextField>
      </VFlex>
      <VFlex xs12 sm7 md5>
        <VBtn class="primary" @click="runWorker" :disabled="isWorking">
          开始查询
        </VBtn>
        <VTooltip bottom>
          <VBtn
            class="error"
            @click="clearWorkers"
            slot="activator"
          >
            重置查询
          </VBtn>
          清空用户名，重置查询状态
        </VTooltip>
        <VTooltip bottom>
          <VBtn @click="saveUsername"
                slot="activator"
                :loading="savingUsername"
                :disable="savingUsername"
                :class="{ green: savingUsername }"
          >
            保存用户名
            <span slot="loader">
              保存成功
            </span>
          </VBtn>
          将用户名保存到本地，下次打开页面的时候会自动填写
          <br>
          （使用“开始查询”按钮也会保存用户名）
        </VTooltip>
      </VFlex>
      <VFlex xs12 md4 v-show="submissionsNum">
        <VChip label color="grey lighten-3" class="elevation-2">
          <span class="title">
            {{ summary }}
          </span>
        </VChip>
      </VFlex>
    </VLayout>
    <VLayout row>
      <VFlex xs12 sm12 md6 lg4 xl3
             v-for="(column, idx) in workerLayout" :key="idx"
      >
        <VLayout column>
          <TransitionGroup
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
              <WorkerCard
                :index="item.index"
              />
            </div>
          </TransitionGroup>
        </VLayout>
      </VFlex>
    </VLayout>
  </VContainer>
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
      this.$store.registerModule('statistics', Store, { preserveState: false })
    },
    destroyed() {
      this.$store.unregisterModule('statistics')
    },
    mounted() {
      this.onResize()
      window.addEventListener('resize', this.onResize, {passive: true})
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
