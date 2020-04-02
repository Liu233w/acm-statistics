Cypress.config('baseUrl', 'http://reverse-proxy')

describe('整体视觉测试', () => {

  beforeEach(() => {
    cy.visit('/statistics')
  })

  it('能够正确渲染', () => {
    cy.viewport(1920, 1080)
    // eslint-disable-next-line cypress/no-unnecessary-waiting
    cy.wait(2000)
    snapshot()
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
        snapshot()
      })
    })
  })
})

describe('爬虫测试', () => {

  beforeEach(() => {
    cy.visit('/statistics')
    // 移除顶栏，以免阻挡截图
    cy.get('header:contains("NWPU-ACM 查询系统")').invoke('hide')
  })

  it('能够启动爬虫', () => {

    cy.server()
    cy.route('https://cors-anywhere.herokuapp.com/http://poj.org/userstatus?user_id=vjudge5',
      'fixture:poj_ok.txt')
      .as('poj_frontend')

    cy.get('div[title="POJ"]').parents('.worker-item').within(() => {

      snapshot('worker-idle')

      cy.get('div:contains("Username") input').type('vjudge5').blur()
      snapshot('worker-typed')

      cy.get('button:contains("refresh")').click()
      cy.wait('@poj_frontend')

      cy.contains('1968')
      cy.contains('277562')
      snapshot('worker-done')
    })
  })

  // TODO: cypress 还没法模拟跨域错误
  it('在前端爬虫错误时可以请求后端爬虫', () => {

    // 看起来xhr还没走proxy，没法跟 proxy server 合到一起
    // TODO: 等 cypress 更新之后尝试使用 proxy server
    cy.server()
    cy.route({
      url: 'https://cors-anywhere.herokuapp.com/http://poj.org/userstatus?user_id=vjudge5',
      status: 500,
      response: 'error',
    }).as('poj_frontend')
    cy.route('/api/crawlers/poj/vjudge5').as('poj_backend')

    cy.mockServer('oj/poj/backend_ok')

    cy.get('div[title="POJ"]').parents('.worker-item').within(() => {
      cy.get('div:contains("Username") input').type('vjudge5')
      cy.get('button:contains("refresh")').click()

      cy.wait('@poj_frontend')
      cy.wait('@poj_backend')
        .its('statusCode').should('be', 200)

      cy.contains('1968')
      cy.contains('277562')
    })
  })

  it('可以停止正在进行的请求', () => {

    cy.server()
    cy.route({
      url: 'https://cors-anywhere.herokuapp.com/http://poj.org/userstatus?user_id=vjudge5',
      response: 'lololo',
      delay: 10000,
    }).as('poj_frontend')

    cy.get('div[title="POJ"]').parents('.worker-item').within(() => {

      cy.get('div:contains("Username") input').type('vjudge5')

      cy.get('button:contains("refresh")').click()

      cy.get('.v-progress-linear')
      snapshot('worker-working')

      cy.get('button:contains("stop")').click()

      snapshot('worker-after-stop')
    })
  })

  it('可以显示异常', () => {

    cy.server()
    cy.route('https://cors-anywhere.herokuapp.com/http://poj.org/userstatus?user_id=Frkfe932fbcv09b',
      'fixture:poj_notExist.txt')
      .as('poj_frontend')

    cy.get('div[title="POJ"]').parents('.worker-item').within(() => {

      cy.get('div:contains("Username") input').type('Frkfe932fbcv09b')
      cy.get('button:contains("refresh")').click()
      cy.wait('@poj_frontend')

      cy.contains('用户不存在')
      snapshot('worker-error')
    })
  })
})

function snapshot(name) {
  if (name) {
    // 让动画播完
    // eslint-disable-next-line cypress/no-unnecessary-waiting
    cy.wait(500)
    cy.matchImageSnapshot(name, {
      capture: 'viewport',
    })
  } else {
    cy.matchImageSnapshot({
      capture: 'viewport',
    })
  }
}
