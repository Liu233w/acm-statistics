<template>
  <VContainer>
    <VLayout row>
      <VFlex xs12 sm6 offset-sm3>
        <VCard>
          <VToolbar color="light-blue" dark>
            <VToolbarTitle>关于</VToolbarTitle>
            <VSpacer />
          </VToolbar>
          <VList>
            <VListTile>
              <VListTileContent>
                <VListTileTitle>版本号</VListTileTitle>
                <VListTileSubTitle>{{ $env.VERSION_NUM }}</VListTileSubTitle>
              </VListTileContent>
            </VListTile>

            <VListTile>
              <VListTileContent>
                <VListTileTitle>构建时间</VListTileTitle>
                <VListTileSubTitle>{{ buildTime }}</VListTileSubTitle>
              </VListTileContent>
            </VListTile>

            <VListTile :href="`https://github.com/Liu233w/acm-statistics/tree/${$env.VERSION_NUM}`">
              <VListTileContent>
                <VListTileTitle>当前版本源代码</VListTileTitle>
                <VListTileSubTitle>
                  https://github.com/Liu233w/acm-statistics/tree/{{ $env.VERSION_NUM }}
                </VListTileSubTitle>
              </VListTileContent>
            </VListTile>

            <VListTile :href="`https://github.com/Liu233w/acm-statistics/commits/${$env.VERSION_NUM}`">
              <VListTileContent>
                <VListTileTitle>最近更新</VListTileTitle>
                <VListTileSubTitle>
                  https://github.com/Liu233w/acm-statistics/commits/{{ $env.VERSION_NUM }}
                </VListTileSubTitle>
              </VListTileContent>
            </VListTile>
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
