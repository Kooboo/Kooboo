<script lang="ts" setup>
import { computed, ref, watch } from "vue";
import {
  getServers,
  getRemoteSites,
  post,
  getRemoteDomains,
} from "@/api/publish";
import type { UserPublish } from "@/api/publish/types";
import type { KeyValue } from "@/global/types";
import DialogFooterBar from "@/components/dialog-footer-bar/index.vue";
import { useI18n } from "vue-i18n";
import {
  rangeRule,
  requiredRule,
  simpleNameRule,
  subDomainRule,
} from "@/utils/validate";

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
  (e: "reload"): void;
}>();

defineProps<{ modelValue: boolean }>();
const { t } = useI18n();
const show = ref(true);
const servers = ref<UserPublish[]>([]);
const sites = ref<KeyValue[]>();
const isSubDomainEdit = ref(false);
const isSiteNameEdit = ref(false);

const toList = ref<KeyValue[]>([
  {
    key: "exist",
    value: t("common.existingSite"),
  },
  {
    key: "new",
    value: t("common.newSite"),
  },
]);

const model = ref({
  serverId: "",
  to: toList.value[0].key,
  siteId: "",
  rootDomain: "",
  siteName: "",
  subDomain: "",
});

const form = ref();

const loadServers = async () => {
  servers.value = await getServers();
};

loadServers();

const rules = computed(() => {
  var data = {
    serverId: [{ type: "string", required: true }],
  } as any;

  if (model.value.serverId) {
    if (model.value.to == "exist") {
      Object.assign(data, {
        siteId: [{ type: "string", required: true }],
      });
    } else {
      Object.assign(data, {
        subDomain: [
          requiredRule(t("common.subDomainRequiredTips")),
          rangeRule(1, 63),
          subDomainRule,
        ],
        rootDomain: requiredRule(t("common.rootDomainRequiredTips")),
        siteName: [
          rangeRule(1, 50),
          requiredRule(t("common.nameRequiredTips")),
          simpleNameRule(),
        ],
      });
    }
  }

  return data;
});

const onServerChange = async () => {
  if (!model.value.serverId) return;
  const server = servers.value.find((f) => f.id == model.value.serverId)!;
  try {
    sites.value = await getRemoteSites(server.serverUrl, server.orgId);
    domains.value = await getRemoteDomains(server.serverUrl, server.orgId);
    model.value.rootDomain = domains.value[0]?.value;
  } catch (error) {
    sites.value = undefined as any;
  }
};

const domains = ref<KeyValue[]>([]);

const onSave = async () => {
  await form.value.validate();
  const server = servers.value.find((f) => f.id == model.value.serverId)!;

  if (model.value.to === "exist") {
    const site = sites.value!.find((f) => f.key == model.value.siteId)!;
    await post({
      remoteServerUrl: server.serverUrl,
      remoteWebSiteId: site.key,
      remoteSiteName: site.value,
      orgId: server.orgId,
      serverName: server.name,
    });
  } else {
    await post({
      remoteServerUrl: server.serverUrl,
      FullDomain: `${model.value.subDomain}.${model.value.rootDomain}`,
      SiteName: model.value.siteName,
      orgId: server.orgId,
      serverName: server.name,
    });
  }

  emit("reload");
  show.value = false;
};

watch(
  () => model.value.subDomain,
  () => {
    if (isSiteNameEdit.value) return;
    model.value.siteName = model.value.subDomain;
  }
);

watch(
  () => model.value.siteName,
  () => {
    if (isSubDomainEdit.value) return;
    model.value.subDomain = model.value.siteName;
  }
);
</script>

<template>
  <el-dialog
    :model-value="show"
    width="600px"
    :close-on-click-modal="false"
    :title="t('common.newSync')"
    @closed="emit('update:modelValue', false)"
  >
    <el-form
      ref="form"
      label-position="top"
      :model="model"
      :rules="rules"
      :validate-on-rule-change="false"
    >
      <el-form-item :label="t('common.server')" prop="serverId">
        <div class="flex items-center space-x-8 w-full">
          <el-select
            v-model="model.serverId"
            class="w-full"
            data-cy="servers"
            @change="onServerChange"
          >
            <el-option
              v-for="item of servers"
              :key="item.id"
              :label="`${item.name} (${item.serverUrl})`"
              :value="item.id"
              data-cy="server-opt"
            />
          </el-select>
        </div>
      </el-form-item>
      <template v-if="model.serverId && sites">
        <el-form-item>
          <el-radio-group
            v-model="model.to"
            class="el-radio-group--rounded h-38px"
          >
            <el-radio-button
              v-for="item of toList"
              :key="item.key"
              :label="item.key"
              >{{ item.value }}</el-radio-button
            >
          </el-radio-group>
        </el-form-item>
        <el-form-item
          v-if="model.to == 'exist'"
          :label="t('common.siteName')"
          prop="siteId"
        >
          <el-select v-model="model.siteId" class="w-full" data-cy="sites">
            <el-option
              v-for="item of sites"
              :key="item.key"
              :label="item.value"
              :value="item.key"
              data-cy="site-opt"
            />
          </el-select>
        </el-form-item>

        <template v-if="model.to == 'new'">
          <el-form-item :label="t('common.siteName')" prop="siteName">
            <el-input
              v-model="model.siteName"
              data-cy="site-name"
              @input="isSiteNameEdit = true"
            />
          </el-form-item>

          <div class="flex items-center space-x-4">
            <div class="flex-1">
              <el-form-item :label="t('common.domain')" prop="subDomain">
                <el-input
                  v-model="model.subDomain"
                  data-cy="site-subDomain"
                  @input="isSubDomainEdit = true"
                />
              </el-form-item>
            </div>

            <el-form-item prop="rootDomain" class="mt-30px">
              <el-select v-model="model.rootDomain" class="w-full">
                <el-option
                  v-for="item of domains"
                  :key="item.key"
                  :value="item.value"
                  :label="item.value"
                  data-cy="root-domain"
                />
              </el-select>
            </el-form-item>
          </div>
        </template>
      </template>
    </el-form>
    <template #footer>
      <DialogFooterBar
        :disabled="!sites"
        @confirm="onSave"
        @cancel="show = false"
      />
    </template>
  </el-dialog>
</template>
