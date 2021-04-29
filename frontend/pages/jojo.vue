<template>
  <v-app light>
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
          <h1 class="text-h3 font-weight-bold mb-6">
            你访问的不是 {{ url }}，是我DIO哒!!
          </h1>
        </v-col>
      </v-row>
    </v-container>
  </v-app>
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
