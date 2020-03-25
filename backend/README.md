# 后端代码 （abp实现）

## 运行环境
- docker docker-compose

## 开发环境
- docker docker-compose （必要）
- dotnet core 3.1
- Visual Studio 2019 （安装docker支持）

## 运行方式

本项目不能脱离docker运行。

### 仅运行
- 仅运行时不需要装有vs 2019或者dotnet core开发环境
- 与其他项目相同，使用 `make build` 进行构建，`make test`进行测试

### 开发
- 可以直接使用visual studio 2019的container tool进行调试
- 使用vs打开sln文件，将 docker-compose 设为启动项目，然后直接调试即可
- 在进行调试之前，需要先在本目录的上级目录运行 `make build` 来构建其他依赖项
- 在调试状态下，可以从 `localhost:8080` 访问数据库信息
- 在 `../build/.env` 文件中查看和修改默认密码等数据
