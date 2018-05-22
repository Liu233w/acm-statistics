/* eslint-disable no-undef */

import {warningHelper} from '~/components/statisticsUtils'

describe('warningHelper', () => {
  it('在正确时不会生成警告', () => {
    const worker = {
      username: 'user1',
      status: 'WORKING',
      submissions: 0,
      solved: 0,
      errorMessage: '.....',
      tokenKey: 0.23333,
      crawlerName: 'cr1',
      key: 0.66666666,
    }
    expect(warningHelper(worker)).toMatchObject([])

    worker.username = ''
    expect(warningHelper(worker)).toMatchObject([])
  })

  it('能够生成用户名以空格开头的警告', () => {
    const worker = {
      username: ' user1',
      status: 'WORKING',
      submissions: 0,
      solved: 0,
      errorMessage: '.....',
      tokenKey: 0.23333,
      crawlerName: 'cr1',
      key: 0.66666666,
    }

    expect(warningHelper(worker)).toMatchObject([
      '用户名以空格开头',
      '用户名含有空格，部分爬虫可能不支持',
    ])
  })

  it('能够生成用户名带有空格的警告', ()=>{
    const worker = {
      username: 'user 1',
      status: 'WORKING',
      submissions: 0,
      solved: 0,
      errorMessage: '.....',
      tokenKey: 0.23333,
      crawlerName: 'cr1',
      key: 0.66666666,
    }

    expect(warningHelper(worker)).toMatchObject([
      '用户名含有空格，部分爬虫可能不支持',
    ])
  })
})
