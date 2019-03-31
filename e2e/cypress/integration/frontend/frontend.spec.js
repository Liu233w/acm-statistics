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