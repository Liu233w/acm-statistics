const webpack = require('webpack')
const nodeExternals = require('webpack-node-externals')
// eslint-disable-next-line no-unused-vars
const resolve = (dir) => require('path').join(__dirname, dir)

module.exports = {
  /*
  ** Headers of the page
  */
  head: {
    title: 'OJ 题量查询系统',
    meta: [
      {charset: 'utf-8'},
      {name: 'viewport', content: 'width=device-width, initial-scale=1'},
      {hid: 'description', name: 'description', content: 'OJ 题量查询系统 -- 西北工业大学 ACM 开发组'},
    ],
    link: [
      {rel: 'icon', type: 'image/x-icon', href: '/favicon.ico'},
      {rel: 'stylesheet', href: 'https://fonts.googleapis.com/css?family=Roboto:300,400,500,700|Material+Icons'},
    ],
  },
  plugins: [
    '~/plugins/vuetify.js',
    '~/plugins/debug.js',
  ],
  css: [
    '~/assets/style/app.styl',
  ],
  /*
  ** Customize the progress bar color
  */
  loading: {color: '#3B8070'},
  /*
  ** Build configuration
  */
  build: {
    babel: {
      plugins: [
        ['transform-imports', {
          'vuetify': {
            'transform': 'vuetify/es5/components/${member}',
            'preventFullImport': true,
          },
        }],
      ],
    },
    vendor: [
      '~/plugins/vuetify.js',
    ],
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
      if (ctx.isServer) {
        config.externals = [
          nodeExternals({
            whitelist: [/^vuetify/],
          }),
        ]
      }
    },
    plugins: [
      new webpack.ProvidePlugin({
        '_': 'lodash',
      }),
    ],
  },
  proxy: [
    'http://localhost:12001/api/crawlers',
  ],
  modules: [
    '@nuxtjs/proxy',
    '~/modules/crawlerLoader',
  ],
}
