Cypress.config('baseUrl', 'http://localhost:3000')

describe('Redirect when visiting un-permitted pages', () => {

  it('can redirect to /login when visiting /settings without being logined', () => {
    cy.visit('/settings')
    cy.location('pathname').should('eq', '/login')
  })

  it('can redirect to /settings when visiting /login and being logined', () => {
    cy.log('Login first')
    cy.visit('/login')
    cy.contains('Username').parent().type('admin')
    cy.contains('Password').parent().type('123qwe')
    cy.contains('Remember me').click()
    cy.get('button').contains('login').click()

    cy.contains('admin', { timeout: 60000 })
    cy.location('pathname').should('eq', '/')

    cy.log('---- TEST ----')
    cy.visit('/login')
    cy.location('pathname').should('eq', '/settings')
  })
})