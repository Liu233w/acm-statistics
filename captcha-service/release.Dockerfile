ARG NODE_BASE_IMAGE
ARG SERVICE_BASE_IMAGE


FROM ${SERVICE_BASE_IMAGE} AS base
RUN rm -rf node_modules


FROM ${NODE_BASE_IMAGE} AS build

WORKDIR /var/project

COPY package.json pnpm-lock.yaml ./
RUN pnpm install --only=production

COPY --from=base /var/project .

FROM ${NODE_BASE_IMAGE}

WORKDIR /var/project
COPY --from=build /var/project .

ENV NODE_ENV production

EXPOSE 80

CMD ["pnpm", "start"]
