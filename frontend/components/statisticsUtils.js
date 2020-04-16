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
    warnings.push('Your username begins with a space.')
  }

  if (_.includes(worker.username, ' ')) {
    warnings.push('Your username includes space, which may not be supported by some crawlers.')
  }

  if (crawlerMeta.virtual_judge && worker.solvedList) {

    const allCrawlerNames = new Set(_.map(
      worker.solvedList,
      item => _.split(item, '-', 2)[0]))

    for (let item of allCrawlerNames) {
      if (item in nullSolvedListCrawlers) {
        warnings.push(`Crawler ${nullSolvedListCrawlers[item]} did not return AC problem list, its result may overlap with this crawler's`)
      }
    }
  }

  if (!crawlerMeta.virtual_judge
    && worker.solvedList === null
    && workerNumberOfCrawler[worker.crawlerName] >= 2) {

    warnings.push('This crawler did not return AC problem list, so the same problem in different OJs can be recognized as different problems.')
  }

  if (!_.isNil(worker.solvedList)
    && worker.solvedList.length !== worker.solved) {
    warnings.push(`The AC number of this crawler is ${worker.solved}, however, there are ${worker.solvedList.length}` +
      ' problems in the AC list, which can be an error of the crawler.')
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
