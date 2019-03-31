ARG NODE_BASE_IMAGE


FROM ${NODE_BASE_IMAGE}

WORKDIR /mocks
RUN npm install -g mockserver-client nodemon

ADD . /mocks
RUN npm link mockserver-client

CMD nodemon --legacy-watch index.js