FROM node:12.4.0-alpine@sha256:3db913c274aad57981cb25ed59c0b8f99b554789d3cf2a0530d63cbb739aed16

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
