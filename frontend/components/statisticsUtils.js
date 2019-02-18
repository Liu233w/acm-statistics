import _ from 'lodash'

/**
 * 根据 worker 信息生成对应的警告
 * @param worker
 * @param crawlerMeta
 * @param nullSolvedListCrawlers
 * @param workerNumberOfCrawler
 * @return {Array<string>}
 */
export function warningHelper(worker, crawlerMeta, {nullSolvedListCrawlers, workerNumberOfCrawler}) {
  const warnings = []

  if (_.startsWith(worker.username, ' ')) {
    warnings.push('用户名以空格开头')
  }

  if (_.includes(worker.username, ' ')) {
    warnings.push('用户名含有空格，部分爬虫可能不支持')
  }

  if (crawlerMeta.virtual_judge && worker.solvedList) {

    const allCrawlerNames = new Set(_.map(
      worker.solvedList,
      item => _.split(item, '-', 2)[0]))

    for (let item of allCrawlerNames) {
      if (item in nullSolvedListCrawlers) {
        warnings.push(`爬虫 ${nullSolvedListCrawlers[item]} 无法返回题目列表，因此它的结果和本爬虫的结果可能会重复计算`)
      }
    }
  }

  if (!crawlerMeta.virtual_judge
    && worker.solvedList === null
    && workerNumberOfCrawler[worker.crawlerName] >= 2) {

    warnings.push('本爬虫无法返回题目列表，因此多个账户的通过题目可能会被重复计算')
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
