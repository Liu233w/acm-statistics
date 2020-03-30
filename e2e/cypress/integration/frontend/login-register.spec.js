Cypress.config('baseUrl', 'http://reverse-proxy')

describe('/login', () => {

  beforeEach(() => {
    cy.visit('/login')
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

describe('/register', () => {

  beforeEach(() => {
    cy.visit('/register')
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
