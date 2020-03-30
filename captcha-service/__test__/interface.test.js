const svgCaptcha = require('svg-captcha')

describe('svg-captcha 接口测试', () => {
    it('能够正确返回数据', () => {
        const captcha = svgCaptcha.create({
            size: 6,
            noise: 2,
        })

        expect(captcha.data).toBeTruthy()
        expect(captcha.text).toBeTruthy()
    })
})