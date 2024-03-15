存放爬虫，可以同时被前端和后端使用。部分爬虫可以同时在服务器
端和浏览器端运行。

## 爬虫编写要求

### 爬虫源代码

- 存放在 [./crawlers/](./crawlers/) 目录中
- 一个文件存放一个爬虫的源代码
- 文件名只能包含大小写英文字母、下划线和数字，且不能以数字开头
- 爬虫使用 superagent 来收发请求，使用 cheerio 来解析请求
- 将 `module.exports` 赋值成爬虫函数即可导出爬虫
- 原则上不建议再引入其他的库。如果需要让爬虫在浏览器上运行，
    不能导入项目中的其他源代码。
- 爬虫函数中可以抛出任何异常（例如“用户不存在”），系统将会正确
    处理这些异常。

#### 爬虫函数格式

参数：
- options: {Object} - 爬虫的参数，可以在 `config.yml` 中配置，参见下文
- username: {String} - 要爬取题量的用户名

返回值:
返回一个对象，其中包含如下属性
- solved: {Number} - 用户在此网站上解决的题目数量
- submissions: {Number} - 用户在此网站上的总提交数
- solvedList: {Array<String>|undefined|null} - 用户通过的题目列表，为 null 或 undefined 表示该爬虫目前无法统计题目列表。
    对于普通的爬虫（非 virtual judge），可以是如下的格式： "1001" 或者 "301-A" 之类的字符串，不需要附加爬虫名称。
    对于 virtual judge 爬虫，请参考下方的文档。
- submissionsByCrawlerName: {Object.{string, number}} - Submissions in each oj, only exists when the crawler is a virtual_judge crawler
    Key is the name of crawler. If crawler does not exists in config, the name defined by virtual_judge.
    Value is the submission count of the OJ.

### 配置文件

- 配置文件用来注册爬虫、确定爬虫类型和给爬虫提供额外信息
- `config.yml` 配置文件会在前端编译时被读取，在 crawler-api-backend
    运行时被读取。
- 由于 `config.yml` 会被纳入版本控制和 docker 生成的镜像中。您可以使用环
    境变量来配置一些敏感信息（例如 vjudge 就需要一个帐号的用户名和密码才能
    访问其 API）。
- 环境变量格式如下： `ACM_STATISTICS_CRAWLER_ENV__crawlers__vjudge__crawler_login_user="myuser"`
  - 必须使用前缀来表示环境变量是爬虫的环境变量，使用`__`来表示对象的字段。
  - 结果将作为json解析，因此需要将字符串形式的值放进双引号中。
  - 环境变量的名称将指定一个 `config.yml` 中的字段，并用值来覆盖它
  - 也可以指定一个json格式的值，并覆盖整个对象，比如
      `ACM_STATISTICS_CRAWLER_ENV__crawler_order=["hdu", "poj"]`

- 请注意，如果您需要在 config 中填写隐私信息，请将爬虫设置成
    server_only。否则编译出的前端静态文件中将会含有您的隐私信息。
    
- 对于 crawler-api-backend 而言，由于在运行期间才会读取 config 文件，因
    此可以用 docker 的 env 选项来设置环境变量。对于 frontend，会在生成 dcoker
    镜像期间读取配置文件，因此需要在 docker-build 时指定 env 来覆盖配置。
    
#### 配置文件格式

- name: {String} - 爬虫的名字，

##### crawlers

是一个对象，含有每个爬虫的配置项

crawlers 中的 key 是爬虫的名字，必须跟 crawlers 文件夹中的爬虫文件名相同
    （不需要含有 `js` 后缀名）。
crawlers 中的 value 是一个字典，其中包含以下字段：
- server_only: {Boolean} - 如果省略，为 false。
    本字段表示爬虫是否只出现在服务器中。
    如果为true，爬虫在前端的代码将是一个对服务器的api请求。
    即不会在前端进行爬取，只在服务器运行此爬虫。
- meta: 元信息，包含标题等等的信息，可以显示在前端或者用在爬虫API中
  - title: {String} - OJ名，这是显示在前端上作为标题的
  - description: {String} - OJ的说明，可以补充一些信息，会在前端显示出来。这是可选的
  - url: {String} - 到OJ的链接，也是可选的。
  - virtual_judge: {Boolean} - 是否是 virtual_judge，在计算题目列表时逻辑会稍有不同。
- 其他字段: 配置文件中可以包含任何其他字段

在运行爬虫时，crawlers中的每一项都会被传递给相应的爬虫（从
options 形参），爬虫可以在运行时读取相应的参数。

您写在 meta 字段中的内容会被编译进前端页面，无论爬虫是否为 server_only

##### crawler_order

一个数组，每个元素是一个字符串，表示爬虫的名字。这里的名字必须和 `crawlers` 中的key对应，
也和爬虫的文件对应。系统将根据这里的配置文件装配爬虫，也就是说只要在配置文件中删除此字段，
本爬虫就不会被装配，不会出现在前端或后端中。这个数组的顺序决定了爬虫在前端页面中的显示顺序。

### 额外信息

在爬虫的 config 属性里也会被传入额外的信息，此信息会在 configReader 中自动生成，也可以被
配置文件中的同名字段覆盖。

- env: {String} - 当前爬虫在哪里运行
  - 'server': 如果爬虫函数正在服务器上运行，传入此值
  - 'browser': 浏览器……

### virtual judge 爬虫

对于 vjudge 这样的爬虫，为了避免重复计算题目列表，需要使用 solvedList 来表示所有通过的题目，然后进行去重。
返回的 solvedList 格式如下： "{爬虫名}-{题目编号}"。爬虫名为对应的爬虫的 name（即文件名）。题目编号必须跟该爬虫返回的编号格式相同。
比如 "hdu-2001"。如果当前不存在此类的爬虫，使用一个和当前爬虫不重复的名字即可。

### 参考

您可以参考 hdu 爬虫和 vjudge 爬虫来编写您的爬虫，这两个分别是
前后端通用爬虫和server_only爬虫。

## 测试

`test/crawlers.test.js` 中有爬虫的测试文件，原则上测试用例应该至少覆盖主路径
