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
              @click="deleteDialog = true"
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
    <v-dialog
      v-model="deleteDialog"
      persistent
      max-width="400"
    >
      <v-card>
        <v-card-title class="headline">
          您确定要删除本帐号吗？
        </v-card-title>
        <v-card-text>
          删除之后，您将不再收到题量统计邮件。之前统计的数据都会丢失。
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            color="darken-1"
            text
            @click="deleteDialog = false"
          >
            取消
          </v-btn>
          <v-btn
            color="red darken-1"
            text
            @click="deleteAccount"
          >
            删除
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script>
export default {
  data() {
    return {
      deleteDialog: false,
    }
  },
  methods: {
    async logout() {
      this.$store.dispatch('session/logout')
      await this.$store.dispatch('session/refreshUser')
      this.$router.push('/')
    },
    async deleteAccount() {
      await this.$axios.$post('/api/services/app/Account/SelfDelete')
      this.$store.dispatch('session/logout')
      this.$router.push('/')
    },
  },
}
</script>
