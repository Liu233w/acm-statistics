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
            :src="randomMeme()"
          />
          <h1 class="text-h1 font-weight-bold mb-6">
            ???
          </h1>

          <p class="mb-12 text-h4 grey--text text--darken-2">
            {{ message }}
          </p>

          <v-btn
            color="primary"
            x-large
            to="/"
          >
            返回主页

            <v-icon right>
              mdi-open-in-new
            </v-icon>
          </v-btn>
        </v-col>
      </v-row>
    </v-container>
  </v-app>
</template>

<script>
import _ from 'lodash'

const memes = [
  '/img/error/cat.jpg',
  '/img/error/metro.jpg',
  '/img/error/man.jpg',
]

export default {
   props: {
    error: {
      type: Object,
      default: null,
    },
  },
  computed: {
    message () {
      return this.error.message || '????????'
    },
  },
  layout: 'none',
  methods: {
    randomMeme() {
      return memes[_.random(memes.length-1)]
    },
  },
}
</script>
