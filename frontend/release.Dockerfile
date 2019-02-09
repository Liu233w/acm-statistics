ARG FRONTEND_BASE_IMAGE
ARG NODE_BASE_IMAGE


FROM ${FRONTEND_BASE_IMAGE} AS build

RUN npm run build
# 排除 devDependencies
RUN rm -rf node_modules && npm install --only=production
# frontend 在运行期间不需要 crawler 模块，这里就不复制了


FROM ${NODE_BASE_IMAGE}

WORKDIR /var/project

COPY --from=build /var/project .

ENV HOST 0.0.0.0 \
    NODE_ENV production \
    PORT 3000 \
    ;

EXPOSE 3000

CMD ["npm", "start"]
