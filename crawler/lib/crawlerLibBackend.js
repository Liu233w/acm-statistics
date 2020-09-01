// the CrawlerLib API used in backend
/// <reference path="../crawlers/index.d.ts" />

const cheerio = require('cheerio')
const superagent = require('superagent')
const _ = require('lodash')

module.exports = {
  parseDom(text) {
    return cheerio.load(text)
  },
  /**
   * @param {RequestConfig} requestConfig 
   * @returns {Promise<CrawlerLib.RequestResponse>}
   */
  async request(requestConfig) {
    // ignore useCorsProxy

    const url = _.startsWith(requestConfig.url, '/')
      ? 'http://reverse-proxy' + requestConfig.url
      : requestConfig.url

    const method = requestConfig.method || 'get'
    const func = superagent[_.toLower(method)]
    if (!func) {
      throw new Error('method is not supported')
    }
    /**
     * @type import('superagent').SuperAgentRequest
     */
    let request = func(url)

    if (requestConfig.query) {
      request = request.query(requestConfig.query)
    }

    if (requestConfig.body) {
      request = request.send(requestConfig.body)
    }

    try {
      const res = await request
      return {
        ok: true,
        status: res.status,
        body: res.body,
        text: res.text,
      }
    } catch (err) {
      if (!err.response) {
        throw err
      }

      return {
        ok: false,
        status: err.response.status,
        text: err.response.text,
        body: err.response.body,
      }
    }
  },
}