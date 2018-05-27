<template>
  <v-card flat>
    <v-card-title primary-title>
      <div class="headline">登录</div>
    </v-card-title>
    <v-container>
      <v-layout row>
        <v-flex>
          <v-form v-model="valid" lazy-validation>
            <v-text-field prepend-icon="person" label="Username Or Email" required v-model="username" :rules="[rules.required]"/>
            <v-text-field prepend-icon="lock" label="Password" required type="password" v-model="password" :rules="[rules.required]"/>
            <v-card-actions>
              <v-btn color="info" block :disabled="!valid">登录</v-btn>
              <v-btn block to="/register">去注册</v-btn>
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
        password: '',
        valid: false,
      }
    },
  }
</script>
