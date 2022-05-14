## makefile for crawler api backend

include ../build/share.mk

help:
	@echo run \"make help\" in root directory to get help

.crawler:
	cd ../crawler && $(MAKE) build
.node-base:
	cd ../build && $(MAKE) -f node-base.mk build

.base: .node-base .crawler
	docker build . \
		-f base.Dockerfile \
		-t $(CrawlerApiBackendBaseTag) \
		$(build-args) \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag) \
		--build-arg CRAWLER_IMAGE=$(CrawlerTag) \
		--build-arg CRAWLER_LIBRARY_PATH=$(CrawlerLibraryPath)

build: .base .crawler
	docker build . \
		-f release.Dockerfile \
		-t $(CrawlerApiBackendTag) \
		$(build-args) \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag) \
		--build-arg BACKEND_BASE_IMAGE=$(CrawlerApiBackendBaseTag) \
		--build-arg CRAWLER_IMAGE=$(CrawlerTag) \
		--build-arg CRAWLER_LIBRARY_PATH=$(CrawlerLibraryPath)

test: .base
	docker run --rm -t $(CrawlerApiBackendBaseTag) pnpm test

run: .base
	docker run $(run-args) $(CrawlerApiBackendBaseTag) $(run-cmd)

clean:
	docker image rm $(CrawlerApiBackendTag) $(CrawlerApiBackendBaseTag); true

test-ci: .base
	docker run --rm \
		-v "$(CURDIR)/coverage:/var/project/coverage" \
		$(CrawlerApiBackendBaseTag) \
		pnpm test -- --colors