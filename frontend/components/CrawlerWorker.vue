<template>
  <v-card>
    <v-card-title primary-title>
      <div class="headline">{{ workerName }}</div>
    </v-card-title>
    <v-container>
      <v-layout row wrap>
        <v-flex xs12>
          <v-text-field
            v-model="username"
            label="Username"
            :disabled="status === WORKER_STATUS.WORKING"
            required
          ></v-text-field>
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
               @click="status = WORKER_STATUS.WORKING"
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
      }
    },
    data() {
      return {
        errorMessage: '',
        WORKER_STATUS: WORKER_STATUS
      }
    },
    watch: {
      status: function (val) {
        if (val === WORKER_STATUS.WORKING) {
          // 启动爬虫
          // TODO: 补充启动爬虫的代码
          const self = this
          setTimeout(() => {
            self.$emit('update:status', WORKER_STATUS.DONE)
            if (Math.random() < 0.5) {
              // 这里暂时模拟成功和失败
              self.$emit('update:solved', 100)
              self.$emit('update:submissions', 200)
            } else {
              self.errorMessage = '错误信息： xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx'
            }
          }, 2000)
        }
      }
    }
  }
</script>

<style scoped>

</style>
