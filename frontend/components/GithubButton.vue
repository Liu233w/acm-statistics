<template>
  <v-tooltip
    left
    v-resize="onResize"
  >
    <template #activator="{ on }">
      <v-btn
        icon
        v-on="on"
        href="https://github.com/Liu233w/acm-statistics"
        target="_blank"
        rel="noopener noreferrer"
        v-show="show"
      >
        <v-icon medium>
          mdi-github
        </v-icon>
      </v-btn>
    </template>
    <span>Star me on GitHub!</span>
  </v-tooltip>
</template>

<script>
export default {
  data() {
    return {
      show: true,
    }
  },
  mounted() {
    this.onResize()
  },
  methods: {
    onResize() {
      const width = window.innerWidth
      if (width < 450) {
        this.show = false
      } else {
        this.show = true
      }
    },
  },
}
</script>
