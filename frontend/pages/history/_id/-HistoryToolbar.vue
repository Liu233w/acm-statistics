<template>
  <v-tooltip bottom>
    <template #activator="{ on }">
      <v-btn
        text
        v-on="on"
        @click="printPage"
        :loading="loading"
      >
        Export image
      </v-btn>
    </template>
    Export the summary report as an image
  </v-tooltip>
</template>

<script>
import htmlToImage from 'html-to-image'

export default {
  data() {
    return {
      loading: false,
    }
  },
  methods: {
    async printPage() {
      this.loading = true
      try {
        const summary = document.getElementById('history-summary')
        const link = document.createElement('a')
        link.download = 'summary.jpg'
        link.href = await htmlToImage.toJpeg(summary, {
          quality: 0.8,
          backgroundColor: 'white',
        })
        link.click()
      } catch (err) {
        this.$store.commit('message/addError', err.message)
      }
      this.loading = false
    },
  },
}
</script>
