Cypress.config('baseUrl', 'http://localhost:3000')

describe('整体视觉测试', () => {

  beforeEach(() => {
    cy.visit('/about')
  })

  it('能够正确渲染', () => {
    cy.viewport(1920, 1080)
    cy.matchImageSnapshot()
  })
})
