acm-statistics
===

NWPU-ACM 查询工具

## 目录结构

- frontend: 前端
- crawler: 题量查询爬虫，可以同时被前端和后端使用
- crawler-api-backend: 题量查询后端，提供了查询API

每个模块的具体内容请参考模块内的 README

## 运行环境
- 在部署时只需要运行 frontend 和 crawler-api-backend
- 至少需要 nodejs 8 以上才能运行
- 三个模块都需要使用 `npm install` 来安装依赖

## 开源协议
- 如无特殊声明，均为 GPL-3.0 协议
- crawler 模块中的 `crawlers` 目录中的文件为 BSD 2-Clause 协议

## 贡献代码

- 欢迎任何人贡献代码（尤其是爬虫部分）。
- git 的提交格式遵循 [Git Commit Angular 规范](https://gist.github.com/stephenparish/9941e89d80e2bc58a153)
    （[中文版](http://www.ruanyifeng.com/blog/2016/01/commit_message_change_log.html)）