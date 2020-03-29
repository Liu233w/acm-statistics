/**
 * 从 linux 格式的 timestamp（精确到秒）生成 javascript 的 Date 格式的对象
 * @param {string} timestamp
 * @return {Date}
 */
export function getDateFromTimestamp(timestamp) {
  return new Date(parseInt(timestamp) * 1000)
}

/**
 * 从axios访问abp的异常中找出错误信息
 * @param {Error} err axios抛出的异常
 */
export function getAbpErrorMessage(err) {
  console.log(err)
  if (!err.response.data) {
    console.error(err)
    return '网络错误'
  } else {
    console.error(err.response.data.error)
    return err.response.data.error.message
  }
}
