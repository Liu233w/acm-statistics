ARG NODE_BASE_IMAGE


FROM ${NODE_BASE_IMAGE}

WORKDIR /mocks
COPY package.json package-lock.json ./
RUN npm install

ADD . /mocks

CMD ["npm", "start"]
