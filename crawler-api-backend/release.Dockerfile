ARG CRAWLER_IMAGE
ARG BACKEND_BASE_IMAGE
ARG NODE_BASE_IMAGE


FROM ${CRAWLER_IMAGE} AS crawler


FROM ${BACKEND_BASE_IMAGE} AS build
ARG CRAWLER_LIBRARY_PATH

RUN rm -rf node_modules && npm install --only=production
COPY --from=crawler ${CRAWLER_LIBRARY_PATH} ./node_modules/crawler


FROM ${NODE_BASE_IMAGE}

WORKDIR /var/project
COPY --from=build /var/project .

ENV NODE_ENV production

EXPOSE 12001

CMD ["npm", "start"]