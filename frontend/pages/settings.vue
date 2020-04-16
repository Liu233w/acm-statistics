<template>
  <v-container>
    <v-layout>
      <v-flex
        xs12
        sm6
        offset-sm3
      >
        <v-card>
          <v-app-bar
            color="light-blue"
            dark
          >
            <v-toolbar-title>Settings</v-toolbar-title>
            <v-spacer />
          </v-app-bar>
          <v-list
            subheader
            three-line
          >
            <v-subheader>Accounts</v-subheader>
            <v-list-item @click="logout">
              <v-list-item-content>
                <v-list-item-title>Sign out</v-list-item-title>
                <v-list-item-subtitle>Logout of this computer</v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
            <v-divider />
            <v-list-item
              input-value="true"
              color="red"
              @click="deleteDialog = true"
            >
              <v-list-item-content>
                <v-list-item-title>Delete Account</v-list-item-title>
                <v-list-item-subtitle>Delete this account and all data related to it</v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
          </v-list>
        </v-card>
      </v-flex>
    </v-layout>
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
export default {
  data() {
    return {
      deleteDialog: false,
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
  },
}
</script>
