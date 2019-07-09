FROM cypress/base:12.6.0@sha256:a51df2ff30a15ecac61f5b19d2afcd52f3286a157647bcbfad80a57461ebe61f

WORKDIR /e2e

COPY package.json package-lock.json ./
RUN npm install

RUN $(npm bin)/cypress verify

ENV http_proxy=http://localhost:1081

COPY . .
