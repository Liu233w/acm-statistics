ARG NODE_BASE_IMAGE

FROM ${NODE_BASE_IMAGE}

RUN apk add --no-cache git
RUN git config --global user.email "test@test.com"
RUN git config --global user.name "Test Name"

RUN pnpm install -g \
  @commitlint/cli \
  @commitlint/config-conventional

WORKDIR /var/project
