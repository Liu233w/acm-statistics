const _ = require('lodash')

const poj = require('../crawlers/poj')
const hdu = require('../crawlers/hdu')
const zoj = require('../crawlers/zoj')
const dashiye = require('../crawlers/dashiye')
const codeforces = require('../crawlers/codeforces')
const uva = require('../crawlers/uva')
const uvalive = require('../crawlers/uvalive')
const fzu = require('../crawlers/fzu')
const spoj = require('../crawlers/spoj')
const timus = require('../crawlers/timus')
const leetcode_cn = require('../crawlers/leetcode_cn')
const vjudge = require('../crawlers/vjudge')
const csu = require('../crawlers/csu')
const loj = require('../crawlers/loj')
const luogu = require('../crawlers/luogu')
const nowcoder = require('../crawlers/nowcoder')
const uestc = require('../crawlers/uestc')
const atcoder = require('../crawlers/atcoder')
const aizu = require('../crawlers/aizu')
const codechef = require('../crawlers/codechef')
const eljudge = require('../crawlers/eljudge')
const bnu = require('../crawlers/bnu')
const codewars = require('../crawlers/codewars')
const uoj = require('../crawlers/uoj')
const nbut = require('../crawlers/nbut')
const nod = require('../crawlers/nod')
const nit = require('../crawlers/nit')
const dmoj = require('../crawlers/dmoj')

const { readConfigs } = require('../lib/configReader')

jest.setTimeout(10000) // 最多10秒

const notExistUsername = 'fmv84zcq3hwu'
const username = 'vjudge5'

// 另外，感谢 @leoloveacm, @2013300262 同学提供测试帐号

describe('poj', () => {

  test('should throw when user does not exist', async () => {
    await expect(poj(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(poj(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await poj(null, username)
    checkRes(res)
    expect(res.solvedList).toContain('1001')
  })

})

describe('hdu', () => {

  const config = {
    env: 'server',
  }

  test('should throw when user does not exist', async () => {
    await expect(hdu(config, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(hdu(config, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await hdu(config, 'vjudge4')
    checkRes(res)
    expect(res.solvedList).toContain('1001')
  })

})

describe('zoj', () => {

  test('should throw when user does not exist', async () => {
    await expect(zoj(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(zoj(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await zoj(null, 'IamRobot')
    checkRes(res)
    expect(res.solvedList).toContain('1001')
    // so it is really slow...
  }, 30000)

})

describe('dashiye', () => {

  test('should throw when user does not exist', async () => {
    await expect(dashiye(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(dashiye(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await dashiye(null, 'root')
    checkRes(res)
    expect(res.solvedList).toContain('1001')
  })

})

describe('codeforces', () => {

  test('should throw when user does not exist', async () => {
    await expect(codeforces(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(codeforces(null, ' ' + notExistUsername)).rejects.toThrow('handle: Field should contain only Latin letters, digits, underscore or dash characters')
  })

  test('should work correctly', async () => {
    const res = await codeforces(null, 'leoloveacm') // 没有找到好的测试多页返回的帐号，还是用这个测试单页吧
    checkRes(res)

    expect(new Set(res.solvedList)).toMatchObject(new Set([
      '754B', '165E', '492E', '338D', '333A', '703A', '540C', '680B', '680A', '676A', '450B', '667A', '667C',
    ]))
  })

})

describe('uva', () => {

  test('should throw when user does not exist', async () => {
    await expect(uva(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(uva(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await uva(null, 'leoloveacm')
    checkRes(res)
    expect(res.solvedList).toContain('1395')
  })

})

describe('uvalive', () => {

  test('should throw when user does not exist', async () => {
    await expect(uvalive(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(uvalive(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await uvalive(null, 'npuacm')
    checkRes(res)
    expect(res.solved).toBe(2)
    expect(res.submissions).toBe(3)
    expect(new Set(res.solvedList)).toMatchObject(new Set([
      '4445',
      '3198',
    ]))
  })

})

describe('fzu', () => {

  test('should throw when user does not exist', async () => {
    await expect(fzu(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(fzu(null, ' ' + notExistUsername))
      .rejects.toThrow('The crawler does not support username with spaces')
  })

  test('should work correctly', async () => {
    const res = await fzu(null, username)
    checkRes(res)
    expect(res.solvedList).toContain('1001')
  })

})

describe('spoj', () => {

  test('should throw when user does not exist', async () => {
    await expect(spoj(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(spoj(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await spoj(null, username)
    checkRes(res)
    expect(res.solvedList).toContain('ABA12D')
  })

})

describe('timus', () => {

  test('should throw when user does not exist', async () => {
    await expect(timus(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(timus(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await timus(null, 'vjudge11')
    checkRes(res)
    expect(res.submissions).toBeGreaterThan(10)
    expect(res.solvedList).toContain('1387')
  })

})

describe('leetcode_cn', () => {

  test('should throw when user does not exist', async () => {
    await expect(leetcode_cn(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(leetcode_cn(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  // eslint-disable-next-line jest/expect-expect
  test('should work correctly', async () => {
    const res = await leetcode_cn(null, 'wwwlsmcom')
    checkRes(res)
  })

})

describe('csu', () => {

  test('should throw when user does not exist', async () => {
    await expect(csu(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(csu(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await csu(null, username)
    checkRes(res)
    expect(res.solvedList).toContain('1001')
  })

})

describe('vjudge', () => {

  let vjConfig

  beforeAll(async () => {
    const config = await readConfigs()
    // config should be read to test the crawler
    // you can set environment variable according to README.md
    vjConfig = config.crawlers.vjudge
  })

  test('ensure config exists and is correct', () => {
    expect(vjConfig).toBeTruthy()
    expect(vjConfig.crawler_login_user).toBeTruthy()
    expect(vjConfig.crawler_login_user).not.toBe('用户名')
    expect(vjConfig.crawler_login_password).toBeTruthy()
    expect(vjConfig.crawler_login_password).not.toBe('密码')

    // console.log(vjConfig)
  })

  test('should throw when user does not exist', async () => {
    await expect(vjudge(vjConfig, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(vjudge(vjConfig, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await vjudge(vjConfig, 'leoloveacm')
    checkRes(res)
    expect(res.solvedList).toContain('codeforces-436B')
    expect(res.submissionsByCrawlerName).toBeTruthy()
    expect(res.submissionsByCrawlerName.hdu).toBeGreaterThan(100)
    expect(res).toMatchSnapshot()
  }, 50000)
})

describe('loj', () => {

  test('should throw when user does not exist', async () => {
    await expect(loj(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await loj(null, 'cz_xuyixuan') // 自定义存在的用户名
    checkRes(res)
    expect(res.solvedList).toContain('103')
  }, 50000)

})

describe('luogu', () => {

  test('should throw when user does not exist', async () => {
    await expect(luogu(null, notExistUsername)).rejects.toThrow('The user does not exist')
  }, 50000)

  test('can recognize username with space', async () => {
    await expect(luogu(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  }, 50000)

  test('should work correctly', async () => {
    const res = await luogu(null, 'CancerGary') // 自定义存在的用户名
    checkRes(res)
    expect(res.solvedList).toContain('P1001')
    expect(res.solvedList.length).toBeGreaterThan(100)
  }, 50000)

  test('should handle the user with submission bigger than 1000 correctly', async () => {
    const res = await luogu(null, 'NaCly_Fish')
    checkRes(res)
    expect(res.submissions).toBeGreaterThan(1000)
  }, 50000)
})

describe('nowcoder', () => {

  test('should throw when user does not exist', async () => {
    await expect(nowcoder(null, '11')).rejects.toThrow('The user does not exist')
  })

  test('should recognize usernames that are not ID', async () => {
    await expect(nowcoder(null, 'wwwlsmcom')).rejects.toThrow('牛客网的输入必须是用户ID（数字格式）')
    await expect(nowcoder(null, '123wwwlsmcom')).rejects.toThrow('牛客网的输入必须是用户ID（数字格式）')
    await expect(nowcoder(null, '123 wwwlsmcom')).rejects.toThrow('牛客网的输入必须是用户ID（数字格式）')
    await expect(nowcoder(null, ' wwwlsmcom')).rejects.toThrow('牛客网的输入必须是用户ID（数字格式）')
  })

  test('should work correctly', async () => {
    const res = await nowcoder(null, '112946')
    checkRes(res)
    expect(res.solvedList).toContain('16632')
  })

})

describe('uestc', () => {

  test('should throw when user does not exist', async () => {
    await expect(uestc(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(uestc(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await uestc(null, 'HeRaNO')
    checkRes(res)
    expect(res.solvedList).toContain('1490')

    const newRes = await uestc(null, 'acm-statistics-test')
    expect(newRes).toMatchObject({
      submissions: 3,
      solved: 1,
      solvedList: ['1'],
    })
  })

})

describe('atcoder', () => {

  test('should throw when user does not exist', async () => {
    await expect(atcoder(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(atcoder(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await atcoder(null, 'wata')

    expect(typeof res.solved).toBe('number')
    expect(typeof res.submissions).toBe('number')

    expect(res.solved).toBeGreaterThan(0)
    expect(res.submissions).toBe(res.solved)
  })

})

describe('aizu', () => {

  test('should throw when user does not exist', async () => {
    await expect(aizu(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(aizu(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await aizu(null, username)
    checkRes(res)
    expect(res.solvedList).toContain('ALDS1_3_C')
  })

})

describe('codechef', () => {

  test('should throw when user does not exist', async () => {
    await expect(codechef(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(codechef(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    // don't know why, but in vjudge2 - vjudge5, solvedList does not match solved count
    const res = await codechef(null, 'vjudge')
    checkRes(res)
    expect(res.solvedList).toContain('KGOOD')
  })

})

describe('eljudge', () => {

  test('should throw when user does not exist', async () => {
    await expect(eljudge(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(eljudge(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await eljudge(null, 'vjudge5')
    checkRes(res)
    expect(res.solvedList).toContain('000')
  })

})

describe('bnu', () => {

  test('should throw when user does not exist', async () => {
    await expect(bnu(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(bnu(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await bnu(null, '51isoft')
    checkRes(res)
    expect(res.solvedList).toBeNull()
  })

})

describe('codewars', () => {

  test('should throw when user does not exist', async () => {
    await expect(codewars(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(codewars(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await codewars(null, 'Liu233w')

    expect(typeof res.solved).toBe('number')
    expect(typeof res.submissions).toBe('number')

    expect(res.solved).toBeGreaterThan(0)
    expect(res.submissions).toBe(res.solved)

    expect(res.solvedList.length).toBe(res.solved)
    expect(res.solvedList).toContain('equal-sides-of-an-array')
  })

})

describe('uoj', () => {

  test('should throw when user does not exist', async () => {
    await expect(uoj(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(uoj(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    // it is an administration account, so I guess it's fine
    const res = await uoj(null, 'matthew99')
    checkRes(res)
    expect(res.solvedList).toContain('30')
  })

})

describe('nbut', () => {

  test('should throw when user does not exist', async () => {
    await expect(nbut(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(nbut(null, ' ' + notExistUsername))
      .rejects.toThrow('The crawler does not support username with spaces')
  })

  test('should work correctly', async () => {
    // it is an administration account, so I guess it's fine
    const res = await nbut(null, username)
    checkRes(res)
    expect(res.solvedList).toContain('1010')
  })

})

describe('nod', () => {

  test('should throw when user does not exist', async () => {
    await expect(nod(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(nod(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    // vjudge
    const res = await nod(null, '张翼德')
    checkRes(res)
    expect(res.solvedList).toContain('1079')
  })

})

describe('nit', () => {

  test('should throw when user does not exist', async () => {
    await expect(nit(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(nit(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await nit(null, 'teito')
    checkRes(res)
    expect(res.solvedList).toContain('nit-100')
    expect(res.solvedList).toContain('hdu-2097')
  })

})

describe('dmoj', () => {

  test('should throw when user does not exist', async () => {
    await expect(dmoj(null, notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('can recognize username with space', async () => {
    await expect(dmoj(null, ' ' + notExistUsername)).rejects.toThrow('The user does not exist')
  })

  test('should work correctly', async () => {
    const res = await dmoj(null, 'Xyene')
    checkRes(res)
    expect(res.solvedList).toContain('aplusb')
  })

})

function checkRes(res) {
  expect(typeof res.solved).toBe('number')
  expect(typeof res.submissions).toBe('number')

  expect(res.solved).toBeGreaterThan(0)
  expect(res.submissions).toBeGreaterThan(0)

  expect(res.submissions).toBeGreaterThan(res.solved)

  if (!_.isNil(res.solvedList)) {
    expect(res.solvedList.length).toBe(res.solved)
  }
}