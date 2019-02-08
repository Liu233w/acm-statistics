## 用于开发用的 shell 的 makefile

include share.mk

# === consts ===
ShellTag = $(CommonTagPrefix)-shell
# 对应的本机上的地址，可以被覆盖
ProjectPath = $(CURDIR)/..

# === targets ===

.PHONY: .node-base build shell

shell: build
	docker run --rm -it -v "$(ProjectPath):/var/project" $(ShellTag)

build: .node-base
	docker build . \
		-t $(ShellTag) \
		-f shell.Dockerfile \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag)

.node-base:
	$(MAKE) -f node-base.mk build