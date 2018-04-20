import axios from 'axios'

// eslint-disable-next-line no-unused-vars
export default ({app}, inject) => {
  if (app.context.isDev && process.client) {
    window.$app = app
    window.$axios = axios
  }
}
