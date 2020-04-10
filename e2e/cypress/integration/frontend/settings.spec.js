Cypress.config('baseUrl', 'http://localhost:3000')

let username

before(() => {
  cy.registerAndGetUsername().then(u => username = u)
})

beforeEach(function () {
  Cypress.Cookies.preserveOnce('OAuthToken')
})

describe('整体视觉测试', () => {

  it('能够正确渲染', () => {

    cy.visit('/settings')
    cy.viewport(1920, 1080)

    // 隐藏用户名
    cy.get(`button:contains("${username}")`)
      .invoke('attr', 'style', 'background-color: black')
    cy.matchImageSnapshot()
  })
})