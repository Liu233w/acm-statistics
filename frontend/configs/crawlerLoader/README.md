crawlerLoader
============================

专门用来引入爬虫的库。

## 引入原因

由于需要让爬虫同时在前端和后端工作，且使用同一套源代码，
本项目采用的方式是将爬虫的源代码在编译期间进行处理，然后作为 Plugin
引入页面中。loader 对爬虫的源代码进行了特殊的处理，server_only
的爬虫会被转换成一个对服务端的 ajax 请求，使用 axios 发送；其他爬虫使用
superagent 直接在浏览器端发起爬取请求。

本项目同时使用了两套 request 库（axios, superagent），axios专门用来向
服务端发送 ajax 请求，superagent 只用在爬虫中。

## 关于 `cors.js`

由于浏览器端的爬虫需要绕过“跨域请求限制”，我对 superagent 进行了一些魔改，
因此在前端的 superagent 只能用在爬虫上。 参见 [cors.js](./cors.js)

我采用前端爬虫的原因就是为了减轻服务器的负担，使用 cors-proxy 把请求操作转移
到别人的服务器上也不失为一种好办法_(:3」∠)_
