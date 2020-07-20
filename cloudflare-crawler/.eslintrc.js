module.exports = {
  root: true,
  env: {
    browser: false,
    node: true,
    es6: true,
    'jest/globals': true,
  },
  parserOptions: {
    parser: 'babel-eslint',
    ecmaVersion: 8,
  },
  extends: [
    'eslint:recommended',
    'plugin:jest/recommended',
  ],
  // required to lint *.vue files
  plugins: [
    'jest',
  ],
  // add your custom rules here
  rules: {
    'semi': [2, 'never'],
    'no-console': 'off',
    'vue/max-attributes-per-line': 'off',
    // 行末逗号：在多行中强制最后一项有逗号，单行中强制没有
    'comma-dangle': ['error', 'always-multiline'],
    'quotes': ['error', 'single'],
  },
}
