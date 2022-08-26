// eslint-disable-next-line no-unused-vars
const resolve = (dir) => require('path').join(__dirname, dir)
const _ = require('lodash')

const sensitiveRouter = require('./configs/sensitive-url-router')

const { readMetaConfigs } = require('crawler')

module.exports = async () => ({
  /*
  ** Headers of the page
  */
  head: {
    title: 'OJ Analyzer',
    meta: [
      { charset: 'utf-8' },
      { name: 'viewport', content: 'width=device-width, initial-scale=1' },
      {
        hid: 'description', name: 'description', content: 'An online tool (crawler) to analyze users performance in online judges (coding competition websites). '
          + 'Supported OJ: ' + _.map(await readMetaConfigs(), 'title').join(', '),
      },
    ],
    link: [
      { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
      { rel: 'stylesheet', href: 'https://fonts.googleapis.com/css?family=Noto+Serif+SC:300,400,500,700' },
    ],
  },
  plugins: [
    '~/plugins/debug.js',
    '~/plugins/font.js',
    { src: '~/plugins/chartjs.js', mode: 'client' },
  ],
  css: [
    '~/assets/style/app.scss',
  ],
  /*
  ** Customize the progress bar color
  */
  loading: { color: '#3B8070' },
  /*
  ** Build configuration
  */
  build: {
    babel: {
      plugins: [
        'lodash',
      ],
    },
    extractCSS: true,
    terser: {
      terserOptions: {
        compress: {
          drop_console: true,
        },
      },
    },
  },
  modules: [
    '~/modules/crawlerLoader',
    '@nuxtjs/component-cache',
    ['nuxt-env', {
      keys: ['VERSION_NUM', 'BUILD_TIME'],
    }],
    ['@nuxtjs/axios', {
      baseURL: 'http://reverse-proxy',
      browserBaseURL: '/',
      debug: true,
    }],
  ],
  buildModules: [
    '@nuxtjs/eslint-module',
    '@nuxtjs/vuetify',
  ],
  vuetify: {
    optionsPath: './vuetify.options.js',
  },
  watchers: {
    // 尽管这是文档里的默认值，但是不设置它的话并不会生效。估计这是一个bug
    webpack: {
      aggregateTimeout: 300,
      poll: 1000,
    },
  },
  router: {
    extendRoutes: sensitiveRouter,
    middleware: [
      'auth',
    ],
  },
})
