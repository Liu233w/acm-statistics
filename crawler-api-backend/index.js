const app = require('./app')

// 将 req.ip 设置为 X-Forward-For 中的 ip
app.proxy = true

console.log('start listening on localhost:12001 ...')

app.listen(12001)