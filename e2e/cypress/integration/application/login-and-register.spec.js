Cypress.config('baseUrl', 'http://reverse-proxy')

describe('/login', () => {

  it('能检测出登录错误', () => {
    cy.visit('/login')
    
  })
})

