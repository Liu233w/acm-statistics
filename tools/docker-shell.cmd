docker run --rm -it --workdir /var/src/project/ --volume "%~dp0..:/var/src/project" -p 3000:3000 --env HOST=0.0.0.0 node:10-alpine /usr/bin/env sh
