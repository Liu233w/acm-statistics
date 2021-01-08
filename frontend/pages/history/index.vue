<template>
  <v-container fluid>
    <v-banner
      v-if="errorMessage !== null"
      color="error"
    >
      {{ errorMessage }}
    </v-banner>
    <template v-else>
      <v-banner>
        Only the latest history per day is saved. You may change your time zone in settings.
        <v-btn
          text
          color="primary"
          to="/settings"
        >
          go settings
        </v-btn>
      </v-banner>
      <v-data-table
        class="mt-4"
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
        :footer-props="footerProps"
      >
        <template #top>
          <v-btn
            color="error"
            :disabled="selected.length === 0"
            @click="deleteSelected"
          >
            delete selected
          </v-btn>
        </template>
        <!-- item.actions is a name -->
        <!-- eslint-disable-next-line -->
        <template #item.actions="{ item }">
          <v-btn
            icon
            :to="'/history/'+item.historyId"
          >
            <v-icon>
              mdi-eye
            </v-icon>
          </v-btn>
          <v-btn
            icon
            @click="deleteOne(item.historyId)"
          >
            <v-icon>
              mdi-delete
            </v-icon>
          </v-btn>
        </template>
      </v-data-table>
      <v-row>
        <advertisement id="135755353" />
      </v-row>
      <line-chart
        :chart-data="chartData"
        :options="chartOptions"
        style="height: 300px"
      />
    </template>
  </v-container>
</template>

<script>
import _ from 'lodash'

import { getAbpErrorMessage } from '~/components/utils'
import LineChart from '~/components/LineChart'
import { PROJECT_TITLE } from '~/components/consts'
import Advertisement from '~/components/Advertisement'

export default {
  components: {
    LineChart,
    Advertisement,
  },
  inject: ['changeLayoutConfig'],
  head: {
    title: `History - ${PROJECT_TITLE}`,
  },
  mounted() {
    this.changeLayoutConfig({
      title: 'History',
    })
  },
  data() {
    return {
      items: [],
      loading: true,
      headers: [
        { text: 'Id', value: 'historyId', align: 'start' },
        { text: 'Query date', value: 'creationTime' },
        { text: 'Solved', value: 'solved' },
        { text: 'Submissions', value: 'submission' },
        { text: 'Actions', value: 'actions' },
      ],
      selected: [],
      itemsPerPage: 10,
      serverItemsLength: null,
      page: 1,
      errorMessage: null,
      chartOptions: {
        responsive: true,
        maintainAspectRatio: false,
        tooltips: {
          mode: 'index',
          intersect: false,
        },
        hover: {
          mode: 'nearest',
          intersect: true,
        },
      },
    }
  },
  computed: {
    chartData() {
      const dateFormatter = new Intl.DateTimeFormat()
      const dates = _.map(this.items, (a) =>
        dateFormatter.format(a.creationTime),
      )
      const solved = _.map(this.items, 'solved')
      const submissions = _.map(this.items, 'submission')
      return {
        labels: _.reverse(dates),
        datasets: [
          {
            label: 'Solved',
            backgroundColor: '#6699ff',
            data: _.reverse(solved),
            filled: '-1',
          },
          {
            label: 'Submissions',
            backgroundColor: '#3d3d5c',
            data: _.reverse(submissions),
            filled: '-1',
          },
        ],
      }
    },
    footerProps() {
      if (this.loading) {
        return { disablePagination: true, disableItemsPerPage: true }
      } else {
        return undefined
      }
    },
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
        const res = await this.$axios.$get(
          '/api/services/app/QueryHistory/GetQueryHistoriesAndSummaries',
          {
            params: {
              maxResultCount: this.itemsPerPage,
              skipCount: (this.page - 1) * this.itemsPerPage,
            },
          },
        )
        this.serverItemsLength = res.result.totalCount
        this.items = res.result.items
        _.forEach(this.items, (item) => {
          item.creationTime = new Date(item.creationTime)
        })
        this.errorMessage = null
        this.loading = false
      } catch (err) {
        this.errorMessage = getAbpErrorMessage(err)
      }
    },
    async deleteOne(id) {
      try {
        await this.$axios.$post(
          '/api/services/app/QueryHistory/DeleteQueryHistory',
          {
            id,
          },
        )
        await this.loadPage()
      } catch (err) {
        this.$store.commit('message/addError', getAbpErrorMessage(err))
      }
    },
    async deleteSelected() {
      try {
        await this.$axios.$post(
          '/api/services/app/QueryHistory/DeleteQueryHistory',
          {
            ids: _.map(this.selected, 'historyId'),
          },
        )
        await this.loadPage()
      } catch (err) {
        this.$store.commit('message/addError', getAbpErrorMessage(err))
      }
    },
  },
}
</script>
