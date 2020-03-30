<template>
  <v-app light>
    <v-container class="fill-height" fluid v-resize="onResize">
      <v-row align="center" justify="center">
        <v-spacer />
        <v-flex xs3 v-show="showBackground">
          <v-card height="500px" class="background" />
        </v-flex>
        <v-flex md4 sm6 xs12 d-flex class="grey lighten-4">
          <v-spacer />
          <v-container class="text-center">
            <nuxt />
          </v-container>
          <v-spacer />
        </v-flex>
        <v-spacer />
      </v-row>
    </v-container>
  </v-app>
</template>

<style scoped>
.background {
  background-image: url("~assets/img/login/background.jpg");
}
</style>

<script>
export default {
  data() {
    return {
      showBackground: true,
    }
  },
  mounted() {
    this.onResize()
  },
  methods: {
    onResize() {
      const width = window.innerWidth
      if (width < 1264) {
        this.showBackground = false // md
      } else {
        this.showBackground = true // xl
      }
    },
  },
}
</script>
