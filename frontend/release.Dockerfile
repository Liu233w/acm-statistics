ARG FRONTEND_BASE_IMAGE
ARG NODE_BASE_IMAGE
ARG CRAWLER_IMAGE


FROM ${CRAWLER_IMAGE} AS crawler


FROM ${FRONTEND_BASE_IMAGE} AS base
RUN pnpm run build
RUN rm -rf node_modules


FROM ${NODE_BASE_IMAGE}
ARG VERSION_NUM
ARG BUILD_TIME

WORKDIR /var/project

COPY --from=crawler /var/project ../crawler

COPY package.json pnpm-lock.yaml ./
RUN pnpm install --only=production

COPY --from=base /var/project .

ENV \
    HOST=0.0.0.0 \
    NODE_ENV=production \
    PORT=3000 \
    VERSION_NUM="${VERSION_NUM}" \
    BUILD_TIME="${BUILD_TIME}" \
    ;=;

EXPOSE 3000

CMD ["pnpm", "start"]
