## makefile for frontend

include ../build/share.mk

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
		--build-arg CRAWLER_IMAGE=$(CrawlerTag) \
		--build-arg CRAWLER_LIBRARY_PATH=$(CrawlerLibraryPath)

build: .base
	docker build . \
		-t $(FrontendTag) \
		$(build-args) \
		-f build.Dockerfile \
		--build-arg FRONTEND_BASE_IMAGE=$(FrontendBaseTag)

test: .base
	docker run --rm -t $(FrontendBaseTag) npm test

# 更新快照
update-snapshot: .base
	docker run --rm -t \
		-v "$(CURDIR)/__test__:/var/project/__test__" \
		$(FrontendBaseTag) npm test -- -u

# 指定 BUILD_FRONTEND 之后将会在 run 之前先编译好前端
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
		npm test -- --colors --ci

# 使用 docker 来进行开发，将运行 npm run dev
dev:
	cd ../build && $(MAKE) -f docker-compose.mk dev-frontend