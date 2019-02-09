ARG CRAWLER_BASE_IMAGE
ARG NODE_BASE_IMAGE


FROM ${CRAWLER_BASE_IMAGE} AS build

# 排除 devDependencies
RUN rm -rf node_modules && npm install --only=production


FROM ${NODE_BASE_IMAGE}

# 预先定义工作路径，便于同其他项目解耦。
# 必须在 FROM 下面定义，否则这里 ARG 会变成空字符串
ARG CRAWLER_LIBRARY_PATH
WORKDIR ${CRAWLER_LIBRARY_PATH}

COPY --from=build /var/project .