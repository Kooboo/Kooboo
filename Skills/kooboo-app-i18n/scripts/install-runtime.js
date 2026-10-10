// Installs independent assets and creates ENV only when absent; preserves existing ENV.
// Never changes main/index/layout or Labels.
// run_skill_script: { skillName: 'kooboo-app-i18n', scriptName: 'scripts/install-runtime.js',
//   arguments: { prefixes: ['app.orders.'], supportedLocales: ['zh', 'en'], dryRun: true } }
const skillName = 'kooboo-app-i18n'
const input = JSON.parse(k.request.body || '{}')
const result = { success: false, objects: [], conflicts: [], integrationRequired: true }
try {
  const locales = input.supportedLocales
  const prefixes = input.prefixes
  const site = k.site.webSite
  if (!Array.isArray(locales) || !locales.length || locales.length > 30 ||
      locales.some(value => typeof value !== 'string' || !/^[A-Za-z]{2,8}(?:-[A-Za-z0-9]{1,8})*$/.test(value)) ||
      new Set(locales.map(value => value.toLowerCase())).size !== locales.length ||
      !locales.includes(String(site.defaultCulture))) throw new Error('supportedLocales must be unique site cultures including the exact site defaultCulture')
  const siteCultures = Object.keys(site.culture)
  if (locales.some(locale => !siteCultures.includes(locale))) throw new Error('Configure the requested cultures on the site before installation')
  if (!Array.isArray(prefixes) || !prefixes.length || prefixes.length > 20 ||
      prefixes.some(value => typeof value !== 'string' || !/^app\.[a-zA-Z0-9_.-]{1,55}\.$/.test(value))) {
    throw new Error('prefixes must be explicit application namespaces, for example app.orders.')
  }
  if (input.dryRun !== undefined && typeof input.dryRun !== 'boolean') throw new Error('dryRun must be boolean')
  const env = k.site.codes.get('ENV')
  if (env && (Number(env.codeType) !== 10 || Number(env.scriptType) !== 1)) {
    throw new Error('ENV must be a module CodeBlock; existing object was preserved')
  }
  const requestedSettings = { prefixes, supportedLocales: locales }
  result.configuration = {
    object: 'ENV', field: 'i18n', requestedValue: requestedSettings,
    status: env ? 'preserved-review-required' : 'not-created'
  }
  const envBody = env ? String(env.body) : '/* application-owned server configuration */\nexport const ENV = ' +
    JSON.stringify({ i18n: requestedSettings }, null, 2) + '\n'
  function asset(path) {
    const body = String(k.ai.callFunction('read_skill_resource', { skillName, resourceName: path }))
    if (!body || body.startsWith('Skill resource not found')) throw new Error('Missing asset: ' + path)
    return body
  }
  const files = [
    { type: 'CodeBlock', name: 'ENV', body: envBody },
    { type: 'CodeBlock', name: 'Services.i18n', body: asset('assets/CodeBlock/Services.i18n.js') },
    { type: 'API', name: 'app.i18n', url: '/api/app-i18n/{action}', body: asset('assets/API/app.i18n.js') },
    { type: 'Script', name: 'vue-i18n.js', url: '/vue-i18n.js', body: asset('assets/Script/vue-i18n.js') },
    { type: 'Script', name: 'service.i18n.js', url: '/service.i18n.js', body: asset('assets/Script/service.i18n.js')
      .replace('const installedSiteId = null', 'const installedSiteId = ' + JSON.stringify(String(site.id))) }
  ]
  function existing(file) {
    return (file.type === 'Script' ? k.site.scripts : k.site.codes).get(file.name)
  }
  function conflict(file) {
    const old = existing(file)
    if (old && (String(old.body) !== file.body || (file.type !== 'Script' &&
        (Number(old.codeType) !== (file.type === 'API' ? 4 : 10) || Number(old.scriptType) !== 1)))) return file.name + ': existing object differs'
    if (file.url) {
      const route = k.site.routes.getByUrl(file.url)
      if (route && (!old || String(route.objectId) !== String(old.id))) return file.url + ': route belongs to another object'
      if (file.type === 'API') {
        const baseRoute = k.site.routes.getByUrl('/api/app-i18n')
        if (baseRoute && (!old || String(baseRoute.objectId) !== String(old.id))) return '/api/app-i18n: route belongs to another object'
      }
      if (old && !route) return file.name + ': existing object has no expected route; preserve and review its routing'
    }
    return null
  }
  result.conflicts = files.map(conflict).filter(Boolean)
  if (result.conflicts.length) return result
  for (const file of files) {
    const receipt = { type: file.type, name: file.name, operation: existing(file) ? 'skipped' : input.dryRun ? 'planned' : 'created' }
    if (receipt.operation === 'created') {
      const changed = conflict(file)
      if (changed) { result.conflicts.push(changed); return result }
      const args = { name: file.name, content: file.body }
      if (file.url) args.url = file.url
      k.ai.callFunction(file.type === 'Script' ? 'create_script' : file.type === 'API' ? 'create_api' : 'create_codeblock', args)
      // Read back persisted content; function calls return CLR objects in KScript.
      const current = existing(file)
      if (!current || String(current.body) !== file.body || conflict(file)) {
        receipt.operation = 'unverified'
        result.objects.push(receipt)
        throw new Error('Could not verify installation of ' + file.name)
      }
    }
    result.objects.push(receipt)
    if (file.name === 'ENV' && !env) result.configuration.status = receipt.operation
  }
  result.success = true
  result.message = input.dryRun ? 'Assets checked; no writes performed. Review configuration and references/integration.md.' : 'Assets ready; verify ENV.i18n and integrate import map, main, Labels and UI using references/integration.md. Existing ENV was not evaluated or modified.'
} catch (error) {
  result.error = String(error.message || error)
}
return result
