<template>
  <v-card flat>
    <v-card-title primary-title>
      <div class="headline">
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
          <v-form
            v-model="valid"
            @submit.prevent="login"
          >
            <v-text-field
              prepend-icon="person"
              label="Username"
              required
              v-model="username"
              :rules="[rules.required]"
              aria-autocomplete="username"
              autocomplete="username"
            />
            <v-text-field
              prepend-icon="lock"
              label="Password"
              required
              type="password"
              v-model="password"
              :rules="[rules.required]"
              aria-autocomplete="current-password"
              autocomplete="current-password"
            />
            <v-checkbox
              v-model="remember"
              label="Remember me"
            />
            <v-row>
              <v-col>
                <v-btn
                  type="submit"
                  color="info"
                  block
                  :disabled="!valid"
                  :loading="loading"
                >
                  login
                </v-btn>
              </v-col>
              <v-col>
                <v-btn
                  block
                  to="/register"
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
        this.$router.push('/')
      } catch (err) {
        this.errorMessage = getAbpErrorMessage(err)
        this.showError = true
      }
      this.loading = false
      return false
    },
  },
}
</script>
