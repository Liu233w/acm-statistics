<template>
  <VContainer>
    <VLayout>
      <VFlex xs12 sm6 offset-sm3>
        <VCard>
          <VAppBar color="light-blue" dark>
            <VAppBarTitle>关于</VAppBarTitle>
            <VSpacer />
          </VAppBar>
          <VList>
            <VListItem>
              <VListItemContent>
                <VListItemTitle>版本号</VListItemTitle>
                <VListItemSubTitle>{{ $env.VERSION_NUM }}</VListItemSubTitle>
              </VListItemContent>
            </VListItem>

            <VListItem>
              <VListItemContent>
                <VListItemTitle>构建时间</VListItemTitle>
                <VListItemSubTitle>{{ buildTime }}</VListItemSubTitle>
              </VListItemContent>
            </VListItem>

            <VListItem :href="`https://github.com/Liu233w/acm-statistics/tree/${$env.VERSION_NUM}`">
              <VListItemContent>
                <VListItemTitle>当前版本源代码</VListItemTitle>
                <VListItemSubTitle>
                  https://github.com/Liu233w/acm-statistics/tree/{{ $env.VERSION_NUM }}
                </VListItemSubTitle>
              </VListItemContent>
            </VListItem>

            <VListItem :href="`https://github.com/Liu233w/acm-statistics/commits/${$env.VERSION_NUM}`">
              <VListItemContent>
                <VListItemTitle>最近更新</VListItemTitle>
                <VListItemSubTitle>
                  https://github.com/Liu233w/acm-statistics/commits/{{ $env.VERSION_NUM }}
                </VListItemSubTitle>
              </VListItemContent>
            </VListItem>
          </VList>
        </VCard>
      </VFlex>
    </VLayout>
  </VContainer>
</template>

<script>
  import {PROJECT_TITLE} from '~/components/consts'
  import {getDateFromTimestamp} from '../components/utils'

  export default {
    data() {
      return {
        buildTime: getDateFromTimestamp(this.$env.BUILD_TIME).toLocaleString(),
      }
    },
    head: {
      title: `关于 - ${PROJECT_TITLE}`,
    },
  }
</script>

<style scoped>

</style>
