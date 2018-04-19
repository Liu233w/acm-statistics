新版 NWPU-ACM 查询系统
===

[![Powered by ZenHub](https://img.shields.io/badge/Powered_by-ZenHub-5e60ba.svg)](https://app.zenhub.com/workspace/o/liu233w/acm-statistics/boards?repos=125616473)
[![Build Status](https://travis-ci.org/Liu233w/acm-statistics.svg?branch=master)](https://travis-ci.org/Liu233w/acm-statistics)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=acm-statistics&metric=alert_status)](https://sonarcloud.io/dashboard?id=acm-statistics)
[![codecov](https://codecov.io/gh/Liu233w/acm-statistics/branch/master/graph/badge.svg)](https://codecov.io/gh/Liu233w/acm-statistics)
[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bgithub.com%2FLiu233w%2Facm-statistics.svg?type=shield)](https://app.fossa.io/projects/git%2Bgithub.com%2FLiu233w%2Facm-statistics?ref=badge_shield)

#### 功能
- 题量查询
#### 开发中
- 历史记录
- 题量追踪
- 邮件提醒
- 排行榜
- 查重
- ……

## 目录结构

- frontend: 前端
- crawler: 题量查询爬虫，可以同时被前端和后端使用
- crawler-api-backend: 题量查询后端，提供了查询API
- backend: 项目后端，提供身份验证、权限验证、邮件发送以及其他需要读写数据库的功能。本模块存储在
    [acm-statistics-abp](https://github.com/Liu233w/acm-statistics-abp) 中，使用 `git subtree` 来同步

每个模块的具体内容请参考模块内的 README

## 运行环境
- 在部署时需要运行 frontend、 crawler-api-backend 和 backend
- backend 的安装及运行方式请参考该模块内的 [README](./backend/README.md)
- 至少需要 nodejs 8 以上才能运行
- 三个模块都需要使用 `npm install` 来安装依赖

## 开源协议
- 如无特殊声明，均为 GPL-3.0 协议
- crawler 模块中的 `crawlers` 目录中的文件为 BSD 2-Clause 协议

## 贡献代码

- 欢迎任何人贡献代码（尤其是爬虫部分）。
- git 的提交格式遵循 [Git Commit Angular 规范](https://gist.github.com/stephenparish/9941e89d80e2bc58a153)
    （[中文版](http://www.ruanyifeng.com/blog/2016/01/commit_message_change_log.html)）


## License
[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bgithub.com%2FLiu233w%2Facm-statistics.svg?type=large)](https://app.fossa.io/projects/git%2Bgithub.com%2FLiu233w%2Facm-statistics?ref=badge_large)