import _ from 'lodash'
import LRU from 'lru-cache'

import '@fortawesome/fontawesome-free/css/all.css'

export default {
  theme: {
    options: {
      minifyTheme: function (val) {
        if (process.env.NODE_ENV === 'production') {
          val = _.replace(val, /[\s|\r\n|\r|\n]/g, '')
        }
        return val
      },
      themeCache: new LRU({
        max: 10,
        maxAge: 1000 * 60 * 60 * 12, // 12 hours
      }),
    },
  },
  icons: {
    iconfont: 'fa',
  },
}
