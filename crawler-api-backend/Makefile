## makefile for crawler api backend

include ../build/share.mk

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
