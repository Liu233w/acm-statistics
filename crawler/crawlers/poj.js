const request = require('superagent')
const cheerio = require('cheerio')

module.exports = function(config, username) {
    return new Promise((resolve, reject) => {
        request
        .get('http://poj.org/userstatus')
        .query({user_id: username})
        .end((err, res) => {
            if (err) {
                reject(err)
            } else if (!res.ok) {
                reject(new Error(`Server Response Error: ${res.status}`))
            } else {
                const $ = cheerio.load(res.text)
                resolve({
                    solved: new Number($('a[href^="status?result=0&user_id="]').text()),
                    submissions: new Number($('a[href^="status?user_id="]').text())
                })
            }
        })
    })
}