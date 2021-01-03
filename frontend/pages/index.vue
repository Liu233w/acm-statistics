<template>
  <dev>
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
            An online tool to analyze users performance in online judges (coding competition websites)
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

    <section>
      <v-layout
        column
        wrap
        class="my-12"
        align-center
      >
        <v-flex
          xs12
          sm4
          class="my-4"
        >
          <div class="text-center">
            <h2 class="headline">
              Guide
            </h2>
            <span class="subheading">
              of the query tool.
            </span>
          </div>
        </v-flex>
        <v-flex xs12>
          <v-container grid-list-xl>
            <v-layout wrap>
              <v-flex
                xs12
                md4
              >
                <v-card
                  class="elevation-0 transparent"
                  :flat="true"
                >
                  <v-img
                    :src="require('~/assets/img/frontpage/guide1.jpg')"
                    height="100px"
                    class="elevation-1"
                  />
                  <v-card-title
                    primary-title
                    class="layout justify-center"
                  >
                    <div class="headline text-center">
                      Query
                    </div>
                  </v-card-title>
                  <v-card-text class="text-center">
                    Enter your username and click QUERY. You can save your usernames and use it later.
                  </v-card-text>
                </v-card>
              </v-flex>
              <v-flex
                xs12
                md4
              >
                <v-card
                  class="elevation-0 transparent"
                  :flat="true"
                >
                  <v-img
                    :src="require('~/assets/img/frontpage/guide2.jpg')"
                    height="100px"
                    class="elevation-1"
                  />
                  <v-card-title
                    primary-title
                    class="layout justify-center"
                  >
                    <div class="headline text-center">
                      Worker Panel
                    </div>
                  </v-card-title>
                  <v-card-text class="text-center">
                    Use different usernames in different OJs, or multiple usernames in the same OJ.
                  </v-card-text>
                </v-card>
              </v-flex>
              <v-flex
                xs12
                md4
              >
                <v-card
                  class="elevation-0 transparent"
                  :flat="true"
                >
                  <v-img
                    :src="require('~/assets/img/frontpage/guide3.jpg')"
                    height="100px"
                    class="elevation-1"
                  />
                  <v-card-title
                    primary-title
                    class="layout justify-center"
                  >
                    <div class="headline text-center">
                      Summary
                    </div>
                  </v-card-title>
                  <v-card-text class="text-center">
                    Click the result panel to view summary.
                    (Need login)
                  </v-card-text>
                </v-card>
              </v-flex>
            </v-layout>
          </v-container>
        </v-flex>
      </v-layout>
    </section>

    <section>
      <v-parallax
        :src="require('~/assets/img/frontpage/back4.jpg')"
        height="380"
      >
        <v-layout
          column
          align-center
          justify-center
        >
          <div class="headline white--text mb-4 text-center response-title">
            If you have any questions,
          </div>
          <em class="white--text response-title">feel free to discuss on GitHub</em>
          <v-btn
            class="blue lighten-2 mt-12"
            dark
            large
            href="https://github.com/Liu233w/acm-statistics/discussions/new"
            target="_blank"
            rel="noopener noreferrer"
          >
            Start Discussion
          </v-btn>
        </v-layout>
      </v-parallax>
    </section>

    <section>
      <v-container grid-list-xl>
        <v-layout
          wrap
          justify-center
          class="my-12"
        >
          <v-flex
            xs12
            sm4
          >
            <v-card
              flat
              class="transparent"
            >
              <v-card-title
                primary-title
                class="layout justify-center"
              >
                <div class="headline">
                  About the website
                </div>
              </v-card-title>
              <v-card-text>
                This project is the former <span class="font-italic">
                  西北工业大学ACM查询系统 (npuacm.info)
                </span>, and the original idea is from <a
                  href="https://kidozh.com/en/"
                  target="_blank"
                  rel="noopener noreferrer"
                >Jiduo Zhang</a>.
                The old domain is broken, so I moved it to the new domain.
              </v-card-text>
            </v-card>
          </v-flex>
          <v-flex
            xs12
            sm4
            offset-sm1
          >
            <v-card
              flat
              class="transparent"
            >
              <v-card-title
                primary-title
                class="layout justify-center"
              >
                <div class="headline">
                  About us
                </div>
              </v-card-title>
              <v-card-text>
                This project is developed and maintained by Shumin Liu and Contributors with ❤.
                Please check <span class="font-italic">about</span> page for contact information.
              </v-card-text>
            </v-card>
          </v-flex>
        </v-layout>
      </v-container>
    </section>
  </dev>
</template>

<script>
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
