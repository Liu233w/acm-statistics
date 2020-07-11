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

Cypress.Commands.overwrite('snapshot', (originalFn) => {
  Cypress.$('link[href^="/_nuxt/"]').remove()
  Cypress.$('script[src^="/_nuxt/"]').remove()

  // 移除 data-v- 开头的属性和 data-vue-ssr-id 属性
  Cypress.$('*').each((i, el) => {
    Cypress.$(el).removeAttr('data-vue-ssr-id')
    for (let key in Cypress.$(el).attr()) {
      // eslint-disable-next-line lodash/prefer-lodash-method
      if (key.startsWith('data-v-')) {
        Cypress.$(el).removeAttr(key)
      }
    }
  })

  // 移除 id="input-XXXX" 和 for="input-XXXX" 属性
  Cypress.$('*').each((i, el) => {
    for (let key in Cypress.$(el).attr()) {
      const value = Cypress.$(el).attr(key)
      if (Cypress._.startsWith(value, 'input-') && Cypress._.includes(['for', 'id'], key)) {
        Cypress.$(el).removeAttr(key)
      }
    }
  })

  // remove aria-owns="list-XXX"
  Cypress.$('*').each((i, el) => {
    for (let key in Cypress.$(el).attr()) {
      const value = Cypress.$(el).attr(key)
      if (Cypress._.startsWith(value, 'list-') && key === 'aria-owns') {
        Cypress.$(el).removeAttr(key)
      }
    }
  })

  // 将 css 中的id属性去掉
  Cypress.$('style').each((i, el) => {
    Cypress.$(el).html(Cypress._.replace(Cypress.$(el).html(), /\[data-v-.*?\]/g, ''))
  })

  // 移除随机数
  const storeEl = Cypress.$(Cypress._.filter(Cypress.$('script'), el => /window\.__NUXT__/.test(Cypress.$(el).html())))
  storeEl.html(Cypress._.replace(storeEl.html(), /,key:\.\d*/g, ''))
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

  cy.location('pathname').should('eq', '/')
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

  cy.location('pathname').should('eq', '/')
  cy.log('>>>>> End login')
})