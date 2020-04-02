Cypress.config('baseUrl', 'http://localhost:3000')

const newUsername = 'user' + ('' + Math.random()).split('.')[1]

before(() => {
  cy.log('先注册一个帐号')
  cy.visit('/register')
  // 先等待验证码（刷新dom）
  cy.contains('验证码').parent().type('validate-text')
  cy.contains('用户名').parent().type(newUsername)
  cy.contains('密码').parent().type('123qwe')
  cy.contains('再次输入密码').parent().type('123qwe')
  cy.get('button').contains('注册').click()
  cy.location('pathname').should('eq', '/')
})

beforeEach(function () {
  Cypress.Cookies.preserveOnce('OAuthToken')
})


describe('整体视觉测试', () => {

  it('能够正确渲染', () => {

    cy.visit('/settings')
    cy.viewport(1920, 1080)

    // 移除顶栏，以免阻挡截图
    cy.get('header:contains("NWPU-ACM 查询系统")').invoke('hide')
    
    // eslint-disable-next-line cypress/no-unnecessary-waiting
    cy.wait(2000)
    cy.matchImageSnapshot()
  })
})