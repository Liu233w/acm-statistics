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
import html2canvas from 'html2canvas'

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
        const canvas = await html2canvas(summary)

        const link = document.createElement('a')
        link.download = 'summary.png'
        link.href = canvas.toDataURL('image/png')
        link.click()

      } catch (err) {
        this.$store.commit('message/addError', err.message)
      }
      this.loading = false
    },
  },
}
</script>
