ARG CRAWLER_IMAGE
ARG NODE_BASE_IMAGE


FROM ${CRAWLER_IMAGE} AS crawler


FROM ${NODE_BASE_IMAGE}

WORKDIR /var/project

COPY --from=crawler /var/project ../crawler

COPY package.json pnpm-lock.yaml ./
RUN pnpm install

COPY . .
