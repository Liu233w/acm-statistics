import axios from 'axios'
import Vue from 'vue'

// eslint-disable-next-line no-unused-vars
export default ({app}, inject) => {
  if (app.context.isDev && process.client) {
    window.$app = app
    window.$axios = axios

    // 打开性能分析
    Vue.config.performance = true
  }
}
