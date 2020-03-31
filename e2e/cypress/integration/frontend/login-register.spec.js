Cypress.config('baseUrl', 'http://localhost:3000')

describe('/login', () => {

  beforeEach(() => {
    cy.visit('/login')
  })

  it('能够正确渲染', () => {
    cy.viewport('macbook-15')
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
    // 如果运行太快，加载的指示条还没有消失
    // eslint-disable-next-line cypress/no-unnecessary-waiting
    cy.wait(1000)
    cy.viewport('macbook-15')
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
