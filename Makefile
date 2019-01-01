CommonTagPrefix = acm-statistics
NodeBaseTag = $(CommonTagPrefix)-node-base
CrawlerTag = $(CommonTagPrefix)-crawler
FrontendBaseTag = $(CommonTagPrefix)-frontend-base
FrontendTag = $(CommonTagPrefix)-frontend
CrawlerApiBackendTag = $(CommonTagPrefix)-crawler-api-backend

CrawlerLibraryPath = /var/project

TargetList = crawler frontend crawler-api-backend

# pass arbitrary argument to make, from https://stackoverflow.com/a/14061796
# usage: make -- run-crawler npm run lint
# pass '--' after make allow you to use '--option' like 'make -- run-fontend npm run test -- --ci'
CmdList := run $(addprefix run-,$(TargetList))
RunCmd := $(findstring $(firstword $(MAKECMDGOALS)),$(CmdList))
RunArgs =
ifneq ($(RunCmd),)
  # use the rest as arguments for "run"
  RunArgs := $(wordlist 2,$(words $(MAKECMDGOALS)),$(MAKECMDGOALS))
  # ...and turn them into do-nothing targets
  $(eval $(RunArgs):;@:)
endif

# == common suffix ==

# use command like `make target=crawler test clean` to invoke `make test-crawler clean-crawler`
# support command like `make target="crawler frontend" build`

AllTarget := $(if $(target),$(target),$(TargetList))

test: $(addprefix test-,$(AllTarget))
	@echo tested target: $(AllTarget)
build: $(addprefix build-,$(AllTarget))
	@echo builded target: $(AllTarget)
run: $(addprefix run-,$(AllTarget))
	@echo run target: $(AllTarget)

ifeq ($(target),)
clean: .clean-node-base $(addprefix clean-,$(TargetList))
	@echo cleaned all target
else
clean: $(addprefix clean-,$(target))
	@echo cleaned $(target)
endif

# == base image ==

.node-base:
ifeq ($(APK_MIRROR),1)
	docker build ./docker -f ./docker/node-base-with-apk-mirror.Dockerfile -t $(NodeBaseTag)
else
	docker build ./docker -f ./docker/node-base.Dockerfile -t $(NodeBaseTag)
endif

.clean-node-base:
	docker image rm $(NodeBaseTag); true

# == crawler ==

build-crawler: .node-base
	docker build ./crawler -t $(CrawlerTag) \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag) \
		--build-arg CRAWLER_LIBRARY_PATH=$(CrawlerLibraryPath)

test-crawler: build-crawler
	docker run --rm $(CrawlerTag) npm test

run-crawler: build-crawler
	docker run -it --rm $(CrawlerTag) $(RunArgs)

clean-crawler:
	docker image rm $(CrawlerTag); true

# == frontend ==

.frontend-base: .node-base build-crawler
	docker build ./frontend -t $(FrontendBaseTag) \
		-f ./frontend/base.Dockerfile \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag) \
		--build-arg CRAWLER_IMAGE=$(CrawlerTag) \
		--build-arg CRAWLER_LIBRARY_PATH=$(CrawlerLibraryPath)

build-frontend: .frontend-base
	docker build ./frontend -t $(FrontendTag) \
		-f ./frontend/build.Dockerfile \
		--build-arg FRONTEND_BASE_IMAGE=$(FrontendBaseTag)

test-frontend: .frontend-base
	docker run --rm $(FrontendBaseTag) npm test

run-frontend: .frontend-base
	docker run -it --rm $(FrontendBaseTag) $(RunArgs)

clean-frontend:
	docker image rm $(FrontendBaseTag) $(FrontendTag); true

# == crawler api backend ==

build-crawler-api-backend: .node-base build-crawler
	docker build ./crawler-api-backend -t $(CrawlerApiBackendTag) \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag) \
		--build-arg CRAWLER_IMAGE=$(CrawlerTag) \
		--build-arg CRAWLER_LIBRARY_PATH=$(CrawlerLibraryPath)

test-crawler-api-backend: build-crawler-api-backend
	docker run --rm $(CrawlerApiBackendTag) npm test

run-crawler-api-backend: build-crawler-api-backend
	docker run -it --rm $(CrawlerApiBackendTag) $(RunArgs)

clean-crawler-api-backend:
	docker image rm $(CrawlerApiBackendTag); true
