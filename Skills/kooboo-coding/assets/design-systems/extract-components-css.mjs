import fs from 'node:fs'
import path from 'node:path'
import { fileURLToPath } from 'node:url'

const root = path.dirname(fileURLToPath(import.meta.url))
const generatedSystems = [
  '21th',
  'apple',
  'barbie',
  'claude',
  'doubao',
  'golden-time',
  'google',
  'minimalist',
  'motion-fit',
  'nerv',
  'tiktok',
  'vercel',
  'vibe-camp'
]
const write = process.argv.includes('--write')
const catalogSystems = JSON.parse(fs.readFileSync(path.join(root, 'catalog.json'), 'utf8'))
  .systems.map(system => system.id)
const rootClassOverrides = new Map([
  ['golden-time/data-table', ['shell-table']],
  ['golden-time/input', ['input-preview']],
  ['golden-time/sidebar-navigation', ['sidebar-demo']]
])
const safeScaffoldNames = new Set([
  'preview',
  'section',
  'row',
  'label',
  'divider',
  'demo',
  'demo-row',
  'demo-label',
  'demo-grid',
  'example',
  'examples',
  'sample',
  'samples',
  'stage',
  'showcase',
  'variant',
  'variants'
])

function readStyle(html, file) {
  const blocks = [...html.matchAll(/<style(?:\s[^>]*)?>([\s\S]*?)<\/style>/gi)]
  if (blocks.length === 0) throw new Error(`No inline style found in ${file}`)
  return blocks.map(match => match[1]).join('\n')
    .replace(/\/\*\s*@component-css-(?:start|end)\s*\*\//g, '')
    .trim()
}

function extractRootUsage(html, rootClasses) {
  const roots = new Set(rootClasses)
  const blocks = []
  const stack = []
  const tags = /<\/?([a-zA-Z][\w:-]*)([^>]*)>/g
  const voidTags = new Set(['area', 'base', 'br', 'col', 'embed', 'hr', 'img', 'input', 'link', 'meta', 'source', 'track', 'wbr'])
  let match
  while ((match = tags.exec(html)) !== null) {
    const raw = match[0]
    const tag = match[1].toLowerCase()
    if (raw.startsWith('</')) {
      for (let index = stack.length - 1; index >= 0; index -= 1) {
        if (stack[index].tag !== tag) continue
        const frame = stack[index]
        stack.length = index
        if (frame.isRoot) blocks.push(html.slice(frame.start, tags.lastIndex))
        break
      }
      continue
    }
    const classMatch = match[2].match(/\bclass\s*=\s*(["'])(.*?)\1/i)
    const classes = new Set(classMatch ? classMatch[2].split(/\s+/).filter(Boolean) : [])
    const frame = { tag, start: match.index, isRoot: [...roots].some(name => classes.has(name)) }
    if (raw.endsWith('/>') || voidTags.has(tag)) {
      if (frame.isRoot) blocks.push(raw)
    } else {
      stack.push(frame)
    }
  }
  if (blocks.length === 0) throw new Error(`Component root ${rootClasses.join(', ')} was not found`)

  const allowedClasses = new Set()
  const allowedTags = new Set()
  for (const block of blocks) {
    for (const classMatch of block.matchAll(/\bclass\s*=\s*(["'])(.*?)\1/gi)) {
      for (const name of classMatch[2].split(/\s+/).filter(Boolean)) allowedClasses.add(name)
    }
    for (const tagMatch of block.matchAll(/<([a-zA-Z][\w:-]*)\b/g)) allowedTags.add(tagMatch[1].toLowerCase())
  }
  return { allowedClasses, allowedTags }
}

function findMatchingBrace(css, openIndex) {
  let depth = 1
  let quote = ''
  let comment = false
  for (let index = openIndex + 1; index < css.length; index += 1) {
    const char = css[index]
    const next = css[index + 1]
    if (comment) {
      if (char === '*' && next === '/') {
        comment = false
        index += 1
      }
      continue
    }
    if (quote) {
      if (char === '\\') index += 1
      else if (char === quote) quote = ''
      continue
    }
    if (char === '/' && next === '*') {
      comment = true
      index += 1
    } else if (char === '"' || char === "'") {
      quote = char
    } else if (char === '{') {
      depth += 1
    } else if (char === '}') {
      depth -= 1
      if (depth === 0) return index
    }
  }
  throw new Error('Unbalanced CSS braces')
}

function splitSelectors(prelude) {
  const selectors = []
  let start = 0
  let parenDepth = 0
  let bracketDepth = 0
  let quote = ''
  for (let index = 0; index < prelude.length; index += 1) {
    const char = prelude[index]
    if (quote) {
      if (char === '\\') index += 1
      else if (char === quote) quote = ''
      continue
    }
    if (char === '"' || char === "'") quote = char
    else if (char === '(') parenDepth += 1
    else if (char === ')') parenDepth -= 1
    else if (char === '[') bracketDepth += 1
    else if (char === ']') bracketDepth -= 1
    else if (char === ',' && parenDepth === 0 && bracketDepth === 0) {
      selectors.push(prelude.slice(start, index).trim())
      start = index + 1
    }
  }
  selectors.push(prelude.slice(start).trim())
  return selectors.filter(Boolean)
}

function isPreviewSelector(selector, scaffoldNames) {
  if (/^(?:html|body|:root)(?:\b|[.#:\[])/i.test(selector)) return true
  if (/^\*(?:::{0,2}[\w-]+)?$/.test(selector)) return true
  for (const name of scaffoldNames) {
    if (new RegExp(`(^|[^\\w-])\\.${name.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')}(?![\\w-])`).test(selector)) return true
  }
  return false
}

function belongsToComponent(selector, usage) {
  if (!usage) return true
  for (const match of selector.matchAll(/\.(-?[_a-zA-Z]+[_a-zA-Z0-9-]*)/g)) {
    if (usage.allowedClasses.has(match[1])) return true
  }
  if (/\[[^\]]+\]/.test(selector)) return true
  const tagMatch = selector.match(/(^|[\s>+~])([a-zA-Z][\w-]*)\b/g) ?? []
  return tagMatch.some(value => usage.allowedTags.has(value.trim().replace(/^[>+~]\s*/, '').toLowerCase()))
}

function scopeSelector(selector, slug) {
  selector = selector.replace(/\biconify-icon\b/g, '.lucide')
  const boundary = `[data-ds-component="${slug}"]`
  const scoped = [`${boundary} ${selector}`]
  if (/^[.#\[]/.test(selector)) scoped.push(`${boundary}${selector}`)
  return scoped.join(',\n')
}

function transformCss(css, slug, scaffoldNames, usage) {
  let output = ''
  let cursor = 0
  while (cursor < css.length) {
    while (cursor < css.length && /\s/.test(css[cursor])) cursor += 1
    if (cursor >= css.length) break
    if (css.startsWith('/*', cursor)) {
      const end = css.indexOf('*/', cursor + 2)
      if (end < 0) throw new Error('Unclosed CSS comment')
      cursor = end + 2
      continue
    }
    const open = css.indexOf('{', cursor)
    if (open < 0) break
    const prelude = css.slice(cursor, open).trim()
    const close = findMatchingBrace(css, open)
    const body = css.slice(open + 1, close).trim()
    cursor = close + 1
    if (!prelude) continue

    if (prelude.startsWith('@')) {
      if (/^@(?:media|supports|container|layer)\b/i.test(prelude)) {
        const nested = transformCss(body, slug, scaffoldNames, usage)
        if (nested.trim()) output += `${prelude}{\n${nested}\n}\n`
      } else if (/^@(?:keyframes|-webkit-keyframes)\b/i.test(prelude)) {
        output += `${prelude}{${body}}\n`
      }
      continue
    }

    const selectors = splitSelectors(prelude)
      .filter(selector => !isPreviewSelector(selector, scaffoldNames))
      .filter(selector => belongsToComponent(selector, usage))
      .map(selector => scopeSelector(selector, slug))
    if (selectors.length > 0) output += `${selectors.join(',\n')}{${body}}\n`
  }
  return output.trim()
}

function titleFromSlug(slug) {
  return slug.split('-').map(part => part.charAt(0).toUpperCase() + part.slice(1)).join(' ')
}

function buildSystem(systemId) {
  const previewDir = path.join(root, systemId, 'preview')
  const files = fs.readdirSync(previewDir)
    .filter(name => /^component-.+\.html$/.test(name))
    .sort()
  if (files.length === 0) throw new Error(`No component previews found for ${systemId}`)

  const components = files.map(name => {
    const slug = name.slice('component-'.length, -'.html'.length)
    const file = path.join(previewDir, name)
    const css = readStyle(fs.readFileSync(file, 'utf8'), file)
    const roots = rootClassOverrides.get(`${systemId}/${slug}`)
    const usage = roots ? extractRootUsage(fs.readFileSync(file, 'utf8'), roots) : null
    return { slug, name, css, usage }
  })

  const sections = components.map(component => {
    const css = transformCss(component.css, component.slug, safeScaffoldNames, component.usage)
    if (!css) throw new Error(`No component CSS remained for ${systemId}/${component.slug}`)
    return `/* ${titleFromSlug(component.slug)} — source: preview/${component.name} */\n${css}`
  })
  return `/*
  ${systemId} components.css
  Generated from component previews by extract-components-css.mjs.
  Preview canvas selectors are removed and every component is isolated.
  Put data-ds-component="<slug>" on the component root or an immediate boundary.
  Preview DOM and class names are the component API; do not rename or prefix them.
  Do not edit this file manually; update the component Preview and regenerate it.
*/

${sections.join('\n\n')}
`
}

function validateCopyableCss(systemId) {
  const target = path.join(root, systemId, 'components.css')
  if (!fs.existsSync(target) || fs.statSync(target).size === 0) {
    throw new Error(`${systemId}/components.css is missing or empty`)
  }

  const css = fs.readFileSync(target, 'utf8').replace(/\/\*[\s\S]*?\*\//g, '')
  const forbidden = [
    { pattern: /(^|})\s*(?:html|body|:root)(?=[\s,.#:[>+~{])/m, label: 'document-level selector' },
    { pattern: /(^|,)\s*\.(?:pv-[\w-]*|preview|demo(?:-[\w-]+)?|showcase)(?=[\s,.#:[>+~{])/m, label: 'Preview scaffold selector' },
    { pattern: /\biconify-icon\b/, label: 'Iconify element selector' }
  ]
  for (const rule of forbidden) {
    if (rule.pattern.test(css)) throw new Error(`${systemId}/components.css contains a ${rule.label}`)
  }
}

let changed = 0
for (const systemId of generatedSystems) {
  const target = path.join(root, systemId, 'components.css')
  const generated = buildSystem(systemId)
  const current = fs.existsSync(target) ? fs.readFileSync(target, 'utf8') : ''
  if (current === generated) {
    process.stdout.write(`${systemId}: clean\n`)
    continue
  }
  changed += 1
  if (write) {
    fs.writeFileSync(target, generated)
    process.stdout.write(`${systemId}: written\n`)
  } else {
    process.stdout.write(`${systemId}: needs regeneration\n`)
  }
}

for (const systemId of catalogSystems) validateCopyableCss(systemId)

if (!write && changed > 0) process.exitCode = 1
