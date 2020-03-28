<template>
  <v-app light>
    <v-container align-content-center>
      <v-layout child-flex class="text-center">
        <v-spacer />
        <v-flex xs3>
          <v-card height="500px" class="background" />
        </v-flex>
        <v-flex xs4 d-flex class="grey lighten-4">
          <v-spacer />
          <v-container class="text-center">
            <nuxt />
          </v-container>
          <v-spacer />
        </v-flex>
        <v-spacer />
      </v-layout>
    </v-container>
  </v-app>
</template>

<style scoped>
  .background {
    background-image: url('~assets/img/login/background.jpg')
  }

  .vertical-center {
  }
</style>
