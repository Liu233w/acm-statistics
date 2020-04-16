/* eslint-disable jest/expect-expect */
jest.mock('svg-captcha')

const request = require('supertest')
const app = require('../src/app').callback()

describe('/api/captcha-service/generate', () => {
    it('可以生成验证码', async () => {
        const res = await request(app).post('/api/captcha-service/generate')
            .expect(200)
        expect(res.body).toMatchObject({
            error: false,
            data: {
                id: expect.any(String),
                captcha: '<svg xmlns="http://www.w3.org/2000/svg" width="150" height="50" viewBox="0,0,150,50" />',
            },
        })
    })
})

describe('/api/captcha-service/validate', () => {

    it('能够进行验证', async () => {
        const id = (await request(app).post('/api/captcha-service/generate')
            .expect(200)).body.data.id

        await request(app).post('/api/captcha-service/validate')
            .send({ id, text: 'validate-text' })
            .expect(200, {
                error: false,
                data: true,
            })
    })

    it('不论大小写', async () => {
        const id = (await request(app).post('/api/captcha-service/generate')
            .expect(200)).body.data.id

        await request(app).post('/api/captcha-service/validate')
            .send({ id, text: 'VALIDATE-text' })
            .expect(200, {
                error: false,
                data: true,
            })
    })

    it('能够在验证码错误时报错', async () => {
        const id = (await request(app).post('/api/captcha-service/generate')
            .expect(200)).body.data.id

        await request(app).post('/api/captcha-service/validate')
            .send({ id, text: 'incorrect' })
            .expect(400, {
                error: true,
                message: 'Incorrect captcha',
            })
    })

    it('不能重复验证', async () => {
        const id = (await request(app).post('/api/captcha-service/generate')
            .expect(200)).body.data.id

        await request(app).post('/api/captcha-service/validate')
            .send({ id, text: 'validate-text' })

        await request(app).post('/api/captcha-service/validate')
            .send({ id, text: 'validate-text' })
            .expect(400, {
                error: true,
                message: 'Please refresh captcha',
            })
    })
})
