module.exports = {
  root: true,
  env: {
    browser: false,
    node: true,
    es6: true,
    "cypress/globals": true,
  },
  extends: [
    'eslint:recommended',
    "plugin:cypress/recommended",
  ],
  // required to lint *.vue files
  plugins: [
    'cypress',
  ],
  // add your custom rules here
  rules: {
    'semi': [2, 'never'],
    'no-console': 'off',
    // 行末逗号：在多行中强制最后一项有逗号，单行中强制没有
    'comma-dangle': ['error', 'always-multiline'],
    'quotes': ['error', 'single'],
  },
}
