Cypress.config('baseUrl', 'http://reverse-proxy')

describe('/', () => {

  beforeEach(() => {
    cy.mockServer('busuanzi')
  })

  it('能够正确渲染', () => {
    cy.visit('/')
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
        cy.visit('/')
        cy.matchImageSnapshot()
      })
    })
  })

})