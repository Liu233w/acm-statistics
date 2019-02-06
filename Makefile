## root makefile

include ./build/share.mk

# == common suffix ==

# use command like `make target=crawler test clean` to invoke `make -C crawler test clean`
# support command like `make target="crawler frontend" build`

TargetList = crawler frontend crawler-api-backend
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
	for dir in $(AllTarget); do \
  	$(MAKE) -C $$dir clean; \
  done
ifeq ($(target),)
	cd ./build && $(MAKE) -f node-base.mk clean
	cd ./build && $(MAKE) -f commitlint.mk clean
	@echo cleaned all target
else
	@echo cleaned $(target)
endif

test-ci:
	@echo testing ci on target: $(AllTarget)
	for dir in $(AllTarget); do \
  	$(MAKE) -C $$dir test-ci; \
  done

# === commitlint ===

.PHONY: test-commit commitlint-travis

test-commit:
	cd ./build && $(MAKE) -f commitlint.mk test-commit

commitlint-travis:
	cd ./build && $(MAKE) -f commitlint.mk commitlint-travis

# === publish image ===
.PHONY: tag-and-push

tag-and-push:
	cd ./build && $(MAKE) -f docker-compose.mk push

# === run all ===
.PHONY: up

up:
	cd ./build && $(MAKE) -f docker-compose.mk up