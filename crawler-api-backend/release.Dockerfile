ARG CRAWLER_IMAGE
ARG BACKEND_BASE_IMAGE
ARG NODE_BASE_IMAGE


FROM ${CRAWLER_IMAGE} AS crawler


FROM ${BACKEND_BASE_IMAGE} AS base
RUN rm -rf node_modules


FROM ${NODE_BASE_IMAGE} AS build
ARG CRAWLER_LIBRARY_PATH

WORKDIR /var/project

COPY package.json package-lock.json ./
RUN pnpm install --only=production && rm -rf node_modules/crawler

COPY --from=crawler ${CRAWLER_LIBRARY_PATH} ./node_modules/crawler

COPY --from=base /var/project .

FROM ${NODE_BASE_IMAGE}

WORKDIR /var/project
COPY --from=build /var/project .

ENV NODE_ENV production

EXPOSE 12001

CMD ["pnpm", "start"]
