import _ from 'lodash'

/**
 * 根据 worker 信息生成对应的警告
 * @param worker
 * @param crawlerMeta
 * @param getters
 * @return {Array<string>}
 */
export function warningHelper(worker, crawlerMeta, getters) {
  const warnings = []

  if (_.startsWith(worker.username, ' ')) {
    warnings.push('用户名以空格开头')
  }

  if (_.includes(worker.username, ' ')) {
    warnings.push('用户名含有空格，部分爬虫可能不支持')
  }

  if (crawlerMeta.virtual_judge && worker.solvedList) {

    for (let item of worker.solvedList) {

      const crawlerName = _.split(item, '-', 2)[0]

      if (crawlerName in getters.nullSolvedListCrawlers) {
        warnings.push(`爬虫 ${getters.nullSolvedListCrawlers[crawlerName]} 无法返回题目列表，因此它的结果和本爬虫的结果可能会有重复`)
      }
    }
  }

  return warnings
}

/**
 * 在 vjudge 中，如果返回的 ac 题目列表包含了支持的爬虫，将其名称换成 爬虫的 title，否则保留原来的名称
 * @param {String[]} solvedList
 * @param {{title: String}} crawlers
 * @return {String[]}
 */
export function mapVirtualJudgeProblemTitle(solvedList, crawlers) {

  return _.map(solvedList, item => {
    const [oj, problem] = _.split(item, '-', 2)
    if (oj in crawlers) {
      return `${crawlers[oj].title}-${problem}`
    } else {
      return item
    }
  })
}
