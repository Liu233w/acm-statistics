Cypress.config('baseUrl', 'http://localhost:3000')

describe('/login', () => {

  it('can show login error', () => {
    cy.visit('/login')
    cy.contains('Username').parent().type('admin')
    cy.contains('Password').parent().type('wrong-password')
    cy.get('button').contains('login').click()
    cy.contains('Invalid user name or password', { timeout: 60000 })
    cy.get('.text-center').matchImageSnapshot('login-failed')
  })

  it('can successfully login', () => {
    cy.visit('/login')
    cy.contains('Username').parent().type('admin')
    cy.contains('Password').parent().type('123qwe')
    cy.get('button').contains('login').click()

    cy.contains('admin', { timeout: 60000 })
    cy.location('pathname').should('eq', '/')
  })

  it('can keep user session', () => {
    cy.visit('/login')
    cy.contains('Username').parent().type('admin')
    cy.contains('Password').parent().type('123qwe')
    cy.contains('Remember me').click()
    cy.get('button').contains('login').click()

    cy.contains('admin', { timeout: 60000 })
    cy.location('pathname').should('eq', '/')

    cy.log('Username still exists after refresh page')
    cy.reload()
    cy.contains('admin')
  })
})

describe('/register', () => {
  it('can show captcha error', () => {
    cy.visit('/register')
    cy.contains('Username').parent().type('user')
    cy.contains('Password').parent().type('1234Qwer')
    cy.contains('Confirm password').parent().type('1234Qwer')
    cy.contains('Captcha').parent().type('wrong')
    cy.get('button').contains('register').click()
    cy.contains('Incorrect captcha', { timeout: 60000 })
    cy.get('.text-center').matchImageSnapshot('register-failed')
  })
})

describe('Register then login', () => {

  const newUsername = 'user' + ('' + Math.random()).split('.')[1]

  it('can register new user starting from homepage', () => {
    cy.log('visit login page from homepage')
    cy.visit('/')
    cy.contains('login').click()
    cy.location('pathname').should('eq', '/login')

    cy.log('visit register page')
    cy.contains('enter register page').click()
    cy.location('pathname').should('eq', '/register')

    cy.log('fill user information')
    // wait dom to refresh (wait captcha)
    cy.contains('Captcha').parent().type('validate-text')
    cy.contains('Username').parent().type(newUsername)
    cy.contains('Password').parent().type('1234Qwer')
    cy.contains('Confirm password').parent().type('1234Qwer')
    cy.get('button').contains('register').click()
    cy.location('pathname').should('eq', '/')

    cy.log('should back to homepage')
    cy.contains(newUsername)
  })

  it('login from homepage then logout', () => {
    cy.log('login from homepage')
    cy.visit('/')
    cy.contains('login').click()
    cy.location('pathname').should('eq', '/login')

    cy.log('enter username and password')
    cy.contains('Username').parent().type(newUsername)
    cy.contains('Password').parent().type('1234Qwer')
    cy.get('button').contains('login').click()
    cy.location('pathname').should('eq', '/')

    cy.log('logout')

    cy.contains(newUsername).click()
    cy.contains('Logout')
    cy.location('pathname').should('eq', '/settings')

    cy.contains('Logout').click()

    cy.contains('login', { timeout: 60000 })
    cy.location('pathname').should('eq', '/')

    cy.log('username does not exist after refresh page')
    cy.reload()
    cy.contains('login')
  })
})