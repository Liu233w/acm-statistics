module.exports = {
  root: true,
  env: {
    browser: true,
    node: true,
    'jest/globals': true,
  },
  extends: [
    'eslint:recommended',
    'plugin:vue/vue3-strongly-recommended',
    'plugin:lodash/recommended',
    'plugin:jest/recommended',
  ],
  // required to lint *.vue files
  plugins: [
    'vue',
    'lodash',
    'jest',
    'vuetify',
  ],
  // add your custom rules here
  rules: {
    'semi': [2, 'never'],
    'no-console': 'off',
    'vue/max-attributes-per-line': 'off',
    'comma-dangle': ['error', 'always-multiline'],
    'quotes': ['error', 'single'],
    // we don't need to do that since we have babel lodash
    'lodash/import-scope': 'off',
    // low readability
    'lodash/matches-prop-shorthand': 'off',
    'lodash/prefer-reject': 'off',
    // force PascalCase
    'vue/component-name-in-template-casing': ['error', 'kebab-case', {
      registeredComponentsOnly: false,
    }],
    'vue/multi-word-component-names': 'off',
    // for vuetify migration
    'vuetify/no-deprecated-classes': 'error',
    'vuetify/grid-unknown-attributes': 'error',
    'vuetify/no-legacy-grid': 'off',
  },
}
