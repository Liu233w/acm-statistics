# makefile for e2e

include ../build/share.mk

# === targets ===

.PHONY: server .node-base build-http-mocks

.node-base:
	cd ../build && $(MAKE) -f node-base.mk build

server:
	cd ../build && $(MAKE) -f docker-compose.mk e2e-up

build-http-mocks: .node-base
	docker build ./http-mocks \
		-t $(E2eMockConfigurerTag) \
		$(build-args) \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag)
