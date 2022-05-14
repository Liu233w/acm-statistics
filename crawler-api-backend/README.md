crawler-api-backend
===

这是一个爬虫微服务。在这里可以通过 Rest API 调用 `crawler` 文件夹中的所有爬虫。
这些爬虫都是在服务器上运行的，因此您可以在任何地方使用本 API。

## API

### GET `/api/crawlers/swagger.json`

返回 swagger 的 json 形式的文档。文档在 [swagger.json](./swagger.json) 中编辑。

如果 `swagger.json` 中的描述与本文档有出入，以 `swagger.json` 中的为准。

### GET `/api/crawlers/:type/:username`

查询某个OJ上的用户题量

#### 参数
- type: OJ类型，只包括在 `/crawler/config.yml` 中定义的类型
- username: 要查询的用户的用户名

#### 成功返回

- 状态码： 200

```json
{
  "error": false,
  "data": {
    "solved": 1,
    "submissions": 2
  }
}
```

#### 失败返回

- 状态码： 400

```json
{
  "error": true,
  "message": "错误信息"
}
```

### GET `/api/crawlers`

返回所有支持查询的 OJ

#### 成功返回
```json
{
  "error": false,
  "data": {
    "poj": {
      "title": "POJ",
      "description": "",
      "url": "http://poj.org/"
    },
    "vjudge": {},
    "...其他OJ": {}
  }
}
```

## 部署

- 使用 `pnpm start` 运行
- 在运行之后会监视 `localhost:80`
