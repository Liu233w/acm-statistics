FROM cypress/base:18.14.1

WORKDIR /e2e

RUN npm install --global pnpm@7

COPY package.json pnpm-lock.yaml ./
RUN pnpm install

RUN $(pnpm bin)/cypress verify

ENV http_proxy=http://localhost:1081

COPY . .
