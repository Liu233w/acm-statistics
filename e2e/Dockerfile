FROM cypress/base:latest@sha256:81b8d9a8fc65c021e27852ca34196c745e7c0d24e56c63b211546179d3f7189a

WORKDIR /e2e

COPY package.json package-lock.json ./
RUN npm install

RUN $(npm bin)/cypress verify

ENV http_proxy=http://localhost:1081

COPY . .
