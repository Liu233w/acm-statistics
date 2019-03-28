# == consts ==

CommonTagPrefix = acm-statistics
NodeBaseTag = $(CommonTagPrefix)-node-base
CrawlerTag = $(CommonTagPrefix)-crawler
CrawlerBaseTag = $(CommonTagPrefix)-crawler-base
FrontendBaseTag = $(CommonTagPrefix)-frontend-base
FrontendTag = $(CommonTagPrefix)-frontend
CrawlerApiBackendTag = $(CommonTagPrefix)-crawler-api-backend
CrawlerApiBackendBaseTag = $(CommonTagPrefix)-crawler-api-backend-base

CrawlerLibraryPath = /var/project

# == phony
.PHONY: test build run clean test-ci help

# == set variables ==

# enable correct path on msys2 in windows
export MSYS2_ARG_CONV_EXCL = *

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

# check if force cache
ifeq ($(FORCE_DOCKER_CACHE),1)
override build-args := $(build-args) $(addprefix --cache-from ,$(shell docker images -a --filter='dangling=false' --format '{{.Repository}}:{{.Tag}}'))
endif
