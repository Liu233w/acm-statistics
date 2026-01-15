FROM node:22.22.0-alpine

# dependency to run codecov on CI
ARG CODECOV=false
RUN if [ "$CODECOV" != "false" ]; then \
    apk add --no-cache \
    bash \
    curl \
    ; fi

RUN npm install --global pnpm@9
