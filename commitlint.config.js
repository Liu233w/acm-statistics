module.exports = {
  extends: ['@commitlint/config-conventional'],
  rules: {
    "subject-case": [0],
    "scope-case": [0],
    "header-max-length": [1, "always", 72],
    "body-max-line-length": [0],
  },
}
