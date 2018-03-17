import test from 'ava'

import poj from './crawlers/poj'

test('test poj', async t => {
    const res = await poj(null, '')
    t.not(res.solved, NaN, '正确获取到solved')
    t.not(res.submissions, NaN, '正确获取到 submissions')
})