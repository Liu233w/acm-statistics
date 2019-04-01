/*
from https://github.com/cypress-io/cypress/issues/495#issuecomment-472723434
 */

module.exports = (on) => {
  on('before:browser:launch', (browser = {}, args) => {
    if (browser.name === 'chrome') {
      args.push('--window-size=200,800')

      // whatever you return here becomes the new args
      return args
    }

    if (browser.name === 'electron') {
      args['width'] = 1600
      args['height'] = 900
      args['resizable'] = false

      // whatever you return here becomes the new args
      return args
    }
  })
}