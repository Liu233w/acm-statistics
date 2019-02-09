ARG FRONTEND_BASE_IMAGE
ARG NODE_BASE_IMAGE
ARG CRAWLER_IMAGE


FROM ${CRAWLER_IMAGE} AS crawler


FROM ${FRONTEND_BASE_IMAGE} AS build
ARG CRAWLER_LIBRARY_PATH

RUN npm run build
# 排除 devDependencies
RUN rm -rf node_modules && npm install --only=production
COPY --from=crawler ${CRAWLER_LIBRARY_PATH} ./node_modules/crawler


FROM ${NODE_BASE_IMAGE}

WORKDIR /var/project

COPY --from=build /var/project .

ENV HOST 0.0.0.0 \
    NODE_ENV production \
    PORT 3000 \
    ;

EXPOSE 3000

CMD ["npm", "start"]
