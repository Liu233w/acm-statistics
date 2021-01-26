FROM cypress/base:14.15.4@sha256:dad8727e107f61f48a3c6ef22cb651069cfd29fd377addcfbd7986c0a2659903

WORKDIR /e2e

COPY package.json package-lock.json ./
RUN npm install

RUN $(npm bin)/cypress verify

ENV http_proxy=http://localhost:1081

COPY . .
