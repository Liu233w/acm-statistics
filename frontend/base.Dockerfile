ARG CRAWLER_IMAGE
ARG NODE_BASE_IMAGE


FROM ${CRAWLER_IMAGE} AS crawler


FROM ${NODE_BASE_IMAGE}
ARG CRAWLER_LIBRARY_PATH

WORKDIR /var/project

RUN apk add --no-cache make gcc g++ python3

COPY package.json pnpm-lock.yaml ./
RUN pnpm install && rm -rf ./node_modules/crawler

COPY --from=crawler ${CRAWLER_LIBRARY_PATH} ./node_modules/crawler

COPY . .

ENV VERSION_NUM=development
ENV BUILD_TIME=0
