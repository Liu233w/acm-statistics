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
  run-args: 给 docker 传递的额外指令。比如在 fontend 目录运行 make run run-cmd="npm test -- --update-snapshot" run-args="-v './__test__:/var/project/__test__'" 可以将开关 -v '...' 传递给 docker
  make-args: run-args 会自动加入 --rm, --interactive, --tty 这三个开关，可以通过分别传入 r/no-rm, i/no-interactive, t/no-tty 来禁用。比如 make run make-args="r i" 会把 run-args 变成 "--tty" （禁用了 --rm 和 --interactive）。注意即使是单个字符，也必须用空格隔开。

clean
清除构建好的 image

test-ci
运行 ci 中的测试。行为稍有不同，比如给jest指定了 --ci 开关，禁用了部分测试等等

test-commit
使用 commitlint 检查当前分支到 master 分支的 HEAD 为止的所有提交

tag-and-push
给构建出的镜像打上标签并发布，默认使用的 namespace 为 'liu233w'。
请参考 ./build/docker-compose.mk 来修改设置

up
使用 docker-compose 启动项目，会自动创建 ./build/.env 配置文件，建议根据上面的说明修改一下配置文件内容，以使用项目的全部功能。
如果在windows上运行，本机需要安装msys2，并使用其shell来运行此命令。（需要配置其接受windows的path）

show-image-size
查看本项目生成的所有镜像的体积。此命令不会生成镜像，而是查看以前生成的镜像。

shell
启动一个docker容器并开启一个 shell。镜像将把整个项目 mount 进容器中，便于在 linux 环境下执行命令和更改项目。

help
展示此帮助文档

各个子项目中也可以执行 make 操作。其中同名的命令和本 makefile 中的功能相同，直接查看文件即可获得说明。

endef

.short-help:
	@echo run \"make help\" to get help

export HELP_MESSAGE
help:
	@echo "$$HELP_MESSAGE" | more
