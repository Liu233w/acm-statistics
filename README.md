# This repo contains the source code of OJ Analyzer

简体中文版：[README_zh-hans.md](./README_zh-hans.md)

[![Powered by ZenHub](https://img.shields.io/badge/Powered_by-ZenHub-5e60ba.svg)](https://app.zenhub.com/workspace/o/liu233w/acm-statistics/boards?repos=125616473)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=acm-statistics&metric=alert_status)](https://sonarcloud.io/dashboard?id=acm-statistics)
[![codecov](https://codecov.io/gh/Liu233w/acm-statistics/branch/master/graph/badge.svg)](https://codecov.io/gh/Liu233w/acm-statistics)
[![Cypress.io](https://img.shields.io/badge/cypress.io-tests-green.svg)](https://dashboard.cypress.io/#/projects/4s32o7/runs)
[![Renovate enabled](https://img.shields.io/badge/renovate-enabled-brightgreen.svg)](https://app.renovatebot.com/dashboard#github/Liu233w/acm-statistics)
[![Mergify Status](https://img.shields.io/badge/Mergify-enabled-green.svg)](https://mergify.io)

<!-- ALL-CONTRIBUTORS-BADGE:START - Do not remove or modify this section -->
[![All Contributors](https://img.shields.io/badge/all_contributors-15-orange.svg?style=flat-square)](#contributors-)
<!-- ALL-CONTRIBUTORS-BADGE:END -->

#### Build status

![Unit Tests](https://github.com/Liu233w/acm-statistics/workflows/Unit%20Tests/badge.svg)
![Test E2E](https://github.com/Liu233w/acm-statistics/workflows/Test%20E2E/badge.svg)

#### Features

- Querying ac/submissions of oj
- Storing querying history

#### Under development

- Email support
- Ranks
- ……

## Directory structure

- frontend: The front end
- crawler: Crawlers to query OJs. Being used by both frontend and backend
- crawler-api-backend: A microservice that provides querying api
- e2e: E2E tests
- backend: The back end, a monoservice
- captcha-service: A microservice that provides captcha support
- ohunt: A stateful, standalone crawler microservice used to support certain OJs such as ZOJ.
- build: Codes to build and deploy the project. Tool chain: docker, docker-compose, GNU make.
- tools: Utility scripts and config files in operation

See the README file in each module for specific documents.

## Developing and deploying in docker

- The project needs docker and docker-compose to function correctly.

### Development

- This project uses makefile to manage dependency between modules. Execute `make help` in repository root to view document.
- GNU make is required.

### Deploy

There are two ways to deploy this project in a server.

#### One-liner

Execute following code in shell to deploy the project to port 3000.

`curl -s https://raw.githubusercontent.com/Liu233w/acm-statistics/master/tools/remote-docker-up.sh | bash`

Vjudge crawler is not available in this way.

#### Config file version

In this way you are able to customise the configuration, enabling all features.

```bash
# Create a folder to store config files
mkdir -p ~/www/acm-statistics
cd ~/www/acm-statistics
# Download runner script and add permissions
curl https://raw.githubusercontent.com/Liu233w/acm-statistics/master/tools/remote-docker-up.sh  -o run.sh
chmod +x run.sh
# Run the script once to generate configuration file. It will exit after the line `.env file created, remember to edit it` is shown.
./run.sh
# Edit the config file following the description in it.
vim .env
# Now we can run the project by the script
./run.sh
```

Then you can use tools such as systemd to run `./run.sh`.

[./tools/acm-statistics.service](./tools/acm-statistics.service) is a template config file of systemd.

`run.sh` checks updates when it is starting. If there are updates to `template.env`, `run.sh` will exit and ask you to compare these two files. **The script compares the line count of the two files to check update, please make sure they are identical when editing.**

## Management

- Set the url of adminer in `.env` file. It is `/adminer` by default.
  - You can view and edit database via adminer.
  - The name of the database is `acm_statistics`. Username is `root`. You can set password in `.env`
- Backups are created automatically in 3:00am each day, stored in `db-backup` folder, which is in the folder that contains config files.

## License

- All source code except the code in `crawler/crawlers` are under AGPL-3.0 license
- The code in `crawler/crawlers` are under BSD 2-Clause license.

## Contribution

- All contribution especially crawlers are welcomed.
- Please follow [Commit Message Conventions](https://gist.github.com/stephenparish/9941e89d80e2bc58a153) when writing git commit messages.
- You may use [cz-cli](https://github.com/commitizen/cz-cli) to help writing commit messages.

## Contributors ✨

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tr>
    <td align="center"><img src="https://avatars0.githubusercontent.com/u/22635759?v=4" width="100px;" alt=""/><br /><sub><a href="https://www.cometeme.tech"><b>Adelard Collins</b></a><a href="https://github.com/cometeme">🔗</a></sub><br /><a href="https://github.com/Liu233w/acm-statistics/issues?q=author%3Acometeme" title="Bug reports">🐛</a></td>
    <td align="center"><img src="https://avatars1.githubusercontent.com/u/64258212?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/BackSlashDelta"><b>BackSlashDelta</b></a><a href="https://github.com/BackSlashDelta">🔗</a></sub><br /><a href="https://github.com/Liu233w/acm-statistics/issues?q=author%3ABackSlashDelta" title="Bug reports">🐛</a></td>
    <td align="center"><img src="https://avatars0.githubusercontent.com/u/35862184?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/bodhisatan"><b>Bodhisatan_Yao</b></a><a href="https://github.com/bodhisatan">🔗</a></sub><br /><a href="https://github.com/Liu233w/acm-statistics/issues?q=author%3Abodhisatan" title="Bug reports">🐛</a></td>
    <td align="center"><img src="https://avatars3.githubusercontent.com/u/25352156?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/Geekxiong"><b>Geekxiong</b></a><a href="https://github.com/Geekxiong">🔗</a></sub><br /><a href="#ideas-Geekxiong" title="Ideas, Planning, & Feedback">🤔</a></td>
    <td align="center"><img src="https://avatars2.githubusercontent.com/u/39403985?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/settings/profile"><b>Halorv</b></a><a href="https://github.com/Halorv">🔗</a></sub><br /><a href="#ideas-Halorv" title="Ideas, Planning, & Feedback">🤔</a></td>
    <td align="center"><img src="https://avatars3.githubusercontent.com/u/11661760?v=4" width="100px;" alt=""/><br /><sub><a href="https://kidozh.com"><b>Kido Zhang</b></a><a href="https://github.com/kidozh">🔗</a></sub><br /><a href="#infra-kidozh" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="#ideas-kidozh" title="Ideas, Planning, & Feedback">🤔</a></td>
  </tr>
  <tr>
    <td align="center"><img src="https://avatars2.githubusercontent.com/u/16333687?v=4" width="100px;" alt=""/><br /><sub><a href="https://liu233w.github.io"><b>Liu233w</b></a><a href="https://github.com/Liu233w">🔗</a></sub><br /><a href="https://github.com/Liu233w/acm-statistics/commits?author=Liu233w" title="Code">💻</a> <a href="#ideas-Liu233w" title="Ideas, Planning, & Feedback">🤔</a> <a href="#infra-Liu233w" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a> <a href="https://github.com/Liu233w/acm-statistics/commits?author=Liu233w" title="Tests">⚠️</a></td>
    <td align="center"><img src="https://avatars1.githubusercontent.com/u/55663936?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/Meulsama"><b>Meulsama</b></a><a href="https://github.com/Meulsama">🔗</a></sub><br /><a href="#ideas-Meulsama" title="Ideas, Planning, & Feedback">🤔</a></td>
    <td align="center"><img src="https://avatars3.githubusercontent.com/u/50655871?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/UserUnknownX"><b>Michael Xiang</b></a><a href="https://github.com/UserUnknownX">🔗</a></sub><br /><a href="https://github.com/Liu233w/acm-statistics/issues?q=author%3AUserUnknownX" title="Bug reports">🐛</a></td>
    <td align="center"><img src="https://avatars1.githubusercontent.com/u/11994295?v=4" width="100px;" alt=""/><br /><sub><a href="http://zhao.wtf"><b>Zhao</b></a><a href="https://github.com/2512821228">🔗</a></sub><br /><a href="https://github.com/Liu233w/acm-statistics/issues?q=author%3A2512821228" title="Bug reports">🐛</a></td>
    <td align="center"><img src="https://avatars.githubusercontent.com/u/49401963?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/bluebear4"><b>bluebear4</b></a><a href="https://github.com/bluebear4">🔗</a></sub><br /><a href="https://github.com/Liu233w/acm-statistics/issues?q=author%3Abluebear4" title="Bug reports">🐛</a></td>
    <td align="center"><img src="https://avatars3.githubusercontent.com/u/22322656?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/ctuu"><b>ct</b></a><a href="https://github.com/ctuu">🔗</a></sub><br /><a href="https://github.com/Liu233w/acm-statistics/issues?q=author%3Actuu" title="Bug reports">🐛</a></td>
  </tr>
  <tr>
    <td align="center"><img src="https://avatars2.githubusercontent.com/u/9880740?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/flylai"><b>flylai</b></a><a href="https://github.com/flylai">🔗</a></sub><br /><a href="https://github.com/Liu233w/acm-statistics/commits?author=flylai" title="Code">💻</a> <a href="https://github.com/Liu233w/acm-statistics/issues?q=author%3Aflylai" title="Bug reports">🐛</a></td>
    <td align="center"><img src="https://avatars3.githubusercontent.com/u/36151020?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/fzu-h4cky"><b>fzu-h4cky</b></a><a href="https://github.com/fzu-h4cky">🔗</a></sub><br /><a href="https://github.com/Liu233w/acm-statistics/issues?q=author%3Afzu-h4cky" title="Bug reports">🐛</a></td>
    <td align="center"><img src="https://avatars2.githubusercontent.com/u/43291744?v=4" width="100px;" alt=""/><br /><sub><a href="https://github.com/zby0327"><b>zby</b></a><a href="https://github.com/zby0327">🔗</a></sub><br /><a href="#ideas-zby0327" title="Ideas, Planning, & Feedback">🤔</a> <a href="https://github.com/Liu233w/acm-statistics/issues?q=author%3Azby0327" title="Bug reports">🐛</a></td>
  </tr>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!
