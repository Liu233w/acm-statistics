export default function () {
  if (process.client) {
    window._mNHandle = window._mNHandle || {}
    window._mNHandle.queue = window._mNHandle.queue || []
    window.medianet_versionId = '3121199'
  }
}
