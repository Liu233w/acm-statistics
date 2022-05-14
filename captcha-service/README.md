captcha-service
===

验证码微服务。目前只有 `/api/captcha-service/generate` 对前端开放。验证部分只能从内部访问。

[测试文件](./__test__/app.spec.js)中有API的用例。

## 本地运行

- 使用 `pnpm start` 运行
- 在运行之后会监视 `localhost:80`

## 部署
- 使用 docker
