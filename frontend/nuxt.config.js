const nodeExternals = require('webpack-node-externals')
const VuetifyLoaderPlugin = require('vuetify-loader/lib/plugin')
const LodashModuleReplacementPlugin = require('lodash-webpack-plugin')
// eslint-disable-next-line no-unused-vars
const resolve = (dir) => require('path').join(__dirname, dir)

module.exports = {
  /*
  ** Headers of the page
  */
  head: {
    title: 'NWPU-ACM 查询系统',
    meta: [
      {charset: 'utf-8'},
      {name: 'viewport', content: 'width=device-width, initial-scale=1'},
      {hid: 'description', name: 'description', content: '西北工业大学ACM基地查题主页，提供国内各大OJ的查询题量、计算Rating的服务。'},
    ],
    link: [
      {rel: 'icon', type: 'image/x-icon', href: '/favicon.ico'},
      {rel: 'stylesheet', href: 'https://fonts.loli.net/css?family=Material+Icons|Noto+Serif+SC:300,400,500,700'},
      {rel: 'stylesheet', href: 'https://cdn.bootcss.com/font-awesome/4.7.0/css/font-awesome.min.css'},
    ],
  },
  plugins: [
    '~/plugins/vuetify.js',
    '~/plugins/debug.js',
    '~/plugins/analysis.js',
  ],
  css: [
    '~/assets/style/app.styl',
  ],
  /*
  ** Customize the progress bar color
  */
  loading: {color: '#3B8070'},
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
      if (process.server) {
        config.externals = [
          nodeExternals({
            whitelist: [/^vuetify/],
          }),
        ]
      }
    },
    transpile: [/^vuetify/],
    plugins: [
      // 参见 https://github.com/lodash/lodash-webpack-plugin 来引入需要的功能
      new LodashModuleReplacementPlugin({
        cloning: true,
        currying: true,
        collections: true,
        shorthands: true,
      }),
      new VuetifyLoaderPlugin(),
    ],
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
  ],
  watchers: {
    // 尽管这是文档里的默认值，但是不设置它的话并不会生效。估计这是一个bug
    webpack: {
      aggregateTimeout: 300,
      poll: 1000,
    },
  },
}
