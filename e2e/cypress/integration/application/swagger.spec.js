beforeEach(() => {
  cy.visit('/swagger')
})

it('can load swagger file of crawler-api-backend', () => {
  cy.get('#select').select('Crawler API', {timeout: 10000})
  cy.contains('crawler-api-backend', {timeout: 10000})
})

describe('can load swagger file of backend', () => {

  it('can visit page', () => {
    cy.get('#select').select('Backend API V1', {timeout: 10000})
    cy.contains('AcmStatisticsBackend API', {timeout: 60000})
  })

  it.skip('can login correctly', () => {
    cy.get('#select').select('Backend API V1', {timeout: 10000})
    cy.get('#authorize', {timeout: 10000}).within(()=>{
      cy.contains('Authorize').click()
    })
    cy.get('#userName').type('admin')
    cy.get('#password').type('123qwe')
    cy.get('.auth-btn-wrapper > .authorize').within(()=>{
      cy.contains('Login').click()
    })

    cy.contains('Logout', {timeout: 10000})

    // 等待dom刷新
    // eslint-disable-next-line cypress/no-unnecessary-waiting
    cy.wait(1000)
    cy.contains('/api/services/app/Role/GetRoles').scrollIntoView()
    cy.contains('/api/services/app/Role/GetRoles').click()

    cy.contains('Try it out').click()
    cy.contains('Execute').click()
    cy.contains('"result"')
    cy.contains('"items"')
  })

})
