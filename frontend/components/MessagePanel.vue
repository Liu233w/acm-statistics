<template>
  <v-expand-transition>
    <v-card
      v-show="show"
      class="message-panel"
    >
      <v-slide-y-transition
        group
        tag="v-list"
        class="pa-0"
      >
        <v-list-item
          v-for="(item,i) in list"
          :key="item.id"
          class="pa-0"
        >
          <v-alert
            dismissible
            :dense="!oneItem"
            :type="item.type"
            @input="closeOne(i)"
            :class="{ 'mb-0': oneItem, 'mb-1': !oneItem }"
          >
            {{ item.message }}
          </v-alert>
        </v-list-item>
      </v-slide-y-transition>
      <v-card-actions
        v-if="!oneItem"
        class="pt-0"
      >
        <v-spacer />
        <v-btn
          text
          color="primary"
          @click="close"
        >
          close
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-expand-transition>
</template>

<script>
import { mapGetters, mapState } from 'vuex'

export default {

  computed: {
    ...mapState('message', [
      'list',
    ]),
    ...mapGetters('message', [
      'top',
    ]),
    show() {
      return this.$store.getters['message/show']
    },
    oneItem() {
      return this.list.length <= 1
    },
  },

  methods: {
    close() {
      this.$store.commit('message/clear')
    },
    closeOne(i) {
      this.$store.commit('message/remove', i)
    },
  },
}
</script>

<style scoped>
.message-panel {
  position: fixed;
  z-index: 1000;
  top: 10px;

  left: 50%;
  transform: translateX(-50%);

  transition: flex 0.3s ease-out;
}
</style>
