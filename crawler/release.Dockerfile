ARG CRAWLER_BASE_IMAGE
ARG NODE_BASE_IMAGE


FROM ${CRAWLER_BASE_IMAGE} AS base

# 排除 devDependencies
RUN rm -rf node_modules

FROM ${NODE_BASE_IMAGE} AS build

WORKDIR /var/project

COPY package.json package-lock.json ./
RUN pnpm install --only=production

COPY --from=base /var/project .


FROM ${NODE_BASE_IMAGE}

# 预先定义工作路径，便于同其他项目解耦。
# 必须在 FROM 下面定义，否则这里 ARG 会变成空字符串
ARG CRAWLER_LIBRARY_PATH
WORKDIR ${CRAWLER_LIBRARY_PATH}

COPY --from=build /var/project .