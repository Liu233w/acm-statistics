#!/bin/bash

# 从 master 分支下载 docker-compose 文件并运行

set -e

if [[ $0 == */bin/bash ]]
|| [[ $0 == */bin/zsh ]]
|| [[ $0 == */bin/fish ]]
|| [[ $0 == */bin/sh ]];
then

# 使用类似于 curl -s ...sh | bash 的方式运行的此脚本，不创建额外的文件
# 这么做是功能受限的

# 导入默认环境变量
export $(curl -s https://raw.githubusercontent.com/Liu233w/acm-statistics/master/build/template.env | xargs)

# 覆盖 namespace （用于下载 docker 镜像）
export DOCKER_REPO=liu233w/

# 执行 docker compose
curl -s https://raw.githubusercontent.com/Liu233w/acm-statistics/master/build/docker-compose.yml \
  | docker-compose -f - up

else

# 将本脚本下载到文件夹下运行，将下载 .env 和 docker-compose.yml 文件，便于用户更改程序参数

cd $(dirname $0)
curl -L -o docker-compose.yml \
  https://raw.githubusercontent.com/Liu233w/acm-statistics/master/build/docker-compose.yml
curl -L -o template.env \
  https://raw.githubusercontent.com/Liu233w/acm-statistics/master/build/template.env

if ! [ -e .env ]; then
  cat template.env | sed 's/^DOCKER_REPO=$/DOCKER_REPO=liu233w\//' > .env
  echo .env file created, remember to edit it
fi

fi

docker-compose up