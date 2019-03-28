## makefile for node base image

include share.mk

NodeBaseOption =
ifdef APK_MIRROR
NodeBaseOption := --build-arg APK_MIRROR=$(APK_MIRROR) $(NodeBaseOption)
endif
ifdef CODECOV
NodeBaseOption := --build-arg CODECOV=$(CODECOV) $(NodeBaseOption)
endif
ifdef NPM_MIRROR
NodeBaseOption := --build-arg NPM_MIRROR=$(NPM_MIRROR) $(NodeBaseOption)
endif

build:
	docker build . \
		$(build-args) \
		-t $(NodeBaseTag) \
		-f node-base.Dockerfile \
		$(NodeBaseOption)

clean:
	docker image rm $(NodeBaseTag); true
