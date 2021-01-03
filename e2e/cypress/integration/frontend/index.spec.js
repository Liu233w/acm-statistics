describe('overall', () => {

  beforeEach(() => {
    cy.visit('/')

    // remove parallax for snapshot
    cy.document().then(document => {
      document.querySelectorAll('.v-parallax__image')
        .forEach(item => item.setAttribute('style', null))
    })
  })

  it('can render correctly', () => {
    cy.viewport(1920, 1080)
    cy.matchImageSnapshot()
  })

})

describe('parallax test', () => {

  beforeEach(() => {
    cy.visit('/')
  })

  describe('first parallax', () => {
    it('can render correctly', () => {
      cy.get('.v-parallax').eq(0).matchImageSnapshot()
    })
  })

  describe('second parallax', () => {
    it('can render correctly', () => {
      cy.get('.v-parallax').eq(1).matchImageSnapshot()
    })
  })
})

describe('other parts', () => {

  beforeEach(() => {
    cy.visit('/')
  })

  it('click ENTER AC STATISTICS button to enter', () => {
    cy.contains('Enter AC Statistics').click()
    cy.shouldHaveUri('/statistics')
  })
})
