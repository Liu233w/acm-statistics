<template>
  <v-tooltip bottom>
    <template v-slot:activator="{ on }">
      <v-btn outlined @click="clicked" v-on="on">
        {{ title }}
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
      if (this.user) {
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
    clicked() {
      if (this.user) {
        this.$store.dispatch('session/logout')
        this.$router.push('/')
      } else {
        this.$router.push('/login')
      }
    },
  },
}
</script>
