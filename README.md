新版 NWPU-ACM 查询系统
===

[![Powered by ZenHub](https://img.shields.io/badge/Powered_by-ZenHub-5e60ba.svg)](https://app.zenhub.com/workspace/o/liu233w/acm-statistics/boards?repos=125616473)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=acm-statistics&metric=alert_status)](https://sonarcloud.io/dashboard?id=acm-statistics)
[![codecov](https://codecov.io/gh/Liu233w/acm-statistics/branch/master/graph/badge.svg)](https://codecov.io/gh/Liu233w/acm-statistics)
[![Cypress.io](https://img.shields.io/badge/cypress.io-tests-green.svg)](https://dashboard.cypress.io/#/projects/4s32o7/runs)
[![Renovate enabled](https://img.shields.io/badge/renovate-enabled-brightgreen.svg)](https://renovatebot.com/)
[![Mergify Status](https://img.shields.io/badge/Mergify-enabled-green.svg)](https://mergify.io)
[![Deliverybot enabled](https://img.shields.io/badge/Deliverybot-enabled-blue.svg)](https://app.deliverybot.dev/Liu233w/acm-statistics/branch/master)
<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-12-orange.svg?style=flat-square)](#contributors-)
<!-- ALL-CONTRIBUTORS-BADGE:END -->

#### 构建状态

![Unit Tests](https://github.com/Liu233w/acm-statistics/workflows/Unit%20Tests/badge.svg)
![Test E2E](https://github.com/Liu233w/acm-statistics/workflows/Test%20E2E/badge.svg)


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
- e2e: 关于 e2e 测试相关的代码。
- backend: 后端代码
- captcha-service: 验证码微服务
- build: 存储了 docker 和 make 相关的代码和配置文件，用于构建和部署
- tools: 存储了部分脚本，各种用途都有

每个模块的具体内容请参考模块内的 README

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
# 试运行脚本以生成配置文件，在显示 `.env file created, remember to edit it` 之后会自动退出脚本
./run.sh
# 编辑配置文件，按照上面的说明进行修改即可
vim .env
# 现在即可正常运行脚本
./run.sh
```

设置成功之后即可使用单独的 `./run.sh` 来运行脚本，使用 systemd 或者其他工具均可。

`./tools/acm-statistics.service` 里是一个 systemd 配置文件的参考。

如果默认的 `template.env` 有更新，`run.sh` 会自动退出并提示您更新 `.env`。**脚本通过比较两个文件的行数来判断是否有更新，在编辑文件时请确保行数一致**

## 管理
- 在 .env 文件中设定 adminer 的url，默认为 `/adminer`
  - 可以查看并修改数据库
  - 数据库名称为 acm_statistics，用户名为 root，密码在 .env 中设定
- 数据库会在每天3:00am自动进行备份，保存在 `/db-backup` 中

## 开源协议
- 如无特殊声明，均为 GPL-3.0 协议
- crawler 模块中的 `crawlers` 目录中的文件为 BSD 2-Clause 协议

## 贡献代码

- 欢迎任何人贡献代码（尤其是爬虫部分）。
- git 的提交格式遵循 [Git Commit Angular 规范](https://gist.github.com/stephenparish/9941e89d80e2bc58a153)
    （[中文版](http://www.ruanyifeng.com/blog/2016/01/commit_message_change_log.html)）
- 您可以使用 [cz-cli](https://github.com/commitizen/cz-cli) 来辅助提交 commit


## Contributors ✨

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tr>
    <td align="center"><img src="https://avatars2.githubusercontent.com/u/16333687?v=4" width="100px;" alt=""/><br /><sub><a href="https://liu233w.github.io"><b>Liu233w</b></a><a href="https://github.com/Liu233w">🔗</a></sub><br /><a href="https://github.com/Liu233w/acm-statistics/commits?author=Liu233w" title="Code">💻</a> <a href="#ideas-Liu233w" title="Ideas, Planning, & Feedback">🤔</a> <a href="#infra-Liu233w" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/Liu233w/acm-statistics/commits?author=Liu233w" title="Tests">⚠️</a></td>
    <td align="center"><img src="https://avatars3.githubusercontent.com/u/11661760?v=4" width="100px;" alt=""/><br /><sub><a href="https://kidozh.com"><b>Kido Zhang</b></a><a href="https://github.com/kidozh">🔗</a></sub><br /><a href="#infra-kidozh" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="#ideas-kidozh" title="Ideas, Planning, & Feedback">🤔</a></td>
    <td align="center"><img src="https://avatars2.githubusercontent.com/u/9880740?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/flylai"><b>flylai</b></a><a href="https://github.com/flylai">🔗</a></sub><br /><a href="https://github.com/Liu233w/acm-statistics/commits?author=flylai" title="Code">💻</a> <a href="https://github.com/Liu233w/acm-statistics/issues?q=author%3Aflylai" title="Bug reports">🐛</a></td>
    <td align="center"><img src="https://avatars3.githubusercontent.com/u/36151020?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/fzu-h4cky"><b>fzu-h4cky</b></a><a href="https://github.com/fzu-h4cky">🔗</a></sub><br /><a href="https://github.com/Liu233w/acm-statistics/issues?q=author%3Afzu-h4cky" title="Bug reports">🐛</a></td>
    <td align="center"><img src="https://avatars1.githubusercontent.com/u/11994295?v=4" width="100px;" alt=""/><br /><sub><a href="http://zhao.wtf"><b>Zhao</b></a><a href="https://github.com/2512821228">🔗</a></sub><br /><a href="https://github.com/Liu233w/acm-statistics/issues?q=author%3A2512821228" title="Bug reports">🐛</a></td>
    <td align="center"><img src="https://avatars0.githubusercontent.com/u/22635759?v=4" width="100px;" alt=""/><br /><sub><a href="https://www.cometeme.tech"><b>Adelard Collins</b></a><a href="https://github.com/cometeme">🔗</a></sub><br /><a href="https://github.com/Liu233w/acm-statistics/issues?q=author%3Acometeme" title="Bug reports">🐛</a></td>
    <td align="center"><img src="https://avatars3.githubusercontent.com/u/22322656?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/ctuu"><b>ct</b></a><a href="https://github.com/ctuu">🔗</a></sub><br /><a href="https://github.com/Liu233w/acm-statistics/issues?q=author%3Actuu" title="Bug reports">🐛</a></td>
  </tr>
  <tr>
    <td align="center"><img src="https://avatars3.githubusercontent.com/u/25352156?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/Geekxiong"><b>Geekxiong</b></a><a href="https://github.com/Geekxiong">🔗</a></sub><br /><a href="#ideas-Geekxiong" title="Ideas, Planning, & Feedback">🤔</a></td>
    <td align="center"><img src="https://avatars2.githubusercontent.com/u/39403985?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/settings/profile"><b>Halorv</b></a><a href="https://github.com/Halorv">🔗</a></sub><br /><a href="#ideas-Halorv" title="Ideas, Planning, & Feedback">🤔</a></td>
    <td align="center"><img src="https://avatars0.githubusercontent.com/u/35862184?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/bodhisatan"><b>Bodhisatan_Yao</b></a><a href="https://github.com/bodhisatan">🔗</a></sub><br /><a href="https://github.com/Liu233w/acm-statistics/issues?q=author%3Abodhisatan" title="Bug reports">🐛</a></td>
    <td align="center"><img src="https://avatars1.githubusercontent.com/u/55663936?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/Meulsama"><b>Meulsama</b></a><a href="https://github.com/Meulsama">🔗</a></sub><br /><a href="#ideas-Meulsama" title="Ideas, Planning, & Feedback">🤔</a></td>
    <td align="center"><img src="https://avatars3.githubusercontent.com/u/50655871?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/UserUnknownX"><b>Michael Xiang</b></a><a href="https://github.com/UserUnknownX">🔗</a></sub><br /><a href="https://github.com/Liu233w/acm-statistics/issues?q=author%3AUserUnknownX" title="Bug reports">🐛</a></td>
  </tr>
</table>

<!-- markdownlint-enable -->
<!-- prettier-ignore-end -->
<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!
