ARG FRONTEND_BASE_IMAGE
ARG NODE_BASE_IMAGE
ARG CRAWLER_IMAGE


FROM ${CRAWLER_IMAGE} AS crawler


FROM ${FRONTEND_BASE_IMAGE} AS base
RUN pnpm run build
RUN rm -rf node_modules


FROM ${NODE_BASE_IMAGE} AS build
ARG CRAWLER_LIBRARY_PATH

WORKDIR /var/project

RUN apk add --no-cache make gcc g++ python3

COPY package.json package-lock.json ./
RUN pnpm install --only=production && rm -rf node_modules/crawler

COPY --from=crawler ${CRAWLER_LIBRARY_PATH} ./node_modules/crawler

COPY --from=base /var/project .


FROM ${NODE_BASE_IMAGE}
ARG VERSION_NUM
ARG BUILD_TIME

WORKDIR /var/project

COPY --from=build /var/project .

ENV \
    HOST=0.0.0.0 \
    NODE_ENV=production \
    PORT=3000 \
    VERSION_NUM="${VERSION_NUM}" \
    BUILD_TIME="${BUILD_TIME}" \
    ;=;

EXPOSE 3000

CMD ["pnpm", "start"]
