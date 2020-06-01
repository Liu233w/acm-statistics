// add Google Adsense

export default function ({ app }) {
  if (process.client) {
    app.router.afterEach(() => {
      const script = document.createElement('script')
      script.setAttribute('src', 'https://pagead2.googlesyndication.com/pagead/js/adsbygoogle.js')
      script.setAttribute('data-ad-client', 'ca-pub-9846042020379030')
      script.async = true
      script.onload = () => {
        // remove tag after loaded
        document.body.removeChild(script)
      }
      document.body.appendChild(script)
    })
  }
}
