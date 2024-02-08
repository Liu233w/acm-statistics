const _ = require('lodash')

import { defineNuxtConfig } from 'nuxt/config'

import { readMetaConfigs } from 'crawler'

module.exports = defineNuxtConfig({
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
          + 'Supported OJ: ' + _.map(readMetaConfigs(), 'title').join(', '),
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
  vite: {
    plugins: [
      require('configs/crawlerLoader').plugin,
    ],
  },
  modules: [
    '~/modules/sensitiveUrlRoutes/index',
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
    middleware: [
      'auth',
    ],
  },
})
