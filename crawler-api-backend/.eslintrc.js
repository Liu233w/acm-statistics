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
    // https://github.com/vuejs/eslint-plugin-vue#priority-a-essential-error-prevention
    // consider switching to `plugin:vue/strongly-recommended` or `plugin:vue/recommended` for stricter rules.
    'plugin:lodash/recommended',
    'plugin:jest/recommended',
  ],
  // required to lint *.vue files
  plugins: [
    'lodash',
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
    // 服务端不用管这个
    'lodash/import-scope': 'off',
    // 这个方法的可读性太低了
    'lodash/matches-prop-shorthand': 'off',
  },
}
