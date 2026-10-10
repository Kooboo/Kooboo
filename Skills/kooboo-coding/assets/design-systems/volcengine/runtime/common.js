// Yuanli Vue runtime. Native ESM, shared with the App Base Vue instance.
import { computed, h, nextTick, onBeforeUnmount, onMounted, ref, useId, watch } from 'vue'
import bundledIcons from '/yuanli-icons.js'
import { loadLucide } from '/lucide.js'

export const version = '0.10.1'
const sizes = ['lg', 'md', 'sm', 'mini']
const oneOf = values => value => values.includes(value)
