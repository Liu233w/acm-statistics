import { Line, mixins } from 'vue-chartjs'

const { reactiveProp } = mixins

export default {
  extends: Line,
  mixins: [reactiveProp],
  props: {
    options: {
      type: Object,
      default: {
        responsive: true,
        maintainAspectRatio: false,
      },
    },
  },
  mounted() {
    this.renderChart(this.chartData, this.options)
  },
}
