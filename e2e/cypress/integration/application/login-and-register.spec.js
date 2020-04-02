Cypress.config('baseUrl', 'http://localhost:3000')

describe('/login', () => {

  it('能检测出登录错误', () => {
    cy.visit('/login')
    cy.contains('用户名').parent().type('admin')
    cy.contains('密码').parent().type('wrong-password')
    cy.get('button').contains('登录').click()
    cy.contains('用户名或密码无效', { timeout: 60000 })
    // 必须等待一下，否则有时候会显示不出来错误
    // eslint-disable-next-line cypress/no-unnecessary-waiting
    cy.wait(1000)
    cy.get('.text-center').matchImageSnapshot('login-failed')
  })

  it('登录成功流程', () => {
    cy.visit('/login')
    cy.contains('用户名').parent().type('admin')
    cy.contains('密码').parent().type('123qwe')
    cy.get('button').contains('登录').click()

    cy.contains('admin', { timeout: 60000 })
    cy.location('pathname').should('eq', '/')
  })

  it('能够保持用户名', () => {
    cy.visit('/login')
    cy.contains('用户名').parent().type('admin')
    cy.contains('密码').parent().type('123qwe')
    cy.contains('保持登录').click()
    cy.get('button').contains('登录').click()

    cy.contains('admin', { timeout: 60000 })
    cy.location('pathname').should('eq', '/')

    cy.log('在刷新页面之后用户名还在')
    cy.reload()
    cy.contains('admin')
  })
})

describe('/register', () => {
  it('能够显示验证码错误', () => {
    cy.visit('/register')
    cy.contains('用户名').parent().type('user')
    cy.contains('密码').parent().type('123qwe')
    cy.contains('再次输入密码').parent().type('123qwe')
    cy.contains('验证码').parent().type('wrong')
    cy.get('button').contains('注册').click()
    cy.contains('验证码不正确', { timeout: 60000 })
    // 必须等待一下，否则有时候会显示不出来错误
    // eslint-disable-next-line cypress/no-unnecessary-waiting
    cy.wait(1000)
    cy.get('.text-center').matchImageSnapshot('register-failed')
  })
})

describe('注册登录流程', () => {

  const newUsername = 'user' + ('' + Math.random()).split('.')[1]

  it('从主页开始注册新用户', () => {
    cy.log('从主页登录')
    cy.visit('/')
    cy.contains('登录').click()
    cy.location('pathname').should('eq', '/login')

    cy.log('进入注册页面')
    cy.contains('去注册').click()
    cy.location('pathname').should('eq', '/register')

    cy.log('填写注册信息')
    // 先等待验证码（刷新dom）
    cy.contains('验证码').parent().type('validate-text')
    cy.contains('用户名').parent().type(newUsername)
    cy.contains('密码').parent().type('123qwe')
    cy.contains('再次输入密码').parent().type('123qwe')
    cy.get('button').contains('注册').click()
    cy.location('pathname').should('eq', '/')

    cy.log('回到主页')
    cy.contains(newUsername)
  })

  it('从主页开始登录并注销', () => {
    cy.log('从主页登录')
    cy.visit('/')
    cy.contains('登录').click()
    cy.location('pathname').should('eq', '/login')

    cy.log('输入用户名和密码')
    cy.contains('用户名').parent().type(newUsername)
    cy.contains('密码').parent().type('123qwe')
    cy.get('button').contains('登录').click()
    cy.location('pathname').should('eq', '/')

    cy.log('注销')
    
    cy.contains(newUsername).click()
    cy.contains('注销')
    cy.location('pathname').should('eq', '/settings')

    cy.contains('注销').click()

    cy.contains('登录', { timeout: 60000 })
    cy.location('pathname').should('eq', '/')

    cy.log('在刷新页面之后用户名不会重新出现')
    cy.reload()
    cy.contains('登录')
  })
})