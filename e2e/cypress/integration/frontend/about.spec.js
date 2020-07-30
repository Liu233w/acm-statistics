Cypress.config('baseUrl', 'http://localhost:3000')

describe('overall', () => {

  beforeEach(() => {
    cy.visit('/about')
  })

  it('can render correctly', () => {
    cy.viewport(1920, 1080)
    cy.matchImageSnapshot()
  })

  it('wechat dialog can be rendered correctly', () => {
    cy.contains('div[role="listitem"]', '微信公众号').click()
    cy.get('.v-dialog.v-dialog--active').within(() => {
      cy.contains('微信公众号')
      cy.matchImageSnapshot()
    })
  })
})
