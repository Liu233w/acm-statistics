FROM cypress/base:14.7.0@sha256:d824405037ac825b1601c672da9775dbcb877c0716907880c4904b2e7c938c8b

WORKDIR /e2e

COPY package.json package-lock.json ./
RUN npm install

RUN $(npm bin)/cypress verify

ENV http_proxy=http://localhost:1081

COPY . .
