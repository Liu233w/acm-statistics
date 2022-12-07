const { defineConfig } = require('cypress')

module.exports = defineConfig({
  e2e: {
    baseUrl: 'http://localhost:3000',
    specPattern: 'cypress/integration/**/*.spec.js',
    setupNodeEvents(on, config) {
      require('@simonsmith/cypress-image-snapshot/plugin')
          .addMatchImageSnapshotPlugin(on, config)
      require('cypress-terminal-report/src/installLogsPrinter')(on)
    },
  },

  retries: {
      runMode: 2,
      openMode: 0,
  },
  reporter: '@simonsmith/cypress-image-snapshot/reporter',
  projectId: '4s32o7',
})