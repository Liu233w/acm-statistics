Cypress.config('baseUrl', 'http://localhost:3000')

describe('访问不被运行的页面时可以自动进行跳转', () => {

  it('未登录时访问/settings可以跳转到/login页面', () => {
    cy.visit('/settings')
    cy.location('pathname').should('eq', '/login')
  })

  it('登录时访问/login页面会自动跳转到/settings页面', () => {
    cy.log('先登录')
    cy.visit('/login')
    cy.contains('用户名').parent().type('admin')
    cy.contains('密码').parent().type('123qwe')
    cy.contains('保持登录').click()
    cy.get('button').contains('登录').click()

    cy.contains('admin', { timeout: 60000 })
    cy.location('pathname').should('eq', '/')

    cy.log('开始测试')
    cy.visit('/login')
    cy.location('pathname').should('eq', '/settings')
  })
})