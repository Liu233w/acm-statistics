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
      'https://cors.ojhunt.com/http://poj.org/userstatus?user_id=vjudge5',
      { fixture: 'poj_ok.txt' })
      .as('poj_frontend')

    cy.get('div[title="POJ"]').parents('.worker').within(() => {

      snapshot('worker-idle')

      cy.get('div:contains("Username") input').type('vjudge5')
      cy.get('div:contains("Username") input').blur()
      snapshot('worker-typed')

      waitAndRefresh()
      cy.wait('@poj_frontend')

      cy.contains('1968')
      cy.contains('277562')
      snapshot('worker-done')
    })
  })

  it('can request crawler-api-backend when get network error', () => {

    cy.intercept(
      'https://cors.ojhunt.com/http://poj.org/userstatus?user_id=vjudge5',
      { forceNetworkError: true }).as('poj_frontend')
    cy.intercept('/api/crawlers/poj/vjudge5').as('poj_backend')

    cy.mockServer('oj/poj/backend_ok')

    cy.get('div[title="POJ"]').parents('.worker').within(() => {
      cy.get('div:contains("Username") input').type('vjudge5')
      waitAndRefresh()

      cy.wait('@poj_frontend')
      cy.wait('@poj_backend')
        .its('response.statusCode').should('eq', 200)

      cy.contains('1968')
      cy.contains('277562')
    })
  })

  it('can stop running query', () => {

    cy.intercept(
      'https://cors.ojhunt.com/http://poj.org/userstatus?user_id=vjudge5',
      { body: 'lololo', delayMs: 10000 },
    ).as('poj_frontend')

    cy.get('div[title="POJ"]').parents('.worker').within(() => {

      cy.get('div:contains("Username") input').type('vjudge5')
      waitAndRefresh()

      cy.get('.v-progress-linear')
      snapshot('worker-working')

      cy.get('button i.mdi-stop').click()

      snapshot('worker-after-stop')
    })
  })

  it('can show crawler errors', () => {

    cy.intercept('https://cors.ojhunt.com/http://poj.org/userstatus?user_id=Frkfe932fbcv09b',
      { fixture: 'poj_notExist.txt' })
      .as('poj_frontend')

    cy.get('div[title="POJ"]').parents('.worker').within(() => {

      cy.get('div:contains("Username") input').type('Frkfe932fbcv09b')
      waitAndRefresh()
      cy.wait('@poj_frontend')

      cy.contains('The user does not exist')
      snapshot('worker-error')
    })
  })

  it('can show crawler warnings', () => {
    cy.get('div[title="POJ"]').parents('.worker').within(() => {

      cy.get('div:contains("Username") input').type(' name with space')
      cy.get('div:contains("Username") input').blur()

      cy.contains('Your username begins with a space.')
      cy.contains('Your username includes space, which may not be supported by some crawlers.')
      snapshot('worker-warning')
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

function waitAndRefresh() {
  // wait for debounce to be executed
  // eslint-disable-next-line cypress/no-unnecessary-waiting
  cy.wait(500)
  cy.get('button i.mdi-refresh').click()
}
