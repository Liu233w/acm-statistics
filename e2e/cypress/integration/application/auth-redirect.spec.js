describe('Redirect when visiting un-permitted pages', () => {

  it('can redirect to /login when visiting /settings without being logined', () => {
    cy.visit('/settings')
    cy.shouldHaveUri('/login')
  })

  it('can redirect to /settings when visiting /login and being logined', () => {
    cy.login('admin')

    cy.log('---- TEST ----')
    cy.visit('/login')
    cy.shouldHaveUri('/settings')
  })
})