<template>
  <v-container fluid>
    <v-row>
      <v-col>
        <v-card>
          <v-card-title>Sign out</v-card-title>
          <v-card-text>
            Logout of this computer.
          </v-card-text>
          <v-card-actions>
            <v-btn
              @click="logout"
              text
            >
              sign out
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-card>
          <v-card-title>Change Password</v-card-title>
          <v-card-text>
            <v-form v-model="valid">
              <v-text-field
                prepend-icon="lock"
                label="Current Password"
                type="password"
                v-model="currentPwd"
                required
                :rules="[rules.required]"
              />
              <v-text-field
                prepend-icon="lock"
                label="New Password"
                type="password"
                v-model="newPwd"
                required
                :rules="[rules.required, rules.password]"
              />
              <v-text-field
                prepend-icon="lock"
                label="Confirm password"
                type="password"
                v-model="pwdRepeat"
                required
                :rules="[rules.required, () => newPwd === pwdRepeat || 'Password must match']"
              />
            </v-form>
          </v-card-text>
          <v-card-actions>
            <v-btn
              color="info"
              text
              :disabled="!valid"
              @click="changePassword"
              :loading="loading"
            >
              submit
            </v-btn>
          </v-card-actions>
          <v-overlay
            :absolute="true"
            :value="changePwdMessage"
          >
            <v-chip
              :color="changePwdMessage.color"
              v-if="changePwdMessage"
              @click="changePwdMessage = null"
            >
              {{ changePwdMessage.message }}
            </v-chip>
          </v-overlay>
        </v-card>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-card>
          <v-card-title class="red--text">
            Delete Account
          </v-card-title>
          <v-card-text>
            Delete this account and all data related to it.
          </v-card-text>
          <v-card-actions>
            <v-btn
              @click="deleteDialog = true"
              text
              color="red"
            >
              delete
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
    <v-dialog
      v-model="deleteDialog"
      persistent
      max-width="400"
    >
      <v-card>
        <v-card-title class="headline">
          Are you sure you want to delete this account?
        </v-card-title>
        <v-card-text>
          After deletion, all data related to it (including query history and analyzes) will be lost.
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            color="darken-1"
            text
            @click="deleteDialog = false"
          >
            Cancel
          </v-btn>
          <v-btn
            color="red darken-1"
            text
            @click="deleteAccount"
          >
            Confirm
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script>
import rulesMixin from '~/components/rulesMixin'
import { getAbpErrorMessage } from '~/components/utils'

export default {
  mixins: [rulesMixin],
  data() {
    return {
      deleteDialog: false,
      currentPwd: '',
      newPwd: '',
      pwdRepeat: '',
      valid: false,
      loading: false,
      changePwdMessage: null,
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
    async changePassword() {
      try {
        await this.$axios.$post('/api/services/app/Account/ChangePassword', {
          currentPassword: this.currentPwd,
          newPassword: this.newPwd,
        })
        this.changePwdMessage = {
          color: 'success',
          message: 'Success!',
        }
      } catch (err) {

        this.changePwdMessage = {
          color: 'error',
          message: getAbpErrorMessage(err),
        }
      }
    },
  },
}
</script>
