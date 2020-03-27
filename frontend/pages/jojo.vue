<template>
  <VApp light>
    <v-container class="fill-height">
      <v-row
        class="mt-n12"
        justify="center"
      >
        <v-col
          class="text-center"
          cols="auto"
        >
          <v-img
            class="mb-12"
            contain
            height="256"
            :src="require('~/assets/img/dio.jpg')"
          />
          <h1 class="display-2 font-weight-bold mb-6">
            你以为是 {{ url }}，其实是我DIO哒!!
          </h1>
        </v-col>
      </v-row>
    </v-container>
  </VApp>
</template>

<script>
export default {
  computed: {
    url() {
      return this.$nuxt.$route.path
    },
  },
  layout: 'none',
}
</script>
