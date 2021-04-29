<template>
  <v-container fluid>
    <v-row>
      <v-col>
        <v-card>
          <v-card-title>Sign out</v-card-title>
          <v-card-text>
            Logout from this computer.
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
          <v-card-title>Auto save query history</v-card-title>
          <v-card-text>
            <v-radio-group v-model="autoSaveHistory">
              <v-radio
                value="true"
                label="Save history after finishing query"
              />
              <v-radio
                value="false"
                label="Only save history when clicking view summary button"
              />
            </v-radio-group>
          </v-card-text>
          <v-card-actions>
            <v-btn
              @click="updateAutoSaveHistoryConfig"
              :loading="autoSaveHistory === null"
              :disabled="autoSaveHistory === settings['App.AutoSaveHistory']"
              text
            >
              save
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-card>
          <v-card-title>Change time zone</v-card-title>
          <v-card-text>
            <p>You may change your time zone every 24 hours.</p>
            <v-select
              v-model="timeZone"
              :items="timeZoneList"
              label="Time Zone"
            />
          </v-card-text>
          <v-card-actions>
            <v-btn
              @click="saveTimeZone"
              text
            >
              save
            </v-btn>
          </v-card-actions>
          <result-overlay
            :value="timeZoneMessage"
            @click="timeZoneMessage = null"
          />
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
                label="Confirm Password"
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
          <result-overlay
            :value="changePwdMessage"
            @click="changePwdMessage = null"
          />
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
        <v-card-title class="text-h5">
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
import { mapState } from 'vuex'

import rulesMixin from '~/components/rulesMixin'
import { getAbpErrorMessage } from '~/components/utils'
import { TIMEZONE_LIST, PROJECT_TITLE } from '~/components/consts'
import ResultOverlay from '~/components/ResultOverlay'

export default {
  mixins: [rulesMixin],
  components: { ResultOverlay },
  inject: ['changeLayoutConfig'],
  mounted() {
    this.changeLayoutConfig({
      title: 'Settings',
    })
  },
  head: {
    title: `Settings - ${PROJECT_TITLE}`,
  },
  computed: {
    ...mapState('session', [
      'settings',
    ]),
  },
  data() {
    return {
      deleteDialog: false,
      currentPwd: '',
      newPwd: '',
      pwdRepeat: '',
      valid: false,
      loading: false,
      changePwdMessage: null,
      timeZoneList: TIMEZONE_LIST,
      timeZone: null,
      timeZoneMessage: null,
      autoSaveHistory: null,
    }
  },
  created() {
    this.timeZone = this.settings['Abp.Timing.TimeZone']
    this.autoSaveHistory = this.settings['App.AutoSaveHistory']
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
      this.changePwdMessage = null
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
    async saveTimeZone() {
      this.timeZoneMessage = null
      try {
        await this.$axios.$post('/api/services/app/UserConfig/SetUserTimeZone', {
          timeZone: this.timeZone,
        })
        await this.$store.dispatch('/session/refreshSettings')
        this.timeZoneMessage = {
          color: 'success',
          message: 'Success!',
        }
      } catch (err) {
        this.timeZoneMessage = {
          color: 'error',
          message: getAbpErrorMessage(err),
        }
      }
    },
    async updateAutoSaveHistoryConfig() {
      const config = this.autoSaveHistory
      this.autoSaveHistory = null
      try {
        await this.$axios.put('/api/services/app/UserConfig/UpdateAutoSaveHistory', {
          autoSaveHistory: config === 'true',
        })
        await this.$store.dispatch('session/refreshSettings')
        this.autoSaveHistory = this.settings['App.AutoSaveHistory']
      } catch (err) {
        this.$store.commit('message/addError', getAbpErrorMessage(err))
      }
    },
  },
}
</script>
