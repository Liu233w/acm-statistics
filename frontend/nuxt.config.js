const nodeExternals = require('webpack-node-externals')
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
      {hid: 'description', name: 'description', content: 'NWPU-ACM 查询系统 -- 西北工业大学 ACM 开发组'},
    ],
    link: [
      {rel: 'icon', type: 'image/x-icon', href: '/favicon.ico'},
      {rel: 'stylesheet', href: 'https://fonts.loli.net/css?family=Roboto:300,400,500,700|Material+Icons'},
    ],
    script: [
      {src: 'http://tajs.qq.com/stats?sId=65546290', charset: 'UTF-8', type: 'text/javascript'},
    ],
  },
  plugins: [
    '~/plugins/vuetify.js',
    '~/plugins/debug.js',
    '~/plugins/ta.js',
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
        'lodash',
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
      // 参见 https://github.com/lodash/lodash-webpack-plugin 来引入需要的功能
      new LodashModuleReplacementPlugin({
        cloning: true,
        currying: true,
        collections: true,
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
