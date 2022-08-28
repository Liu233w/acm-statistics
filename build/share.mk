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
OHuntTag = $(CommonTagPrefix)-ohunt
OHuntBaseTag = $(CommonTagPrefix)-ohunt-base

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

RM = docker run --rm -v $(shell pwd):/app -w /app alpine rm -f
RMR = docker run --rm -v $(shell pwd):/app -w /app alpine rm -rf

# == detect system and set command ==
ifeq ($(findstring cmd.exe,$(SHELL)),cmd.exe)
    # cmd shell
    CP = copy
    MKDIR = md
else # Linux or cygwin/msys
    CP = cp
    MKDIR = mkdir
endif

# === consts ==
# the version number is shown in about page
ifndef VERSION_NUM
VERSION_NUM := $(shell git log -1 --pretty=%H)
endif
# a timestamp shown in about page and page footer
ifndef BUILD_TIME
    ifdef OS # windows
        BUILD_TIME := $(shell powershell -command "[int32](New-TimeSpan -Start (Get-Date "01/01/1970") -End ((Get-Date).ToUniversalTime())).TotalSeconds")
    else
        BUILD_TIME := $(shell date +%s)
    endif
endif
