// declare of global object used in crawler file

declare namespace CrawlerLib {

  /**
   * Request on internet (it may go throw cors-proxy or not).
   * 
   * It may have different behaviour when requesting the services of ohunt.
   * 
   * It does not throw if response code is 4XX or 5XX.
   * @param config The config to request
   */
  function request(config: RequestConfig): Promise<RequestResponse>
  // TODO: wrap the throw

  /**
   * The response of the request
   */
  interface RequestResponse {

    /**
     * The status code is not 4XX or 5XX
     */
    ok: boolean

    /**
     * The status code
     */
    status: number

    /**
     * The text content of the response
     */
    text: string

    /**
     * The deserialized json body, if the response is json
     */
    body: unknown
  }

  /**
   * Parse the dom and return cheerio object
   * @param text The string of the dom to parse
   */
  function parseDom(text: string): CheerioStatic
}

/**
 * the config to request
 */
interface RequestConfig {

  /**
   * The url to request
   */
  url: string

  /**
   * The method to request
   */
  method: string

  /**
   * Whether use cors proxy in browser.
   * 
   * By default, it does not use cors proxy if url starts with
   * `/`, and vice versa.
   */
  useCorsProxy: boolean

  /**
   * The query string to be serialized into url
   */
  query: Record<string, string | number>

  /**
   * The body to be sent on POST
   */
  body: Record<string, any>
}