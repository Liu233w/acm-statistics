import { Bar, mixins } from 'vue-chartjs'

const { reactiveProp } = mixins

export default {
  extends: Bar,
  mixins: [reactiveProp],
  data() {
    return {
      options: {
        responsive: true,
      },
    }
  },
  mounted() {
    this.renderChart(this.chartData, this.options)
  },
}
