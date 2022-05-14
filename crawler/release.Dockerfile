ARG CRAWLER_BASE_IMAGE
ARG NODE_BASE_IMAGE


FROM ${CRAWLER_BASE_IMAGE} AS base

RUN rm -rf node_modules

FROM ${NODE_BASE_IMAGE}

WORKDIR /var/project

COPY package.json pnpm-lock.yaml ./
RUN pnpm install --only=production

COPY --from=base /var/project .
