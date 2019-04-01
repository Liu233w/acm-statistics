e2e
=============================

用于 E2E 测试的项目。

## 本地启动
- 需要安装 docker
- 使用 `make server` 启动项目
- 同时也会启动一个 mock-server 和其配置工具，以便模拟项目的外部环境（主要是模拟爬虫等的外部调用）
- 在确保项目完全启动之后，使用 `make open` 启动 cypress。
- 上一步是运行在本地的，因此必须先使用 `npm install` 来安装依赖

## 视觉测试
- 这个项目使用 cypress 来进行视觉测试，测试必须在 docker 中运行，以保证生成的图片一致
- 使用 `make open` 启动的本地项目会忽略视觉测试快照的不一致。`make open` 只是用在编写测试上的。
- 使用 `make test` 在docker中运行全部E2E测试，包括视觉测试
- 使用 `make update-snapshot` 更新本地的视觉测试快照

## Mock Server
- 项目使用了 mock-server 作为 proxy 来控制外部调用
- `http-mocks` 文件夹里是一个 mock-server 的控制器，用来动态添加mock
- `GET http://mock-configurer/路径` 可以激活 `http-mocks/mocks` 文件夹下的相应mock，其中每个js文件可以返回一个object，
    object可以嵌套，在调用的时候也需要在路径中把它写出来。
- 给 cypress 新增加了一个命令 `mockServer`，用来激活特定的mock。比如 `cy.mockServer('oj/poj/backend_ok')` 将激活
    `http-mocks/mocks/oj.js` 文件夹里的 `poj.backend_ok` 函数。
  - 因为cypress的task必须要执行完毕。而mock server的某些mock不会结束，因此会阻塞cypress，只能单独拿到另一个项目里。

## examples
- examples 文件夹里是 cypress 自动生成的例子，这个可以留在编写测试的时候进行参考。

## CI
- `make ci` 可以模拟在 CI 中进行测试（使用docker），将会：
  - 使用 `--report` 参数把报告发送给 `dashboard.cypress.io`
  - mount .git 文件夹，以便生成报告。