<template>
  <v-card>
    <v-card-title primary-title>
      <div class="headline">{{ workerName }}</div>
    </v-card-title>
    <v-container>
      <v-layout row wrap>
        <v-flex xs12>
          <v-text-field
            v-model="localUsername"
            label="Username"
            :disabled="status === WORKER_STATUS.WORKING"
            required
            @keyup.enter="$emit('update:status', WORKER_STATUS.WORKING)"
          />
        </v-flex>
      </v-layout>
    </v-container>
    <v-card-text v-show="status === WORKER_STATUS.DONE" >
      <template v-if="errorMessage">
        <span class="red--text">{{ errorMessage }}</span>
      </template>
      <template v-else>
        <span class="grey--text">SOLVED: </span> {{ solved }}
        <br/>
        <span class="grey--text">SUBMISSIONS: </span> {{ submissions }}
      </template>
    </v-card-text>
    <v-card-text v-show="status === WORKER_STATUS.WORKING">
      <v-container>
        <v-layout row>
          <v-spacer></v-spacer>
          <v-flex xs2>
            <v-progress-circular indeterminate color="primary"></v-progress-circular>
          </v-flex>
          <v-spacer></v-spacer>
        </v-layout>
      </v-container>
    </v-card-text>
    <v-card-actions>
      <v-spacer></v-spacer>
      <v-tooltip bottom>
        <v-btn icon
               slot="activator"
               :disabled="status === WORKER_STATUS.WORKING"
               @click="$emit('update:status', WORKER_STATUS.WORKING)"
        >
          <v-icon>refresh</v-icon>
        </v-btn>
        <span>重新爬取此处信息</span>
      </v-tooltip>
    </v-card-actions>
  </v-card>
</template>

<script>
  import {WORKER_STATUS} from './consts'

  export default {
    name: "crawler-worker",
    props: {
      status: {
        type: String,
        default: WORKER_STATUS.WAITING
      },
      username: {
        type: String,
        required: true
      },
      solved: {
        type: Number,
        default: 0
      },
      submissions: {
        type: Number,
        default: 0
      },
      workerName: {
        type: String,
        required: true
      },
      func: {
        type: Function,
        required: true
      }
    },
    data() {
      return {
        localUsername: this.username,
        errorMessage: '',
        WORKER_STATUS: WORKER_STATUS
      }
    },
    watch: {
      status: async function (val) {
        if (val === WORKER_STATUS.WORKING) {
          // 启动爬虫
          try {
            const res = await this.func(this.localUsername)
            this.$emit('update:status', WORKER_STATUS.DONE)
            this.$emit('update:solved', res.solved)
            this.$emit('update:submissions', res.submissions)
          } catch (err) {
            this.$emit('update:status', WORKER_STATUS.DONE)
            this.errorMessage = err.message
          }
        }
      },
      username: function(val) {
        this.localUsername = val
      },
      localUsername: function(val) {
        this.$emit('update:status', WORKER_STATUS.WAITING)
        this.$emit('update:solved', 0)
        this.$emit('update:submissions', 0)
      }
    }
  }
</script>

<style scoped>

</style>
