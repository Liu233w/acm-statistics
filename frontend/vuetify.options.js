import _ from 'lodash'
import LRU from 'lru-cache'

import '@fortawesome/fontawesome-free/css/all.css'

import colors from 'vuetify/lib/util/colors'

export default {
  theme: {
    options: {
      minifyTheme: function (css) {
        return process.env.NODE_ENV === 'production'
          ? _.replace(css, /[\r\n|\r|\n]/g, '')
          : css
      },
      themeCache: new LRU({
        max: 10,
        maxAge: 1000 * 60 * 60 * 12, // 12 hours
      }),
    },
    themes: {
      light: {
        primary: colors.blue.darken2,
        accent: colors.blue.accent2,
        secondary: colors.grey.lighten1,
        info: colors.blue.lighten1,
        warning: colors.amber.darken2,
        error: colors.red.accent4,
        success: colors.green.lighten2,
      },
    },
  },
  icons: {
    iconfont: 'fa',
  },
}
