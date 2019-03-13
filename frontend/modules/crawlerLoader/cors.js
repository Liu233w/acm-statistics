/**
 * 魔改 superagent 以解决同源问题——使用 cors proxy
 * 必须以字符串形式引入
 */

const corsProxyUrl = 'https://cors-anywhere.herokuapp.com/'

const getFunc = superagent.get
const postFunc = superagent.post

superagent.get = function (url) {
  return getFunc(corsProxyUrl + url)
}

superagent.post = function (url) {
  return postFunc(corsProxyUrl + url)
}
