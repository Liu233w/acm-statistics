/* eslint-disable no-undef */

// 在 require 的时候程序会直接 require 爬虫，如果在 beforeAll 里面 mock 的话就晚了
jest.mock('../../crawler/lib/configReader')
jest.mock('../../crawler/lib/functionGenerator')

const supertest = require('supertest')
const app = require('../app')

let server
let request

beforeAll(() => {
  server = app.listen(12001)
  request = supertest(server)
})

afterAll(() => {
  server.close()
})

test('/api/crawlers/swagger.json swagger should match snapshot', async () => {
  const res = await request.get('/api/crawlers/swagger.json')
  expect(res.status).toBe(200)
  expect(res.body).toMatchSnapshot()
})

test('/api/crawlers should return crawler list', async () => {
  const res = await request.get('/api/crawlers')
  expect(res.status).toBe(200)
  expect(res.body).toMatchObject({
    error: false,
    data: {
      crawler1: {
        title: 'Crawler1',
        description: 'Description1',
        url: 'http://www.c1.com',
      },
      crawler2: {
        title: 'Crawler2',
      },
      crawler_for_server: {
        title: 'CrawlerForServer',
      },
    },
  })
})

describe('/api/crawlers/:type/:username', () => {
  it('在结果正确时应该返回题量', async () => {
    const res = await request.get('/api/crawlers/crawler1/user')
    expect(res.status).toBe(200)
    expect(res.body).toMatchObject({
      error: false,
      data: {
        solved: 101,
        submissions: 230,
      },
    })
  })

  it('在爬虫不存在时应该返回 400', async () => {
    const res = await request.get('/api/crawlers/notExist/user')
    expect(res.status).toBe(400)
    expect(res.body).toMatchObject({
      error: true,
      message: '不存在此OJ的爬虫',
    })
  })

  it('在爬虫报错时应该返回错误结果', async () => {
    const res = await request.get('/api/crawlers/crawler1/reject')
    expect(res.status).toBe(400)
    expect(res.body).toMatchObject({
      error: true,
      message: '用户不存在',
    })
  })
})

test('在访问不存在的资源时返回 404', async () => {
  const res = await request.get('/notExists')
  expect(res.status).toBe(404)
  expect(res.body).toMatchObject({
    error: true,
    message: '404 Not Found',
  })
})