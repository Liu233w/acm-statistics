<template>
  <v-container>
    <v-layout>
      <v-flex
        xs12
        sm6
        offset-sm3
      >
        <v-card>
          <v-app-bar
            color="light-blue"
            dark
          >
            <v-toolbar-title>用户设置</v-toolbar-title>
            <v-spacer />
          </v-app-bar>
          <v-list
            subheader
            three-line
          >
            <v-subheader>帐号动作</v-subheader>
            <v-list-item @click="logout">
              <v-list-item-content>
                <v-list-item-title>注销（登出）</v-list-item-title>
                <v-list-item-subtitle>退出本帐号</v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
          </v-list>
          <v-divider />
          <v-list
            subheader
            three-line
          >
            <v-subheader>帐号设置</v-subheader>
            <v-list-item
              input-value="true"
              color="red"
              @click=";"
            >
              <v-list-item-content>
                <v-list-item-title>删除帐号</v-list-item-title>
                <v-list-item-subtitle>删除本帐号和与其关联的邮箱</v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
          </v-list>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
export default {
  methods: {
    async logout() {
      this.$store.dispatch('session/logout')
      await this.$store.dispatch('session/refreshUser')
      this.$router.push('/')
    },
  },
}
</script>
