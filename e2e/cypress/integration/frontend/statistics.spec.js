Cypress.config('baseUrl', 'http://reverse-proxy')

describe('整体视觉测试', () => {

  beforeEach(() => {
    cy.visit('/statistics')

    // 隐藏左边侧栏
    cy.get('button i:contains("menu")').click({force: true})
  })

  it('能够正确渲染', () => {
    cy.viewport(1920, 1080)
    // eslint-disable-next-line cypress/no-unnecessary-waiting
    cy.wait(1000)
    cy.matchImageSnapshot({
      capture: 'viewport',
    })
  })

  describe('能够在不同尺寸的手持设备下正确渲染', () => {
    [
      'ipad-2',
      'ipad-mini',
      'iphone-6+',
      'iphone-6',
      'iphone-5',
    ].forEach(item => {

      it(item.toString(), () => {
        cy.viewport(item)
        // eslint-disable-next-line cypress/no-unnecessary-waiting
        cy.wait(1000)
        cy.matchImageSnapshot({
          capture: 'viewport',
        })
      })
    })
  })
})

// describe('爬虫测试', () => {
//
//   beforeEach(() => {
//     cy.visit('/statistics')
//     // 隐藏左边侧栏
//     cy.get('button i:contains("menu")').click({force: true})
//     cy.scrollTo(0, 0)
//   })
//
//   it.only('能够启动爬虫', () => {
//
//     cy.get('div[title="POJ"]').parents('.v-sheet').within(() => {
//       cy.matchImageSnapshot()
//       cy.get('input[aria-label="Username"]').type('vjudge5')
//       cy.matchImageSnapshot()
//     })
//
//   })
// })
