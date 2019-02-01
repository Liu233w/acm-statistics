import _ from 'lodash'
import LRU from 'lru-cache'

import Vue from 'vue'
import Vuetify from 'vuetify/lib'

Vue.use(Vuetify, {
  options: {
    minifyTheme: function (val) {
      if (process.env.NODE_ENV === 'production') {
        val = _.replace(val, /[\s|\r\n|\r|\n]/g, '')
      }
      return val
    },
    themeCache: new LRU({
      max: 10,
      maxAge: 1000 * 60 * 60 * 12, // 12 hour
    }),
  },
})
