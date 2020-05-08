FROM cypress/base:14.0.0@sha256:a45c93bf7437334abdb3fa71b34780fad877a012a7e8e1b4f1e961d8152670a4

WORKDIR /e2e

COPY package.json package-lock.json ./
RUN npm install

RUN $(npm bin)/cypress verify

ENV http_proxy=http://localhost:1081

COPY . .
