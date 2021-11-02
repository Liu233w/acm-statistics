module.exports = {
  root: true,
  env: {
    browser: true,
    node: true,
    'jest/globals': true,
  },
  extends: [
    'eslint:recommended',
    // https://github.com/vuejs/eslint-plugin-vue#priority-a-essential-error-prevention
    // consider switching to `plugin:vue/strongly-recommended` or `plugin:vue/recommended` for stricter rules.
    'plugin:vue/strongly-recommended',
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
    // 行末逗号：在多行中强制最后一项有逗号，单行中强制没有
    'comma-dangle': ['error', 'always-multiline'],
    'quotes': ['error', 'single'],
    // 有 babel-lodash，不需要单独引入也可以 tree-shaking
    'lodash/import-scope': 'off',
    // 这个方法的可读性太低了
    'lodash/matches-prop-shorthand': 'off',
    'lodash/prefer-reject': 'off',
    // 强制使用 PascalCase
    'vue/component-name-in-template-casing': ['error', 'kebab-case', {
      registeredComponentsOnly: false,
    }],
    'vue/multi-word-component-names': 'off',
    // 便于 vuetify 迁移
    'vuetify/no-deprecated-classes': 'error',
    'vuetify/grid-unknown-attributes': 'error',
    'vuetify/no-legacy-grid': 'off',
  },
}
