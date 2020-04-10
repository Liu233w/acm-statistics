Cypress.config('baseUrl', 'http://localhost:3000')

describe('未登录时将用户常用查询保存到Cookie', () => {

  it('能够正常运作', () => {
    // arrange
    cy.visit('/statistics')

    cy.get('.v-input:contains("统一设置用户名") input').should('have.value', '')
    cy.get('.worker-item:contains("CodeForces") input').should('have.value', '')

    cy.get('.v-input:contains("统一设置用户名") input').type('main-name')
    // 等待其他worker更新
    cy.get('.worker-item:contains("CodeForces") input').should('have.value', 'main-name')
    cy.get('.worker-item:contains("HDU") input').clear().type('user1')

    // act
    cy.contains('保存用户名').click()
    cy.contains('保存成功')

    cy.reload()
    cy.get('.worker-item:contains("HDU") input').should('have.value', 'user1')
    cy.get('.v-input:contains("统一设置用户名") input').should('have.value', 'main-name')

    cy.matchImageSnapshot()
  })
})

describe('登录时将用户常用查询保存到服务器', () => {

  let username
  before(() => {
    cy.registerAndGetUsername().then(u => {
      username = u
    })
    cy.clearCookies()
  })

  it('能够保存查询', () => {
    cy.login(username)

    cy.visit('/statistics')

    cy.get('.v-input:contains("统一设置用户名") input').should('have.value', '')
    cy.get('.worker-item:contains("CodeForces") input').should('have.value', '')

    cy.get('.v-input:contains("统一设置用户名") input').type('main-name')
    // 等待其他worker更新
    cy.get('.worker-item:contains("CodeForces") input').should('have.value', 'main-name')
    cy.get('.worker-item:contains("HDU") input').clear().type('user1')

    cy.contains('保存用户名').click()
    cy.contains('保存成功')

    // 隐藏用户名
    cy.get(`button:contains("${username}")`)
      .invoke('attr', 'style', 'background-color: black')
    cy.matchImageSnapshot()
  })

  it('能够获取已保存的查询', () => {
    cy.login(username)

    cy.visit('/statistics')
    cy.get('.worker-item:contains("HDU") input').should('have.value', 'user1')
    cy.get('.v-input:contains("统一设置用户名") input').should('have.value', 'main-name')

    cy.get('.worker-item:contains("CodeForces") input').should('have.value', 'main-name')

    // 隐藏用户名
    cy.get(`button:contains("${username}")`)
      .invoke('attr', 'style', 'background-color: black')
    cy.matchImageSnapshot()
  })
})