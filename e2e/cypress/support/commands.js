// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add("login", (email, password) => { ... })
//
//
// -- This is a child command --
// Cypress.Commands.add("drag", { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add("dismiss", { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This is will overwrite an existing command --
// Cypress.Commands.overwrite("visit", (originalFn, url, options) => { ... })

/*
 * 使用 cy.mockServer('busuanzi') 或者 cy.mockServer('oj/poj') 这样的命令来进行
 */
Cypress.Commands.add('mockServer', path => {
  cy.request(`http://mock-configurer/${path}`)
})

Cypress.Commands.overwrite('matchImageSnapshot', (originalFn, maybeName, commandOptions) => {
  let waitTimeperiod = 1000
  if (!maybeName) {
    waitTimeperiod = 2000
  }
  // eslint-disable-next-line cypress/no-unnecessary-waiting
  cy.wait(waitTimeperiod)
  return originalFn(maybeName, commandOptions)
})

Cypress.Commands.add('registerAndGetUsername', () => {

  cy.log('<<<<< 开始注册流程')

  const username = 'user' + ('' + Math.random()).slice(2, 8).padEnd(6, '0')

  cy.visit('/register')
  cy.contains('验证码').parent().type('validate-text')
  cy.contains('用户名').parent().type(username)
  cy.contains('密码').parent().type('123qwe')
  cy.contains('再次输入密码').parent().type('123qwe')
  cy.get('button').contains('注册').click()

  cy.location('pathname').should('eq', '/')
  cy.log('>>>>> 结束注册流程')

  return cy.wrap(username)
})

Cypress.Commands.add('login', username => {
  cy.log('<<<<< 开始登录流程')
  cy.visit('/login')
  cy.contains('用户名').parent().type(username)
  cy.contains('密码').parent().type('123qwe')
  cy.get('button').contains('登录').click()

  cy.location('pathname').should('eq', '/')
  cy.log('>>>>> 结束登录流程')
})