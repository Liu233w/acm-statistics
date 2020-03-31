# == consts ==

CommonTagPrefix = acm-statistics
NodeBaseTag = $(CommonTagPrefix)-node-base
CrawlerTag = $(CommonTagPrefix)-crawler
CrawlerBaseTag = $(CommonTagPrefix)-crawler-base
FrontendBaseTag = $(CommonTagPrefix)-frontend-base
FrontendTag = $(CommonTagPrefix)-frontend
CrawlerApiBackendTag = $(CommonTagPrefix)-crawler-api-backend
CrawlerApiBackendBaseTag = $(CommonTagPrefix)-crawler-api-backend-base
E2eMockConfigurerTag = $(CommonTagPrefix)-e2e-mock-configurer
E2eBaseTag = $(CommonTagPrefix)-e2e-base
BackendTag = $(CommonTagPrefix)-backend
BackendBaseTag = $(CommonTagPrefix)-backend-base
CaptchaServiceTag = $(CommonTagPrefix)-captcha-service
CaptchaServiceBaseTag = $(CommonTagPrefix)-captcha-service-base

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

# == detect system and set command ==
ifdef OS # windows
   RM = del /Q
   CP = copy
else
   ifeq ($(shell uname), Linux)
      RM = rm -f
      CP = cp
   endif
endif
