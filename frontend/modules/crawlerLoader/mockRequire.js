/**
 * 魔改爬虫里的 require，导入前端的库
 * 必须以字符串形式引入
 */
const require = lib => {
  if (lib === 'superagent') {
    return superagent
  } else if (lib === 'cheerio') {
    return {
      load: html => {
        const elem = jquery(html)
        return selector => jquery(selector, elem)
      },
    }
  } else {
    throw new Error('不能导入除了 superagent 和 cheerio 之外的类库')
  }
}
