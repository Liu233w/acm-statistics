ARG CRAWLER_IMAGE
ARG BACKEND_BASE_IMAGE
ARG NODE_BASE_IMAGE


FROM ${CRAWLER_IMAGE} AS crawler


FROM ${BACKEND_BASE_IMAGE} AS base
RUN rm -rf node_modules


FROM ${NODE_BASE_IMAGE} AS build

WORKDIR /var/project

COPY --from=crawler /var/project ../crawler

COPY package.json pnpm-lock.yaml ./
RUN pnpm install --only=production

COPY --from=base /var/project .

FROM ${NODE_BASE_IMAGE}

WORKDIR /var/project

COPY --from=crawler /var/project ../crawler
COPY --from=build /var/project .

ENV NODE_ENV production

EXPOSE 12001

CMD ["pnpm", "start"]
