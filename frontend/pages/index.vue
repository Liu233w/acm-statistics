<template>
  <v-app light>
    <v-app-bar class="white">
      <v-toolbar-title>{{ projectTitle }}</v-toolbar-title>
      <v-spacer />
      <user-status />
      <github-button />
    </v-app-bar>
    <v-content>
      <section>
        <v-parallax
          :src="require('~/assets/img/frontpage/back7-brighten.jpg')"
          height="600"
        >
          <v-layout
            column
            align-center
            justify-center
            class="white--text"
          >
            <img
              :src="require('~/assets/img/logo.png')"
              alt="Vuetify.js"
              height="190"
              class="mb-4"
            >
            <h2 class="black--text headline text-center font-weight-light">
              ACM-ICPC 协会
            </h2>
            <div style="height: 50px" />
            <h1 class="black--text mb-2 display-1 text-center index-title font-weight-bold">
              Welcome to {{ projectTitle }}
            </h1>
            <div class="black--text subheading mb-4 text-center index-title">
              Developed by Shumin Liu and Contributors
            </div>
            <v-btn
              class="blue lighten-2 black--text mt-12"
              dark
              large
              nuxt
              to="/statistics"
            >
              Enter AC Statistics
            </v-btn>
          </v-layout>
        </v-parallax>
      </section>

      <v-footer>
        <nuxt-link
          to="/about"
          class="white--text ml-4 body-2"
        >
          &copy; 2018 - {{ buildYear }} Shumin Liu and Contributors
        </nuxt-link>
        <v-spacer />
        <span class="white--text ml-4 body-2">陕ICP备17008184号</span>
      </v-footer>
    </v-content>
  </v-app>
</template>

<script>
import { PROJECT_TITLE } from '~/components/consts'
import { getDateFromTimestamp } from '~/components/utils'
import UserStatus from '~/components/UserStatus'
import GithubButton from '~/components/GithubButton'

export default {
  layout: 'none',
  components: {
    UserStatus,
    GithubButton,
  },
  data() {
    return {
      dialog: false,
      projectTitle: PROJECT_TITLE,
      buildYear: getDateFromTimestamp(this.$env.BUILD_TIME).getFullYear(),
    }
  },
}
/*!
Portions of this source file taken from vuetifyjs/parallax-starter <https://github.com/vuetifyjs/parallax-starter>

The MIT License (MIT)

Copyright (c) 2016 John Jeremy Leider

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
  */
</script>

<style scoped>
.index-title {
  text-shadow: 1px 0 #ecf5ff, 0 1px #ecf5ff, 0 -1px #ecf5ff, -1px 0 #ecf5ff;
}

.response-title {
  text-shadow: 1px 0 #000000, 0 1px #000000, 0 -1px #000000, -1px 0 #000000;
}
</style>
