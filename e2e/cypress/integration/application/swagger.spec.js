Cypress.config('baseUrl', 'http://reverse-proxy')

beforeEach(() => {
  cy.visit('/swagger')
})

it('能够加载crawler-api-backend的swagger文件', () => {
  cy.get('#select').select('爬虫 API', {timeout: 10000})
  cy.contains('crawler-api-backend', {timeout: 10000})
})

describe('能够加载后端的swagger文件', () => {

  it('能够访问页面', () => {
    cy.get('#select').select('后端 API V1', {timeout: 10000})
    cy.contains('AcmStatisticsBackend API', {timeout: 60000})
  })

  it('可以正常登录', () => {
    cy.get('#select').select('后端 API V1', {timeout: 10000})
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
    cy.contains('/api/services/app/Role/GetRoles').scrollIntoView().click()

    cy.contains('Try it out').click()
    cy.contains('Execute').click()
    cy.contains('"result"')
    cy.contains('"items"')
  })

})
