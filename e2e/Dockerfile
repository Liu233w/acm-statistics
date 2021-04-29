FROM cypress/base:14.16.0@sha256:5a248444b630e727508570ae5e68d64c6e1f3c28087fbb2063726e790ca59dfa

WORKDIR /e2e

COPY package.json package-lock.json ./
RUN npm install

RUN $(npm bin)/cypress verify

ENV http_proxy=http://localhost:1081

COPY . .
