Cypress.config('baseUrl', 'http://reverse-proxy')

describe('整体视觉测试', () => {

  beforeEach(() => {
    cy.mockServer('busuanzi')

    cy.visit('/')

    // 移除卷动图像，防止它影响到快照
    cy.document().then(document => {
      document.querySelectorAll('.v-parallax__image')
        .forEach(item => item.setAttribute('style', null))
    })
  })

  it('能够正确渲染', () => {
    cy.matchImageSnapshot()
  })

  describe('能够在不同尺寸的手持设备下正确渲染', () => {
    [
      'ipad-2',
      'ipad-mini',
      'iphone-6+',
      'iphone-6',
      'iphone-5',
    ].forEach(item => {

      it(item.toString(), () => {
        cy.viewport(item)
        cy.matchImageSnapshot()
      })
    })
  })

})

describe('卷动图片的视觉测试', () => {

  beforeEach(() => {
    cy.visit('/')
  })

  describe('第一张卷动图片', () => {
    it('能够正常显示', () => {
      cy.get('.v-parallax').eq(0).matchImageSnapshot()
    })
  })

  describe('第二章卷动图片', () => {
    it('能够正常显示', () => {
      cy.get('.v-parallax').eq(1).matchImageSnapshot()
    })
  })
})

describe('其他部分', () => {

  beforeEach(() => {
    cy.visit('/')
  })

  it('点击进入查题按钮可以进入查题', () => {
    cy.contains('进入 OJ 题量统计').click()
    cy.url().should('be', '/statistics')
  })

  it('显示微信公众号窗口', () => {
    cy.contains('div[role="listitem"]', '微信公众号').click()
    cy.get('.v-dialog.v-dialog--active').within(() => {
      cy.contains('微信公众号')
      cy.matchImageSnapshot()
    })
  })
})
