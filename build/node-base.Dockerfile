FROM node:16.3.0-alpine@sha256:3e0f83e34a8a3e03d96e78fac8412b83790896c0abd4b112c7f6d0dcef60bca6

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
