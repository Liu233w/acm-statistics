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
  // eslint-disable-next-line cypress/no-unnecessary-waiting
  cy.wait(1000)
  return originalFn(maybeName, commandOptions)
})
