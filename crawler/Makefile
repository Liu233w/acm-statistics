## makefile for crawler

include ../build/share.mk

help:
	@echo run \"make help\" in root directory to get help

.node-base:
	cd ../build && $(MAKE) -f node-base.mk build

.base: .node-base
	docker build . \
		-f base.Dockerfile \
		-t $(CrawlerBaseTag) \
		$(build-args) \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag)

build: .base
	docker build . \
		-f release.Dockerfile \
		-t $(CrawlerTag) \
		$(build-args) \
		--build-arg CRAWLER_BASE_IMAGE=$(CrawlerBaseTag) \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag)

test: .base
	docker run --rm -t $(CrawlerBaseTag) pnpm test

run: .base
	docker run $(run-args) $(CrawlerBaseTag) $(run-cmd)

clean:
	docker image rm $(CrawlerTag) $(CrawlerBaseTag); true

test-ci: .base
	docker run --rm \
		-v "$(CURDIR)/coverage:/var/project/coverage" \
		$(CrawlerBaseTag) \
		pnpm test -- --colors "--testPathPattern=__test__/(?!crawlers\\.test).*\\.js"
