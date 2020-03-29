<template>
  <v-card flat>
    <v-card-title primary-title>
      <div class="headline">
        注册
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
          <v-form v-model="valid" lazy-validation>
            <v-text-field
              prepend-icon="person"
              label="用户名"
              v-model="username"
              required
              :rules="[rules.required]"
            />
            <v-text-field
              prepend-icon="lock"
              label="密码"
              type="password"
              v-model="password"
              required
              :rules="[rules.required]"
            />
            <v-text-field
              prepend-icon="lock"
              label="再次输入密码"
              type="password"
              v-model="pwdRepeat"
              required
              :rules="[rules.required, () => password === pwdRepeat || '两次密码必须相同']"
            />
            <v-row>
              <v-col>
                <span v-html="captchaSvg" />
              </v-col>
              <v-col>
                <v-text-field
                  prepend-icon="verified_user"
                  label="验证码"
                  v-model="captchaText"
                  required
                  :rules="[rules.required]"
                >
                  <template v-slot:append>
                    <v-btn icon @click="refreshCaptcha">
                      <v-icon>refresh</v-icon>
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
                >
                  注册
                </v-btn>
              </v-col>
              <v-col>
                <v-btn block to="/login">
                  去登录
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
    }
  },
  methods: {
    async register() {
      try {
        await this.$axios.post('/api/services/app/Account/Register', {
          username: this.username,
          password: this.password,
          captchaText: this.captchaText,
          captchaId: this.captchaId,
        })

        await this.$store.dispatch('/session/login', {
          username: this.username,
          password: this.password,
          remember: false,
        })
        this.$router.push('/')

      } catch (err) {
        this.errorMessage = getAbpErrorMessage(err)
        this.showError = true
      }
    },
    async refreshCaptcha() {
      this.captchaText = ''
      try {
        const res = await this.$axios.post('/api/captcha-service/generate')
        this.captchaId = res.data.data.id
        this.captchaSvg = res.data.data.captcha
      } catch (error) {
        this.errorMessage = error.message
        this.showError = true
      }
    },
  },
  async mounted() {
    await this.refreshCaptcha()
  },
}
</script>
