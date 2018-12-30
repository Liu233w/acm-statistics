FROM node:10-alpine

WORKDIR /var/project

ADD crawler crawler
RUN cd crawler && npm i

ADD frontend/package.json frontend/package-lock.json frontend/
RUN cd frontend && npm i && npm run postinstall

ADD frontend frontend
RUN cd frontend && npm run build

ENV HOST 0.0.0.0

CMD ["/usr/bin/env", "sh"]
