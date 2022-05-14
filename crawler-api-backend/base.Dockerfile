ARG CRAWLER_IMAGE
ARG NODE_BASE_IMAGE


FROM ${CRAWLER_IMAGE} AS crawler


FROM ${NODE_BASE_IMAGE}
ARG CRAWLER_LIBRARY_PATH

WORKDIR /var/project

COPY package.json package-lock.json ./
RUN pnpm install && rm -rf ./node_modules/crawler

COPY --from=crawler ${CRAWLER_LIBRARY_PATH} ./node_modules/crawler

COPY . .
