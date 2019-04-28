FROM cypress/base:10.15.3@sha256:12495f525d030ffcfa312c3604084f5994d8f6a5e3bac314d3529a70a77c75b9

WORKDIR /e2e

COPY package.json package-lock.json ./
RUN npm install

RUN $(npm bin)/cypress verify

ENV http_proxy=http://localhost:1081

COPY . .
