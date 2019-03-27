/**
 * 从 linux 格式的 timestamp（精确到秒）生成 javascript 的 Date 格式的对象
 * @param {string} timestamp
 * @return {Date}
 */
export function getDateFromTimestamp(timestamp) {
  return new Date(parseInt(timestamp) * 1000)
}
