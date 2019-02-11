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
        <VLayout column xs12 v-else>
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
    </VContainer>
  </VCard>
</template>

<script>
  import {WORKER_STATUS} from '~/components/consts'
  import {warningHelper} from '~/components/statisticsUtils'

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
      ...mapGetters('statistics', [
        'workerNumberOfCrawler',
        'workerIdxOfCrawler',
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
        return warningHelper(this.worker)
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
