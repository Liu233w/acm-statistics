// trying

const superagent = require('superagent')

addEventListener('fetch', event => {
  event.respondWith(handleRequest(event.request))
})

/**
 * Respond with hello worker text
 * @param {Request} request
 */
async function handleRequest(request) {

  const res = await superagent.get('https://google.com')
  return new Response(res.text, {
    headers: { 'content-type': 'text/html' },
  })
}
