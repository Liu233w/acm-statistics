<template>
  <div/>
</template>

<script>
  /*
   * 专门用来处理爬虫命令的组件，这个组件负责单独更新爬虫状态，
   */

  import {WORKER_STATUS} from './consts'

  export default {
    name: 'CrawlerWorker',
    props: {
      status: {
        type: String,
        required: true,
      },
      username: {
        type: String,
        required: true,
      },
      solved: {
        type: Number,
        required: true,
      },
      submissions: {
        type: Number,
        required: true,
      },
      errorMessage: {
        type: String,
        required: true,
      },
      func: {
        type: Function,
        required: true,
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
  }
</script>
