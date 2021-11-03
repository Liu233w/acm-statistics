<template>
  <v-card flat>
    <v-card-title primary-title>
      <div class="text-h5">
        Register
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
              v-model="username"
              required
              :rules="[rules.required]"
            />
            <v-text-field
              prepend-icon="mdi-lock"
              label="Password"
              type="password"
              v-model="password"
              required
              :rules="[rules.required, rules.password]"
            />
            <v-text-field
              prepend-icon="mdi-lock"
              label="Confirm password"
              type="password"
              v-model="pwdRepeat"
              required
              :rules="[rules.required, () => password === pwdRepeat || 'Password must match']"
            />
            <v-row>
              <v-col>
                <span v-html="captchaSvg" />
              </v-col>
              <v-col>
                <v-text-field
                  prepend-icon="mdi-shield-check"
                  label="Captcha"
                  v-model="captchaText"
                  required
                  :rules="[rules.required]"
                >
                  <template #append>
                    <v-btn
                      icon
                      @click="refreshCaptcha"
                    >
                      <v-icon>mdi-refresh</v-icon>
                    </v-btn>
                  </template>
                </v-text-field>
              </v-col>
            </v-row>
            <v-row>
              <v-col>
                <v-btn
                  color="info"
                  block
                  :disabled="!valid && captchaText == ''"
                  @click="register"
                  :loading="loading"
                >
                  register
                </v-btn>
              </v-col>
              <v-col>
                <v-btn
                  block
                  @click="goLogin"
                >
                  enter login page
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
      email: '',
      password: '',
      pwdRepeat: '',
      valid: false,
      captchaId: '',
      captchaSvg: '',
      captchaText: '',
      errorMessage: '',
      showError: false,
      loading: false,
    }
  },
  methods: {
    async register() {
      this.loading = true
      try {
        await this.$axios.$post('/api/services/app/Account/Register', {
          username: this.username,
          password: this.password,
          captchaText: this.captchaText,
          captchaId: this.captchaId,
        })

        await this.$store.dispatch('session/login', {
          username: this.username,
          password: this.password,
          remember: false,
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
    async refreshCaptcha() {
      this.captchaText = ''
      try {
        const res = await this.$axios.$post('/api/captcha-service/generate')
        this.captchaId = res.data.id
        this.captchaSvg = res.data.captcha
      } catch (error) {
        this.errorMessage = error.message
        this.showError = true
      }
    },
    goLogin() {
      this.$router.push({
        path: '/login',
        query: {
          redirect: this.$route.query.redirect,
        },
      })
    },
  },
  async mounted() {
    await this.refreshCaptcha()
  },
}
</script>
