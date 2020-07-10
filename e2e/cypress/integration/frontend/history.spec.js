Cypress.config('baseUrl', 'http://localhost:3000')

let summaryUrl
let username

before(() => {
  cy.registerAndGetUsername().then(u => {
    username = u
  })

  cy.log('save a history')

  cy.server()
  cy.route('https://cors-anywhere.herokuapp.com/http://acm.hdu.edu.cn/userstatus.php?user=wwwlsmcom',
    'fixture:summary_hdu.txt')
    .as('summary_hdu')
  cy.route('/api/crawlers/vjudge/wwwlsmcom',
    'fixture:summary_vjudge.txt')
    .as('summary_vjudge')
  cy.route('post', 'https://cors-anywhere.herokuapp.com/https://leetcode-cn.com/graphql',
    'fixture:summary_leetcode.txt')
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

  // wait for page loading
  cy.contains('export image', { matchCase: false })

  cy.location('pathname')
    .then(s => {
      summaryUrl = s
      console.log('url', s)
      return s.startsWith('/history/')
    })
    .should('be.true')

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
      .invoke('attr', 'style', 'background-color: black')
    // hide account username
    cy.get(`button:contains("${username}")`)
      .invoke('attr', 'style', 'background-color: black')

    cy.matchImageSnapshot()
  })
})

describe('history page', () => {

  it('should render correctly', () => {
    cy.login(username)
    cy.visit('/history')

    // hide id
    cy.get('tbody td.text-start:nth-child(2)')
      .invoke('attr', 'style', 'background-color: black')
    // hide generate time
    cy.get('tbody td.text-start:nth-child(3)')
      .invoke('attr', 'style', 'background-color: black')
    // hide account username
    cy.get(`button:contains("${username}")`)
      .invoke('attr', 'style', 'background-color: black')

    cy.matchImageSnapshot()
  })

  describe('when click view', () => {
    it('should go to summary page', () => {

      cy.login(username)
      cy.visit('/history')

      cy.get('i.mdi-eye').parents('a').click()

      // wait for page loading
      cy.contains('export image', { matchCase: false })

      cy.location('pathname').should('eq', summaryUrl)
    })
  })

  describe('when click delete', () => {

    before(() => {
      cy.server()
      cy.route('/api/services/app/QueryHistory/GetQueryHistoriesAndSummaries')
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

      cy.server()
      cy.route('/api/services/app/QueryHistory/GetQueryHistoriesAndSummaries',
        'fixture:history_list.json')
        .as('get-list')

      cy.get('i.mdi-menu').parents('button').click()
      cy.contains('history', { matchCase: false }).click()
      cy.wait('@get-list')
    })

    it('should render correctly', () => {
      // hide account username
      cy.get(`button:contains("${username}")`)
        .invoke('attr', 'style', 'background-color: black')

      cy.matchImageSnapshot()
    })

    it('can delete multiple items correctly', () => {

      for (let i = 1; i <= 4; ++i) {
        cy.get(`tbody tr:nth-child(${i}) td:nth-child(1) > div`).click()
      }

      // hide account username
      cy.get(`button:contains("${username}")`)
        .invoke('attr', 'style', 'background-color: black')
      cy.matchImageSnapshot()

      cy.route('POST', '/api/services/app/QueryHistory/DeleteQueryHistory',
        {})
        .as('delete')

      cy.contains('delete selected', { matchCase: false }).click()

      cy.wait('@delete')
      cy.get('@delete').its('request.body').should('deep.equal', {
        ids: [7, 8, 9, 10],
      })
    })
  })

})