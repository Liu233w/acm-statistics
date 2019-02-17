const log4js = require('log4js')
const fs = require('fs')
const _ = require('lodash')

const logConfig = require('../config/log') // 加载配置文件

log4js.configure(logConfig) // 将配置添加到log4js中

let logUtil = {}


// 确定目录是否存在，如果不存在则创建目录
const createPath = (pathStr) => {
  if (!fs.existsSync(pathStr)) {
    fs.mkdirSync(pathStr)
    console.log(`createPath:${pathStr}`)
  }
}

// 初始化log相关目录
const initLogPath = () => {
  //创建log的根目录'logs'
  if (logConfig.baseLogPath) {
    //创建log根目录
    createPath(logConfig.baseLogPath)

    //根据不同的logType创建不同的子目录
    _.forEach(item => {
      if (item.path) {
        createPath(logConfig.baseLogPath + item.path)
      }
    })
  }
}

// 自动初始化log输出所需要的目录
initLogPath()

const errLogger = log4js.getLogger('error')
const resLogger = log4js.getLogger('response')

//封装错误日志
logUtil.logError = function (ctx, error, resTime) {
  if (ctx && error) {
    errLogger.error(formatError(ctx, error, resTime))
  }
}

//封装响应日志
logUtil.logResponse = function (ctx, resTime) {
  if (ctx) {
    resLogger.info(formatRes(ctx, resTime))
  }
}

//格式化响应日志
const formatRes = (ctx, resTime) => {
  var logText = new String()

  //响应日志开始
  logText += '\n' + '*************** response log start ***************' + '\n'

  //添加请求日志
  logText += formatReqLog(ctx.request, resTime)

  //响应状态码
  logText += 'response status: ' + ctx.status + '\n'

  //响应内容
  logText += 'response body: skipped\n'
  // logText += 'response body: ' + '\n' + JSON.stringify(ctx.body) + '\n'

  //响应日志结束
  logText += '*************** response log end ***************' + '\n'

  return logText
}

//格式化错误日志
const formatError = (ctx, err, resTime) => {
  var logText = ''

  //错误信息开始
  logText += '\n*************** error log start ***************\n'

  //添加请求日志
  logText += formatReqLog(ctx.request, resTime)

  //错误名称
  logText += `err name:${err.name}\n`
  //错误信息
  logText += `err message:${err.message}\n`
  //错误详情
  logText += `err stack:${err.stack}\n`

  //错误信息结束
  logText += '*************** error log end ***************\n'

  return logText
}

//格式化请求日志
const formatReqLog = (req, resTime) => {
  let logText = ''
  let method = req.method

  //访问方法
  logText += `request method: ${method}\n`

  //请求原始地址
  logText += `request originalUrl: ${req.originalUrl}\n`

  //客户端ip
  logText += `request client ip: ${req.ip}\n`

  //请求参数
  if (method === 'GET') {
    logText += `request query: ${JSON.stringify(req.query)}\n`
  } else {
    logText += `request body: \n${JSON.stringify(req.body)}\n`
  }

  //服务器响应时间
  logText += `response time:${resTime}\n`

  return logText
}

const middleware = async (ctx, next) => {
  //响应开始时间
  const start = new Date()
  //请求处理完毕的时刻 减去 开始处理请求的时刻 = 处理请求所花掉的时间
  let ms
  try {
    await next()

    ms = new Date() - start

    //记录响应日志
    logUtil.logResponse(ctx, ms)
  } catch (error) {
    ms = new Date() - start

    //记录异常日志
    logUtil.logError(ctx, error, ms)
  }
}

module.exports = middleware