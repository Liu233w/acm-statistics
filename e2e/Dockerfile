FROM cypress/base:11.13.0@sha256:68e93c8bd4003fb99541c4ee23c27771f3c07a81e7bd795715bb253eb7e279b4

WORKDIR /e2e

COPY package.json package-lock.json ./
RUN npm install

RUN $(npm bin)/cypress verify

ENV http_proxy=http://localhost:1081

COPY . .
