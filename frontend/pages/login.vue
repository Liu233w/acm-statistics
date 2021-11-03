<template>
  <v-card flat>
    <v-card-title primary-title>
      <div class="text-h5">
        Login
      </div>
    </v-card-title>
    <v-container>
      <v-row>
        <v-alert
          type="error"
          v-model="showError"
          dismissible
          transition="fade-transition"
          class="mb-3"
        >
          {{ errorMessage }}
        </v-alert>
      </v-row>
      <v-layout>
        <v-flex>
          <v-form v-model="valid">
            <v-text-field
              prepend-icon="mdi-account"
              label="Username"
              required
              v-model="username"
              :rules="[rules.required]"
            />
            <v-text-field
              prepend-icon="mdi-lock"
              label="Password"
              required
              type="password"
              v-model="password"
              :rules="[rules.required]"
            />
            <v-checkbox
              v-model="remember"
              label="Remember me"
            />
            <v-row>
              <v-col>
                <v-btn
                  color="info"
                  block
                  :disabled="!valid"
                  @click="login"
                  :loading="loading"
                >
                  login
                </v-btn>
              </v-col>
              <v-col>
                <v-btn
                  block
                  @click="goRegister"
                >
                  enter register page
                </v-btn>
              </v-col>
            </v-row>
          </v-form>
        </v-flex>
      </v-layout>
    </v-container>
  </v-card>
</template>

<script>
import rulesMixin from '~/components/rulesMixin'
import { getAbpErrorMessage } from '~/components/utils'

export default {
  layout: 'login',
  mixins: [rulesMixin],
  data() {
    return {
      username: '',
      password: '',
      remember: false,
      valid: false,
      errorMessage: '',
      showError: false,
      loading: false,
    }
  },
  methods: {
    async login() {
      this.loading = true
      try {
        await this.$store.dispatch('session/login', {
          username: this.username,
          password: this.password,
          remember: this.remember,
        })

        const redirect = this.$route.query.redirect
        if (redirect) {
          this.$router.push(redirect)
        } else {
          this.$router.push('/')
        }
      } catch (err) {
        this.errorMessage = getAbpErrorMessage(err)
        this.showError = true
      }
      this.loading = false
    },
    goRegister() {
      this.$router.push({
        path: '/register',
        query: {
          redirect: this.$route.query.redirect,
        },
      })
    },
  },
}
</script>
