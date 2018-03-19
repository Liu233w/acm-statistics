import test from 'ava'
import _ from 'lodash'

import poj from '../crawlers/poj'
import vjudge from '../crawlers/vjudge'
import {ensureConfigAndRead} from '../lib/configReader'

const notExistUsername = 'fmV84ZCQ3hwu'
// 感谢 @leoloveacm 同学提供帐号
const username = 'leoloveacm'

test('test poj - 用户不存在时抛出异常', async t => {
  const err = await t.throws(async () => await poj(null, notExistUsername))
  t.is(err.message, '用户不存在')
})

test('test poj', async t => {
  const res = await poj(null, username)
  checkRes(res, t)
})

test.skip('test vjudge', async t => {
  const config = await ensureConfigAndRead()
  // 需要读取设置，因此没法在 ci 里面测试
  const vjConfig = _.find(config.crawlers, item=>item.name === 'vjudge')

  t.not(vjConfig, undefined, '必须要保证配置文件正确才能进行此项测试')
  t.not(vjConfig.crawler_login_user, undefined, '必须要保证配置文件正确才能进行此项测试')
  t.not(vjConfig.crawler_login_user, '用户名', '必须要保证配置文件正确才能进行此项测试')

  const res = await vjudge(vjConfig, username)
  console.log(res)
  checkRes(res, t)
})

function checkRes(res, t) {
  t.is(typeof res.solved, 'number', '正确获取到 solved')
  t.is(typeof res.submissions, 'number', '正确获取到 submissions')

  t.not(res.solved, NaN, '正确获取到 solved')
  t.not(res.submissions, NaN, '正确获取到 submissions')
}