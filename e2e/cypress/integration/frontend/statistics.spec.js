describe('overall', () => {

  beforeEach(() => {
    cy.visit('/statistics')
  })

  it('can render correctly', () => {
    // reset workers to make heights correct
    cy.contains('reset', { matchCase: false }).click()
    snapshot()
  })

})

describe('crawler test', () => {

  beforeEach(() => {
    cy.visit('/statistics')
  })

  it('can start a worker', () => {

    cy.intercept(
      'https://acm-statistics-cors.herokuapp.com/http://poj.org/userstatus?user_id=vjudge5',
      { fixture: 'poj_ok.txt' })
      .as('poj_frontend')

    cy.get('div[title="POJ"]').parents('.worker').within(() => {

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

  it('can request crawler-api-backend when get network error', () => {

    cy.intercept(
      'https://acm-statistics-cors.herokuapp.com/http://poj.org/userstatus?user_id=vjudge5',
      { forceNetworkError: true }).as('poj_frontend')
    cy.intercept('/api/crawlers/poj/vjudge5').as('poj_backend')

    cy.mockServer('oj/poj/backend_ok')

    cy.get('div[title="POJ"]').parents('.worker').within(() => {
      cy.get('div:contains("Username") input').type('vjudge5')
      cy.get('button:contains("refresh")').click()

      cy.wait('@poj_frontend')
      cy.wait('@poj_backend')
        .its('response.statusCode').should('eq', 200)

      cy.contains('1968')
      cy.contains('277562')
    })
  })

  it('can stop running query', () => {

    cy.intercept(
      'https://acm-statistics-cors.herokuapp.com/http://poj.org/userstatus?user_id=vjudge5',
      { body: 'lololo', delayMs: 10000 },
    ).as('poj_frontend')

    cy.get('div[title="POJ"]').parents('.worker').within(() => {

      cy.get('div:contains("Username") input').type('vjudge5')

      cy.get('button:contains("refresh")').click()

      cy.get('.v-progress-linear')
      snapshot('worker-working')

      cy.get('button:contains("stop")').click()

      snapshot('worker-after-stop')
    })
  })

  it('can show crawler errors', () => {

    cy.intercept('https://acm-statistics-cors.herokuapp.com/http://poj.org/userstatus?user_id=Frkfe932fbcv09b',
      { fixture: 'poj_notExist.txt' })
      .as('poj_frontend')

    cy.get('div[title="POJ"]').parents('.worker').within(() => {

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
