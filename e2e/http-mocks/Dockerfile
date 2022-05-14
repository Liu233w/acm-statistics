ARG NODE_BASE_IMAGE


FROM ${NODE_BASE_IMAGE}

WORKDIR /mocks
COPY package.json package-lock.json ./
RUN pnpm install

ADD . /mocks

CMD ["pnpm", "start"]
