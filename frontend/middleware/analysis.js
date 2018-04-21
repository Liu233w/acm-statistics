/* 本文件用于在每个页面添加新的网站分析的 script，这些 script 必须在每次路由切换的时候被添加 */

function addScriptTag(src) {
  return new Promise(resolve => {
    const script = document.createElement('script')
    script.setAttribute('src', src)
    script.async = true
    script.onload = () => {
      // 在加载完成后移除 script 标签。
      document.body.removeChild(script)
      resolve()
    }
    document.body.appendChild(script)
  })
}

export default async function () {
  if (process.client) {
    await addScriptTag('//tajs.qq.com/stats?sId=65546290')
    await addScriptTag('//dn-lbstatics.qbox.me/busuanzi/2.3/busuanzi.pure.mini.js')
  }
}
