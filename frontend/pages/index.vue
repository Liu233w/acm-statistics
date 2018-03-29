<template>
  <v-layout column justify-center align-center>
    <v-flex xs12 sm8 md6>
      <div class="text-xs-center">
        <img src="/img/logo.png" alt="ACM-ICPC 协会">
      </div>
      <v-card>
        <v-card-title class="headline">欢迎使用 ACM-ICPC OJ题量查询工具</v-card-title>
        <v-card-text>
          <p>全新异步API + 浏览器端爬虫，查询速度加倍</p>
          <p><a href="/swagger">免费提供题目查询 Rest API</a></p>
          <p>完全开源</p>
          <p>更多OJ支持中...</p>
          <p>
            本工具目前还在开发中，欢迎
            <a href="https://github.com/Liu233w/acm-statistics/issues" target="_blank">向我们提出意见</a>
          </p>
          <hr class="my-3">
          <a href="mailto:support@npuacm.info">联系我们</a>
          <br>
          <a href="https://github.com/Liu233w/acm-statistics" target="_blank">源代码</a>
          <br>
          <a href="http://npuacm.info/">返回旧版查询系统</a>
        </v-card-text>
      </v-card>
    </v-flex>
    <ta/>
  </v-layout>
</template>
