<template>
  <div :id="id" />
</template>

<script>
export default {
  props: {
    id: {
      type: String,
      required: true,
    },
    size: {
      type: String,
      required: false,
      default: '728x90',
    },
  },
  mounted() {
    try {
      window._mNHandle.queue.push(function () {
        window._mNDetails.loadTag(this.id, this.size, this.id)
      })
    } catch (error) {
      // ignore
    }
  },
}
</script>
