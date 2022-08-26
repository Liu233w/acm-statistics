const ratelimit = require('koa-ratelimit')

const db = new Map()

module.exports = ratelimit({
  driver: 'memory',
  db: db,
  duration: 3 * 60 * 1000,
  errorMessage: 'Request limit exceed: 30 every 3 minutes',
  max: 30,
})