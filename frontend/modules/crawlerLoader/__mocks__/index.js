const fs = require('fs-extra')
const join = require('path').join
const VirtualModulePlugin = require('virtual-module-webpack-plugin')

module.exports = async function () {
  this.options.build.plugins.push(new VirtualModulePlugin({
    moduleName: 'dynamic/crawlers.js',
    contents: await fs.readFileSync(join(__dirname, 'crawlers.js'), 'utf-8'),
  }))
  this.addVendor(['axios'])
}
