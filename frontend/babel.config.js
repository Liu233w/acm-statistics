module.exports = api => {
  const isTest = api.env('test')
  if (!isTest) {
    throw new Error('the config is only for jest test')
  }

  return {
    plugins: [
      '@babel/plugin-transform-runtime',
    ],
    presets: [['@babel/preset-env', { modules: false }]],
    env: {
      test: {
        presets: [['@babel/preset-env', { targets: { node: 'current' } }]],
      },
    },
  }
}
