## makefile for commitlint

include share.mk

# === consts ===
CommitlintImageTag = $(CommonTagPrefix)-commitlint-image
# 用来执行 commitlint 的地址，可以被覆盖
CommitlintValidPath = $(CURDIR)/..

# === targets ===
.PHONY: test-commit commitlint-travis

.node-base:
	$(MAKE) -f node-base.mk build

build: .node-base
	docker build . \
		-t $(CommitlintImageTag) \
		-f commitlint.Dockerfile \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag)

clean:
	docker image rm $(CommitlintImageTag); true

test-commit: build
	docker run --rm \
		-v "$(CommitlintValidPath):/var/project" \
		$(CommitlintImageTag) \
		commitlint --from master --color

# travis only, cannot run on windows
commitlint-travis: SHELL:=/bin/bash
commitlint-travis: build
	docker run --rm \
		-v "$(CommitlintValidPath):/var/project" \
		--env CI=true \
		--env-file <(env | grep TRAVIS) \
		$(CommitlintImageTag) \
		commitlint-travis --color