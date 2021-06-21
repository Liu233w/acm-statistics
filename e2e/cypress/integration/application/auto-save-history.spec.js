// eslint-disable-next-line
Cypress.on('uncaught:exception', (err, runnable) => {
  // returning false here prevents Cypress from
  // failing the test
  // Stop the uncaught network error
  return false
})

beforeEach(() => {
  cy.intercept(
    'https://acm-statistics-cors.herokuapp.com/http://acm.hdu.edu.cn/userstatus.php?user=wwwlsmcom',
    { fixture: 'summary_hdu.txt' }).as('summary_hdu')
  cy.intercept('POST', '/api/services/app/QueryHistory/SaveOrReplaceQueryHistory')
    .as('save-history')
})

function specs(enterStatistics) {

  describe('at default', () => {

    it('saves history automatically', () => {
      cy.registerAndGetUsername()
      enterStatisticsAndQuery(enterStatistics)
      cy.wait('@save-history')
      cy.log('assert')
      cy.visit('/history')
      cy.get('i.mdi-delete').should('have.length', 1)
    })
  })

  describe('set settings to not save history', () => {

    let username

    before(() => {
      cy.registerAndGetUsername().then(u => username = u)
      cy.clearCookies()
    })

    it('should change settings successfully', () => {
      cy.login(username)
      cy.visit('/settings')
      cy.contains('Auto save query history').parents('.v-card').within(() => {
        cy.get('input[value="false"]').click({ force: true })
        cy.get('button').contains('save', { matchCase: false }).click()
        cy.matchImageSnapshot()
      })
    })

    it('should not auto save history', () => {
      cy.login(username)
      enterStatisticsAndQuery(enterStatistics)
      cy.log('assert')
      cy.visit('/history')
      cy.get('i.mdi-delete').should('have.length', 0)
    })

    it('should go to summary when click view summary', () => {
      cy.login(username)
      enterStatisticsAndQuery(enterStatistics)
      cy.contains('/ submission', { matchCase: false }).click()
      cy.location('pathname').should('satisfy', val => val.startsWith('/history/'))
    })

    it('should have one record in history', () => {
      cy.login(username)
      cy.visit('/history')
      cy.get('i.mdi-delete').should('have.length', 1)
    })
  })

}

describe('when directly enter statistics page', () => specs(() => {
  cy.visit('/statistics')
}))

describe('when enter statistics page from other page', () => specs(() => {
  cy.visit('/about')
  cy.get('i.mdi-menu').parents('button').click()
  cy.contains('statistics', { matchCase: false }).click()
}))

function enterStatisticsAndQuery(enterFunc) {

  cy.log('save a history')
  enterFunc()

  cy.get('div[title="HDU"]').parents('.worker').within(() => {

    cy.get('div:contains("Username") input').type('wwwlsmcom')

    // wait for debounce to be executed
    // eslint-disable-next-line cypress/no-unnecessary-waiting
    cy.wait(500)
    cy.get('button:contains("refresh")').click()
    cy.wait('@summary_hdu')

    cy.contains('34')
    cy.contains('72')
  })
}