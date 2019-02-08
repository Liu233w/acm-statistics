ARG NODE_BASE_IMAGE

FROM ${NODE_BASE_IMAGE}

RUN apk add --no-cache \
  git \
  curl \
  bash \
  ;

WORKDIR /var/project

CMD ["bash"]