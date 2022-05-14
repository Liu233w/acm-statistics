ARG NODE_BASE_IMAGE


FROM ${NODE_BASE_IMAGE}

WORKDIR /var/project

COPY package.json package-lock.json ./
RUN pnpm install

COPY . .
