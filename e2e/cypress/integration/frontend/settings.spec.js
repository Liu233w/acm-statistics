Cypress.config('baseUrl', 'http://localhost:3000')

let username

before(() => {
  cy.registerAndGetUsername().then(u => username = u)
})

beforeEach(function () {
  Cypress.Cookies.preserveOnce('OAuthToken')
})

describe('overall', () => {

  it('can render correctly', () => {

    cy.visit('/settings')
    cy.viewport(1920, 1080)

    // hide account username
    cy.get(`button:contains("${username}")`)
      .invoke('attr', 'style', 'background-color: black')
    cy.matchImageSnapshot()
  })
})