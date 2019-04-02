Cypress.config('baseUrl', 'http://reverse-proxy')

describe('整体视觉测试', () => {

  beforeEach(() => {
    cy.visit('/about')

    // 隐藏左边侧栏
    cy.get('button i:contains("menu")').click({force: true})
  })

  it('能够正确渲染', () => {
    cy.matchImageSnapshot({
      blackout: [
        ':nth-child(1) > .v-list__tile > .v-list__tile__content > .v-list__tile__sub-title',
        ':nth-child(2) > .v-list__tile > .v-list__tile__content > .v-list__tile__sub-title',
        ':nth-child(3) > .v-list__tile > .v-list__tile__content > .v-list__tile__sub-title',
        ':nth-child(4) > .v-list__tile > .v-list__tile__content > .v-list__tile__sub-title',
      ],
    })
  })
})
