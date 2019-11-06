FROM cypress/base:12.13.0@sha256:261915a13354ae73ea765c3d4a0cfa82b675e2206d8c55fe0acadfe782ace1be

WORKDIR /e2e

COPY package.json package-lock.json ./
RUN npm install

RUN $(npm bin)/cypress verify

ENV http_proxy=http://localhost:1081

COPY . .
