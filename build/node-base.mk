## makefile for node base image

include share.mk

NodeBaseOption =
ifdef CODECOV
NodeBaseOption := --build-arg CODECOV=$(CODECOV) $(NodeBaseOption)
endif

build:
	docker build . -f node-base.Dockerfile -t $(NodeBaseTag) $(NodeBaseOption)

clean:
	docker image rm $(NodeBaseTag); true
