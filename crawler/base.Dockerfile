ARG NODE_BASE_IMAGE


FROM ${NODE_BASE_IMAGE}

WORKDIR /var/project

COPY package.json pnpm-lock.yaml ./
RUN pnpm install

COPY . .
