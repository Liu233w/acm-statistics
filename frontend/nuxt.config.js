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
    title: 'OJ Hunt',
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
      { rel: 'stylesheet', href: 'https://fonts.googleapis.com/css?family=Material+Icons|Noto+Serif+SC:300,400,500,700' },
    ],
    script: [
      // { src: 'https://pagead2.googlesyndication.com/pagead/js/adsbygoogle.js', 'data-ad-client': 'ca-pub-9846042020379030', async: true },
      // { src: 'https://contextual.media.net/dmedianet.js?cid=8CUE23IW2', async: true },
    ],
  },
  plugins: [
    '~/plugins/debug.js',
    '~/plugins/analysis.js',
  ],
  css: [
    '~/assets/style/app.scss',
  ],
  /*
  ** Customize the progress bar color
  */
  loading: { color: '#3B8070' },
  // 根据请求的浏览器版本决定babel的preset
  modern: 'server',
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
    /*
    ** Run ESLint on save
    */
    extend(config, ctx) {
      if (ctx.isDev && ctx.isClient) {
        config.module.rules.push({
          enforce: 'pre',
          test: /\.(js|vue)$/,
          loader: 'eslint-loader',
          exclude: /(node_modules)/,
        })
      }
    },
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
