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
	@echo cleaned all target
else
	@echo cleaned $(target)
endif