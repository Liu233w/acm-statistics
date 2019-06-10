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

build-http-mocks: .node-base
	docker build ./http-mocks \
		-t $(E2eMockConfigurerTag) \
		$(build-args) \
		--build-arg NODE_BASE_IMAGE=$(NodeBaseTag)

.base:
	docker build . \
		-t $(E2eBaseTag) \
		$(build-args)

ci: .base
	# 让字体走外网，防止因为连接问题导致快照错误
	http_proxy=http://localhost:1081 curl http://mock-configurer/travis/mock_font_awesome
	http_proxy=http://localhost:1081 curl http://mock-configurer/travis/mock_font_cdn
	docker run --rm -t --ipc=host --network=host \
		$(shell printenv | grep -E '^TRAVIS' | sed 's/"/\\"/g; s/\(.*\)/"\1"/g; s/^/-e /g') \
	 	$(E2eBaseTag) \
		npm test -- --record --key $(CYPRESS_RECORD_KEY)

ci-no-record: .base
	# 让字体走外网，防止因为连接问题导致快照错误
	http_proxy=http://localhost:1081 curl http://mock-configurer/travis/mock_font_awesome
	http_proxy=http://localhost:1081 curl http://mock-configurer/travis/mock_font_cdn
	docker run --rm -t --ipc=host --network=host \
		$(shell printenv | grep -E '^TRAVIS' | sed 's/"/\\"/g; s/\(.*\)/"\1"/g; s/^/-e /g') \
	 	$(E2eBaseTag) \
		npm test

run: .base
	docker run $(run-args) $(E2eBaseTag) $(run-cmd)

# test 也mount，这样的话就可以查看图片的diff了
test: .base
	docker run --rm -t --ipc=host --network=host \
		-v "$(CURDIR)/cypress:/e2e/cypress" \
	 	$(E2eBaseTag) \
		npm test

open: export http_proxy=http://localhost:1081
open:
	npm start -- --env failOnSnapshotDiff=false

update-snapshot: .base
	docker run --rm -t --ipc=host --network=host \
		-v "$(CURDIR)/cypress:/e2e/cypress" \
	 	$(E2eBaseTag) \
		npm test -- --env updateSnapshots=true
