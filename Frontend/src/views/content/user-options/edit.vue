<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import type { Schema } from "./user-options";
import { useRouter } from "vue-router";
import { useRouteSiteId } from "@/hooks/use-site-id";
import { updateOptions, getUserOptions } from "@/api/content/user-options";
import { getQueryString } from "@/utils/url";
import ObjectOption from "./object-option.vue";
import { errorMessage } from "@/components/basic/message";

const { t } = useI18n();
const schema = ref<Schema[]>([]);
const router = useRouter();
const data = ref<any>({});
const id = getQueryString("id")!;
const showCodeDialog = ref(false);
const code = ref("{}");

getUserOptions(id).then((rsp) => {
  schema.value = JSON.parse(rsp.schema);
  data.value = rsp.data ? JSON.parse(rsp.data) : {};
});

function goBack() {
  router.goBackOrTo(useRouteSiteId({ name: "useroptions" }));
}

async function save() {
  await updateOptions({
    id: id,
    setting: JSON.stringify(data.value),
  });
  goBack();
}

function onShowCodeDialog() {
  code.value = JSON.stringify(data.value, null, 4);
  showCodeDialog.value = true;
}

function onCodeSave() {
  data.value = JSON.parse(code.value);
  showCodeDialog.value = false;
}
</script>

<template>
  <div class="p-24 mb-64">
    <el-form label-position="top">
      <div
        class="rounded-normal bg-fff dark:bg-[#252526] mt-16 py-24 px-56px relative"
      >
        <IconButton
          icon="icon-code"
          :tip="t('common.editCode')"
          class="absolute top-24 right-24"
          @click="onShowCodeDialog"
        />
        <ObjectOption :schemas="schema" :data="data" />
      </div>
    </el-form>

    <el-dialog
      v-model="showCodeDialog"
      width="800px"
      :close-on-click-modal="false"
      :title="t('common.editCode')"
    >
      <div v-if="showCodeDialog" @keydown.enter="onCodeSave">
        <MonacoEditor v-model="code" language="json" />
      </div>

      <template #footer>
        <DialogFooterBar
          @confirm="onCodeSave"
          @cancel="showCodeDialog = false"
        />
      </template>
    </el-dialog>

    <KBottomBar
      :permission="{
        feature: 'userOptions',
        action: 'edit',
      }"
      @cancel="goBack"
      @save="save"
    />
  </div>
</template>
