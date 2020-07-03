// 在 require 的时候程序会直接 require 爬虫，如果在 beforeAll 里面 mock 的话就晚了
jest.mock('crawler')

const request = require('supertest')
const app = require('../app').callback()

test('/api/crawlers/swagger.json swagger should match snapshot', async () => {
  const res = await request(app).get('/api/crawlers/swagger.json')
    .expect(200)
  expect(res.body).toMatchSnapshot()
})

test('/api/crawlers should return crawler list', async () => {
  await request(app)
    .get('/api/crawlers')
    .expect(200, {
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
  it('should work correctly', async () => {
    await request(app)
      .get('/api/crawlers/crawler1/user')
      .expect(200, {
        error: false,
        data: {
          solved: 101,
          submissions: 230,
        },
      })
  })

  it('should return 400 when crawler does not exist', async () => {
    await request(app)
      .get('/api/crawlers/notExist/user')
      .expect(400, {
        error: true,
        message: 'Crawler of the oj does not exist',
      })
  })

  it('should return error when crawler return error', async () => {
    await request(app)
      .get('/api/crawlers/crawler1/reject')
      .expect(400, {
        error: true,
        message: 'The user does not exist',
      })
  })
})

test('should return 404 when visiting url that does not exist', async () => {
  await request(app)
    .get('/notExists')
    .expect(404, {
      error: true,
      message: '404 Not Found',
    })
})
