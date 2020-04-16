Cypress.config('baseUrl', 'http://localhost:3000')

describe('overall', () => {

  beforeEach(() => {
    cy.visit('/about')
  })

  it('can render correctly', () => {
    cy.viewport(1920, 1080)
    cy.matchImageSnapshot()
  })
})
