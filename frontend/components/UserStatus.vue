<template>
  <v-tooltip bottom>
    <template v-slot:activator="{ on }">
      <v-btn text @click="clicked" v-on="on">
        <v-icon left v-show="!session.login">
          fa fa-sign-in-alt
        </v-icon> {{ title }}
      </v-btn>
    </template>
    {{ tooltip }}
  </v-tooltip>
</template>

<script>
export default {
  computed: {
    session() {
      return this.$store.state.session
    },
    title() {
      if (this.session.login) {
        return this.session.username
      } else {
        return '登录'
      }
    },
    tooltip() {
      if (this.session.login) {
        return '点我注销'
      } else {
        return '点我登录'
      }
    },
  },
  methods: {
    async clicked() {
      if (this.session.login) {
        this.$store.dispatch('session/logout')
        await this.$store.dispatch('session/refreshUser')
        this.$router.push('/')
      } else {
        this.$router.push('/login')
      }
    },
  },
}
</script>
