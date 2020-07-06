<template>
  <v-btn
    class="mr-2"
    icon
    @click="back"
  >
    <v-icon>
      mdi-arrow-left-circle
    </v-icon>
  </v-btn>
</template>

<script>
export default {
  methods: {
    back() {
      this.$router.push('/history')
    },
  },
}
</script>
