Cypress.config('baseUrl', 'http://localhost:3000')

describe('overall', () => {

  beforeEach(() => {
    cy.visit('/statistics')
  })

  it('can render correctly in 1080p', () => {
    cy.viewport(1920, 1080)
    snapshot()
  })

  it('can render correctly', () => {
    snapshot()
  })

})

describe('crawler test', () => {

  beforeEach(() => {
    cy.visit('/statistics')
    // remove top bar to prevent blocking content
    cy.get('header:contains("NWPU-ACM 查询系统")').invoke('hide')
  })

  it('can start a worker', () => {

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

  // TODO: cypress cannot simulate cross-origin error
  it('can request crawler-api-backend when get network error', () => {

    // it seems that xhr does not go through proxy, cannot use proxy server to simulate the response.
    // TODO: use proxy server after cypress update
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

  it('can stop running query', () => {

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

  it('can show crawler errors', () => {

    cy.server()
    cy.route('https://cors-anywhere.herokuapp.com/http://poj.org/userstatus?user_id=Frkfe932fbcv09b',
      'fixture:poj_notExist.txt')
      .as('poj_frontend')

    cy.get('div[title="POJ"]').parents('.worker-item').within(() => {

      cy.get('div:contains("Username") input').type('Frkfe932fbcv09b')
      cy.get('button:contains("refresh")').click()
      cy.wait('@poj_frontend')

      cy.contains('The user does not exist')
      snapshot('worker-error')
    })
  })
})

function snapshot(name) {
  if (name) {
    cy.matchImageSnapshot(name, {
      capture: 'viewport',
    })
  } else {
    cy.matchImageSnapshot({
      capture: 'viewport',
    })
  }
}
