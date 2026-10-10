export const YlUpload = {
  name: 'YlUpload',
  props: {
    modelValue: { type: Array, default: () => [] },
    request: Function,
    accept: { type: String, default: '' },
    multiple: Boolean, disabled: Boolean,
    autoUpload: { type: Boolean, default: true },
    maxSize: { type: Number, default: Infinity },
    limit: { type: Number, default: Infinity },
    listType: { type: String, default: 'text', validator: oneOf(['text', 'picture']) },
    label: { type: String, default: '选择文件' },
    hint: { type: String, default: '也可以将文件拖到这里' }
  },
  emits: ['update:modelValue', 'reject', 'success', 'error', 'remove'],
  setup(props, { emit, expose, slots }) {
    const input = ref(null)
    const files = ref([...props.modelValue])
    const message = ref('')
    const dragging = ref(false)
    const tasks = new Map()
    const previews = new Map()
    const prefix = useId()
    let serial = 0, disposed = false
    const release = id => {
      const task = tasks.get(id)
      tasks.delete(id)
      task?.abort()
      const url = previews.get(id)
      if (url) URL.revokeObjectURL(url)
      previews.delete(id)
    }
    const publish = next => { files.value = next; emit('update:modelValue', next) }
    const patch = (id, values) => publish(files.value.map(item => item.id === id ? { ...item, ...values } : item))
    watch(() => props.modelValue, next => {
      for (const item of files.value) {
        const replacement = next.find(candidate => candidate.id === item.id)
        if (!replacement || replacement.raw !== item.raw) release(item.id)
      }
      files.value = [...next]
    }, { flush: 'sync' })
    const cancel = id => {
      if (props.disabled || !tasks.has(id)) return
      const task = tasks.get(id)
      tasks.delete(id)
      task.abort()
      patch(id, { status: 'ready', progress: 0, error: '' })
    }
    const upload = async id => {
      const item = files.value.find(file => file.id === id)
      if (disposed || props.disabled || !props.request || !item?.raw || tasks.has(id) || item.status === 'success') return
      const task = new AbortController()
      tasks.set(id, task)
      const current = () => !disposed && tasks.get(id) === task && files.value.some(file => file.id === id && file.raw === item.raw)
      patch(id, { status: 'uploading', progress: 0, error: '' })
      try {
        const response = await props.request(item.raw, { signal: task.signal, onProgress: value => {
          if (current() && Number.isFinite(value)) patch(id, { progress: Math.round(Math.max(0, Math.min(99, value))) })
        } })
        if (!current()) return
        patch(id, { status: 'success', progress: 100, response })
        emit('success', { ...item, status: 'success', progress: 100, response })
      } catch (error) {
        if (!current()) return
        const reason = error?.message || '上传失败，请重试'
        patch(id, { status: 'error', error: reason })
        emit('error', { file: item, error })
      } finally {
        if (tasks.get(id) === task) tasks.delete(id)
      }
    }
    const remove = id => {
      if (props.disabled) return
      const item = files.value.find(file => file.id === id)
      if (!item) return
      release(id)
      publish(files.value.filter(file => file.id !== id))
      emit('remove', item)
    }
    const selectFiles = selected => {
      if (disposed || props.disabled) return
      const next = [...files.value], added = [], errors = []
      const rules = props.accept.toLowerCase().split(',').map(rule => rule.trim()).filter(Boolean)
      const selectedFiles = Array.from(selected || [])
      for (const [index, raw] of selectedFiles.entries()) {
        let reason = ''
        if (!props.multiple && index > 0) reason = '每次只能选择一个文件'
        else if (next.length >= props.limit) reason = '已达到文件数量上限'
        else if (raw.size > props.maxSize) reason = '文件大小超过限制'
        else if (rules.length && !rules.some(rule => rule.startsWith('.') ? raw.name.toLowerCase().endsWith(rule)
          : rule.endsWith('/*') ? raw.type.toLowerCase().startsWith(rule.slice(0, -1)) : raw.type.toLowerCase() === rule)) reason = '文件类型不符合要求'
        if (reason) { errors.push(raw.name + '：' + reason); emit('reject', { file: raw, reason }); continue }
        const item = { id: `${prefix}-file-${++serial}`, name: raw.name, size: raw.size, type: raw.type, raw, status: 'ready', progress: 0, error: '' }
        if (/^image\/(png|jpeg|gif|webp|avif|bmp)$/i.test(raw.type) && typeof URL !== 'undefined' && typeof URL.createObjectURL === 'function') {
          previews.set(item.id, URL.createObjectURL(raw))
        }
        next.push(item); added.push(item)
      }
      message.value = errors.join('；')
      publish(next)
      if (props.autoUpload && props.request) added.forEach(item => upload(item.id))
    }
    onBeforeUnmount(() => { disposed = true; for (const id of new Set([...tasks.keys(), ...previews.keys()])) release(id) })
    expose({ upload, cancel, remove, selectFiles })
    const action = (label, handler, name) => h(YlButton, { size: 'sm', variant: 'text', disabled: props.disabled, 'aria-label': `${label} ${name}`, onClick: handler }, () => label)
    const labels = { ready: '待上传', uploading: '上传中', success: '上传成功', error: '上传失败' }
    return () => h('div', { class: ['yl-upload', { 'yl-upload-disabled': props.disabled }] }, [
      h('div', { class: ['yl-upload-dropzone', { 'yl-upload-dragging': dragging.value && !props.disabled }],
        onDragover: event => { event.preventDefault(); if (!props.disabled) dragging.value = true },
        onDragleave: () => { dragging.value = false },
        onDrop: event => { event.preventDefault(); dragging.value = false; selectFiles(event.dataTransfer?.files) }
      }, [
        h('input', { ref: input, type: 'file', hidden: true, accept: props.accept, multiple: props.multiple, disabled: props.disabled,
          onChange: event => { selectFiles(event.target.files); event.target.value = '' } }),
        h(YlButton, { disabled: props.disabled, icon: 'plus', onClick: () => input.value?.click() }, () => props.label),
        h('div', { class: 'yl-upload-hint' }, slots.default?.() || props.hint)
      ]),
      h('p', { class: 'yl-upload-error', role: 'status' }, message.value),
      h('ul', { class: ['yl-upload-list', `yl-upload-list-${props.listType}`] }, files.value.map(item => h('li', { key: item.id, class: 'yl-upload-item' }, [
        props.listType === 'picture' && previews.has(item.id) ? h('img', { class: 'yl-upload-preview', src: previews.get(item.id), alt: item.name }) : null,
        h('div', { class: 'yl-upload-details' }, [
          h('div', { class: 'yl-upload-name' }, item.name),
          h('div', { class: 'yl-upload-status', 'aria-live': 'polite' }, `${Math.ceil(item.size / 1024)} KB · ${!props.request && item.status === 'ready' ? '已选择，尚未上传' : labels[item.status] || '已选择'}`),
          item.status === 'uploading' ? h(YlProgress, { value: item.progress, label: `${item.name} 上传进度` }) : null,
          item.status === 'error' ? h('div', { class: 'yl-upload-error', role: 'status' }, item.error) : null
        ]),
        h('div', { class: 'yl-upload-actions' }, [
          tasks.has(item.id) ? action('取消', () => cancel(item.id), item.name)
            : props.request && item.raw && item.status !== 'success' ? action(item.status === 'error' ? '重试' : '上传', () => upload(item.id), item.name) : null,
          action('移除', () => remove(item.id), item.name)
        ])
      ])))
    ])
  }
}
