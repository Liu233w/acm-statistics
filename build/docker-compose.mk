## makefile for docker-compose

include share.mk

# === variables ===
# the docker repo name (username) to publish image
RepoName = liu233w/
# use git commit hash as image tag
CommitHash := $(shell git log -1 --pretty=%H)
# the image to publish
Images = FrontendTag CrawlerApiBackendTag BackendTag CaptchaServiceTag OHuntTag

# === inner ===
ImageToTag := $(foreach n,$(Images),$(n)-tag)
ImageToPush := $(foreach n,$(Images),$(n)-push)
# variables for each images
$(ImageToTag): Image = $($(subst -tag,,$@))
$(ImageToPush): Image = $($(subst -push,,$@))
$(ImageToTag) $(ImageToPush): ImageNameWithHash = $(RepoName)$(Image):$(CommitHash)
$(ImageToTag) $(ImageToPush): ImageNameWithLatest = $(RepoName)$(Image):latest

# === targets ===

.PHONY: .build tag push up dev-frontend .build-dev e2e-up

.build:
	$(MAKE) -C ../crawler-api-backend build
	$(MAKE) -C ../frontend build
	$(MAKE) -C ../backend build
	$(MAKE) -C ../captcha-service build
	$(MAKE) -C ../ohunt build

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
	docker-compose up $(compose-args)

# additionally run mock-server when start the server for e2e test
# Remove the old database
e2e-up: .build .env
	$(MAKE) -C ../e2e build-http-mocks
	$(RMR) backend-db || echo remove failed
	$(MKDIR) backend-db
	docker-compose -f docker-compose.yml -f docker-compose.e2e.yml up $(compose-args)

.env:
	$(CP) template.env .env

dev-frontend: .build
	docker-compose -f docker-compose.yml -f docker-compose.dev-frontend.yml up $(compose-args)
