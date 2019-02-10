ARG FRONTEND_BASE_IMAGE
ARG NODE_BASE_IMAGE
ARG CRAWLER_IMAGE


FROM ${CRAWLER_IMAGE} AS crawler


FROM ${FRONTEND_BASE_IMAGE} AS base
RUN npm run build
RUN rm -rf node_modules


FROM ${NODE_BASE_IMAGE} AS build
ARG CRAWLER_LIBRARY_PATH

WORKDIR /var/project

COPY package.json package-lock.json ./
RUN npm install --only=production

COPY --from=crawler ${CRAWLER_LIBRARY_PATH} ./node_modules/crawler

COPY --from=base /var/project .


FROM ${NODE_BASE_IMAGE}

WORKDIR /var/project

COPY --from=build /var/project .

ENV HOST 0.0.0.0 \
    NODE_ENV production \
    PORT 3000 \
    ;

EXPOSE 3000

CMD ["npm", "start"]
