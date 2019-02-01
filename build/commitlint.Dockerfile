ARG NODE_BASE_IMAGE

FROM ${NODE_BASE_IMAGE}

RUN apk add --no-cache git

RUN npm install -g \
  @commitlint/cli \
  @commitlint/config-conventional \
  @commitlint/travis-cli

WORKDIR /var/project