FROM node:16.13.0-alpine@sha256:8f1827381eb7fca5a79ad21cb42e935546bedf67d9f668519a7db69d77d812bf

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
