## makefile for crawler api backend

include ../build/share.mk

help:
	@echo run \"make help\" in root directory to get help

.crawler:
	cd ../crawler && $(MAKE) build
.node-base:
	cd ../build && $(MAKE) -f node-base.mk build

build: .node-base .crawler
	docker build . \
		-t $(CrawlerApiBackendTag) \
		$(build-args) \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag) \
		--build-arg CRAWLER_IMAGE=$(CrawlerTag) \
		--build-arg CRAWLER_LIBRARY_PATH=$(CrawlerLibraryPath)

test: build
	docker run --rm -t $(CrawlerApiBackendTag) npm test

run: build
	docker run $(run-args) $(CrawlerApiBackendTag) $(run-cmd)

clean:
	docker image rm $(CrawlerApiBackendTag); true

test-ci: build
	docker run --rm \
		-v "$(CURDIR)/coverage:/var/project/coverage" \
		$(CrawlerApiBackendTag) \
		npm test -- --colors