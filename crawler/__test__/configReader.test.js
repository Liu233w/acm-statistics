const configReader = require('../lib/configReader')
const _ = require('lodash')

test('readConfigs 能够正确读取配置', async () => {
  const config = await configReader.readConfigs()
  expect(config.crawlers.length).not.toBe(0)

  const sguConfig = _.find(config.crawlers, item => item.name === 'sgu')
  expect(sguConfig).toBeTruthy()
  expect(sguConfig.meta).toBeTruthy()

})

describe('mergeConfigWithEnv', () => {

  it('能够正确合并信息', () => {

    const baseConfig = {
      a: {
        b: [{}],
        d: 333,
      },
    }

    configReader.mergeConfigWithEnv(baseConfig, {
      'ACM_STATISTICS_CRAWLER_ENV:a:b:1:c': 12,
    })

    expect(baseConfig).toMatchObject({
      a: {
        b: [{}, {c: 12}],
        d: 333,
      },
    })
  })

  it('能够合并json的信息', () => {

    const baseConfig = {
      a: 1,
    }

    configReader.mergeConfigWithEnv(baseConfig, {
      'ACM_STATISTICS_CRAWLER_ENV_OBJECT:a': '{"b":{"c":"aaa"}}',
    })

    expect(baseConfig).toMatchObject({
      a: {
        b: {
          c: 'aaa',
        },
      },
    })
  })

  it('只导入有指定前缀的环境变量', ()=>{

    const baseConfig = {}
    configReader.mergeConfigWithEnv(baseConfig, {
      'NNNNNN': 'asdf',
    })
    expect(baseConfig).toMatchObject({})

  })
})

test('readMetaConfigs 能够读取元信息', async () => {
  const meta = await configReader.readMetaConfigs()
  expect(meta.sgu).toBeTruthy()
  expect(meta.sgu.title).toBe('SGU')
  expect(meta.sgu.description).toBeTruthy()
})
