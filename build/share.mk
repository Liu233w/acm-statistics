# == consts ==

CommonTagPrefix = acm-statistics
NodeBaseTag = $(CommonTagPrefix)-node-base
CrawlerTag = $(CommonTagPrefix)-crawler
FrontendBaseTag = $(CommonTagPrefix)-frontend-base
FrontendTag = $(CommonTagPrefix)-frontend
CrawlerApiBackendTag = $(CommonTagPrefix)-crawler-api-backend

CrawlerLibraryPath = /var/project

# == phony
.PHONY: test build run clean test-ci

# == set variables ==

# enable correct path on msys2 in windows
export MSYS2_ARG_CONV_EXCL = *

# 当前 makefile 所在的目录的绝对地址
# 由于延迟求值，因此是在引用这个变量的位置进行获取
MakefilePathDirectory = $(dir $(realpath $(firstword $(MAKEFILE_LIST))))

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

