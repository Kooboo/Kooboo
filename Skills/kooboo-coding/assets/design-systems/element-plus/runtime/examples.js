// Acceptance compositions: never installed as business fixtures.
import { computed, ref } from 'vue';
import { ElInput, ElButton, ElTable, ElTableColumn, ElForm, ElFormItem, ElDatePicker, ElConfigProvider, epZhCn, ElContainer, ElHeader, ElAside, ElMain, ElMenu, ElMenuItem, ElAlert } from '/element-plus.js';

export const RecordEditor = {
  name: 'RecordEditor',
  components: { ElForm, ElFormItem, ElInput, ElButton, ElAlert },
  props: { save: { type: Function, required: true } },
  emits: ['saved'],
  setup(props, { emit }) {
    const form = ref(null), model = ref({ name: '' }), busy = ref(false), error = ref('');
    const rules = { name: [{ required: true, message: '请输入名称', trigger: 'blur' }] };
    async function submit() {
      if (busy.value) return;
      try { await form.value.validate(); } catch { return; }
      busy.value = true; error.value = '';
      try { const saved = await props.save({ ...model.value }); emit('saved', saved); }
      catch (failure) { error.value = failure.message || '保存失败'; }
      finally { busy.value = false; }
    }
    return { form, model, rules, busy, error, submit };
  },
  template: `<ElForm ref="form" :model="model" :rules="rules" label-position="top"><ElFormItem label="名称" prop="name"><ElInput v-model="model.name" /></ElFormItem><ElAlert v-if="error" :title="error" type="error" :closable="false" /><ElButton type="primary" :loading="busy" @click="submit">保存</ElButton></ElForm>`
};

export const SearchableTable = {
  name: 'SearchableTable', components: { ElInput, ElTable, ElTableColumn },
  props: { records: { type: Array, default: () => [] } },
  setup(props) { const keyword = ref(''); return { keyword, rows: computed(() => props.records.filter(row => row.name.includes(keyword.value))) }; },
  template: `<ElInput v-model="keyword" clearable aria-label="搜索记录" /><ElTable :data="rows" row-key="id" empty-text="没有匹配记录"><ElTableColumn prop="name" label="名称" /><ElTableColumn prop="status" label="状态" /></ElTable>`
};

export const LocalDateRange = {
  components: { ElDatePicker, ElConfigProvider },
  setup() { return { dates: ref([]), epZhCn }; },
  template: `<ElConfigProvider :locale="epZhCn"><ElDatePicker v-model="dates" type="daterange" value-format="YYYY-MM-DD" start-placeholder="开始日期" end-placeholder="结束日期" /><output>{{ JSON.stringify(dates) }}</output></ElConfigProvider>`
};

export const EditorLayout = {
  components: { ElContainer, ElHeader, ElAside, ElMain, ElMenu, ElMenuItem },
  setup() { return { active: ref('content') }; },
  template: `<ElContainer direction="vertical"><ElHeader>内容编辑</ElHeader><ElContainer><ElAside width="180px"><ElMenu :default-active="active" @select="active = $event"><ElMenuItem index="content">内容</ElMenuItem><ElMenuItem index="settings">设置</ElMenuItem></ElMenu></ElAside><ElMain><slot :active="active" /></ElMain></ElContainer></ElContainer>`
};
