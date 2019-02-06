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

# docker 的镜像名称
export DOCKER_TAG=:latest
export DOCKER_REPO=liu233w/

# 输出端口
export EXPOSE_PORT=3000

# 执行 docker compose
curl -s https://raw.githubusercontent.com/Liu233w/acm-statistics/master/build/docker-compose.yml \
  | docker-compose -f - up

else

# 将本脚本下载到文件夹下运行，将下载 .env 和 docker-compose.yml 文件，便于用户更改程序参数

cd $(dirname $0)
curl -L -o docker-compose.yml \
  https://raw.githubusercontent.com/Liu233w/acm-statistics/master/build/docker-compose.yml
curl -L -o .env.template \
  https://raw.githubusercontent.com/Liu233w/acm-statistics/master/build/.env.template

echo remember to edit your .env file
cp .env.template .env

docker-compose up