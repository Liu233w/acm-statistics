// 用于将 es6 格式的 module 转换成 node 能识别的格式
module.exports = require('babel-jest').createTransformer({
  presets: ['node8'],
})
