## makefile for captcha service

include ../build/share.mk

help:
	@echo run \"make help\" in root directory to get help

.node-base:
	cd ../build && $(MAKE) -f node-base.mk build

.base: .node-base
	docker build . \
		-f base.Dockerfile \
		-t $(CaptchaServiceBaseTag) \
		$(build-args) \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag)

build: .base
	docker build . \
		-f release.Dockerfile \
		-t $(CaptchaServiceTag) \
		$(build-args) \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag) \
		--build-arg SERVICE_BASE_IMAGE=$(CaptchaServiceBaseTag)

test: .base
	docker run --rm -t $(CaptchaServiceBaseTag) pnpm test

run: .base
	docker run $(run-args) $(CaptchaServiceBaseTag) $(run-cmd)

clean:
	docker image rm $(CaptchaServiceBaseTag) $(CaptchaServiceTag); true

test-ci: .base
	docker run --rm \
		-v "$(CURDIR)/coverage:/var/project/coverage" \
		$(CaptchaServiceBaseTag) \
		pnpm test -- --colors
