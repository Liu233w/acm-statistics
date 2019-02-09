新版 NWPU-ACM 查询系统
===

[![Powered by ZenHub](https://img.shields.io/badge/Powered_by-ZenHub-5e60ba.svg)](https://app.zenhub.com/workspace/o/liu233w/acm-statistics/boards?repos=125616473)
[![Build Status](https://travis-ci.org/Liu233w/acm-statistics.svg?branch=master)](https://travis-ci.org/Liu233w/acm-statistics)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=acm-statistics&metric=alert_status)](https://sonarcloud.io/dashboard?id=acm-statistics)
[![codecov](https://codecov.io/gh/Liu233w/acm-statistics/branch/master/graph/badge.svg)](https://codecov.io/gh/Liu233w/acm-statistics)

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
- build: 存储了 docker 和 make 相关的代码和配置文件，用于构建和部署
- tools: 存储了部分脚本，各种用途都有

每个模块的具体内容请参考模块内的 README

## 运行环境
- 在部署时需要运行 frontend、 crawler-api-backend 和 backend
- backend 的安装及运行方式请参考该模块内的 [README](./backend/README.md)
- 至少需要 nodejs 8 以上才能运行
- 三个模块都需要使用 `npm install` 来安装依赖

## docker 方式部署、开发

- 目前的跨模块调用已经改成了基于docker的代码，因此有些功能（比如调用 crawler-api-backend）必须使用 docker 来启动
- 要使用这个功能，必须安装 docker 和 docker-compose

### 开发
- 本项目使用了 makefile 来管理模块间的依赖，请在根目录执行 `make help` 来查看说明。
- 要使用此方式进行开发，开发机还必须安装有 GNU make

### 部署

docker 方式简化了部署难度，这里有两种部署方式。请确保服务器安装了最新版本的 docker 和 docker-compose

#### 一行代码版
在 shell 中执行 `curl -s https://raw.githubusercontent.com/Liu233w/acm-statistics/master/tools/remote-docker-up.sh | bash` 即可将整个项目部署到 3000 端口。

这样做的话将无法使用 vjudge 爬虫，所以还是建议使用下面的配置文件版本。

#### 配置文件版
上面的一行代码版无法更改配置，建议用下面的这个配置文件版，按下面的步骤进行部署：

```bash
# 建立一个存放脚本和配置文件的文件夹，这里可以随便挑你喜欢的路径
mkdir -p ~/www/acm-statistics
cd ~/www/acm-statistics
# 下载脚本、添加权限
curl https://raw.githubusercontent.com/Liu233w/acm-statistics/master/tools/remote-docker-up.sh  -o run.sh
chmod +x run.sh
# 试运行脚本以生成配置文件，在显示 `.env file created, remember to edit it` 之后按 Ctrl+C 退出脚本
./run.sh
# 编辑配置文件，按照上面的说明进行修改即可
vim .env
# 现在即可正常运行脚本
./run.sh
```

设置成功之后即可使用单独的 `./run.sh` 来运行脚本，使用 systemd 或者其他工具均可。

## 开源协议
- 如无特殊声明，均为 GPL-3.0 协议
- crawler 模块中的 `crawlers` 目录中的文件为 BSD 2-Clause 协议

## 贡献代码

- 欢迎任何人贡献代码（尤其是爬虫部分）。
- git 的提交格式遵循 [Git Commit Angular 规范](https://gist.github.com/stephenparish/9941e89d80e2bc58a153)
    （[中文版](http://www.ruanyifeng.com/blog/2016/01/commit_message_change_log.html)）
- 您可以使用 [cz-cli](https://github.com/commitizen/cz-cli) 来辅助提交 commit

