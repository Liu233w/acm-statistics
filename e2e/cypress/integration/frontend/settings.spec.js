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
    cy.shouldHaveUri('/')
    cy.login(username, '1234QWer')
  })
})

describe('change time zone', () => {
  before(() => {
    cy.intercept('POST', '/api/services/app/UserConfig/SetUserTimeZone').as('set-time-zone')
  })

  it('should work correctly', () => {
    cy.visit('/settings')

    cy.contains('Change time zone').parent().within(() => {
      cy.get('.v-select').click()
      cy.root().parents('html')
        .contains('(GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna')
        .click()
      cy.contains('save').click()
    })

    cy.wait('@set-time-zone')

    cy.reload()
    // hide header
    cy.get('header').invoke('hide')

    cy.contains('Change time zone').parent().matchImageSnapshot()
  })
})

describe('delete account', () => {
  it('should work correctly', () => {
    cy.visit('/settings')
    cy.get('button:contains("delete")').click()
    cy.get('.v-dialog').within(() => {
      cy.matchImageSnapshot()
      cy.get('button:contains("Confirm")').click()
    })
    cy.shouldHaveUri('/')
  })
})