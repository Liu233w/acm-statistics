FROM cypress/base:12.1.0@sha256:d90d281785d457e0329fb44f06652fce15898ee858fb6c732d353b4169a11301

WORKDIR /e2e

COPY package.json package-lock.json ./
RUN npm install

RUN $(npm bin)/cypress verify

ENV http_proxy=http://localhost:1081

COPY . .
