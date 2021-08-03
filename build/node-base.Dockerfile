FROM node:16.6.1-alpine@sha256:09bd531e7820caeb6ede505b06ca64ad92d09f92b53180ca50c8e9c50beb19a7

# 将 apk 源替换成 ustc 版本
ARG APK_MIRROR=false
RUN if [ "$APK_MIRROR" != "false" ]; then \
    sed -i 's/dl-cdn.alpinelinux.org/mirrors.ustc.edu.cn/g' /etc/apk/repositories && \
    sed -i 's/http:/https:/g' /etc/apk/repositories && \
    apk update \
    ; fi

# 在 ci 上运行 codecov 需要的依赖
ARG CODECOV=false
RUN if [ "$CODECOV" != "false" ]; then \
    apk add --no-cache \
    bash \
    curl \
    ; fi

# npm 的国内镜像源
ARG NPM_MIRROR=false
RUN if [ "$NPM_MIRROR" != "false" ]; then \
    npm config set registry https://registry.npm.taobao.org \
    ; fi
