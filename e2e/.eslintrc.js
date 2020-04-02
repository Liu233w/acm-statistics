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
    'plugin:lodash/recommended',
  ],
  // required to lint *.vue files
  plugins: [
    'cypress',
    'lodash',
  ],
  // add your custom rules here
  rules: {
    'semi': [2, 'never'],
    'no-console': 'off',
    // 行末逗号：在多行中强制最后一项有逗号，单行中强制没有
    'comma-dangle': ['error', 'always-multiline'],
    'quotes': ['error', 'single'],
    // 测试端不需要 tree-shaking
    'lodash/import-scope': 'off',
    // 这个方法的可读性太低了
    'lodash/matches-prop-shorthand': 'off',
    'lodash/prefer-reject': 'off',
    // cypress 里用不了这个
    'lodash/prefer-lodash-method': 'off',
  },
}
