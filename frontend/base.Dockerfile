ARG CRAWLER_IMAGE
ARG NODE_BASE_IMAGE


FROM ${CRAWLER_IMAGE} AS crawler


FROM ${NODE_BASE_IMAGE}

WORKDIR /var/project

RUN apk add --no-cache make gcc g++ python3

COPY --from=crawler /var/project ../crawler

COPY package.json pnpm-lock.yaml .npmrc ./
RUN pnpm install

COPY . .

ENV VERSION_NUM=development
ENV BUILD_TIME=0
