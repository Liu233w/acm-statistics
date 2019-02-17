<template>
  <VCard>
    <VToolbar card dense class="blue-grey lighten-5">
      <VToolbarTitle :title="crawlerTitle">
        {{ crawlerTitle }}
      </VToolbarTitle>
      <VSpacer />
      <VToolbarItems>
        <VTooltip bottom v-show="workerNum >= 2">
          <VBtn icon
                slot="activator"
                @click="removeWorker"
          >
            <VIcon>delete</VIcon>
          </VBtn>
          <span>
            移除此窗格
          </span>
        </VTooltip>
        <VTooltip bottom v-show="myWorkerIdxOfCrawler == workerNum">
          <VBtn icon
                slot="activator"
                @click="addWorker"
          >
            <VIcon>add_circle</VIcon>
          </VBtn>
          <span>
            添加一个此 OJ 的窗格
          </span>
        </VTooltip>
        <VTooltip bottom v-show="crawlerUrl">
          <VBtn icon
                slot="activator"
                @click="openOj"
          >
            <VIcon>link</VIcon>
          </VBtn>
          <span>
            转到此OJ
          </span>
        </VTooltip>
        <Transition name="fade">
          <VTooltip bottom v-if="worker.status === WORKER_STATUS.WORKING">
            <VBtn icon
                  slot="activator"
                  @click="stopWorker"
            >
              <VIcon>stop</VIcon>
            </VBtn>
            <span>停止查询</span>
          </VTooltip>
          <VTooltip bottom v-else>
            <VBtn icon
                  slot="activator"
                  @click="startWorker"
            >
              <VIcon>refresh</VIcon>
            </VBtn>
            <span>重新爬取此处信息</span>
          </VTooltip>
        </Transition>
      </VToolbarItems>
    </VToolbar>
    <VContainer>
      <VLayout row>
        <VFlex xs12>
          <span class="grey--text" v-show="crawlerDescription">
            {{ crawlerDescription }}
          </span>
        </VFlex>
      </VLayout>
      <VLayout row wrap>
        <VFlex xs12>
          <VTextField
            v-model="username"
            label="Username"
            :disabled="worker.status === WORKER_STATUS.WORKING"
            required
            @keyup.enter="startWorker"
            :loading="worker.status === WORKER_STATUS.WORKING"
            clearable
          />
        </VFlex>
      </VLayout>
      <template v-if="warnings">
        <VLayout row xs12 v-for="item in warnings" :key="item">
          <VFlex align-self-start shrink>
            <VIcon color="orange darken-2">
              warning
            </VIcon>
          </VFlex>
          <VFlex align-self-start>
            <span>{{ item }}</span>
          </VFlex>
        </VLayout>
      </template>
      <template v-if="worker.status === WORKER_STATUS.DONE">
        <VLayout row xs12 v-if="worker.errorMessage">
          <VFlex align-self-start shrink>
            <VIcon color="red">
              error
            </VIcon>
          </VFlex>
          <VFlex align-self-start>
            <span>{{ worker.errorMessage }}</span>
          </VFlex>
        </VLayout>
        <!-- 直接把 v-else 放到 VTooltip 上会引发bug导致内容无法正常删除 -->
        <!-- TODO: 等待此bug修复 -->
        <div v-else>
          <VTooltip bottom>
            <template #activator="tip">
              <VLayout column xs12
                       ripple :class="{ 'elevation-2': hoverOnResult }"
                       @mouseover="hoverOnResult = true" @mouseleave="hoverOnResult = false"
                       @click="solvedListDialog = true"
                       v-on="tip.on"
              >
                <VFlex>
                  <span class="grey--text">
                    SOLVED:
                  </span>
                  {{ worker.solved }}
                </VFlex>
                <VFlex>
                  <span class="grey--text">
                    SUBMISSIONS:
                  </span>
                  {{ worker.submissions }}
                </VFlex>
              </VLayout>
            </template>
            <span>查看通过的题目列表</span>
          </VTooltip>
        </div>
      </template>
    </VContainer>

    <VDialog v-model="solvedListDialog" max-width="500" scrollable>
      <VCard xs12 md8 lg6>
        <VCardTitle>
          <span class="headline">
            用户 {{ worker.username }} 在 {{ crawler.title }} 通过的题目列表
          </span>
        </VCardTitle>
        <VCardText v-if="solvedListStatus === 'empty'">
          当前没有通过的题目
        </VCardText>
        <VCardText v-else-if="solvedListStatus === 'none'">
          当前爬虫无法获取通过的题目列表
        </VCardText>
        <VCardText v-else>
          <VContainer justify-center>
            <VLayout row wrap>
              <VFlex xs4 sm3 v-for="item in prettifiedSolvedList" :key="item">
                <VChip>{{ item }}</VChip>
              </VFlex>
            </VLayout>
          </VContainer>
        </VCardText>
        <VDivider />
        <VCardActions>
          <VBtn color="blue darken-1" flat @click="solvedListDialog = false">
            Close
          </VBtn>
        </VCardActions>
      </VCard>
    </VDialog>
  </VCard>
</template>

<script>
  import {WORKER_STATUS} from '~/components/consts'
  import {warningHelper, mapVirtualJudgeProblemTitle} from '~/components/statisticsUtils'

  import {mapGetters} from 'vuex'
  import _ from 'lodash'

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
        hoverOnResult: false,
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
        set(username) {
          this.$store.dispatch('statistics/updateUsername', {
            index: this.index,
            username,
          })
        },
      },
      workerNum() {
        return this.workerNumberOfCrawler[this.crawlerName]
      },
      myWorkerIdxOfCrawler() {
        return this.workerIdxOfCrawler[this.index]
      },
      warnings() {
        const nullSolvedListCrawlers = this.nullSolvedListCrawlers
        return warningHelper(this.worker, this.crawler, {nullSolvedListCrawlers})
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

        if (this.crawler.virtual_judge) {
          return mapVirtualJudgeProblemTitle(
            this.worker.solvedList,
            this.$store.state.statistics.crawlers)
        } else {
          return _.map(this.worker.solvedList, item => `${this.crawler.title}-${item}`)
        }
      },
    },
  }
</script>

<style scoped>
  .fade-enter-active, .fade-leave-active {
    transition: opacity .5s;
  }

  .fade-enter, .fade-leave-to /* .fade-leave-active below version 2.1.8 */
  {
    opacity: 0;
  }
</style>
