let summaryUrl
let username

before(() => {
  cy.registerAndGetUsername().then(u => {
    username = u
  })

  cy.log('save a history')

  cy.intercept(
    'https://acm-statistics-cors.herokuapp.com/http://acm.hdu.edu.cn/userstatus.php?user=wwwlsmcom',
    { fixture: 'summary_hdu.txt' })
    .as('summary_hdu')
  cy.intercept('/api/crawlers/vjudge/wwwlsmcom',
    { fixture: 'summary_vjudge.txt' })
    .as('summary_vjudge')
  cy.intercept('post', 'https://acm-statistics-cors.herokuapp.com/https://leetcode-cn.com/graphql',
    { fixture: 'summary_leetcode.txt' })
    .as('summary_leetcode')

  cy.visit('/statistics')

  cy.get('div[title="HDU"]').parents('.worker').within(() => {

    cy.get('div:contains("Username") input').type('wwwlsmcom')

    cy.get('button:contains("refresh")').click()
    cy.wait('@summary_hdu')

    cy.contains('34')
    cy.contains('72')
  })

  cy.get('div[title="LeetCode_CN"]').parents('.worker').within(() => {

    cy.get('div:contains("Username") input').type('wwwlsmcom')

    cy.get('button:contains("refresh")').click()
    cy.wait('@summary_leetcode')

    cy.contains('2')
    cy.contains('4')
  })

  cy.get('div[title="VJudge"]').parents('.worker').within(() => {

    cy.get('div:contains("Username") input').type('wwwlsmcom')

    cy.get('button:contains("refresh")').click()
    cy.wait('@summary_vjudge')

    cy.contains('161')
    cy.contains('704')
  })

  cy.contains('SOLVED: 192 / SUBMISSION', { matchCase: false }).click()

  cy.url().should('contain', '/history/')
    .then(s => {
      summaryUrl = s
      console.log('url', s)
    })

  cy.clearCookies()
})

describe('summary page', () => {

  it('should render correctly', () => {
    cy.login(username)
    cy.visit(summaryUrl)

    // hide generate time
    cy.get('strong:contains("Generated at")')
      .parent()
      .invoke('attr', 'style', 'background-color: black')
    cy.get('.v-toolbar__title')
      .invoke('text', '[GENERATED DATE]')
    // hide account username
    cy.get(`button:contains("${username}")`)
      .invoke('attr', 'style', 'background-color: black')

    cy.matchImageSnapshot()
  })

  it('should be able to sort the list', () => {
    cy.login(username)
    cy.visit(summaryUrl)

    // hide generate time
    cy.get('strong:contains("Generated at")')
      .parent()
      .invoke('attr', 'style', 'background-color: black')
    cy.get('.v-toolbar__title')
      .invoke('text', '[GENERATED DATE]')
    // hide account username
    cy.get(`button:contains("${username}")`)
      .invoke('attr', 'style', 'background-color: black')

    cy.contains('Solved').click()

    cy.matchImageSnapshot()
  })
})

describe('history page', () => {

  it('should show history correctly', () => {
    // it already has one record
    cy.login(username)
    cy.visit('/history')

    cy.get('tbody tr').within(() => {
      cy.contains('192')
      cy.contains('780')
    })
  })

  describe('when click view', () => {
    it('should go to summary page', () => {

      cy.login(username)
      cy.visit('/history')

      cy.get('i.mdi-eye').parents('a').click()

      cy.url().should('eq', summaryUrl)
    })
  })

  describe('when click delete', () => {

    before(() => {
      cy.intercept('/api/services/app/QueryHistory/GetQueryHistoriesAndSummaries?maxResultCount=10&skipCount=0')
        .as('get-list')
    })

    it('should delete item', () => {

      cy.login(username)
      cy.visit('/history')

      cy.get('i.mdi-delete').parents('button').click()

      cy.wait('@get-list')

      cy.contains('no data available', { matchCase: false })
    })

  })

  describe('for history with multiple pages', () => {

    beforeEach(() => {
      cy.login(username)
      // go page from other pages to prevent ssr
      cy.visit('/about')

      cy.intercept(
        '/api/services/app/QueryHistory/GetQueryHistoriesAndSummaries?maxResultCount=10&skipCount=0',
        { fixture: 'history_list.json' })
        .as('get-list')
      cy.intercept(
        '/api/services/app/QueryHistory/GetQueryHistoriesAndSummaries?maxResultCount=10&skipCount=10',
        { fixture: 'history_list-skip10.json' })
        .as('get-list-2')
      cy.intercept(
        '/api/services/app/QueryHistory/GetQueryHistoriesAndSummaries?maxResultCount=5&skipCount=0',
        { fixture: 'history_list-max5.json' })
        .as('get-list-3')

      cy.get('i.mdi-menu').parents('button').click()
      cy.contains('history', { matchCase: false }).click()
      cy.wait('@get-list')

      // hide account username
      cy.get(`button:contains("${username}")`)
        .invoke('attr', 'style', 'background-color: black')
    })

    it('should render correctly', () => {
      cy.matchImageSnapshot()
    })

    it('can go to next page', () => {
      cy.get('i.mdi-chevron-right').parents('button').click()
      cy.wait('@get-list-2')
      cy.get('table').matchImageSnapshot()
    })

    it('can set page size', () => {
      cy.get('div[aria-haspopup="listbox"]').click()
      cy.get('div[role="listbox"]')
        .contains('5')
        .parents('div[role="option"]')
        .click()
      cy.wait('@get-list-3')
      cy.get('table').matchImageSnapshot()
    })

    it('can delete multiple items correctly', () => {

      for (let i = 1; i <= 4; ++i) {
        cy.get(`tbody tr:nth-child(${i}) td:nth-child(1) > div`).click()
      }

      cy.matchImageSnapshot()

      cy.intercept('POST', '/api/services/app/QueryHistory/DeleteQueryHistory',
        req => req.reply({ success: true }))
        .as('delete')

      cy.contains('delete selected', { matchCase: false }).click()

      cy.wait('@delete')
      cy.get('@delete').its('request.body').should('deep.equal', {
        ids: [7, 8, 9, 10],
      })
    })
  })

})