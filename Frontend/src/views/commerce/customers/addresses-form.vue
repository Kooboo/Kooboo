<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import type { Address } from "@/api/commerce/customer";
import CreateAddressDialog from "./create-address-dialog.vue";
import EditAddressDialog from "./edit-address-dialog.vue";

const { t } = useI18n();
const showCreateAddressDialog = ref(false);
const showEditAddressDialog = ref(false);
const selected = ref<Address>();

const props = defineProps<{
  list: Address[];
}>();

function onCreate(value: Address) {
  props.list.forEach((m) => (m.isDefault = false));
  props.list.push(value);
}

function onUpdate(value: Address) {
  var index = props.list.indexOf(selected.value!);
  props.list.splice(index, 1, value);
}

function setAsDefault(value: Address) {
  props.list.forEach((m) => (m.isDefault = false));
  value.isDefault = true;
}

function onDelete(index: number) {
  props.list.splice(index, 1);
  if (props.list.length && props.list.every((e) => !e.isDefault)) {
    props.list[0].isDefault = true;
  }
}
</script>

<template>
  <div class="w-full">
    <div class="space-y-8">
      <div
        v-for="(item, index) of list"
        :key="index"
        class="border border-gray p-12 rounded-normal space-y-4"
      >
        <div class="flex items-center space-x-8">
          <ElTag v-if="item.isDefault" round type="success">{{
            t("common.default")
          }}</ElTag>
          <ElTag
            v-else
            round
            type="info"
            class="cursor-pointer"
            @click="setAsDefault(item)"
            >{{ t("common.setAsDefault") }}</ElTag
          >
          <div class="flex-1" />
          <el-tooltip placement="top" :content="t('common.edit')">
            <el-icon
              class="iconfont icon-a-writein hover:text-blue text-l"
              @click="
                selected = item;
                showEditAddressDialog = true;
              "
            />
          </el-tooltip>
          <el-tooltip placement="top" :content="t('common.delete')">
            <el-icon
              class="iconfont icon-delete text-orange text-l"
              @click="onDelete(index)"
            />
          </el-tooltip>
        </div>
        <div class="p-4">
          <div>{{ item.firstName }} {{ item.lastName }} {{ item.phone }}</div>
          <div class="text-s">
            {{ item.country }} {{ item.city }} {{ item.province }}
            {{ item.zip }} {{ item.address1 }}
          </div>
        </div>
      </div>
      <el-button round type="primary" @click="showCreateAddressDialog = true">
        <div class="flex items-center">
          <el-icon class="mr-16 iconfont icon-a-addto" />
          {{ t("common.addAddress") }}
        </div>
      </el-button>
    </div>
    <CreateAddressDialog
      v-if="showCreateAddressDialog"
      v-model="showCreateAddressDialog"
      @create="onCreate"
    />
    <EditAddressDialog
      v-if="showEditAddressDialog"
      v-model="showEditAddressDialog"
      :model="selected!"
      @update="onUpdate"
    />
  </div>
</template>
