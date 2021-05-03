## root makefile

include ./build/share.mk

.PHONY: default
default: .short-help ;

# == common suffix ==

# use command like `make target=crawler test clean` to invoke `make -C crawler test clean`
# support command like `make target="crawler frontend" build`

TargetList = crawler frontend crawler-api-backend backend captcha-service e2e ohunt
AllTarget := $(if $(target),$(target),$(TargetList))

test:
	@echo testing target: $(AllTarget)
	for dir in $(AllTarget); do \
  	$(MAKE) -C $$dir test; \
  done

build:
	@echo building target: $(AllTarget)
	for dir in $(AllTarget); do \
  	$(MAKE) -C $$dir build; \
  done

run:
	@echo running target: $(AllTarget)
	for dir in $(AllTarget); do \
  	$(MAKE) -C $$dir run; \
  done

clean:
	# remove all stopped containers
	docker rm $(shell docker ps -a -q)
	for dir in $(AllTarget); do \
  	$(MAKE) -C $$dir clean; \
  done
ifeq ($(target),)
	cd ./build && $(MAKE) -f node-base.mk clean
	cd ./build && $(MAKE) -f commitlint.mk clean
	cd ./build && $(MAKE) -f shell.mk clean
	@echo cleaned all target
	@echo running docker system prune
	docker system prune -f
else
	@echo cleaned $(target)
endif

test-ci:
	@echo testing ci on target: $(AllTarget)
	for dir in $(AllTarget); do \
  	$(MAKE) -C $$dir test-ci; \
  done

# === commitlint ===

.PHONY: test-commit

test-commit:
	cd ./build && $(MAKE) -f commitlint.mk test-commit

# === publish image ===
.PHONY: tag-and-push

tag-and-push:
	cd ./build && $(MAKE) -f docker-compose.mk push

# === run all ===
.PHONY: up

up:
	cd ./build && $(MAKE) -f docker-compose.mk up

# === util command ==

.PHONY: show-image-size shell

# 输出项目中 latest 标签标记的镜像的大小
show-image-size:
	docker images --format "table {{.Repository}}:{{.Tag}}\t{{.Size}}" --filter=reference='acm-statistics*:latest'

shell:
	cd build && $(MAKE) -f shell.mk shell

# === help ===

.PHONY: help

define HELP_MESSAGE

Makefile of acm-statistics

Available goals:
test build run clean test-ci test-commit commitlint-travis tag-and-push up view-image-size shell help

You can use `target` variable to set target module when using test, build, run, clean, 
and test-ci commands. E.g. `make test target="frontend crawler"` means running test only
in frontend and crawler module. If target is not specified, run the command on all modules.

Besides, the dependency is automatically resolved by makefile. So you do not need to run
build before test.

Documents of available goals:

test
Running all tests

build
build the project or certain modules.
If argument `build-args` is specified, it is attached to all `docker build` commands

run
Run shell command in modules.
E.g. `make run run-cmd="npm run lint"` runs `npm run lint` in all modules.
Available parameters:

  run-cmd: The command to be run.

  run-args: The extra arguments sent to docker. E.g. run following commands in fontend directory to send argument -v '...' to docker: 
    >> make run run-cmd="npm test -- --update-snapshot" run-args="-v './__test__:/var/project/__test__'"

	make-args: run-args will automatically send following switches to docker: --rm, --interactive, --tty, which can be turned off by following switches: `r`/`no-rm`, `i`/`no-interactive`, `t`/`no-tty`. 
    E.g. the following command turns `run-args` into `--tty` (`--rm` and `--interactive` are disabled):
		>> make run make-args="r i"
		Noticed that commands should be separated even they are single letters.

clean
Clean images that are built

test-ci
Run tests in CI environment. It behaves differently than normal tests. E.g. Specifying `--ci` to jest and disabling tests that require network.

test-commit
Lint commits from master branch to HEAD by commitlint.

tag-and-push
Tag the built images and publish. By default, it uses `liu233w` as namespace.
You may refer to `./build/docker-compose.mk` to change this behaviour.

up
Run the project using docker-compose. It automatically creates config file `./build/.env`.
It is recommended to modify the file based on the comments inside.
If you run it on windows, it is recommended to use msys2 shell after configure it to accept the path of windows

show-image-size
Show the size of all images built by the project. It does not create new images.

shell
Spawn a shell inside docker container and mount the whole project into it. So you can run commands and modify the project in Linux environment.

help
Show this doc.

Most of the sub-directory supports make commands like the root directory. View the `Makefile` for more information.

endef

.short-help:
	@echo run \"make help\" to get help

export HELP_MESSAGE
help:
	@echo "$$HELP_MESSAGE" | more
