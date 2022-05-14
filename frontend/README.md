# acm-statistics-frontend

> ACM查题网站前端

## Using Docker

- `make dev` 启动开发模式（需要先使用pnpm安装依赖）
- `make update-snapshot` 更新快照
- `make test` 或 `make test-ci` 来运行测试

## Build Setup

``` bash
# install dependencies
$ pnpm install # Or yarn install

# serve with hot reload at localhost:3000
$ pnpm run dev

# build for production and launch server
$ pnpm run build
$ pnpm start
```

## 其他命令

- `pnpm run lint` 运行代码质量检测
- `pnpm run lintfix` 自动修复部分代码质量问题
- `pnpm run analyze` 分析build出来的文件大小、分布，生成报告
- `pnpm run test` 运行测试
- `pnpm run snapshot` 交互式更新快照

For detailed explanation on how things work, check out the [Nuxt.js](https://github.com/nuxt/nuxt.js) and [Vuetify.js](https://vuetifyjs.com/) documentation.

## 版权声明
- 本产品的主页部分使用了 [vuetifyjs/parallax-starter](https://github.com/vuetifyjs/parallax-starter) 模板，根据其开源协议，本项目的[主页源代码](./pages/index.vue)中包含了其协议声明。
