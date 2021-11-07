const ratelimit = require('koa-ratelimit')

const db = new Map()

module.exports = ratelimit({
  driver: 'memory',
  db: db,
  duration: 60000,
  errorMessage: 'Request limit exceed: 10 per minute',
  max: 10,
})