<template>
  <VApp light>
    <VAppBar class="white">
      <VToolbarTitle>
        {{ projectTitle }}
      </VToolbarTitle>
      <VSpacer />
      <VTooltip left>
        <template #activator="{ on }">
          <VBtn icon v-on="on" href="https://github.com/Liu233w/acm-statistics" target="_blank">
            <VIcon medium>
              fab fa-github
            </VIcon>
          </VBtn>
        </template>
        <span>Star me on Github!</span>
      </VTooltip>
    </VAppBar>
    <VContent>
      <section>
        <VParallax :src="require('~/assets/img/frontpage/back7-brighten.jpg')" height="600">
          <VLayout
            column
            align-center
            justify-center
            class="white--text"
          >
            <img :src="require('~/assets/img/logo.png')" alt="Vuetify.js" height="190" class="mb-4">
            <h2 class="black--text headline text-center font-weight-light">
              ACM-ICPC 协会
            </h2>
            <div style="height: 50px" />
            <h1 class="black--text mb-2 display-1 text-center index-title font-weight-bold">
              欢迎使用 {{ projectTitle }}
            </h1>
            <div class="black--text subheading mb-4 text-center index-title">
              由 NWPU-ACM 技术组强力驱动
            </div>
            <VBtn
              class="blue lighten-2 black--text mt-12"
              dark
              large
              nuxt
              to="/statistics"
            >
              进入 OJ 题量统计
            </VBtn>
          </VLayout>
        </VParallax>
      </section>

      <section>
        <VLayout
          column
          wrap
          class="my-12"
          align-center
        >
          <VFlex xs12 sm4 class="my-4">
            <div class="text-center">
              <h2 class="headline">
                全新查询系统，更加稳定
              </h2>
              <span class="subheading">
                异步API + 浏览器端爬虫，速度加倍
              </span>
            </div>
          </VFlex>
          <VFlex xs12>
            <VContainer grid-list-xl>
              <VLayout wrap>
                <VFlex xs12 md4>
                  <VCard class="elevation-0 transparent" :flat="true">
                    <VCardText class="text-center">
                      <VIcon x-large class="blue--text text--lighten-2">
                        link
                      </VIcon>
                    </VCardText>
                    <VCardTitle primary-title class="layout justify-center">
                      <div class="headline text-center">
                        免费提供题量查询API
                      </div>
                    </VCardTitle>
                    <VCardText class="text-center">
                      我们提供了一个 restful api 来供您使用，您可以在任意的客户端、网站中调用此api来查询某用户在oj上的提交数/通过数。
                      访问我们的
                      <a href="/swagger" target="_blank">
                        api文档
                      </a>以获取更多信息。
                    </VCardText>
                  </VCard>
                </VFlex>
                <VFlex xs12 md4>
                  <VCard class="elevation-0 transparent" :flat="true">
                    <VCardText class="text-center">
                      <VIcon x-large class="blue--text text--lighten-2">
                        fab fa-github
                      </VIcon>
                    </VCardText>
                    <VCardTitle primary-title class="layout justify-center">
                      <div class="headline text-center">
                        完全开源
                      </div>
                    </VCardTitle>
                    <VCardText class="text-center">
                      访问我们的
                      <a href="https://github.com/Liu233w/acm-statistics" target="_blank">
                        项目地址
                      </a>
                      以获取本网站的最新源代码，您可以在自己的服务器上部署本网站或者在自己的项目中使用我们的爬虫代码。
                      欢迎向我们提出问题建议、贡献代码或给我们点赞（Star），您的支持是我们前进的动力
                    </VCardText>
                  </VCard>
                </VFlex>
                <VFlex xs12 md4>
                  <VCard class="elevation-0 transparent" :flat="true">
                    <VCardText class="text-center">
                      <VIcon x-large class="blue--text text--lighten-2">
                        build
                      </VIcon>
                    </VCardText>
                    <VCardTitle primary-title class="layout justify-center">
                      <div class="headline text-center">
                        更多功能支持中
                      </div>
                    </VCardTitle>
                    <VCardText class="text-center">
                      旧版查询系统中的功能正在被移植到新版查询系统中，如果您需要旧版查询系统中的功能，请
                      <a href="//npuacm.info/">
                        单击此处返回旧版查询系统
                      </a>
                    </VCardText>
                  </VCard>
                </VFlex>
              </VLayout>
            </VContainer>
          </VFlex>
        </VLayout>
      </section>

      <section>
        <VParallax :src="require('~/assets/img/frontpage/back4.jpg')" height="380">
          <VLayout column align-center justify-center>
            <div class="headline white--text mb-4 text-center response-title">
              欢迎向我们提出意见以帮助本网站进步
            </div>
            <a :href="mailToGithubLink">
              <em class="white--text response-title">
                或者单击此处向本项目发送邮件
              </em>
            </a>
            <VBtn
              class="blue lighten-2 mt-12"
              dark
              large
              href="https://github.com/Liu233w/acm-statistics/issues/new"
              target="_blank"
            >
              Submit New Issue
            </VBtn>
          </VLayout>
        </VParallax>
      </section>

      <section>
        <VContainer grid-list-xl>
          <VLayout wrap justify-center class="my-12">
            <VFlex xs12 sm4>
              <VCard class="elevation-0 transparent" :flat="true">
                <VCardTitle primary-title class="layout justify-center">
                  <div class="headline">
                    关于我们
                  </div>
                </VCardTitle>
                <VCardText>
                  <p>
                    西北工业大学ACM协会技术组致力于为广大学生提供各类计算机、算法、计算机类竞赛方面的设施及服务。
                    欢迎各位有识之士加入。
                  </p>
                  <p id="busuanzi_container_site_pv" style="display: none;">
                    本站总访问量: <span id="busuanzi_value_site_pv" />次<br>
                  </p>
                  <p id="busuanzi_container_site_uv" style="display: none;">
                    本站总访客数: <span id="busuanzi_value_site_uv" />次
                  </p>
                </VCardText>
              </VCard>
            </VFlex>
            <VFlex xs12 sm4 offset-sm1>
              <VCard class="elevation-0 transparent" :flat="true">
                <VCardTitle primary-title class="layout justify-center">
                  <div class="headline">
                    联系我们
                  </div>
                </VCardTitle>
                <VList class="transparent">
                  <VListItem href="https://jq.qq.com/?_wv=1027&k=5qgzfQc" target="_blank">
                    <VListItemAction>
                      <VIcon class="blue--text text--lighten-2">
                        fab fa-qq
                      </VIcon>
                    </VListItemAction>
                    <VListItemContent>
                      <VListItemTitle>198820787</VListItemTitle>
                    </VListItemContent>
                  </VListItem>
                  <VListItem @click="showAddress">
                    <VListItemAction>
                      <VIcon class="blue--text text--lighten-2">
                        place
                      </VIcon>
                    </VListItemAction>
                    <VListItemContent>
                      <VListItemTitle>陕西省西安市长安区西北工业大学长安校区计算机学院110室</VListItemTitle>
                    </VListItemContent>
                  </VListItem>
                  <VListItem href="mailto:support@npuacm.info">
                    <VListItemAction>
                      <VIcon class="blue--text text--lighten-2">
                        email
                      </VIcon>
                    </VListItemAction>
                    <VListItemContent>
                      <VListItemTitle>support@npuacm.info</VListItemTitle>
                    </VListItemContent>
                  </VListItem>
                  <VListItem @click="dialog = !dialog">
                    <VListItemAction>
                      <VIcon class="blue--text text--lighten-2">
                        fab fa-weixin
                      </VIcon>
                    </VListItemAction>
                    <VListItemContent>
                      <VListItemTitle>微信公众号</VListItemTitle>
                    </VListItemContent>
                  </VListItem>
                </VList>
              </VCard>
            </VFlex>
          </VLayout>
          <VDialog v-model="dialog" max-width="500px">
            <VCard>
              <VCardTitle>
                <span>微信公众号</span>
              </VCardTitle>
              <VCardText>
                <img :src="require('~/assets/img/wechat.jpg')" alt="公众号二维码" style="height: 300px">
              </VCardText>
              <VCardActions>
                <VBtn color="primary" text @click.stop="dialog=false">
                  关闭
                </VBtn>
              </VCardActions>
            </VCard>
          </VDialog>
        </VContainer>
      </section>

      <VFooter class="blue darken-2">
        <NuxtLink to="/about" class="white--text ml-4 body-2">
          &copy; 2018 - {{ buildYear }} NWPU-ACM 技术组
        </NuxtLink>
        <VSpacer />
        <span class="white--text ml-4 body-2">
          陕ICP备17008184号
        </span>
      </VFooter>
    </VContent>
  </VApp>
</template>

<script>
  import {PROJECT_TITLE} from '~/components/consts'
  import {getDateFromTimestamp} from '~/components/utils'

  export default {
    layout: 'none',
    data() {
      return {
        dialog: false,
        projectTitle: PROJECT_TITLE,
        buildYear: getDateFromTimestamp(this.$env.BUILD_TIME).getFullYear(),
      }
    },
    methods: {
      showAddress() {
        window.alert('陕西省西安市长安区西北工业大学长安校区计算机学院110室')
      },
    },
    computed: {
      mailToGithubLink() {
        const body = encodeURIComponent('请注意，您经此链接发送的所有信息都将被发送到' +
          'https://github.com/Liu233w/acm-statistics/issues，任何人都有权限浏览其内容。')
        return `mailto:acm-statistics@fire.fundersclub.com?body=${body}`
      },
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
