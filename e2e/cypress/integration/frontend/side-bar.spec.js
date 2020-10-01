describe('when not logged in', () => {
  it('should show side bar correctly', () => {

    cy.visit('/about')

    cy.get('.v-app-bar__nav-icon').click()
    cy.get('.v-navigation-drawer__content').matchImageSnapshot()

  })
})

describe('when logged in', () => {

  let username

  before(() => {
    cy.registerAndGetUsername().then(u => {
      username = u
    })
    cy.clearCookies()
  })

  beforeEach(() => {
    cy.login(username)
  })

  it('should show side bar correctly', () => {

    cy.visit('/about')

    cy.get('.v-app-bar__nav-icon').click()
    cy.get('.v-navigation-drawer__content').matchImageSnapshot()

  })
})