const configReader = require('../lib/configReader')
const _ = require('lodash')

test('readConfigs 能够正确读取配置', () => {
  const config = configReader.readConfigs()
  expect(config.crawlers.length).not.toBe(0)

  const pojConfig = config.crawlers.poj
  expect(pojConfig).toBeTruthy()
  expect(pojConfig.meta).toBeTruthy()

})

test('readCrawlerConfigs 能够正确读取配置', () => {
  const config = configReader.readCrawlerConfigs()
  expect(config.length).not.toBe(0)

  const pojConfig = _.find(config, item => item.name === 'poj')
  expect(pojConfig).toBeTruthy()
  expect(pojConfig.meta).toBeTruthy()
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
      'ACM_STATISTICS_CRAWLER_ENV:a': '{"b":{"c":"aaa"}}',
    })

    expect(baseConfig).toMatchObject({
      a: {
        b: {
          c: 'aaa',
        },
      },
    })
  })

  it('只导入有指定前缀的环境变量', () => {

    const baseConfig = {}
    configReader.mergeConfigWithEnv(baseConfig, {
      'NNNNNN': 'asdf',
    })
    expect(baseConfig).toMatchObject({})

  })

  it('能够导入字符串环境变量', () => {

    const baseConfig = {}
    configReader.mergeConfigWithEnv(baseConfig, {
      'ACM_STATISTICS_CRAWLER_ENV:a': '"{asdf}"',
    })
    expect(baseConfig).toMatchObject({
      a: '{asdf}',
    })
  })
})

test('readMetaConfigs 能够读取元信息', () => {
  const meta = configReader.readMetaConfigs()
  expect(meta.vjudge).toBeTruthy()
  expect(meta.vjudge.title).toBe('VJudge')
  expect(meta.vjudge.description).toBeTruthy()
  expect(meta.vjudge.virtual_judge).toBe(true)
})
