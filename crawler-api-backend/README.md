crawler-api-backend
===

这是一个爬虫微服务。在这里可以通过 Rest API 调用 `crawler` 文件夹中的所有爬虫。
这些爬虫都是在服务器上运行的，因此您可以在任何地方使用本 API。

## API

### GET `/api/crawlers/:type/:username`

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