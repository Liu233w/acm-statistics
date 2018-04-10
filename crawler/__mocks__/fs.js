const {fs} = require('memfs')
const {promisify} = require('util')

// 实际代码使用了 fs-extra 来支持 promise，这里的mock必须手动添加此支持
fs.readFile = promisify(fs.readFile)

module.exports = fs
