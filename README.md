acm-statistics
===

[![Powered by ZenHub](https://img.shields.io/badge/Powered_by-ZenHub-5e60ba.svg)](https://app.zenhub.com/workspace/o/liu233w/acm-statistics/boards?repos=125616473)
[![Build Status](https://travis-ci.org/Liu233w/acm-statistics.svg?branch=master)](https://travis-ci.org/Liu233w/acm-statistics)

NWPU-ACM 查询工具

## 项目状态
[![](https://codescene.io/projects/2599/status.svg)](https://codescene.io/projects/2599/jobs/latest-successful/results)

|模块|状态|
|----|------|
|frontend|[![](https://www.versioneye.com/user/projects/5ab717300fb24f0ac49c2bcb/badge.svg)](https://www.versioneye.com/user/projects/5ab717300fb24f0ac49c2bcb)|
|crawler|[![](https://www.versioneye.com/user/projects/5ab717300fb24f4489395c40/badge.svg)](https://www.versioneye.com/user/projects/5ab717300fb24f4489395c40)|
|crawler-api-backend|[![](https://www.versioneye.com/user/projects/5ab7172e0fb24f0ac49c2bbd/badge.svg)](https://www.versioneye.com/user/projects/5ab7172e0fb24f0ac49c2bbd)|

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
