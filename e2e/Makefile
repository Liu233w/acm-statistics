# makefile for e2e

include ../build/share.mk

# === targets ===

.PHONY: server .node-base build-http-mocks test open ci .base lint

.node-base:
	cd ../build && $(MAKE) -f node-base.mk build

## 覆盖跟时间和版本有关的变量，用于快照。
server: export VERSION_NUM = 0
server: export BUILD_TIME = 0
server:
	cd ../build && $(MAKE) -f docker-compose.mk e2e-up

## 等待server就位。后端启动最慢，等它启动就差不多了
wait-server:
	docker run --rm -t --ipc=host --network=host \
		jwilder/dockerize -timeout 30m \
		-wait http://localhost:3000/api/services/app/Session/GetCurrentLoginInformations

build-http-mocks: .node-base
	docker build ./http-mocks \
		-t $(E2eMockConfigurerTag) \
		$(build-args) \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag)

.base:
ifeq ($(pull-e2e-base-image),)
	docker build . \
		-t $(E2eBaseTag) \
		$(build-args)
else
	docker pull liu233w/acm-statistics-e2e-base
	docker tag liu233w/acm-statistics-e2e-base $(E2eBaseTag)
endif

ci: .base
	docker run --rm -t --ipc=host --network=host \
		$(shell printenv | grep -E '^GITHUB' | sed 's/"/\\"/g; s/\(.*\)/"\1"/g; s/^/-e /g') \
		-v "$(CURDIR)/cypress:/e2e/cypress" \
	 	$(E2eBaseTag) \
		pnpm test -- --record --key $(CYPRESS_RECORD_KEY)

ci-no-record: .base
	docker run --rm -t --ipc=host --network=host \
		$(shell printenv | grep -E '^GITHUB' | sed 's/"/\\"/g; s/\(.*\)/"\1"/g; s/^/-e /g') \
		-v "$(CURDIR)/cypress:/e2e/cypress" \
	 	$(E2eBaseTag) \
		pnpm test

run: .base
	docker run $(run-args) $(E2eBaseTag) $(run-cmd)

# test 也mount，这样的话就可以查看图片的diff了
test: .base
	docker run --rm -t --ipc=host --network=host \
		-v "$(CURDIR)/cypress:/e2e/cypress" \
	 	$(E2eBaseTag) \
		pnpm test

open: export http_proxy=http://localhost:1081
open:
	pnpm start -- --env failOnSnapshotDiff=false

update-snapshot: .base
	docker run --rm -t --ipc=host --network=host \
		-v "$(CURDIR)/cypress:/e2e/cypress" \
	 	$(E2eBaseTag) \
		pnpm test -- --env updateSnapshots=true

upd: .base wait-server update-snapshot test update-html-snapshot

clean:
	docker image rm $(E2eBaseTag) $(E2eMockConfigurerTag); true

# ===== 使用 jest 对html进行快照 ===========
test-html: .base
	docker run --rm -t --ipc=host --network=host \
	$(E2eBaseTag) pnpm test-html

# 更新快照
update-html-snapshot: .base
	docker run --rm -t --ipc=host --network=host \
		-v "$(CURDIR)/__test__:/e2e/__test__" \
		$(E2eBaseTag) pnpm test-html --updateSnapshot

test-html-ci: .base
	docker run --rm --ipc=host --network=host \
		$(E2eBaseTag) \
		pnpm test-html --colors --ci
