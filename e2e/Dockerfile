FROM cypress/base:10

WORKDIR /e2e

COPY package.json package-lock.json ./
RUN npm install

RUN $(npm bin)/cypress verify

ENV http_proxy=http://host.docker.internal:1081

COPY . .
