/**
 * 魔改 superagent 以解决同源问题——使用 cors proxy
 */

import superagent from 'superagent'

const corsProxyUrl = 'https://acm-statistics-cors.herokuapp.com/'

const getFunc = superagent.get
const postFunc = superagent.post

superagent.get = function (url) {
  // eslint-disable-next-line lodash/prefer-lodash-method
  if (url.startsWith('/')) {
    // 调用爬虫API
    return getFunc(url)
  } else {
    return getFunc(corsProxyUrl + url)
  }
}

superagent.post = function (url) {
  return postFunc(corsProxyUrl + url)
}
