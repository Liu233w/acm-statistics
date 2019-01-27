## makefile for crawler

include ../build/share.mk

.node-base:
	cd ../build && $(MAKE) -f node-base.mk build

build: .node-base
	docker build . \
		-t $(CrawlerTag) \
		$(build-args) \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag) \
		--build-arg CRAWLER_LIBRARY_PATH=$(CrawlerLibraryPath)

test: build
	docker run --rm -t $(CrawlerTag) npm test

run: build
	docker run $(run-args) $(CrawlerTag) $(run-cmd)

clean:
	docker image rm $(CrawlerTag); true