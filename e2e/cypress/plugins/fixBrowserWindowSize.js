/*
from https://github.com/cypress-io/cypress/issues/495#issuecomment-472723434
 */

module.exports = (on) => {
  on('before:browser:launch', (browser = {}, options = { args: [] }) => {

    const args = options.args

    if (browser.name === 'chrome') {
      args.push('--window-size=200,800')
    }
    else if (browser.name === 'electron') {
      args['width'] = 1600
      args['height'] = 900
      args['resizable'] = false
    }
  })
}
