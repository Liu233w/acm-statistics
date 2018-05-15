/**
 * 确定查题页面中 worker 的排列方式。使用竖向排列：
 * 1 5 9
 * 2 6 10
 * 3 7
 * 4 8
 *
 * 多余的会从第一列向后补充
 * @param workers {Array<{Object}>}
 * @param columnNum {number}
 */
export default function (workers, columnNum) {
  const length = workers.length
  const maxItemNumberInColumn = Math.ceil(length / columnNum)

  const layout = new Array(columnNum)

  let i = 0
  let lastCrawlerName = null
  let workerIdxOfCrawler = 0

  const getNextWorkerInfo = () => {

    const worker = workers[i]
    if (worker.crawlerName !== lastCrawlerName) {
      lastCrawlerName = worker.crawlerName
      workerIdxOfCrawler = 0
    }
    ++workerIdxOfCrawler

    return {
      index: i,
      // 序号，从 1 开始
      workerIdxOfCrawler: workerIdxOfCrawler,
      crawlerName: worker.crawlerName,
      key: worker.key,
    }
  }

  // 填充除了最后一列之外的其他列
  for (let column = 0; column < columnNum - 1; ++column) {

    // 填充一列（这时候即使遍历完所有除了最后一列之外的列，也不可能用完 worker）
    layout[column] = new Array(maxItemNumberInColumn)
    for (let j = 0; j < maxItemNumberInColumn; ++j) {
      layout[column][j] = getNextWorkerInfo()
      ++i
    }
  }

  // 填充剩余的 worker （此时worker数量一定小于总体的列数）
  layout[columnNum - 1] = []
  for (; i < length; ++i) {
    layout[columnNum - 1].push(getNextWorkerInfo())
  }

  return layout
}
