CommonTagPrefix = acm-statistics
NodeBaseTag = $(CommonTagPrefix)-node-base
CrawlerTag = $(CommonTagPrefix)-crawler
FrontendBaseTag = $(CommonTagPrefix)-frontend-base
FrontendTag = $(CommonTagPrefix)-frontend
CrawlerApiBackendTag = $(CommonTagPrefix)-crawler-api-backend

CrawlerLibraryPath = /var/project

TargetList = crawler frontend crawler-api-backend

# == resolve run-args ==
ifeq ($(filter r no-rm,$(make-args)),)
override run-args := $(run-args) --rm
endif
ifeq ($(filter i no-interactive,$(make-args)),)
override run-args := $(run-args) --interactive
endif
ifeq ($(filter t no-tty,$(make-args)),)
override run-args := $(run-args) --tty
endif

# == common suffix ==

# use command like `make target=crawler test clean` to invoke `make test-crawler clean-crawler`
# support command like `make target="crawler frontend" build`

AllTarget := $(if $(target),$(target),$(TargetList))

.PHONY: test
test: $(addprefix test-,$(AllTarget))
	@echo tested target: $(AllTarget)
.PHONY: build
build: $(addprefix build-,$(AllTarget))
	@echo builded target: $(AllTarget)
.PHONY: run
run: $(addprefix run-,$(AllTarget))
	@echo run target: $(AllTarget)

.PHONY: clean
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
		$(build-args) \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag) \
		--build-arg CRAWLER_LIBRARY_PATH=$(CrawlerLibraryPath)

test-crawler: build-crawler
	docker run --rm -t $(CrawlerTag) npm test

run-crawler: build-crawler
	docker run $(run-args) $(CrawlerTag) $(run-cmd)

clean-crawler:
	docker image rm $(CrawlerTag); true

# == frontend ==

.frontend-base: .node-base build-crawler
	docker build ./frontend -t $(FrontendBaseTag) \
		$(build-args) \
		-f ./frontend/base.Dockerfile \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag) \
		--build-arg CRAWLER_IMAGE=$(CrawlerTag) \
		--build-arg CRAWLER_LIBRARY_PATH=$(CrawlerLibraryPath)

build-frontend: .frontend-base
	docker build ./frontend -t $(FrontendTag) \
		$(build-args) \
		-f ./frontend/build.Dockerfile \
		--build-arg FRONTEND_BASE_IMAGE=$(FrontendBaseTag)

test-frontend: .frontend-base
	docker run --rm -t $(FrontendBaseTag) npm test

update-frontend-snapshot: .frontend-base
	docker run --rm -t \
		-v "$(CURDIR)/frontend/__test__:/var/project/__test__" \
		$(FrontendBaseTag) npm test -- -u

run-frontend: .frontend-base
	docker run $(run-args) $(FrontendBaseTag) $(run-cmd)

clean-frontend:
	docker image rm $(FrontendBaseTag) $(FrontendTag); true

# == crawler api backend ==

build-crawler-api-backend: .node-base build-crawler
	docker build ./crawler-api-backend -t $(CrawlerApiBackendTag) \
		$(build-args) \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag) \
		--build-arg CRAWLER_IMAGE=$(CrawlerTag) \
		--build-arg CRAWLER_LIBRARY_PATH=$(CrawlerLibraryPath)

test-crawler-api-backend: build-crawler-api-backend
	docker run --rm -t $(CrawlerApiBackendTag) npm test

run-crawler-api-backend: build-crawler-api-backend
	docker run $(run-args) $(CrawlerApiBackendTag) $(run-cmd)

clean-crawler-api-backend:
	docker image rm $(CrawlerApiBackendTag); true
