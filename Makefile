NodeBaseTag = acm-statistics-node-base
CrawlerTag = acm-statistics-crawler
FrontendBaseTag = acm-statistics-frontend-base
FrontendTag = acm-statistics-frontend

CodecovEnvCmd = bash <(curl -s https://codecov.io/env)
CodecovEnv = $(shell $(CodecovEnvCmd))
CodecovCmd = bash <(curl -s https://codecov.io/bash)

CrawlerLibraryPath = /var/project

node-base:
ifeq ($(APK_MIRROR),1)
	docker build ./docker -f ./docker/node-base-with-apk-mirror.Dockerfile -t $(NodeBaseTag)
else
	docker build ./docker -f ./docker/node-base.Dockerfile -t $(NodeBaseTag)
endif

build-crawler: node-base
	docker build ./crawler -t $(CrawlerTag) \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag) \
		--build-arg CRAWLER_LIBRARY_PATH=$(CrawlerLibraryPath)
test-crawler: build-crawler
	docker run --rm $(CrawlerTag) npm test
test-crawler-ci: build-crawler
	docker run --rm $(CodecovEnv) $(CrawlerTag) /bin/bash -c "\
		apk add --no-cache bash curl && \
		npm test -- '--testPathPattern=__test__/(?!crawlers\.test).*\.js' && \
		$(CodecovCmd)"
clean-crawler:
	docker image rm $(CrawlerTag); true

frontend-base: node-base build-crawler
	docker build ./frontend -t $(FrontendBaseTag) \
		-f ./frontend/base.Dockerfile \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag) \
		--build-arg CRAWLER_IMAGE=$(CrawlerTag) \
		--build-arg CRAWLER_LIBRARY_PATH=$(CrawlerLibraryPath)
build-frontend: frontend-base
	docker build ./frontend -t $(FrontendTag) \
		-f ./frontend/build.Dockerfile \
		--build-arg FRONTEND_BASE_IMAGE=$(FrontendBaseTag)
test-frontend: frontend-base
	docker run --rm $(FrontendBaseTag) npm test
test-frontend-ci: frontend-base
	docker run --rm $(CodecovEnv) $(FrontendBaseTag) /bin/bash -c "\
		apk add --no-cache bash curl && \
		npm test -- --ci && \
		$(CodecovCmd)"
clean-frontend:
	docker image rm $(FrontendBaseTag) $(FrontendTag); true

clean: clean-crawler clean-frontend
