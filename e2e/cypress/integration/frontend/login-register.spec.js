Cypress.config('baseUrl', 'http://localhost:3000')

describe('/login', () => {

  beforeEach(() => {
    cy.visit('/login')
  })

  it('can render correctly', () => {
    cy.viewport(1920, 1080)
    cy.matchImageSnapshot()
  })
})

describe('/register', () => {

  beforeEach(() => {
    cy.visit('/register')
  })

  it('can render correctly', () => {
    cy.viewport(1920, 1080)
    cy.matchImageSnapshot()
  })

})
