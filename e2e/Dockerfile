FROM cypress/base:13.3.0@sha256:06af44874ee8637ac751f2d80ebf118e5ab4870d06f1f01a444b0634e0e68ad9

WORKDIR /e2e

COPY package.json package-lock.json ./
RUN npm install

RUN $(npm bin)/cypress verify

ENV http_proxy=http://localhost:1081

COPY . .
