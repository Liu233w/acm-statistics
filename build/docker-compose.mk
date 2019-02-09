## makefile for docker-compose

include share.mk

# === variables ===
# 发布镜像时使用的前缀（用于表示用户名）
RepoName = liu233w/
# 使用 git commit hash 来表示镜像的tag
CommitHash := $(shell git log -1 --pretty=%H)
# 要发布的镜像变量名
Images = FrontendTag CrawlerApiBackendTag

# === inner ===
# 内部使用的变量
ImageToTag := $(foreach n,$(Images),$(n)-tag)
ImageToPush := $(foreach n,$(Images),$(n)-push)
# 单独的 target 的内部变量
$(ImageToTag): Image = $($(subst -tag,,$@))
$(ImageToPush): Image = $($(subst -push,,$@))
$(ImageToTag) $(ImageToPush): ImageNameWithHash = $(RepoName)$(Image):$(CommitHash)
$(ImageToTag) $(ImageToPush): ImageNameWithLatest = $(RepoName)$(Image):latest

# === targets ===

.PHONY: .build tag push up dev .build-dev

.build:
	$(MAKE) -C ../crawler-api-backend build
	$(MAKE) -C ../frontend build

# 为了开发而构建的目标
.build-dev:
	$(MAKE) -C ../crawler-api-backend build
	# 前端直接映射本地的目录，不需要构建

tag: .build $(ImageToTag)

push: tag $(ImageToPush)

$(ImageToTag):
	@echo try to tag $(ImageNameWithHash) and $(ImageNameWithLatest)
	docker tag $(Image) $(ImageNameWithHash)
	docker tag $(Image) $(ImageNameWithLatest)

$(ImageToPush):
	@echo try to push $(ImageNameWithHash) and $(ImageNameWithLatest)
	docker push $(ImageNameWithHash)
	docker push $(ImageNameWithLatest)

up: .build .env
	docker-compose up

.env:
	cp template.env .env

dev: .build-dev
	docker-compose -f docker-compose.yml -f docker-compose.dev.yml up