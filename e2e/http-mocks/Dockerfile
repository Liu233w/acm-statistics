ARG NODE_BASE_IMAGE


FROM ${NODE_BASE_IMAGE}

RUN apk add --update --no-cache python3 make gcc g++

WORKDIR /mocks
COPY package.json pnpm-lock.yaml ./
RUN pnpm install

ADD . /mocks

CMD ["pnpm", "start"]
