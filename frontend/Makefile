## makefile for frontend

include ../build/share.mk

# === targets ===

help:
	@echo run \"make help\" in root directory to get help

.crawler:
	cd ../crawler && $(MAKE) build
.node-base:
	cd ../build && $(MAKE) -f node-base.mk build

.base: .node-base .crawler
	docker build . \
		-t $(FrontendBaseTag) \
		$(build-args) \
		-f base.Dockerfile \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag) \
		--build-arg CRAWLER_IMAGE=$(CrawlerTag)

build: .base
	docker build . \
		-f release.Dockerfile \
		-t $(FrontendTag) \
		$(build-args) \
		--build-arg VERSION_NUM=$(VERSION_NUM) \
		--build-arg BUILD_TIME=$(BUILD_TIME) \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag) \
		--build-arg FRONTEND_BASE_IMAGE=$(FrontendBaseTag) \
		--build-arg CRAWLER_IMAGE=$(CrawlerTag)

test: .base
	docker run --rm -t $(FrontendBaseTag) pnpm test

# Update snapshot
update-snapshot: .base
	docker run --rm -t \
		-v "$(CURDIR)/__test__:/var/project/__test__" \
		$(FrontendBaseTag) pnpm test -- -u

# if BUILD_FRONTEND is specified, it will compile the frontend before `run` command
ifdef BUILD_FRONTEND
run: build
	docker run $(run-args) $(FrontendTag) $(run-cmd)
else
run: .base
	docker run $(run-args) $(FrontendBaseTag) $(run-cmd)
endif

clean:
	docker image rm $(FrontendBaseTag) $(FrontendTag); true

test-ci: .base
	docker run --rm \
		-v "$(CURDIR)/coverage:/var/project/coverage" \
		$(FrontendBaseTag) \
		pnpm test -- --colors --ci

# Use docker to develop. It runs `pnpm run dev`
dev:
	cd ../build && $(MAKE) -f docker-compose.mk dev-frontend
