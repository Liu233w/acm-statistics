<template>
  <v-container fluid>
    <v-banner
      v-if="errorMessage !== null"
      color="error"
    >
      {{ errorMessage }}
    </v-banner>
    <v-data-table
      v-else
      disable-filtering
      disable-sort
      v-model="selected"
      :headers="headers"
      :items="items"
      :loading="loading"
      show-select
      :items-per-page.sync="itemsPerPage"
      :server-items-length="serverItemsLength"
      :page.sync="page"
      item-key="historyId"
    />
  </v-container>
</template>

<script>
import { getAbpErrorMessage } from '~/components/utils'
import _ from 'lodash'

export default {
  data() {
    return {
      items: [],
      loading: true,
      headers: [
        { text: 'Id', value: 'historyId', align: 'start' },
        { text: 'Query date', value: 'creationTime' },
        { text: 'Solved', value: 'solved' },
        { text: 'Submissions', value: 'submission' },
      ],
      selected: [],
      itemsPerPage: 10,
      serverItemsLength: null,
      page: 1,
      errorMessage: null,
    }
  },
  fetch() {
    return this.loadPage()
  },
  watch: {
    itemsPerPage() {
      return this.loadPage()
    },
    page() {
      return this.loadPage()
    },
  },
  methods: {
    async loadPage() {
      this.loading = true
      try {
        const res = await this.$axios
          .$get('/api/services/app/QueryHistory/GetQueryHistoriesAndSummaries', {
            maxResultCount: this.itemsPerPage,
            skipCount: (this.page - 1) * this.itemsPerPage,
          })
        this.serverItemsLength = res.result.totalCount
        this.items = res.result.items
        _.forEach(this.items, item => {
          item.creationTime = new Date(item.creationTime)
        })
        this.errorMessage = null
        this.loading = false
      } catch (err) {
        this.errorMessage = getAbpErrorMessage(err)
      }
    },
  },
}
</script>
