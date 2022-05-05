FROM cypress/base:latest

WORKDIR /e2e

COPY package.json package-lock.json ./
RUN npm install

RUN $(npm bin)/cypress verify

ENV http_proxy=http://localhost:1081

COPY . .
