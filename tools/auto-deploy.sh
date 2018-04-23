#!/bin/bash

# 检查git是否需要更新，如果需要的话，就拉取最新的更改，然后重新构建
# 如果需要更新而执行脚本失败，则返回 1
# 这个脚本可以放进 crontab 自动执行

git fetch
reslog=$( git log HEAD..origin/master --oneline)
if [[ "${reslog}" != "" ]] ; then
    sh ./deploy-all.sh || exit 1
fi
