import _ from 'lodash'

/**
 * 根据 worker 信息生成对应的警告
 * @param worker
 * @return {Array<string>}
 */
export function warningHelper(worker) {
  const warnings = []

  if (_.startsWith(worker.username, ' ')) {
    warnings.push('用户名以空格开头')
  }

  if (_.includes(worker.username, ' ')) {
    warnings.push('用户名含有空格，部分爬虫可能不支持')
  }

  return warnings
}
