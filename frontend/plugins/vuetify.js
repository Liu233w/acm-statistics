import _ from 'lodash'
import LRU from 'lru-cache'

import Vue from 'vue'
import {
  Vuetify,
  VApp,
  VCard,
  VNavigationDrawer,
  VFooter,
  VList,
  VBtn,
  VIcon,
  VGrid,
  VToolbar,
  VTextField,
  VTooltip,
  VProgressCircular,
  VForm,
  VProgressLinear,
  VAvatar,
  VJumbotron,
  VParallax,
  VDialog,
  VChip,
} from 'vuetify'

Vue.use(Vuetify, {
  components: {
    VApp,
    VCard,
    VNavigationDrawer,
    VFooter,
    VList,
    VBtn,
    VIcon,
    VGrid,
    VToolbar,
    VTextField,
    VTooltip,
    VProgressCircular,
    VForm,
    VProgressLinear,
    VAvatar,
    VJumbotron,
    VParallax,
    VDialog,
    VChip,
  },
  options: {
    minifyTheme: function (val) {
      if (process.env.NODE_ENV === 'production') {
        val = _.replace(val, /[\s|\r\n|\r|\n]/g, '')
      }
      return val
    },
    themeCache: LRU({
      max: 10,
      maxAge: 1000 * 60 * 60 * 12, // 12 hour
    }),
  },
})
