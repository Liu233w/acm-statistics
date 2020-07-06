<template>
  <v-container>
    <v-card-text
      v-if="summaryError"
      class="text-center"
    >
      <p class="title error--text mt-5">
        {{ summaryError }}
      </p>
    </v-card-text>
    <v-card-text
      v-else-if="summary == null"
      class="text-center"
    >
      <v-progress-circular
        :size="100"
        color="primary"
        indeterminate
        class="mt-10"
      />
    </v-card-text>
    <v-container v-else>
      <v-row justify="center">
        <v-col>
          <v-list>
            <v-list-item v-if="summary.mainUsername">
              <v-list-item-content>
                <v-list-item-title><strong>Main username:</strong> {{ summary.mainUsername }}</v-list-item-title>
              </v-list-item-content>
            </v-list-item>
            <v-list-item>
              <v-list-item-content>
                <v-list-item-title><strong>SOLVED:</strong> {{ summary.solved }}</v-list-item-title>
              </v-list-item-content>
            </v-list-item>
            <v-list-item>
              <v-list-item-content>
                <v-list-item-title><strong>SUBMISSION:</strong> {{ summary.submission }}</v-list-item-title>
              </v-list-item-content>
            </v-list-item>
            <v-list-item>
              <v-list-item-content>
                <v-list-item-title><strong>Generated at</strong> {{ new Date(summary.generateTime) }}</v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </v-list>
          <v-simple-table>
            <template v-slot:default>
              <thead>
                <tr>
                  <th
                    class="text-left"
                    scope="col"
                  >
                    Crawler
                  </th>
                  <th
                    class="text-left"
                    scope="col"
                  >
                    Username
                  </th>
                  <th
                    class="text-left"
                    scope="col"
                  >
                    Solved
                  </th>
                  <th
                    class="text-left"
                    scope="col"
                  >
                    Submission
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="item in workerSummaryList"
                  :key="`${item.crawler}`"
                >
                  <td scope="row">
                    {{ item.crawler }}
                  </td>
                  <td>{{ item.username }}</td>
                  <td>{{ item.solved }}</td>
                  <td>{{ item.submissions }}</td>
                </tr>
              </tbody>
            </template>
          </v-simple-table>
          <bar-chart
            :chart-data="chartData"
            style="height: 300px"
          />
          <v-list dense>
            <v-subheader
              v-if="summary.summaryWarnings.length > 0"
              class="red--text"
            >
              WARNINGS
            </v-subheader>
            <v-list-item
              v-for="item in summary.summaryWarnings"
              :key="item"
            >
              <v-list-item-content>
                {{ $store.state.statistics.crawlers[item.crawlerName].title }}:
                {{ item.content }}
              </v-list-item-content>
            </v-list-item>
          </v-list>
        </v-col>
      </v-row>
    </v-container>
  </v-container>
</template>

<script>
import _ from 'lodash'

import { getAbpErrorMessage } from '~/components/utils'
import BarChart from '~/components/BarChart'

export default {
  components: {
    BarChart,
  },
  data() {
    return {
      summary: null,
      summaryError: null,
      crawlers: {},
    }
  },
  computed: {
    workerSummaryList() {
      // module not loaded
      if (!this.summary) {
        return []
      }

      const res = []
      for (const summary of this.summary.queryCrawlerSummaries) {
        const usernames = _.map(summary.usernames, item => {
          if (item.fromCrawlerName) {
            return `[${item.username} in ${this.crawlers[item.fromCrawlerName].title}]`
          } else {
            return item.username
          }
        })
        const isVirtualJudge = this.crawlers[summary.crawlerName].virtual_judge
        res.push({
          crawler: this.crawlers[summary.crawlerName].title + (isVirtualJudge ? ' (Not Merged)' : ''),
          username: usernames.join(', '),
          solved: summary.solved,
          submissions: summary.submission,
        })
      }
      return res
    },
    chartData() {
      const solvedList = _.map(this.workerSummaryList, 'solved')
      const submissionsList = _.map(this.workerSummaryList, 'submissions')
      return {
        labels: _.map(this.workerSummaryList, 'crawler'),
        datasets: [
          {
            label: 'solved',
            data: solvedList,
            backgroundColor: '#6699ff',
          },
          {
            label: 'submissions',
            data: submissionsList,
            backgroundColor: '#3d3d5c',
          },
        ],
      }
    },
  },
  async fetch() {

    try {

      const crawlers = await this.$axios.$get('/api/crawlers')
      this.crawlers = crawlers.data

      const summaryResult = await this.$axios.$get('/api/services/app/QueryHistory/GetQuerySummary', {
        params: {
          queryHistoryId: this.$route.params.id,
        },
      })
      this.summary = summaryResult.result
    } catch (err) {
      this.summaryError = getAbpErrorMessage(err)
    }
  },
}
</script>
