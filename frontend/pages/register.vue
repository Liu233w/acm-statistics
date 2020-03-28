<template>
  <v-card flat>
    <v-card-title primary-title>
      <div class="headline">
        注册
      </div>
    </v-card-title>
    <v-container>
      <v-layout>
        <v-flex>
          <v-form v-model="valid" lazy-validation>
            <v-text-field prepend-icon="email" label="Email" v-model="email" required
                          :rules="[rules.required, rules.email]"/>
            <v-text-field prepend-icon="person" label="Username" v-model="username" required :rules="[rules.required]" />
            <v-text-field prepend-icon="lock" label="Password" type="password" v-model="password" required
                          :rules="[rules.required]"
            />
            <v-text-field prepend-icon="lock" label="Password Again" type="password" v-model="pwdRepeat" required
                          :rules="[rules.required, () => password === pwdRepeat || '两次密码必须相同']"
            />
            <v-card-actions>
              <v-btn color="info" block :disabled="!valid">
                注册
              </v-btn>
              <v-btn block to="/login">
                去登录
              </v-btn>
            </v-card-actions>
          </v-form>
        </v-flex>
      </v-layout>
    </v-container>
  </v-card>
</template>

<script>
  import rulesMixin from '~/components/rulesMixin'

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
      }
    },
    methods: {
      // submit() {
      //   return null
      // },
    },
  }
</script>
