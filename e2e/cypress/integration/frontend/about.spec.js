Cypress.config('baseUrl', 'http://reverse-proxy')

describe('整体视觉测试', () => {

  beforeEach(() => {
    cy.visit('/about')
  })

  it('能够正确渲染', () => {
    cy.viewport(1920, 1080)
    // eslint-disable-next-line cypress/no-unnecessary-waiting
    cy.wait(2000)
    cy.matchImageSnapshot()
  })
})
