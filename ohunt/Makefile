## makefile for ohunt

include ../build/share.mk

help:
	@echo run \"make help\" in root directory to get help

.base:
	docker build . \
		-f dev.Dockerfile \
		-t $(OHuntBaseTag) \
		$(build-args)

build:
	docker build . \
		-f OHunt.Web/Dockerfile \
		-t $(OHuntTag) \
		$(build-args)

test: .base
	docker run --rm -t $(OHuntBaseTag) dotnet test

run: .base
	docker run $(run-args) $(OHuntBaseTag) $(run-cmd)

clean:
	docker image rm $(OHuntTag) $(OHuntBaseTag); true

test-ci: .base
	docker run --rm \
		-v "$(CURDIR)/OHunt.Tests/TestResults:/src/OHunt.Tests/TestResults" \
		$(OHuntBaseTag) \
		dotnet test --collect:"XPlat Code Coverage" --filter "Category!=WithNetwork"

dev-db: # start a new database to develop the project
	docker run -it --rm -e MYSQL_DATABASE=ohunt -e MYSQL_ROOT_PASSWORD=123qwe -p 3306:3306 mysql:8.0
