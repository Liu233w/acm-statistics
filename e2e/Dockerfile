FROM cypress/base:13.6.0@sha256:2c8065350d40f13208899da0383bae2324c232cde30fabe0767a53a75091d38e

WORKDIR /e2e

COPY package.json package-lock.json ./
RUN npm install

RUN $(npm bin)/cypress verify

ENV http_proxy=http://localhost:1081

COPY . .
