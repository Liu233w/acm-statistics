ARG NODE_BASE_IMAGE

FROM ${NODE_BASE_IMAGE}

# 预先定义工作路径，便于同其他项目解耦。
# 必须在 FROM 下面定义，否则这里 ARG 会变成空字符串
ARG CRAWLER_LIBRARY_PATH
WORKDIR ${CRAWLER_LIBRARY_PATH}

COPY package.json package-lock.json ./
RUN npm install

COPY . .
