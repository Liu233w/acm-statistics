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

describe('change password', () => {
  it('can report error when password is wrong', () => {
    cy.visit('/settings')
    cy.contains('Change Password').parent().within(() => {
      cy.contains('Current Password').parent().type('wrong')
      cy.contains('New Password').parent().type('1234QWer')
      cy.contains('Confirm Password').parent().type('1234QWer')
      cy.contains('submit').click()
      cy.matchImageSnapshot()
    })
  })

  it('can work correctly', () => {
    cy.visit('/settings')
    cy.contains('Change Password').parent().within(() => {
      cy.contains('Current Password').parent().type('1234Qwer')
      cy.contains('New Password').parent().type('1234QWer')
      cy.contains('Confirm Password').parent().type('1234QWer')
      cy.contains('submit').click()
      cy.contains('Success!')
      cy.matchImageSnapshot()
    })

    cy.log('re-login to test new password')
    cy.get('button:contains("sign out")').click()
    cy.url().should('be', '/')
    cy.login(username, '1234QWer')
  })
})

describe('delete account', () => {
  it('shoule work correctly', () => {
    cy.visit('/settings')
    cy.get('button:contains("delete")').click()
    cy.get('.v-dialog').within(() => {
      cy.matchImageSnapshot()
      cy.get('button:contains("Confirm")').click()
    })
    cy.url().should('be', '/')
  })
})