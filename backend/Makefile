## makefile for crawler api backend

include ../build/share.mk

help:
	@echo run \"make help\" in root directory to get help

.base:
	docker build . \
		-f dev.Dockerfile \
		-t $(BackendBaseTag) \
		$(build-args)

build:
	docker build . \
		-f src/AcmStatisticsBackend.Web.Host/Dockerfile \
		-t $(BackendTag) \
		$(build-args)

test: .base
	docker run --rm -t $(BackendBaseTag) dotnet test

run: .base
	docker run $(run-args) $(BackendBaseTag) $(run-cmd)

clean:
	docker image rm $(BackendTag) $(BackendBaseTag); true

test-ci: .base
	docker run --rm \
		-v "$(CURDIR)/test/AcmStatisticsBackend.Tests/TestResults:/src/test/AcmStatisticsBackend.Tests/TestResults" \
		$(BackendBaseTag) \
		dotnet test --collect:"XPlat Code Coverage"
