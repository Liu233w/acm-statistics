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

  cy.log('<<<<< Start register')

  const username = 'user' + ('' + Math.random()).slice(2, 8).padEnd(6, '0')

  cy.visit('/register')
  cy.contains('Captcha').parent().type('validate-text')
  cy.contains('Username').parent().type(username)
  cy.contains('Password').parent().type('1234Qwer')
  cy.contains('Confirm password').parent().type('1234Qwer')
  cy.get('button').contains('register').click()

  cy.shouldHaveUri('/')
  cy.log('>>>>> End register')

  return cy.wrap(username)
})

Cypress.Commands.add('login', (username, password) => {
  if (!password) {
    if (username === 'admin') {
      password = '123qwe'
    } else {
      password = '1234Qwer'
    }
  }
  cy.log('<<<<< Start login')
  cy.visit('/login')
  cy.contains('Username').parent().type(username)
  cy.contains('Password').parent().type(password)
  cy.get('button').contains('login').click()

  cy.shouldHaveUri('/')
  cy.log('>>>>> End login')
})

Cypress.Commands.add('shouldHaveUri', (uri, config = {}) => {
  const timeout = config.timeout || 30000

  return cy.url({ timeout: timeout })
    .should('eq', Cypress.config().baseUrl + uri)
})