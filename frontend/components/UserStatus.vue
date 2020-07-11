<template>
  <v-tooltip bottom>
    <template v-slot:activator="{ on }">
      <v-btn text @click="clicked" v-on="on">
        <v-icon left v-show="!session.login">
          mdi-login
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
        return 'login'
      }
    },
    tooltip() {
      if (this.session.login) {
        return 'Enter settings'
      } else {
        return 'Click to login'
      }
    },
  },
  methods: {
    async clicked() {
      if (this.session.login) {
        this.$router.push('/settings')
      } else {
        this.$router.push('/login')
      }
    },
  },
}
</script>
