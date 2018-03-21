import axios from 'axios'

export default (ctx, inject) => {
    let crawlers = {}
    // 直接注入爬虫的源代码，已经自动注入了设置，server side only 的代码会转换成一个 axios 请求
    <% for (let key in options.crawlers) { %>
      crawlers.<%=key%> = <%=options.crawlers[key]%>
    <% } %>
    ctx.$crawlers = crawlers

    const crawlerMeta = <%=JSON.stringify(options.meta)%>
    ctx.$crawlerMeta = crawlerMeta

    inject('crawlers', crawlers)
    inject('crawlerMeta', crawlerMeta)
}
