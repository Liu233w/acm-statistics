export default ({app}, inject) => {
  if (app.context.isDev && process.client) {
    window.$app = app
  }
}
