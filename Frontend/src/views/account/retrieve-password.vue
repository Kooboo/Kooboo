<script lang="ts" setup>
import { ref, watch } from "vue";
import { useRoute } from "vue-router";
import { getQueryString } from "@/utils/url";
import PhonePanel from "./retrieve-panels/phone-panel.vue";
import EmailPanel from "./retrieve-panels/email-panel.vue";

const route = useRoute();
const retrieveTypeEnum = ["phone", "email"];
const retrieveType = ref(getRetrieveType());

watch(
  () => route.query,
  () => {
    retrieveType.value = getRetrieveType();
  }
);

function getRetrieveType() {
  const retrieveType = getQueryString("type") || "phone";
  if (retrieveTypeEnum.includes(retrieveType)) {
    return retrieveType;
  } else {
    return "phone";
  }
}
</script>

<template>
  <div class="h-full overflow-hidden">
    <el-scrollbar>
      <div class="retrieve-password">
        <PhonePanel v-if="retrieveType === 'phone'" />
        <EmailPanel v-else-if="retrieveType === 'email'" />
      </div>
    </el-scrollbar>
  </div>
</template>

<style lang="scss" scoped>
.retrieve-password {
  &--nav {
    margin: 20px 0;
    .iconfont {
      margin-right: 7px;
    }
    cursor: pointer;
  }
  &--protocol {
    margin-top: 8px;
  }
}
</style>
