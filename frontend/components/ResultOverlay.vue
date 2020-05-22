<template>
  <v-overlay
    :absolute="true"
    :value="value"
  >
    <v-chip
      :color="value.color"
      v-if="value"
      @click="$emit('click')"
    >
      {{ value.message }}
    </v-chip>
  </v-overlay>
</template>

<script>
export default {
  props: {
    value: {
      // { color: String, message: String }
      type: Object,
      default: null,
    },
  },
}
</script>
