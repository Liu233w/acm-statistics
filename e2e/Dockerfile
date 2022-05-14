FROM cypress/base:latest

WORKDIR /e2e

COPY package.json pnpm-lock.yaml ./
RUN pnpm install

RUN $(pnpm bin)/cypress verify

ENV http_proxy=http://localhost:1081

COPY . .
