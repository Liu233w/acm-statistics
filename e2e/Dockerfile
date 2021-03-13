FROM cypress/base:13.8.0@sha256:a56fd75c11b60a09cea2e785fc8feca4d23e4652c3b53213f3c9f4ad25ec063b

WORKDIR /e2e

COPY package.json package-lock.json ./
RUN npm install

RUN $(npm bin)/cypress verify

ENV http_proxy=http://localhost:1081

COPY . .
