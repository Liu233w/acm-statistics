FROM cypress/base:12.12.0@sha256:db835ab4fd8623a25929edbdafdf1fe6e1b04fdf1298a1d2551fbba32e2d56d8

WORKDIR /e2e

COPY package.json package-lock.json ./
RUN npm install

RUN $(npm bin)/cypress verify

ENV http_proxy=http://localhost:1081

COPY . .
