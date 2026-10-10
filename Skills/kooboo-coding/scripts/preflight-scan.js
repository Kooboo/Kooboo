// Kooboo Preflight 扫描器。
//
// 这个脚本做三件事：
// 1. 根据 artifacts 按需读取 references/Preflight/rules/*.yaml 中的规则。
// 2. 根据 profile、对象实际能力和 objectType 选出适用规则。
// 3. 通过 read_object_text 读取 artifacts 的最新内容，然后做正则扫描。
//    如果 artifact 显式传了 content，则直接扫描该 content，适合未保存的临时代码片段。
//
// 使用方式：
// run_skill_script("kooboo-coding", "scripts/preflight-scan.js", {
//   taskType: "site-page",
//   layers: ["frontend", "design"],
//   deliveryStage: "static-first",
//   mode: "capability",
//   artifacts: [{ name: "home", objectType: "Page" }]
// })

var SKILL_NAME = "kooboo-coding";
var RULE_RESOURCES = {
    "common": "references/Preflight/rules/common.yaml",
    "frontend": "references/Preflight/rules/frontend.yaml",
    "forms": "references/Preflight/rules/forms.yaml",
    "template": "references/Preflight/rules/template.yaml",
    "k-data": "references/Preflight/rules/k-data.yaml",
    "kscript": "references/Preflight/rules/kscript.yaml",
    "multilingual": "references/Preflight/rules/multilingual.yaml",
    "data-contract": "references/Preflight/rules/data-contract.yaml",
    "commerce": "references/Preflight/rules/commerce.yaml",
    "commerce-template": "references/Preflight/rules/commerce-template.yaml",
    "payment": "references/Preflight/rules/payment.yaml",
    "security": "references/Preflight/rules/security.yaml",
    "assets": "references/Preflight/rules/assets.yaml",
    "assembly": "references/Preflight/rules/assembly.yaml"
};

var CROSS_RULE_ID = "PF-KOBOO-TEMPLATE-007";
var PLACEHOLDER_NAME_RULE_ID = "PF-KOBOO-TEMPLATE-013";
var LAYOUT_READ_RULE_ID = "PF-KOBOO-TEMPLATE-014";
var VIEW_REFERENCE_RULE_ID = "PF-KOBOO-RESOURCE-001";
var LOCAL_RESOURCE_URL_RULE_ID = "PF-KOBOO-RESOURCE-002";
var DEBUG_PLAYBOOK_SKILL_NAME = "debug";
var DEBUG_PLAYBOOK_RESOURCE_NAME = "references/common-errors.md";

// 规则/检查逻辑版本。任何规则 YAML、custom check 或扫描逻辑变化时都必须递增，
// 否则内容哈希缓存会复用旧规则的结果。
var RULE_VERSION = "2026-10-09.1";
var SITE_PROFILE = "site";
var APP_PROFILE = "app";
var FOCUSED_MODE = "focused";
var CAPABILITY_MODE = "capability";

// Debug 是独立 skill。Playbook 必须携带可直接传给 read_skill_resource 的
// skillName/resourceName，不能依赖相对路径猜测资源归属。
function createDebugPlaybook(anchor) {
    return {
        skillName: DEBUG_PLAYBOOK_SKILL_NAME,
        resourceName: DEBUG_PLAYBOOK_RESOURCE_NAME,
        anchor: String(anchor || "").replace(/^#/, "")
    };
}

// 从 Kooboo request 中读取参数。没有传入时使用 fallback。
function getArg(name, fallback) {
    var value = k.request.get(name);
    return value === null || value === undefined || value === "" ? fallback : value;
}

function getBoolArg(name, fallback) {
    var value = getArg(name, fallback);
    if (typeof value === "boolean") return value;
    return /^(?:true|1|yes)$/i.test(String(value || ""));
}

// Kooboo 函数参数可能传数组，也可能传逗号分隔字符串。
// 统一转成数组，后续规则过滤就能用同一套逻辑。
function asArray(value) {
    if (value === null || value === undefined || value === "") return [];
    if (Array.isArray(value)) return value;
    if (typeof value === "string") {
        var trimmed = value.trim();
        if (!trimmed) return [];
        if (trimmed[0] === "[") {
            try {
                var parsed = JSON.parse(trimmed);
                return Array.isArray(parsed) ? parsed : [parsed];
            } catch (e) {
                return trimmed.split(",").map(function (x) { return x.trim(); }).filter(Boolean);
            }
        }
        return trimmed.split(",").map(function (x) { return x.trim(); }).filter(Boolean);
    }
    return [value];
}

// 通过 read_skill_resource 读取当前 skill 内的 YAML 规则文件。
function readRulesText(resourceName) {
    return String(k.ai.callFunction("read_skill_resource", {
        skillName: SKILL_NAME,
        resourceName: resourceName
    }) || "");
}

function addRuleResource(resources, key) {
    var resourceName = RULE_RESOURCES[key];
    if (resourceName && !resources[resourceName]) resources[resourceName] = true;
}

function selectRuleResources(artifacts, profile) {
    var resources = {};
    var groupCapabilities = {
        common: "common", frontend: "frontend", forms: "forms", template: "template",
        "k-data": "k-data", kscript: "kscript", multilingual: "multilingual",
        "data-contract": "data", commerce: "commerce", "commerce-template": "commerce",
        payment: "payment", security: "code", assets: "assets", assembly: "html"
    };
    for (var i = 0; i < artifacts.length; i++) {
        var caps = artifacts[i]._capabilities || detectCapabilities(artifacts[i], { profile: profile });
        for (var group in groupCapabilities) {
            if (group === "commerce-template" && !caps["k-data"]) continue;
            if (caps[groupCapabilities[group]]) addRuleResource(resources, group);
        }
        if (caps["server-script"] || caps["template-data"]) addRuleResource(resources, "template");
    }

    var result = [];
    for (var name in resources) {
        if (Object.prototype.hasOwnProperty.call(resources, name)) result.push(name);
    }
    return result;
}

function readSelectedRules(resources) {
    var rules = [];
    for (var i = 0; i < resources.length; i++) {
        var text = readRulesText(resources[i]);
        var definitions = parseRulesYaml(text);
        if (!definitions.length) throw new Error("Rule resource is empty or unavailable: " + resources[i]);
        definitions.forEach(function (rule) {
            var profiles = asArray(rule.profiles);
            if (!profiles.length || profiles.some(function (profile) { return profile !== SITE_PROFILE && profile !== APP_PROFILE; }) || !rule.capability) throw new Error("Rule requires an explicit profile and capability: " + rule.id);
            rules.push(rule);
        });
    }
    return rules;
}

// 从 Kooboo 站点对象读取最新文本。
// 这样 artifacts 只需要传 name/objectType，不需要把完整 content 作为参数传入，节省大量 token。
function readObjectText(nameOrId, objectType) {
    var result = k.ai.callFunction("read_object_text", {
        nameOrId: nameOrId,
        objectType: objectType,
        showLineNumber: false
    });

    // read_object_text 正常应返回原始文本；这里兼容可能的对象包装格式。
    if (typeof result === "string") return result;
    if (result && typeof result.content === "string") return result.content;
    if (result && typeof result.text === "string") return result.text;
    if (result === null || result === undefined) return null;
    return String(result);
}

// 读取站点对象清单，用于校验 View 名称和静态资源 Route URL。
// list_site_object 在不同调用边界可能返回对象或 JSON 字符串，这里统一成小写字段。
var siteObjectListCache = {};

function readSiteObjectList(objectType) {
    var cacheKey = String(objectType || "").toLowerCase();
    if (Object.prototype.hasOwnProperty.call(siteObjectListCache, cacheKey)) {
        return siteObjectListCache[cacheKey];
    }
    try {
        var result = k.ai.callFunction("list_site_object", {
            objectType: objectType
        });
        if (typeof result === "string") {
            try {
                result = JSON.parse(result);
            } catch (e) {
                siteObjectListCache[cacheKey] = null;
                return null;
            }
        }
        var rawList = result && (result.list || result.List);
        // k.ai.callFunction returns CLR List<T> in Jint, not a native JS Array.
        // Materialize that JSON-compatible collection before validating shape.
        if (rawList && !Array.isArray(rawList)) {
            rawList = JSON.parse(JSON.stringify(rawList));
        }
        if (!Array.isArray(rawList)) {
            siteObjectListCache[cacheKey] = null;
            return null;
        }
        var list = [];
        for (var i = 0; i < rawList.length; i++) {
            var item = rawList[i] || {};
            var sizeRaw = item.size !== undefined ? item.size : item.Size;
            var size = null;
            if (typeof sizeRaw === "number") {
                size = sizeRaw;
            } else if (typeof sizeRaw === "string" && sizeRaw !== "") {
                var parsedSize = parseInt(sizeRaw, 10);
                size = isNaN(parsedSize) ? null : parsedSize;
            }
            list.push({
                id: String(item.id || item.Id || "").trim(),
                name: String(item.name || item.Name || "").trim(),
                url: String(item.url || item.Url || "").trim(),
                size: size,
                layoutName: typeof item.layoutName === "string" ? item.layoutName : (typeof item.LayoutName === "string" ? item.LayoutName : null)
            });
        }
        siteObjectListCache[cacheKey] = list;
        return list;
    } catch (e) {
        siteObjectListCache[cacheKey] = null;
        return null;
    }
}

// 去掉 YAML 字符串两侧引号，并把双反斜杠还原成 JS 正则需要的反斜杠。
// 例如 YAML 中 "\\bh-screen\\b" 会还原为 "\bh-screen\b"。
function stripQuotes(value) {
    var s = String(value || "").trim();
    if ((s[0] === "\"" && s[s.length - 1] === "\"") || (s[0] === "'" && s[s.length - 1] === "'")) {
        return s.substring(1, s.length - 1).replace(/\\\\/g, "\\");
    }
    return s;
}

// 解析形如 [frontend, design] 的内联 YAML 数组。
// 本脚本只支持 Preflight 规则文件里用到的受控 YAML 子集，不是通用 YAML 解析器。
function parseInlineArray(value) {
    var s = String(value || "").trim();
    if (s[0] !== "[" || s[s.length - 1] !== "]") return null;
    var body = s.substring(1, s.length - 1).trim();
    if (!body) return [];
    var items = [];
    var cur = "";
    var quote = "";
    for (var i = 0; i < body.length; i++) {
        var ch = body[i];
        if (quote) {
            cur += ch;
            if (ch === quote && body[i - 1] !== "\\") quote = "";
        } else if (ch === "\"" || ch === "'") {
            quote = ch;
            cur += ch;
        } else if (ch === ",") {
            items.push(stripQuotes(cur));
            cur = "";
        } else {
            cur += ch;
        }
    }
    if (cur.trim()) items.push(stripQuotes(cur));
    return items;
}

// 解析 Preflight 规则 YAML。
// 为了在 Kooboo 运行时里不依赖 npm 包，这里实现了一个轻量解析器。
// 约束：每条规则以 "- id:" 开头；普通字段是一行；数组使用 [a, b] 或多行 "- item"。
function parseRulesYaml(text) {
    var lines = String(text || "").split(/\r?\n/);
    var rules = [];
    var current = null;
    var currentListKey = null;

    function finishRule() {
        if (current && current.id) rules.push(current);
        current = null;
        currentListKey = null;
    }

    for (var i = 0; i < lines.length; i++) {
        var raw = lines[i];
        if (!raw.trim() || raw.trim()[0] === "#") continue;
        if (/^rules:\s*$/.test(raw.trim())) continue;

        var ruleStart = raw.match(/^\s*-\s+id:\s*(.+)\s*$/);
        if (ruleStart) {
            finishRule();
            current = { id: stripQuotes(ruleStart[1]) };
            continue;
        }

        if (!current) continue;

        var listItem = raw.match(/^\s*-\s+(.+)\s*$/);
        if (listItem && currentListKey) {
            current[currentListKey].push(stripQuotes(listItem[1]));
            continue;
        }

        var prop = raw.match(/^\s+([A-Za-z0-9_-]+):\s*(.*)$/);
        if (!prop) continue;
        var key = prop[1];
        var value = prop[2];
        var arr = parseInlineArray(value);
        if (arr !== null) {
            current[key] = arr;
            currentListKey = null;
        } else if (value === "") {
            current[key] = [];
            currentListKey = key;
        } else {
            current[key] = stripQuotes(value);
            currentListKey = null;
        }
    }
    finishRule();
    return rules;
}

function findRuleById(rules, id) {
    for (var i = 0; i < rules.length; i++) {
        if (rules[i] && rules[i].id === id) return rules[i];
    }
    return null;
}

// 规范化 artifact.objectType，避免 Page/page、CodeBlock/codeblock 这类大小写差异影响匹配。
function normalizeObjectType(objectType) {
    var s = String(objectType || "").toLowerCase();
    if (s === "page") return "Page";
    if (s === "layout") return "Layout";
    if (s === "view") return "View";
    if (s === "style" || s === "css") return "Style";
    if (s === "script" || s === "javascript") return "Script";
    if (s === "api") return "Api";
    if (s === "codeblock" || s === "code block" || s === "service") return "CodeBlock";
    return objectType || "Unknown";
}

// artifact.name 用于报告展示；artifact.nameOrId 优先用于读取对象内容。
function getArtifactNameOrId(artifact) {
    return artifact.nameOrId || artifact.name || artifact.id || "";
}

// 判断 candidates 中是否有任一项出现在 values 中。大小写不敏感。
function includesAny(values, candidates) {
    var map = {};
    for (var i = 0; i < values.length; i++) map[String(values[i]).toLowerCase()] = true;
    for (var j = 0; j < candidates.length; j++) {
        if (map[String(candidates[j]).toLowerCase()]) return true;
    }
    return false;
}

// Rule capabilities and positive metadata requirements determine applicability;
// taskType/layers describe the request and cannot disable detected code checks.
function hasNonEmptyArtifactField(artifact, fieldName) {
    var expected = String(fieldName || "").toLowerCase();
    for (var key in artifact) {
        if (!Object.prototype.hasOwnProperty.call(artifact, key)) continue;
        if (String(key).toLowerCase() !== expected) continue;
        var value = artifact[key];
        if (value === null || value === undefined || value === false) return false;
        if (Array.isArray(value)) return value.length > 0;
        return String(value).trim() !== "";
    }
    return false;
}

function artifactHasRequiredFields(rule, artifact) {
    var fields = asArray(rule.requiredArtifactFields);
    for (var i = 0; i < fields.length; i++) {
        if (!hasNonEmptyArtifactField(artifact, fields[i])) return false;
    }
    return true;
}

function ruleApplies(rule, context, artifact) {
    var profiles = asArray(rule.profiles);
    var appliesTo = asArray(rule.appliesTo);
    var profileOk = profiles.length === 0 || includesAny(profiles, [context.profile]);
    var objectOk = appliesTo.length === 0 || includesAny(appliesTo, [artifact.objectType]);
    return profileOk && objectOk && needsCapability(rule, artifact, context) && artifactHasRequiredFields(rule, artifact);
}

// Capabilities come from hydrated content and platform metadata. Model layers are
// descriptive only: omitting a layer must not turn off a check for code in use.
function detectCapabilities(artifact, context) {
    var text = String(artifact.content || "");
    var type = normalizeObjectType(artifact.objectType);
    var caps = { common: true };
    var html = includesAny(["Page", "Layout", "View"], [type]);
    var code = includesAny(["Api", "CodeBlock", "Script"], [type]) || /<script\b/i.test(text);
    if (html) caps.html = true;
    if (html || type === "Style" || type === "Script") caps.frontend = true;
    if (html || type === "Style" || type === "Script") caps.assets = true;
    if (code || html) caps.code = true;
    if (code) caps.data = true;
    if (type === "Api" || type === "CodeBlock" || /\bk\.(?:api|request|response|cookie|commerce|DB|site|content|payment)\b/.test(text)) caps.kscript = true;
    if (/<(?:form|input|select|textarea)\b/i.test(text)) caps.forms = true;
    if (type === "Layout" || artifact.layoutName || /<(?:layout|placeholder|view)\b|\bk-(?:placeholder|attribute|for|if|content)\s*=/i.test(text)) caps.template = true;
    if (/<script\b[^>]*\benv\s*=|\bk\.(?:api|request|response|cookie|commerce|DB|site|content|payment)\b/i.test(text)) caps["server-script"] = html;
    if (context.profile === SITE_PROFILE && /<(?:k-data|query|let|when|else-if|else|try|catch|redirect|map)\b|\bk-attribute\s*=/i.test(text)) caps["k-data"] = true;
    if (caps["k-data"] || /\bk-(?:content|attribute)\s*=/.test(text)) caps["template-data"] = true;
    if (caps.template) caps.assets = true;
    if (artifact._multilingual === true) artifact.multilingual = true;
    else artifact.multilingual = false;
    if (/<script\b[^>]*\btype\s*=\s*["'](?:module|importmap)["']|\bimport\s*(?:\(|["'{*])|\bfrom\s*["']/i.test(text)) caps.modules = true;
    if (/\bk\.commerce\.|<query\b[^>]*\bsource\s*=\s*["']commerce["']/i.test(text) || artifact.commerceModel || artifact.commerceSurface) caps.commerce = true;
    if (/\bk\.payment\.|\bk\.commerce\.order\.(?:pay|create)\s*\(/i.test(text) || (caps.commerce && /payment|checkout|order/.test(String(artifact.commerceSurface || "") + " " + String(artifact.commerceCapability || "")))) caps.payment = true;
    if (/\bk\.payment\.alipayForm\b/i.test(text)) caps.alipay = true;
    if (artifact._multilingual === true) caps.multilingual = true;
    return caps;
}

function artifactPlatformMetadata(artifact) {
    if (!includesAny(["Page", "Api", "Style", "Script"], [artifact.objectType])) return;
    if (!artifact._platformMetadataLoaded) {
        var list = readSiteObjectList(artifact.objectType);
        var key = String(getArtifactNameOrId(artifact)).toLowerCase();
        var record = list && list.filter(function (item) { return item.id.toLowerCase() === key || item.name.toLowerCase() === key; })[0];
        if (record) {
            if (record.url) artifact.url = record.url;
            if (artifact.objectType === "Page" && record.layoutName !== null) {
                artifact.layoutName = record.layoutName;
                artifact._layoutKnown = true;
            }
        }
        artifact._platformMetadataLoaded = true;
    }
    if (artifact.objectType === "Page" && !artifact._layoutKnown) {
        var layout = /<layout\b([^>]*)>/i.exec(String(artifact.content || ""));
        if (layout) artifact.layoutName = readTagAttribute(layout[1], "id");
        // Caller-provided source can be scanned as an unsaved object. Persisted
        // objects require the repository binding, including an explicit empty one.
        artifact._layoutUnknown = artifact.contentSource !== "argument";
    }
}

function readPlatformSettings() {
    try {
        var result = normalizeFunctionResult(k.ai.callFunction("get_site_info", {}));
        var settings = result && (result.siteSettings || result.SiteSettings);
        if (!settings) return null;
        var enabled = settings.isMultiLingual !== undefined ? settings.isMultiLingual : settings.IsMultiLingual;
        return typeof enabled === "boolean" ? { multilingual: enabled } : null;
    } catch (e) { return null; }
}

function needsCapability(rule, artifact, context) {
    var caps = artifact._capabilities || detectCapabilities(artifact, context);
    if (rule.capability && !caps[rule.capability]) return false;
    var required = asArray(rule.requires);
    for (var i = 0; i < required.length; i++) if (!caps[required[i]]) return false;
    return true;
}

// 根据正则命中的字符位置计算行号，便于报告定位。
function countLine(content, index) {
    var before = String(content || "").slice(0, index);
    return before.split(/\r?\n/).length;
}

// 将 YAML 中的 pattern 字符串编译成 JS RegExp。
// 编译失败会被当成一个 finding 返回，方便维护者发现规则写错。
function compilePattern(pattern) {
    try {
        return new RegExp(pattern, "gim");
    } catch (e) {
        return null;
    }
}

function pushCustomMatch(matches, pattern, line, excerpt) {
    matches.push({
        pattern: pattern,
        line: line,
        excerpt: String(excerpt || "").replace(/\s+/g, " ").trim()
    });
}

function readHtmlAttribute(attrText, name) {
    var re = new RegExp("(?:^|\\s)" + name + "\\s*=\\s*(?:\\\"([^\\\"]*)\\\"|'([^']*)'|([^\\s>]+))", "i");
    var match = re.exec(String(attrText || ""));
    if (!match) return "";
    return match[1] !== undefined ? match[1] : (match[2] !== undefined ? match[2] : match[3]);
}

function hasHtmlAttribute(attrText, name) {
    var re = new RegExp("(?:^|\\s)" + name + "(?:\\s*=|(?=\\s|$))", "i");
    return re.test(String(attrText || ""));
}

function normalizeArbitraryUtilityValue(value) {
    return String(value || "").replace(/_/g, " ").trim().toLowerCase();
}

function cssColorIsTransparent(value) {
    var normalized = String(value || "").trim().toLowerCase();
    if (/\btransparent\b/.test(normalized)) return true;
    if (/#[0-9a-f]{3}0\b/i.test(normalized) || /#[0-9a-f]{6}00\b/i.test(normalized)) return true;
    if (/(?:rgba|hsla)\([^)]*,\s*0(?:\.0+)?%?\s*\)/i.test(normalized)) return true;
    return /(?:rgb|hsl|color)\([^)]*\/\s*0(?:\.0+)?%?\s*\)/i.test(normalized);
}

function cssHasNonZeroLength(value) {
    return /(?:^|[\s,(])(?:[1-9]\d*(?:\.\d+)?|0?\.\d*[1-9]\d*)(?:px|rem|em|ex|ch|vw|vh|vmin|vmax|cm|mm|in|pt|pc)\b/i.test(String(value || ""));
}

function arbitraryUtilityValueIsVisible(utility) {
    var match = /\[([^\]]+)\]/.exec(String(utility || ""));
    if (!match) return true;
    var value = normalizeArbitraryUtilityValue(match[1]);
    if (/^(?:none|transparent|0(?:\.0+)?(?:px|rem|em|%)?)$/.test(value)) return false;
    if (cssColorIsTransparent(value)) return false;
    if (/\b(?:var|calc|min|max|clamp)\(/.test(value)) return true;
    return cssHasNonZeroLength(value);
}

function isVisibleFocusUtilityName(utility) {
    var name = String(utility || "").toLowerCase();
    if (/^ring(?:$|-(?:[1-9]\d*|0?\.5))$/.test(name)) return true;
    if (/^ring-\[[^\]]+\]$/.test(name)) return arbitraryUtilityValueIsVisible(name);
    if (/^outline(?:$|-(?:[1-9]\d*|0?\.5))$/.test(name)) return true;
    if (/^outline-\[[^\]]+\]$/.test(name)) return arbitraryUtilityValueIsVisible(name);
    if (/^shadow(?:$|-)/.test(name) && name !== "shadow-none") return arbitraryUtilityValueIsVisible(name);
    if (/^border-\[[^\]]+\]$/.test(name)) return arbitraryUtilityValueIsVisible(name);
    return /^border-(?!0(?:\D|$)|transparent(?:\D|$))/.test(name);
}

function splitUtilityToken(token) {
    var result = [];
    var current = "";
    var squareDepth = 0;
    var roundDepth = 0;
    var text = String(token || "");
    for (var i = 0; i < text.length; i++) {
        var ch = text[i];
        if (ch === "\\" && i + 1 < text.length) {
            current += ch + text[++i];
            continue;
        }
        if (ch === "[") squareDepth++;
        else if (ch === "]") squareDepth = Math.max(0, squareDepth - 1);
        else if (ch === "(") roundDepth++;
        else if (ch === ")") roundDepth = Math.max(0, roundDepth - 1);
        if (ch === ":" && squareDepth === 0 && roundDepth === 0) {
            result.push(current);
            current = "";
            continue;
        }
        current += ch;
    }
    result.push(current);
    return result;
}

function isCompositeFocusVariant(variant) {
    var text = String(variant || "");
    return text === "focus-within" ||
        /^has-\[[^\]]*:focus(?:-visible)?[^\]]*\]$/.test(text) ||
        /^\[&:has\([^\]]*:focus(?:-visible)?[^\]]*\)\]$/.test(text);
}

function collectVisibleFocusUtilityScopes(classValue, composite) {
    var result = [];
    var tokens = String(classValue || "").split(/\s+/);
    for (var i = 0; i < tokens.length; i++) {
        var parts = splitUtilityToken(tokens[i]);
        if (parts.length < 2) continue;
        var utility = String(parts[parts.length - 1] || "").replace(/^!/, "");
        if (!isVisibleFocusUtilityName(utility)) continue;
        var foundFocusState = false;
        var conditions = [];
        for (var partIndex = 0; partIndex < parts.length - 1; partIndex++) {
            var variant = parts[partIndex];
            var isFocusState = composite ? isCompositeFocusVariant(variant) : /^(?:focus|focus-visible)$/.test(variant);
            if (isFocusState) foundFocusState = true;
            else conditions.push(variant);
        }
        if (foundFocusState) result.push(conditions);
    }
    return result;
}

function collectOutlineRemovalScopes(classValue, styleValue) {
    var result = [];
    var tokens = String(classValue || "").split(/\s+/);
    for (var i = 0; i < tokens.length; i++) {
        var parts = splitUtilityToken(tokens[i]);
        var leaf = String(parts[parts.length - 1] || "").replace(/^!/, "").toLowerCase();
        var removes = leaf === "outline-none" || leaf === "outline-0" ||
            (/^outline-\[[^\]]+\]$/.test(leaf) && !arbitraryUtilityValueIsVisible(leaf));
        if (!removes) continue;
        var conditions = [];
        for (var partIndex = 0; partIndex < parts.length - 1; partIndex++) {
            if (!/^(?:focus|focus-visible)$/.test(parts[partIndex])) conditions.push(parts[partIndex]);
        }
        result.push(conditions);
    }
    if (cssRemovesFocusOutline(String(styleValue || ""))) result.push([]);
    return result;
}

function minimumBreakpointRank(condition) {
    var ranks = { sm: 1, md: 2, lg: 3, xl: 4, "2xl": 5 };
    return ranks[String(condition || "")] || 0;
}

function focusIndicatorScopeCovers(removalScope, indicatorScope) {
    for (var indicatorIndex = 0; indicatorIndex < indicatorScope.length; indicatorIndex++) {
        var requiredCondition = indicatorScope[indicatorIndex];
        if (removalScope.indexOf(requiredCondition) >= 0) continue;
        var requiredRank = minimumBreakpointRank(requiredCondition);
        var coveredByWiderBreakpoint = false;
        if (requiredRank) {
            for (var removalIndex = 0; removalIndex < removalScope.length; removalIndex++) {
                if (minimumBreakpointRank(removalScope[removalIndex]) >= requiredRank) {
                    coveredByWiderBreakpoint = true;
                    break;
                }
            }
        }
        if (!coveredByWiderBreakpoint) return false;
    }
    return true;
}

function allOutlineRemovalScopesAreCovered(removalScopes, indicatorScopes) {
    for (var removalIndex = 0; removalIndex < removalScopes.length; removalIndex++) {
        var covered = false;
        for (var indicatorIndex = 0; indicatorIndex < indicatorScopes.length; indicatorIndex++) {
            if (focusIndicatorScopeCovers(removalScopes[removalIndex], indicatorScopes[indicatorIndex])) {
                covered = true;
                break;
            }
        }
        if (!covered) return false;
    }
    return true;
}

function isLikelyFocusableTag(tagName, attrText) {
    var tag = String(tagName || "").toLowerCase();
    if (hasHtmlAttribute(attrText, "hidden") || hasHtmlAttribute(attrText, "inert")) return false;
    if (/^(input|textarea|select|button)$/.test(tag) && hasHtmlAttribute(attrText, "disabled")) return false;
    if (tag === "input" && String(readHtmlAttribute(attrText, "type") || "").toLowerCase() === "hidden") return false;
    if (/^(input|textarea|select|button|summary)$/.test(tag)) return true;
    if ((tag === "a" || tag === "area") && /(?:^|\s)href\s*=/i.test(attrText)) return true;
    if (hasHtmlAttribute(attrText, "tabindex")) {
        var tabindex = String(readHtmlAttribute(attrText, "tabindex") || "").trim();
        return tabindex === "" || !/^-\d+$/.test(tabindex);
    }
    if (hasHtmlAttribute(attrText, "contenteditable")) {
        var editable = String(readHtmlAttribute(attrText, "contenteditable") || "").trim().toLowerCase();
        return editable === "" || editable === "true" || editable === "plaintext-only";
    }
    return false;
}

function parseCssDeclarations(declarations) {
    var result = {};
    var re = /(?:^|;)\s*([A-Za-z-]+)\s*:\s*([^;]*)/g;
    var match;
    while ((match = re.exec(String(declarations || ""))) !== null) {
        result[String(match[1] || "").toLowerCase()] = String(match[2] || "").replace(/\s*!important\s*$/i, "").trim();
    }
    return result;
}

function cssValueIsZero(value) {
    return /^0(?:\.0+)?(?:px|rem|em|%)?$/i.test(String(value || "").trim());
}

function cssRemovesFocusOutline(declarations) {
    var map = parseCssDeclarations(declarations);
    if (map.outline && (/^(?:none|transparent|0(?:\.0+)?(?:px|rem|em|%)?)(?:\s|$)/i.test(map.outline) || cssColorIsTransparent(map.outline))) return true;
    if (map["outline-width"] && cssValueIsZero(map["outline-width"])) return true;
    if (map["outline-style"] && /^none$/i.test(map["outline-style"])) return true;
    return !!(map["outline-color"] && cssColorIsTransparent(map["outline-color"]));
}

function cssShadowIsVisible(value) {
    var normalized = String(value || "").trim().toLowerCase();
    if (!normalized || normalized === "none" || cssColorIsTransparent(normalized)) return false;
    if (/\b(?:var|calc|min|max|clamp)\(/.test(normalized)) return true;
    return cssHasNonZeroLength(normalized);
}

function cssHasVisibleFocusReplacement(declarations) {
    var map = parseCssDeclarations(declarations);
    var outlineDisabledByLonghand = (map["outline-style"] && /^none$/i.test(map["outline-style"])) ||
        (map["outline-width"] && cssValueIsZero(map["outline-width"])) ||
        (map["outline-color"] && cssColorIsTransparent(map["outline-color"]));
    if (map.outline &&
        !/^(?:none|transparent|0(?:\.0+)?(?:px|rem|em|%)?)(?:\s|$)/i.test(map.outline) &&
        !cssColorIsTransparent(map.outline) &&
        !outlineDisabledByLonghand) return true;
    if (map["outline-style"] && !/^none$/i.test(map["outline-style"]) &&
        map["outline-width"] && !cssValueIsZero(map["outline-width"]) &&
        (!map["outline-color"] || !cssColorIsTransparent(map["outline-color"]))) return true;
    if (map["box-shadow"] && cssShadowIsVisible(map["box-shadow"])) return true;
    var borderProperties = ["border", "border-color", "border-top", "border-right", "border-bottom", "border-left"];
    for (var i = 0; i < borderProperties.length; i++) {
        var value = map[borderProperties[i]];
        if (value && !/^(?:none|transparent|0(?:\.0+)?(?:px|rem|em|%)?)(?:\s|$)/i.test(value) && !cssColorIsTransparent(value)) return true;
    }
    return false;
}

function maskCssComments(text) {
    return String(text || "").replace(/\/\*[\s\S]*?\*\//g, function (comment) {
        return comment.replace(/[^\r\n]/g, " ");
    });
}

function splitCssSelectorList(selectorText) {
    var result = [];
    var current = "";
    var squareDepth = 0;
    var roundDepth = 0;
    var quote = "";
    var text = String(selectorText || "");
    for (var i = 0; i < text.length; i++) {
        var ch = text[i];
        if (quote) {
            current += ch;
            if (ch === "\\") {
                if (i + 1 < text.length) current += text[++i];
            } else if (ch === quote) {
                quote = "";
            }
            continue;
        }
        if (ch === "\"" || ch === "'") {
            quote = ch;
            current += ch;
            continue;
        }
        if (ch === "[") squareDepth++;
        else if (ch === "]") squareDepth = Math.max(0, squareDepth - 1);
        else if (ch === "(") roundDepth++;
        else if (ch === ")") roundDepth = Math.max(0, roundDepth - 1);
        if (ch === "," && squareDepth === 0 && roundDepth === 0) {
            if (current.trim()) result.push(current.trim());
            current = "";
            continue;
        }
        current += ch;
    }
    if (current.trim()) result.push(current.trim());
    return result;
}

function selectorContainsCompositeFocus(selector) {
    var text = String(selector || "");
    return /:focus-within\b/i.test(text) || /:has\([^)]*:focus(?:-visible)?\b[^)]*\)/i.test(text);
}

function selectorContainsDirectFocus(selector) {
    var text = String(selector || "")
        .replace(/:has\([^)]*:focus(?:-visible)?\b[^)]*\)/gi, "")
        .replace(/:focus-within\b/gi, "");
    return /:focus(?:-visible)?\b/i.test(text);
}

function selectorIdentityTokens(selector) {
    var text = String(selector || "")
        .replace(/\[[^\]]*\]/g, " ")
        .replace(/::?[A-Za-z-]+(?:\([^)]*\))?/g, " ");
    var result = [];
    var seen = {};
    var re = /(#[A-Za-z_][\w-]*|\.[A-Za-z_][\w-]*|(?:^|[\s>+~,(])([A-Za-z][\w-]*))/g;
    var match;
    while ((match = re.exec(text)) !== null) {
        var token = String(match[1] && (match[1][0] === "." || match[1][0] === "#") ? match[1] : match[2] || "").toLowerCase();
        if (!token || seen[token]) continue;
        seen[token] = true;
        result.push(token);
    }
    return result;
}

function elementIdentityTokens(tagName, attrText) {
    var result = [String(tagName || "").toLowerCase()];
    var id = String(readHtmlAttribute(attrText, "id") || "").trim().toLowerCase();
    if (id) result.push("#" + id);
    var classes = String(readHtmlAttribute(attrText, "class") || "").split(/\s+/);
    for (var i = 0; i < classes.length; i++) {
        if (/^[A-Za-z_][\w-]*$/.test(classes[i])) result.push("." + classes[i].toLowerCase());
    }
    return result;
}

function identityTokensAreCovered(required, available) {
    if (!required.length) return false;
    var map = {};
    for (var i = 0; i < available.length; i++) map[available[i]] = true;
    for (var j = 0; j < required.length; j++) {
        if (!map[required[j]]) return false;
    }
    return true;
}

function domScopeTokens(stack, currentTokens) {
    var result = [];
    for (var i = 0; i < stack.length; i++) result = result.concat(stack[i].identityTokens || []);
    return result.concat(currentTokens || []);
}

function domScopeMatchesSelector(stack, currentTokens, selector) {
    return identityTokensAreCovered(selectorIdentityTokens(selector), domScopeTokens(stack, currentTokens));
}

function compositeSelectorCoversDirectFocus(directSelector, compositeSelector) {
    return identityTokensAreCovered(
        selectorIdentityTokens(compositeSelector),
        selectorIdentityTokens(directSelector)
    );
}

function extractCssRules(text, hasHtml) {
    var regions = [];
    var styleRe = /<style\b[^>]*>([\s\S]*?)<\/style>/gim;
    var styleMatch;
    while ((styleMatch = styleRe.exec(text)) !== null) {
        regions.push({
            text: styleMatch[1] || "",
            offset: (styleMatch.index || 0) + styleMatch[0].indexOf(styleMatch[1] || "")
        });
    }
    if (!regions.length && !hasHtml) regions.push({ text: text, offset: 0 });

    var rules = [];
    for (var regionIndex = 0; regionIndex < regions.length; regionIndex++) {
        var masked = maskCssComments(regions[regionIndex].text);
        var cssRuleRe = /([^{}]+)\{([^{}]*)\}/gim;
        var cssRule;
        while ((cssRule = cssRuleRe.exec(masked)) !== null) {
            rules.push({
                index: regions[regionIndex].offset + (cssRule.index || 0),
                selector: cssRule[1] || "",
                declarations: cssRule[2] || ""
            });
        }
    }
    return rules;
}

// PF-FRONTEND-005 不能只搜 outline-none：组合输入框常由实际祖先通过
// focus-within / :has(:focus-visible) 统一绘制焦点环。HTML 中按真实祖先关系和
// 响应式/状态作用域判断覆盖；纯 Style 中只接受同一控件身份范围内的组合 selector，
// 不能因为两个 selector 仅共享 .form 一类宽泛祖先就认定已经补偿。
function scanVisibleFocusIndicator(content) {
    var text = String(content || "");
    var matches = [];
    var stack = [];
    var hasHtml = /<\/?[A-Za-z][\w:-]*\b[^>]*>/i.test(text);
    var cssRules = extractCssRules(text, hasHtml);
    var compositeSelectors = [];
    var directIndicatorSelectors = [];
    var directRemovalEntries = [];
    for (var ruleIndex = 0; ruleIndex < cssRules.length; ruleIndex++) {
        var selectors = splitCssSelectorList(cssRules[ruleIndex].selector);
        for (var selectorIndex = 0; selectorIndex < selectors.length; selectorIndex++) {
            var selector = selectors[selectorIndex];
            if (cssHasVisibleFocusReplacement(cssRules[ruleIndex].declarations)) {
                if (selectorContainsCompositeFocus(selector)) compositeSelectors.push(selector);
                if (selectorContainsDirectFocus(selector)) directIndicatorSelectors.push(selector);
            } else if (cssRemovesFocusOutline(cssRules[ruleIndex].declarations) && selectorContainsDirectFocus(selector)) {
                directRemovalEntries.push({
                    selector: selector,
                    rule: cssRules[ruleIndex],
                    matchedFocusableElement: false
                });
            }
        }
    }
    var voidTags = {
        area: true, base: true, br: true, col: true, embed: true, hr: true,
        img: true, input: true, link: true, meta: true, param: true,
        source: true, track: true, wbr: true
    };
    var tagRe = /<(\/?)\s*([A-Za-z][\w:-]*)\b([^>]*)>/gim;
    var tagMatch;

    while ((tagMatch = tagRe.exec(text)) !== null && matches.length < 20) {
        var isClosing = tagMatch[1] === "/";
        var tagName = String(tagMatch[2] || "").toLowerCase();
        var attrText = tagMatch[3] || "";

        if (isClosing) {
            for (var closeIndex = stack.length - 1; closeIndex >= 0; closeIndex--) {
                if (stack[closeIndex].tagName !== tagName) continue;
                stack.length = closeIndex;
                break;
            }
            continue;
        }

        var classValue = readHtmlAttribute(attrText, "class");
        var styleValue = readHtmlAttribute(attrText, "style");
        var currentIdentityTokens = elementIdentityTokens(tagName, attrText);
        var removalScopes = collectOutlineRemovalScopes(classValue, styleValue);
        var matchingRemovalEntries = [];
        for (var removalEntryIndex = 0; removalEntryIndex < directRemovalEntries.length; removalEntryIndex++) {
            if (!domScopeMatchesSelector(stack, currentIdentityTokens, directRemovalEntries[removalEntryIndex].selector)) continue;
            removalScopes.push([]);
            matchingRemovalEntries.push(directRemovalEntries[removalEntryIndex]);
        }
        var indicatorScopes = collectVisibleFocusUtilityScopes(classValue, false)
            .concat(collectVisibleFocusUtilityScopes(classValue, true));
        for (var directIndex = 0; directIndex < directIndicatorSelectors.length; directIndex++) {
            if (domScopeMatchesSelector(stack, currentIdentityTokens, directIndicatorSelectors[directIndex])) indicatorScopes.push([]);
        }
        for (var ancestorIndex = stack.length - 1; ancestorIndex >= 0; ancestorIndex--) {
            indicatorScopes = indicatorScopes.concat(stack[ancestorIndex].focusWithinIndicatorScopes || []);
        }
        for (var compositeIndex = 0; compositeIndex < compositeSelectors.length; compositeIndex++) {
            if (domScopeMatchesSelector(stack, currentIdentityTokens, compositeSelectors[compositeIndex])) indicatorScopes.push([]);
        }
        var focusSuppressedByAncestor = false;
        for (var suppressionIndex = 0; suppressionIndex < stack.length; suppressionIndex++) {
            if (stack[suppressionIndex].suppressesDescendantFocus) {
                focusSuppressedByAncestor = true;
                break;
            }
        }

        var isFocusable = isLikelyFocusableTag(tagName, attrText) && !focusSuppressedByAncestor;
        var missingVisibleIndicator = removalScopes.length &&
            !allOutlineRemovalScopesAreCovered(removalScopes, indicatorScopes);
        if (isFocusable) {
            for (var matchedEntryIndex = 0; matchedEntryIndex < matchingRemovalEntries.length; matchedEntryIndex++) {
                matchingRemovalEntries[matchedEntryIndex].matchedFocusableElement = true;
            }
        }

        if (isFocusable && missingVisibleIndicator) {
            pushCustomMatch(
                matches,
                "custom:visibleFocusIndicator",
                countLine(text, tagMatch.index || 0),
                tagMatch[0]
            );
        }

        var selfClosing = /\/\s*>$/.test(tagMatch[0]) || !!voidTags[tagName];
        if (!selfClosing) {
            stack.push({
                tagName: tagName,
                focusWithinIndicatorScopes: collectVisibleFocusUtilityScopes(classValue, true),
                identityTokens: currentIdentityTokens,
                suppressesDescendantFocus: hasHtmlAttribute(attrText, "hidden") ||
                    hasHtmlAttribute(attrText, "inert") ||
                    (tagName === "fieldset" && hasHtmlAttribute(attrText, "disabled"))
            });
        }
    }

    for (var removalIndex = 0; removalIndex < directRemovalEntries.length && matches.length < 20; removalIndex++) {
        var removalEntry = directRemovalEntries[removalIndex];
        if (removalEntry.matchedFocusableElement) continue;
        var coveredByComposite = false;
        for (var relatedIndex = 0; relatedIndex < compositeSelectors.length; relatedIndex++) {
            if (compositeSelectorCoversDirectFocus(removalEntry.selector, compositeSelectors[relatedIndex])) {
                coveredByComposite = true;
                break;
            }
        }
        if (coveredByComposite) continue;
        pushCustomMatch(
            matches,
            "custom:visibleFocusIndicator",
            countLine(text, removalEntry.rule.index),
            removalEntry.selector + " { " + removalEntry.rule.declarations + " }"
        );
    }

    return matches;
}

// PF-FRONTEND-007 只报告高置信度的原生 select 箭头空间问题：
// 1. select 与 input/textarea 共用紧凑 padding，且没有后续同范围专用修正；
// 2. 已显式自定义箭头，但同时给出了过小的 end padding 或零 inset；
// 3. 更高优先级的 background shorthand 明确清除了已有自定义箭头。
// 不对外部 UI 框架、仅使用 class 的组件、CSS 变量/calc 或包装层箭头做猜测。
function splitCssValueParts(value) {
    var result = [];
    var current = "";
    var depth = 0;
    var quote = "";
    var text = String(value || "").trim();
    for (var i = 0; i < text.length; i++) {
        var ch = text[i];
        if (quote) {
            current += ch;
            if (ch === "\\" && i + 1 < text.length) current += text[++i];
            else if (ch === quote) quote = "";
            continue;
        }
        if (ch === "\"" || ch === "'") {
            quote = ch;
            current += ch;
            continue;
        }
        if (ch === "(") depth++;
        else if (ch === ")") depth = Math.max(0, depth - 1);
        if (/\s/.test(ch) && depth === 0) {
            if (current) result.push(current);
            current = "";
            continue;
        }
        current += ch;
    }
    if (current) result.push(current);
    return result;
}

function cssLengthToPixels(value) {
    var normalized = String(value || "").trim().toLowerCase();
    if (!normalized || /\b(?:var|calc|min|max|clamp|env)\(/.test(normalized)) return null;
    var match = /^(-?(?:\d+(?:\.\d+)?|\.\d+))(px|rem|em)?$/.exec(normalized);
    if (!match) return null;
    var number = parseFloat(match[1]);
    var unit = match[2] || "";
    if (!unit && number !== 0) return null;
    if (unit === "rem" || unit === "em") return number * 16;
    return number;
}

function readSelectEndPaddingPixels(map) {
    var direct = map["padding-inline-end"] || map["padding-right"];
    if (direct) return cssLengthToPixels(direct);

    var inlineParts = splitCssValueParts(map["padding-inline"] || "");
    if (inlineParts.length === 1) return cssLengthToPixels(inlineParts[0]);
    if (inlineParts.length === 2) return cssLengthToPixels(inlineParts[1]);

    var parts = splitCssValueParts(map.padding || "");
    if (parts.length === 1) return cssLengthToPixels(parts[0]);
    if (parts.length === 2 || parts.length === 3 || parts.length === 4) {
        return cssLengthToPixels(parts[1]);
    }
    return null;
}

function selectorTargetsNativeSelect(selector) {
    return selectorIdentityTokens(selector).indexOf("select") >= 0;
}

function selectorTargetsTextControl(selector) {
    var tokens = selectorIdentityTokens(selector);
    return tokens.indexOf("input") >= 0 || tokens.indexOf("textarea") >= 0;
}

function normalizeCssSelectorKey(selector) {
    return String(selector || "")
        .trim()
        .toLowerCase()
        .replace(/\s+/g, " ")
        .replace(/\s*([>+~])\s*/g, "$1");
}

function sameSelectorIdentity(left, right) {
    var leftTokens = selectorIdentityTokens(left).slice().sort();
    var rightTokens = selectorIdentityTokens(right).slice().sort();
    if (leftTokens.length !== rightTokens.length) return false;
    for (var i = 0; i < leftTokens.length; i++) {
        if (leftTokens[i] !== rightTokens[i]) return false;
    }
    return leftTokens.length > 0;
}

function selectorSpecificityScore(selector) {
    var text = String(selector || "");
    var ids = (text.match(/#[A-Za-z_][\w-]*/g) || []).length;
    var classes = (text.match(/\.[A-Za-z_][\w-]*/g) || []).length;
    var attributes = (text.match(/\[[^\]]+\]/g) || []).length;
    var pseudoClasses = (text.match(/:(?!:)[A-Za-z-]+(?:\([^)]*\))?/g) || []).length;
    var tokens = selectorIdentityTokens(text);
    var tags = 0;
    for (var i = 0; i < tokens.length; i++) {
        if (tokens[i][0] !== "." && tokens[i][0] !== "#") tags++;
    }
    return ids * 100 + (classes + attributes + pseudoClasses) * 10 + tags;
}

function selectorHasTransientControlState(selector) {
    return /:(?:hover|focus|focus-visible|active|disabled|enabled|invalid|valid|required|optional|checked|open)\b/i.test(String(selector || ""));
}

function mergeCssDeclarationMaps(target, source) {
    var result = target || {};
    if (Object.prototype.hasOwnProperty.call(source, "background")) {
        delete result["background-image"];
        delete result["background-position"];
        delete result["background-position-x"];
        delete result["background-repeat"];
        delete result["background-size"];
        delete result["background-color"];
    }
    if (Object.prototype.hasOwnProperty.call(source, "padding")) {
        delete result["padding-right"];
        delete result["padding-inline"];
        delete result["padding-inline-end"];
    }
    for (var key in source) {
        if (Object.prototype.hasOwnProperty.call(source, key)) result[key] = source[key];
    }
    return result;
}

function cssAppearanceIsNone(map) {
    var value = map.appearance || map["-webkit-appearance"] || map["-moz-appearance"] || "";
    return /^none$/i.test(String(value).trim());
}

function cssHasBackgroundImage(map) {
    var value = map["background-image"] || map.background || "";
    var normalized = String(value).trim().toLowerCase();
    if (!normalized || normalized === "none") return false;
    return /\b(?:url|image-set|cross-fade|(?:repeating-)?(?:linear|radial|conic)-gradient|var)\(/.test(normalized);
}

function cssBackgroundDefinitelyClearsImage(map) {
    if (map["background-image"] && /^none$/i.test(String(map["background-image"]).trim())) return true;
    if (map["background-image"] && cssHasBackgroundImage(map)) return false;
    if (!Object.prototype.hasOwnProperty.call(map, "background")) return false;
    var value = String(map.background || "").trim().toLowerCase();
    if (!value || /\b(?:var|env)\(/.test(value)) return false;
    return !/\b(?:url|image-set|cross-fade|(?:repeating-)?(?:linear|radial|conic)-gradient)\(/.test(value);
}

function cssHasCustomControlChrome(map) {
    var hasBorder = false;
    for (var key in map) {
        if (Object.prototype.hasOwnProperty.call(map, key) && /^border(?:-|$)/.test(key)) {
            hasBorder = true;
            break;
        }
    }
    var hasBackground = !!(map.background || map["background-color"]);
    return hasBorder && hasBackground;
}

function cssHasPositiveArrowInset(map) {
    var value = map["background-position"] || map["background-position-x"] || map.background || "";
    var normalized = String(value).trim().toLowerCase();
    if (!normalized) return null;
    if (/\b(?:var|calc|min|max|clamp)\(/.test(normalized)) return true;
    var rightInset = /\bright\s+(-?(?:\d+(?:\.\d+)?|\.\d+)(?:px|rem|em))\b/.exec(normalized);
    if (rightInset) {
        var insetPixels = cssLengthToPixels(rightInset[1]);
        return insetPixels !== null && insetPixels > 0;
    }
    var percent = /(?:^|\s)(\d+(?:\.\d+)?)%\b/.exec(normalized);
    if (percent) {
        var percentValue = parseFloat(percent[1]);
        return percentValue > 50 && percentValue < 100;
    }
    if (/\bright\b/.test(normalized) || /(?:^|\s)100%\b/.test(normalized)) return false;
    return null;
}

function cssHasKnownSelectSafety(map) {
    var endPadding = readSelectEndPaddingPixels(map);
    if (endPadding !== null && endPadding >= 24) return true;
    return cssAppearanceIsNone(map) && cssHasBackgroundImage(map) &&
        endPadding !== null && endPadding >= 24 && cssHasPositiveArrowInset(map) !== false;
}

function buildSelectStyleGroups(cssRules) {
    var groupsByKey = {};
    var groups = [];
    for (var ruleIndex = 0; ruleIndex < cssRules.length; ruleIndex++) {
        var selectors = splitCssSelectorList(cssRules[ruleIndex].selector);
        var sharesWithTextControl = false;
        for (var otherIndex = 0; otherIndex < selectors.length; otherIndex++) {
            if (selectorTargetsTextControl(selectors[otherIndex])) sharesWithTextControl = true;
        }
        var declarations = parseCssDeclarations(cssRules[ruleIndex].declarations);
        for (var selectorIndex = 0; selectorIndex < selectors.length; selectorIndex++) {
            var selector = selectors[selectorIndex];
            if (!selectorTargetsNativeSelect(selector)) continue;
            var groupKey = normalizeCssSelectorKey(selector);
            var group = groupsByKey[groupKey];
            if (!group) {
                group = {
                    selector: selector,
                    declarations: {},
                    firstIndex: cssRules[ruleIndex].index,
                    lastIndex: cssRules[ruleIndex].index,
                    sharedWithTextControl: false
                };
                groupsByKey[groupKey] = group;
                groups.push(group);
            }
            group.declarations = mergeCssDeclarationMaps(group.declarations, declarations);
            group.lastIndex = cssRules[ruleIndex].index;
            if (sharesWithTextControl) group.sharedWithTextControl = true;
        }
    }
    return groups;
}

function hasLaterSameScopeSelectSafety(groups, group) {
    for (var i = 0; i < groups.length; i++) {
        var candidate = groups[i];
        if (candidate === group || candidate.lastIndex <= group.lastIndex) continue;
        if (!sameSelectorIdentity(group.selector, candidate.selector)) continue;
        if (selectorSpecificityScore(candidate.selector) < selectorSpecificityScore(group.selector)) continue;
        if (cssHasKnownSelectSafety(candidate.declarations)) return true;
    }
    return false;
}

function scanSelectArrowInset(content) {
    var text = String(content || "");
    var hasHtml = /<\/?[A-Za-z][\w:-]*\b[^>]*>/i.test(text);
    var groups = buildSelectStyleGroups(extractCssRules(text, hasHtml));
    var matches = [];
    var seen = {};

    function report(group, reason) {
        var key = group.lastIndex + ":" + normalizeCssSelectorKey(group.selector) + ":" + reason;
        if (seen[key] || matches.length >= 20) return;
        seen[key] = true;
        pushCustomMatch(
            matches,
            "custom:selectArrowInset:" + reason,
            countLine(text, group.lastIndex),
            group.selector + " { " + reason + " }"
        );
    }

    for (var groupIndex = 0; groupIndex < groups.length; groupIndex++) {
        var group = groups[groupIndex];
        var map = group.declarations;
        var endPadding = readSelectEndPaddingPixels(map);

        if (!selectorHasTransientControlState(group.selector) &&
            group.sharedWithTextControl && cssHasCustomControlChrome(map) &&
            endPadding !== null && endPadding < 16 &&
            !hasLaterSameScopeSelectSafety(groups, group)) {
            report(group, "shared-control-padding-leaves-no-select-arrow-safety-area");
        }

        if (!selectorHasTransientControlState(group.selector) &&
            cssAppearanceIsNone(map) && cssHasBackgroundImage(map)) {
            var inset = cssHasPositiveArrowInset(map);
            if (endPadding !== null && endPadding < 24) {
                report(group, "custom-arrow-end-padding-is-too-small");
            } else if (inset === false) {
                report(group, "custom-arrow-is-positioned-flush-to-the-end-edge");
            }
        }
    }

    for (var arrowIndex = 0; arrowIndex < groups.length; arrowIndex++) {
        var arrowGroup = groups[arrowIndex];
        if (!cssAppearanceIsNone(arrowGroup.declarations) || !cssHasBackgroundImage(arrowGroup.declarations)) continue;
        var arrowTokens = selectorIdentityTokens(arrowGroup.selector);
        var arrowSpecificity = selectorSpecificityScore(arrowGroup.selector);
        for (var clearIndex = 0; clearIndex < groups.length; clearIndex++) {
            var clearGroup = groups[clearIndex];
            if (clearGroup === arrowGroup || !cssBackgroundDefinitelyClearsImage(clearGroup.declarations)) continue;
            if (clearGroup.lastIndex < arrowGroup.lastIndex || selectorHasTransientControlState(clearGroup.selector)) continue;
            var clearTokens = selectorIdentityTokens(clearGroup.selector);
            if (!identityTokensAreCovered(arrowTokens, clearTokens)) continue;
            if (selectorSpecificityScore(clearGroup.selector) < arrowSpecificity) continue;
            report(clearGroup, "background-shorthand-clears-a-custom-select-arrow");
        }
    }

    return matches;
}

// —— PF-INTEGRATION-001 专用检查 ——
// fetch 的错误处理识别不使用固定字符窗口，而是识别三种结构信号：
// 1. Promise 链：fetch 之后的同一表达式内存在 catch（.catch( / catch( / } catch (...)）。
// 2. try/catch 块：fetch 位于某个 { } 块内，且同一外层块中 fetch 前有 try、后有 catch。
// 3. 状态检查（保留旧规则语义）：fetch 后较短窗口内有 .ok / .status / statusText。
// 固定窗口（旧版 {0,260}）会把“fetch 后长 Promise 链再 catch”或“try 内 fetch 但 catch 较远”
// 误判为没有错误处理；这里用括号/块结构判断，避免该类 HIGH 误报。
function buildBracePairs(text) {
    var pairs = [];
    var stack = [];
    var i = 0;
    var n = text.length;
    var quote = "";
    var inLineComment = false;
    var inBlockComment = false;
    while (i < n) {
        var ch = text[i];
        var next = i + 1 < n ? text[i + 1] : "";
        if (inLineComment) {
            if (ch === "\n") inLineComment = false;
            i++;
            continue;
        }
        if (inBlockComment) {
            if (ch === "*" && next === "/") {
                inBlockComment = false;
                i += 2;
                continue;
            }
            i++;
            continue;
        }
        if (quote) {
            if (ch === "\\") {
                i += 2;
                continue;
            }
            if (ch === quote) quote = "";
            i++;
            continue;
        }
        if (ch === "/" && next === "/") {
            inLineComment = true;
            i += 2;
            continue;
        }
        if (ch === "/" && next === "*") {
            inBlockComment = true;
            i += 2;
            continue;
        }
        if (ch === "\"" || ch === "'" || ch === "`") {
            quote = ch;
            i++;
            continue;
        }
        if (ch === "{") {
            stack.push(i);
            i++;
            continue;
        }
        if (ch === "}") {
            if (stack.length > 0) {
                var open = stack.pop();
                pairs.push({ open: open, close: i });
            }
            i++;
            continue;
        }
        i++;
    }
    return pairs;
}

// 用空格遮蔽字符串与注释，同时保留字符位置。结构判断因此不会把注释里的
// try/catch 或字符串内容误当成 JavaScript 控制流。
function buildCodeMask(text) {
    var chars = String(text || "").split("");
    var quote = "";
    var inLineComment = false;
    var inBlockComment = false;
    for (var i = 0; i < chars.length; i++) {
        var ch = chars[i];
        var next = i + 1 < chars.length ? chars[i + 1] : "";
        if (inLineComment) {
            if (ch === "\n") {
                inLineComment = false;
            } else {
                chars[i] = " ";
            }
            continue;
        }
        if (inBlockComment) {
            chars[i] = " ";
            if (ch === "*" && next === "/") {
                chars[i + 1] = " ";
                inBlockComment = false;
                i++;
            }
            continue;
        }
        if (quote) {
            chars[i] = " ";
            if (ch === "\\") {
                if (i + 1 < chars.length) chars[++i] = " ";
            } else if (ch === quote) {
                quote = "";
            }
            continue;
        }
        if (ch === "/" && next === "/") {
            chars[i] = chars[i + 1] = " ";
            inLineComment = true;
            i++;
            continue;
        }
        if (ch === "/" && next === "*") {
            chars[i] = chars[i + 1] = " ";
            inBlockComment = true;
            i++;
            continue;
        }
        if (ch === "\"" || ch === "'" || ch === "`") {
            chars[i] = " ";
            quote = ch;
        }
    }
    return chars.join("");
}

// 返回所有包含 index 的块（按 open 升序，最后一个是最内层）。
function findEnclosingBlocks(pairs, index) {
    var blocks = [];
    for (var i = 0; i < pairs.length; i++) {
        if (pairs[i].open < index && index < pairs[i].close) blocks.push(pairs[i]);
    }
    blocks.sort(function (a, b) {
        return a.open - b.open;
    });
    return blocks;
}

// 只有 fetch 真正位于 try 的 body，且该 body 紧跟自己的 catch 时才算被处理。
// 不能使用“某个外层块中前面出现 try、后面出现 catch”的宽松匹配，否则相邻的
// 两个独立 try/catch 会错误覆盖夹在中间的裸 fetch。
function fetchCoveredByEnclosingTryCatch(codeMask, bracePairs, fetchIndex) {
    var blocks = findEnclosingBlocks(bracePairs, fetchIndex);
    for (var i = blocks.length - 1; i >= 0; i--) {
        var block = blocks[i];
        var before = codeMask.substring(0, block.open);
        if (!/\btry\s*$/.test(before)) continue;
        var after = codeMask.substring(block.close + 1);
        if (/^\s*catch\b/.test(after)) return true;
    }
    return false;
}

// 跳过字符串/注释后，在代码位置匹配响应状态检查（忽略注释与字符串中的 .ok/.status）。
function codeWindowHasResponseStatusCheck(text, start, windowLen) {
    var n = text.length;
    var end = Math.min(n, start + windowLen);
    var quote = "";
    var inLineComment = false;
    var inBlockComment = false;
    for (var i = start; i < end; i++) {
        var ch = text[i];
        var next = i + 1 < n ? text[i + 1] : "";
        if (inLineComment) {
            if (ch === "\n") inLineComment = false;
            continue;
        }
        if (inBlockComment) {
            if (ch === "*" && next === "/") {
                inBlockComment = false;
                i++;
            }
            continue;
        }
        if (quote) {
            if (ch === "\\") {
                i++;
            } else if (ch === quote) {
                quote = "";
            }
            continue;
        }
        if (ch === "/" && next === "/") {
            inLineComment = true;
            i++;
            continue;
        }
        if (ch === "/" && next === "*") {
            inBlockComment = true;
            i++;
            continue;
        }
        if (ch === "\"" || ch === "'" || ch === "`") {
            quote = ch;
            continue;
        }
        var slice = text.substring(i, Math.min(n, i + 12));
        if (/^\.ok\b/.test(slice) || /^\.status\b/.test(slice) || /^statusText\b/.test(slice)) return true;
    }
    return false;
}

// 从 openParen（指向 '('）找到匹配的右括号；找不到返回 -1。
function findMatchingParen(text, openParen) {
    var n = text.length;
    if (openParen < 0 || openParen >= n || text[openParen] !== "(") return -1;
    var depth = 0;
    var quote = "";
    var inLineComment = false;
    var inBlockComment = false;
    for (var i = openParen; i < n; i++) {
        var ch = text[i];
        var next = i + 1 < n ? text[i + 1] : "";
        if (inLineComment) {
            if (ch === "\n") inLineComment = false;
            continue;
        }
        if (inBlockComment) {
            if (ch === "*" && next === "/") {
                inBlockComment = false;
                i++;
            }
            continue;
        }
        if (quote) {
            if (ch === "\\") {
                i++;
            } else if (ch === quote) {
                quote = "";
            }
            continue;
        }
        if (ch === "/" && next === "/") {
            inLineComment = true;
            i++;
            continue;
        }
        if (ch === "/" && next === "*") {
            inBlockComment = true;
            i++;
            continue;
        }
        if (ch === "\"" || ch === "'" || ch === "`") {
            quote = ch;
            continue;
        }
        if (ch === "(") {
            depth++;
            continue;
        }
        if (ch === ")") {
            depth--;
            if (depth === 0) return i;
        }
    }
    return -1;
}

// 从 pos 起若存在 .catch( / ?.catch( / .catch?.( / ?.catch?.( 则视为有 Promise 拒绝处理。
function startsWithCatchCall(text, pos) {
    var slice = text.substring(pos, Math.min(text.length, pos + 24)).replace(/^\s+/, "");
    return /^\??\.\s*catch\s*(?:\?\.)?\s*\(/.test(slice);
}

// 从 pos 起沿 .then/.catch/.finally 链前进，若遇到 catch 调用返回 true。
function promiseChainHasCatchFrom(text, pos) {
    var n = text.length;
    var i = pos;
    var guard = 0;
    while (i < n && guard < 40) {
        guard++;
        while (i < n && /\s/.test(text[i])) i++;
        if (i >= n) return false;
        var slice = text.substring(i, Math.min(n, i + 28));
        var member = slice.match(/^\??\.\s*([A-Za-z_$][\w$]*)\s*(?:\?\.)?\s*/);
        if (!member) return false;
        var method = member[1];
        i += member[0].length;
        while (i < n && /\s/.test(text[i])) i++;
        if (i >= n || text[i] !== "(") return false;
        if (method === "catch") return true;
        var close = findMatchingParen(text, i);
        if (close < 0) return false;
        i = close + 1;
    }
    return false;
}

// fetch 位于 Promise.all/race/allSettled(...) 内，且该调用之后有 .catch 链。
function fetchCoveredByOuterPromiseCatch(text, fetchIndex) {
    var re = /Promise\s*\.\s*(?:all|race|allSettled)\s*\(/gi;
    var m;
    while ((m = re.exec(text)) !== null) {
        var openParen = m.index + m[0].length - 1;
        if (openParen > fetchIndex) break;
        var closeParen = findMatchingParen(text, openParen);
        if (closeParen < 0 || fetchIndex > closeParen || fetchIndex < openParen) continue;
        if (promiseChainHasCatchFrom(text, closeParen + 1)) return true;
    }
    return false;
}

function fetchHasErrorHandling(text, fetchIndex, bracePairs, codeMask) {
    var n = text.length;
    var start = fetchIndex + 5; // "fetch" 长度
    var OK_CHECK_WINDOW = 400;
    var CHAIN_WINDOW = 8000;

    // 1) 状态检查：仅在代码位（非注释/字符串）识别 .ok / .status / statusText。
    if (codeWindowHasResponseStatusCheck(text, start, OK_CHECK_WINDOW)) return true;

    // 2) 外包 Promise.all/race/allSettled(...).catch(...)
    if (fetchCoveredByOuterPromiseCatch(text, fetchIndex)) return true;

    // 3) Promise 链：同一表达式内的 .then/.catch/.finally；深度归零后遇非链式 token 或分号则结束（抑制 ASI 邻接误判）。
    var depth = 0;
    var chainEnd = Math.min(n, start + CHAIN_WINDOW);
    var quote = "";
    var inLineComment = false;
    var inBlockComment = false;
    var catchSeen = false;
    for (var i = start; i < chainEnd; i++) {
        var ch = text[i];
        var next = i + 1 < n ? text[i + 1] : "";
        if (inLineComment) {
            if (ch === "\n") inLineComment = false;
            continue;
        }
        if (inBlockComment) {
            if (ch === "*" && next === "/") {
                inBlockComment = false;
                i++;
            }
            continue;
        }
        if (quote) {
            if (ch === "\\") {
                i++;
            } else if (ch === quote) {
                quote = "";
            }
            continue;
        }
        if (ch === "/" && next === "/") {
            inLineComment = true;
            i++;
            continue;
        }
        if (ch === "/" && next === "*") {
            inBlockComment = true;
            i++;
            continue;
        }
        if (ch === "\"" || ch === "'" || ch === "`") {
            quote = ch;
            continue;
        }
        if (ch === "(" || ch === "[" || ch === "{") {
            depth++;
            continue;
        }
        if (ch === ")" || ch === "]" || ch === "}") {
            depth--;
            if (depth < 0) break;
            continue;
        }
        if (depth === 0) {
            if (/\s/.test(ch)) continue;
            if (ch === ";") break;
            var look = text.substring(i, Math.min(n, i + 28));
            var member = look.match(/^\??\.\s*([A-Za-z_$][\w$]*)\s*(?:\?\.)?\s*/);
            if (member) {
                if (member[1] === "catch" && startsWithCatchCall(text, i)) {
                    catchSeen = true;
                    break;
                }
                // 继续进入 .then( / .finally(；把 i 停在 '(' 前，让上方 depth++ 在下一轮处理。
                i += member[0].length - 1;
                continue;
            }
            // 新语句 / ASI / 非链式 token：结束本表达式，避免把邻接 other().catch 算进来。
            break;
        }
    }
    if (catchSeen) return true;

    // 4) try/catch：匹配实际包围 fetch 的 try body 及其对应 catch。
    return fetchCoveredByEnclosingTryCatch(codeMask, bracePairs, fetchIndex);
}

function scanFetchErrorHandling(content) {
    var text = String(content || "");
    var matches = [];
    var bracePairs = buildBracePairs(text);
    var codeMask = buildCodeMask(text);
    var fetchRe = /(?:^|[^A-Za-z0-9_$])(?:await\s+)?fetch\s*\(/gim;
    var m;
    while ((m = fetchRe.exec(text)) !== null && matches.length < 20) {
        var fetchIndex = m.index + m[0].indexOf("fetch");
        if (!fetchHasErrorHandling(text, fetchIndex, bracePairs, codeMask)) {
            var excerpt = text.substring(fetchIndex, Math.min(text.length, fetchIndex + 140))
                .replace(/\s+/g, " ").trim();
            pushCustomMatch(matches, "custom:fetchErrorHandling", countLine(text, fetchIndex), excerpt);
        }
        if (fetchRe.lastIndex === m.index) fetchRe.lastIndex++;
    }
    return matches;
}

function scanKAttributeNoMath(content) {
    var matches = [];
    var attrRe = /k-attribute\s*=\s*("([^"]*)"|'([^']*)')/gim;
    var operatorRe = /(^|[^A-Za-z0-9_.$\]])([A-Za-z0-9_.$\]\)]+)\s*([+\-*/%])\s*([A-Za-z0-9_.$\[(]+)/;
    var attrMatch;

    while ((attrMatch = attrRe.exec(content)) !== null && matches.length < 20) {
        var rawValue = attrMatch[2] !== undefined ? attrMatch[2] : attrMatch[3];
        var braceRe = /\{([^{}]+)\}/g;
        var braceMatch;

        while ((braceMatch = braceRe.exec(rawValue)) !== null && matches.length < 20) {
            var expression = String(braceMatch[1] || "").trim();
            // Class maps commonly contain quoted Tailwind names such as
            // {'bg-primary': condition}. Quoted literals are not expressions.
            var expressionWithoutStrings = expression.replace(/(['"])(?:\\.|(?!\1)[\s\S])*\1/g, "");
            if (!operatorRe.test(expressionWithoutStrings)) continue;

            var startIndex = (attrMatch.index || 0) + braceMatch.index;
            pushCustomMatch(
                matches,
                "custom:kAttributeNoMath",
                countLine(content, startIndex),
                rawValue
            );
        }
    }

    return matches;
}

function countKAttributeOnTag(attrText) {
    var count = 0;
    var attrRe = /(?:^|\s)k-attribute\s*=/gi;
    while (attrRe.exec(attrText) !== null) {
        count++;
    }
    return count;
}

function scanDuplicateKAttribute(content) {
    var matches = [];
    var tagRe = /<([A-Za-z][\w:-]*)(\s[^>]*?)(\/?)>/g;
    var tagMatch;

    while ((tagMatch = tagRe.exec(content)) !== null && matches.length < 20) {
        var attrText = tagMatch[2] || "";
        if (countKAttributeOnTag(attrText) <= 1) continue;

        var idx = tagMatch.index || 0;
        pushCustomMatch(
            matches,
            "custom:duplicateKAttribute",
            countLine(content, idx),
            content.substring(idx, Math.min(content.length, idx + 160))
        );
    }

    return matches;
}

function scanDuplicateKDataAttributes(content) {
    var matches = [];
    var tagRe = /<(let|query|map|when|else-if)\b([^>]*)>/gim;
    var tagMatch;

    while ((tagMatch = tagRe.exec(content)) !== null && matches.length < 20) {
        var seen = {};
        var duplicates = [];
        var attrText = tagMatch[2] || "";
        var attrRe = /(?:^|\s)([A-Za-z_:][\w:.-]*)\s*=/g;
        var attrMatch;
        while ((attrMatch = attrRe.exec(attrText)) !== null) {
            var attrName = String(attrMatch[1] || "").toLowerCase();
            if (seen[attrName] && duplicates.indexOf(attrName) === -1) duplicates.push(attrName);
            seen[attrName] = true;
        }
        if (duplicates.length === 0) continue;
        pushCustomMatch(
            matches,
            "custom:duplicateKDataAttributes",
            countLine(content, tagMatch.index || 0),
            tagMatch[0] + " duplicates: " + duplicates.join(", ")
        );
    }

    return matches;
}

function scanClientScriptMustache(content) {
    var matches = [];
    var scriptRe = /<script\b([^>]*)>([\s\S]*?)<\/script>/gim;
    var scriptMatch;
    while ((scriptMatch = scriptRe.exec(content)) !== null && matches.length < 20) {
        var attrs = scriptMatch[1] || "";
        if (/\benv\s*=\s*(?:"server"|'server'|server)/im.test(attrs)) continue;
        if (/json|template/i.test(readTagAttribute(attrs, "type"))) continue;
        var body = (scriptMatch[2] || "").replace(/(["'`])(?:\\.|(?!\1)[^\\])*?\1/g, "");
        var mustache = /\{\{[^{}]+\}\}/g.exec(body);
        if (!mustache) continue;
        pushCustomMatch(
            matches,
            "custom:clientScriptMustache",
            countLine(content, (scriptMatch.index || 0) + (mustache.index || 0)),
            mustache[0] + " is not Kooboo interpolation inside a client script"
        );
    }
    return matches;
}

function scanKDataBranchTagsInsideKData(content) {
    var matches = [];
    var tagRe = /<(\/?)\s*(k-data|when|else-if|else)\b[^>]*>/gim;
    var depth = 0;
    var tagMatch;

    while ((tagMatch = tagRe.exec(content)) !== null && matches.length < 20) {
        var isClosing = tagMatch[1] === "/";
        var tagName = String(tagMatch[2] || "").toLowerCase();

        if (tagName === "k-data") {
            if (isClosing) {
                depth = Math.max(0, depth - 1);
            } else {
                depth++;
            }
            continue;
        }

        if (!isClosing && depth === 0) {
            var idx = tagMatch.index || 0;
            var excerpt = content.substring(idx, Math.min(content.length, idx + 140));
            pushCustomMatch(
                matches,
                "custom:kDataBranchTagsInsideKData",
                countLine(content, idx),
                excerpt
            );
        }
    }

    return matches;
}

function scanKDataBranchStructure(content) {
    var matches = [];
    var stack = [];
    var voidTags = {
        area: true, base: true, br: true, col: true, embed: true, hr: true,
        img: true, input: true, link: true, meta: true, param: true,
        source: true, track: true, wbr: true
    };
    var tagRe = /<(\/?)\s*([A-Za-z][\w:-]*)\b[^>]*>/gim;
    var tagMatch;

    while ((tagMatch = tagRe.exec(content)) !== null && matches.length < 20) {
        var isClosing = tagMatch[1] === "/";
        var tagName = String(tagMatch[2] || "").toLowerCase();

        if (isClosing) {
            for (var closeIndex = stack.length - 1; closeIndex >= 0; closeIndex--) {
                if (stack[closeIndex].name !== tagName) continue;
                stack.length = closeIndex;
                break;
            }
            continue;
        }

        var parent = stack.length ? stack[stack.length - 1] : null;
        if (tagName === "else-if" || tagName === "else") {
            var invalidReason = "";
            if (!parent || parent.name !== "when") {
                invalidReason = "<" + tagName + "> must be a direct child of <when>";
            } else if (tagName === "else-if" && parent.sawElse) {
                invalidReason = "<else-if> cannot appear after <else> in the same <when>";
            } else if (tagName === "else" && parent.sawElse) {
                invalidReason = "<when> can contain only one direct <else> branch";
            }

            if (invalidReason) {
                pushCustomMatch(
                    matches,
                    "custom:kDataBranchStructure",
                    countLine(content, tagMatch.index || 0),
                    invalidReason + ": " + tagMatch[0]
                );
            } else if (tagName === "else") {
                parent.sawElse = true;
            }
        }

        var isSelfClosing = /\/\s*>$/.test(tagMatch[0]) || !!voidTags[tagName];
        if (!isSelfClosing) stack.push({ name: tagName, sawElse: false });
    }

    return matches;
}

function resolveArtifactUrl(artifact) {
    var direct = String((artifact && (artifact.url || artifact.Url)) || "").trim();
    if (direct) return direct;
    var list = readSiteObjectList(artifact && artifact.objectType ? artifact.objectType : "Api");
    if (!Array.isArray(list)) return "";
    var id = String((artifact && (artifact.id || artifact.nameOrId)) || "").trim().toLowerCase();
    var name = String((artifact && (artifact.name || artifact.nameOrId)) || "").trim().toLowerCase();
    for (var i = 0; i < list.length; i++) {
        var item = list[i] || {};
        if ((id && String(item.id || "").toLowerCase() === id) ||
            (name && String(item.name || "").toLowerCase() === name)) {
            return String(item.url || "").trim();
        }
    }
    return "";
}

function scanApiNamedActionRoute(content, artifact) {
    var matches = [];
    if (!artifact || artifact.objectType !== "Api") return matches;
    var namedActionRe = /k\.api\.(?:get|post|put|patch|delete)\s*\(\s*["'][^"']+["']/gim;
    var firstNamedAction = namedActionRe.exec(content);
    if (!firstNamedAction) return matches;

    var url = resolveArtifactUrl(artifact);
    if (!url || /\{action\}/i.test(url)) return matches;
    pushCustomMatch(
        matches,
        "custom:apiNamedActionRoute",
        countLine(content, firstNamedAction.index || 0),
        firstNamedAction[0] + " requires an API Url containing {action}; current Url is " + url
    );
    return matches;
}

function scanKDataQueryNoSelfClosing(content) {
    var matches = [];
    var queryStartRe = /<query\b/gi;
    var queryStart;

    while ((queryStart = queryStartRe.exec(content)) !== null && matches.length < 20) {
        var startIndex = queryStart.index || 0;
        var quote = "";
        var tagEnd = -1;

        for (var i = queryStartRe.lastIndex; i < content.length; i++) {
            var ch = content[i];
            if (quote) {
                if (ch === quote) quote = "";
                continue;
            }
            if (ch === "\"" || ch === "'") {
                quote = ch;
                continue;
            }
            if (ch === ">") {
                tagEnd = i;
                break;
            }
        }

        if (tagEnd < 0) break;
        queryStartRe.lastIndex = tagEnd + 1;

        var lastContentIndex = tagEnd - 1;
        while (lastContentIndex >= startIndex && /\s/.test(content[lastContentIndex])) {
            lastContentIndex--;
        }
        if (content[lastContentIndex] !== "/") continue;

        pushCustomMatch(
            matches,
            "custom:kDataQueryNoSelfClosing",
            countLine(content, startIndex),
            content.substring(startIndex, Math.min(content.length, tagEnd + 1))
        );
    }

    return matches;
}

function readTagAttribute(attrText, name) {
    var re = new RegExp("(?:^|\\s)" + name + "\\s*=\\s*(?:\"([^\"]*)\"|'([^']*)'|([^\\s>]+))", "i");
    var match = re.exec(String(attrText || ""));
    if (!match) return "";
    return String(match[1] !== undefined ? match[1] : (match[2] !== undefined ? match[2] : match[3] || "")).trim();
}

function hasTagAttribute(attrText, name) {
    var re = new RegExp("(?:^|\\s)" + name + "(?:\\s*=|(?=\\s|$))", "i");
    return re.test(String(attrText || ""));
}

function isEnabledBooleanTagAttribute(attrText, name) {
    if (!hasTagAttribute(attrText, name)) return false;
    var value = readTagAttribute(attrText, name);
    return !value || value.toLowerCase() === "true";
}

function commerceSurfaceIs(artifact, names) {
    var surface = String((artifact && artifact.commerceSurface) || "").trim().toLowerCase();
    if (includesAny(names, [surface])) return true;
    var text = String(artifact && artifact.content || "");
    var operations = {
        login: /k\.commerce\.customer\.login\s*\(/,
        register: /k\.commerce\.customer\.register\s*\(/,
        logout: /k\.commerce\.customer\.logout\s*\(/,
        "auth-status": /k\.commerce\.customer\.(?:currentId|get)\b/,
        cart: /k\.commerce\.cart\./,
        checkout: /k\.commerce\.order\.create\s*\(|k\.payment\.[\w$]+\.charge\s*\(/,
        orders: /k\.commerce\.order\.list\s*\(/,
        "order-detail": /k\.commerce\.order\.get\s*\(/,
        products: /<query\b(?=[^>]*source\s*=\s*["']commerce["'])(?=[^>]*resource\s*=\s*["']product["'])(?=[^>]*action\s*=\s*["'](?:list|paged)["'])/i
    };
    return names.some(function (name) { return operations[name] && operations[name].test(text); });
}

function inferCommerceSurface(artifact) {
    var name = String((artifact && (artifact.name || artifact.nameOrId)) || "").trim().toLowerCase();
    var normalized = name.replace(/\s+/g, "-").replace(/^\/+/, "");
    if (/^(?:auth[-_])?login(?:[-_]|$)|^login[-_]?form$/.test(normalized)) return "login";
    if (/^(?:auth[-_])?register(?:[-_]|$)|^register[-_]?form$/.test(normalized)) return "register";
    if (/^(?:auth[-_])?(?:me|status)(?:[-_]|$)/.test(normalized)) return "auth-status";
    if (/^(?:auth[-_])?logout(?:[-_]|$)/.test(normalized)) return "logout";
    if (/^(?:site[-_])?header(?:[-_]|$)/.test(normalized)) return "header";
    if (/^products(?:[-_]|$)/.test(normalized)) return "products";
    if (/^product[-_]?detail(?:[-_]|$)/.test(normalized)) return "product-detail";
    if (/^cart(?:[-_]|$)/.test(normalized)) return "cart";
    if (/^checkout(?:[-_]|$)/.test(normalized)) return "checkout";
    if (/^(?:pay|payment)(?:[-_]|$)/.test(normalized)) return "checkout";
    if (/^(?:order[-_]?create|create[-_]?order)(?:[-_]|$)/.test(normalized)) return "checkout";
    if (/^order[-_]?result(?:[-_]|$)/.test(normalized)) return "order-result";
    if (/^order[-_]?detail(?:[-_]|$)/.test(normalized)) return "order-detail";
    if (/^orders?(?:[-_]|$)/.test(normalized)) return "orders";
    if (/^account(?:[-_]|$)/.test(normalized)) return "account";
    return "";
}

function scanEnvServerWrapper(content) {
    var matches = [];
    var re = /<([a-z][\w:-]*)\b[^>]*\benv\s*=\s*(?:"server"|'server'|server)[^>]*>/gim;
    var match;
    while ((match = re.exec(content)) !== null && matches.length < 20) {
        if (String(match[1] || "").toLowerCase() === "script") continue;
        pushCustomMatch(
            matches,
            "custom:envServerWrapper",
            countLine(content, match.index || 0),
            match[0] + " should remove env=server from ordinary template containers"
        );
    }
    return matches;
}

function scanCommerceSourceOfTruth(content, artifact) {
    var matches = [];
    if (!commerceSurfaceIs(artifact, ["products", "product-detail", "header", "cart", "checkout", "order-result", "order-detail", "account", "orders"])) return matches;

    var queryRe = /<query\b([^>]*)>/gim;
    var queryMatch;
    while ((queryMatch = queryRe.exec(content)) !== null && matches.length < 20) {
        var attrText = queryMatch[1] || "";
        var source = readTagAttribute(attrText, "source").toLowerCase();
        var resource = readTagAttribute(attrText, "resource").toLowerCase();
        if (resource !== "product" && resource !== "category" && resource !== "cart" && resource !== "order") continue;
        if (source === "commerce") continue;
        pushCustomMatch(
            matches,
            "custom:commerceSourceOfTruth",
            countLine(content, queryMatch.index || 0),
            "Commerce resource " + resource + " uses source=" + (source || "(missing)")
        );
    }
    return matches;
}

function scanCommerceListingPrice(content, artifact) {
    var matches = [];
    if (!commerceSurfaceIs(artifact, ["products"])) return matches;
    var re = /\b[A-Za-z_$][\w$]*\.variants\s*\[\s*0\s*\]\.price\b/gim;
    var match;
    while ((match = re.exec(content)) !== null && matches.length < 20) {
        pushCustomMatch(
            matches,
            "custom:commerceListingPrice",
            countLine(content, match.index || 0),
            match[0] + " should use product.price for listing/starting price"
        );
    }
    return matches;
}

function queryWhereHasActiveTrue(whereValue) {
    var normalized = String(whereValue || "").replace(/\s+/g, "").toLowerCase();
    return /active:(?:\{\$eq:true\}|true)/.test(normalized);
}

function scanCommerceProductsFilter(content, artifact) {
    var matches = [];
    if (!commerceSurfaceIs(artifact, ["products"])) return matches;

    var hasCategoryQuery = false;
    var hasProductQuery = false;
    var productUsesCategory = false;
    var queryRe = /<query\b([^>]*)>/gim;
    var queryMatch;
    while ((queryMatch = queryRe.exec(content)) !== null && matches.length < 20) {
        var attrText = queryMatch[1] || "";
        var source = readTagAttribute(attrText, "source").toLowerCase();
        var resource = readTagAttribute(attrText, "resource").toLowerCase();
        var whereValue = readTagAttribute(attrText, "where");
        if (source !== "commerce") continue;
        if (resource === "category") {
            hasCategoryQuery = true;
            if (!queryWhereHasActiveTrue(whereValue)) {
                pushCustomMatch(matches, "custom:commerceProductsFilter", countLine(content, queryMatch.index || 0), "Category query must filter active=true");
            }
        }
        if (resource === "product") {
            hasProductQuery = true;
            if (/\bcategory\s*:/.test(whereValue)) productUsesCategory = true;
            if (!queryWhereHasActiveTrue(whereValue)) {
                pushCustomMatch(matches, "custom:commerceProductsFilter", countLine(content, queryMatch.index || 0), "Product query must filter active=true");
            }
        }
    }

    if (!hasCategoryQuery) pushCustomMatch(matches, "custom:commerceProductsFilter", 1, "Products surface is missing an active Commerce category query");
    if (!hasProductQuery) pushCustomMatch(matches, "custom:commerceProductsFilter", 1, "Products surface is missing an active Commerce product query");

    var hasCategoryInput = /<let\b(?=[^>]*\bsource\s*=\s*["']queryString["'])(?=[^>]*\bfrom\s*=\s*["']category["'])[^>]*>/im.test(content);
    if (!hasCategoryInput) pushCustomMatch(matches, "custom:commerceProductsFilter", 1, "Products surface must read selected category from queryString");

    if (/\bcategory\s*:\s*["']all["']/im.test(content) || /\bfrom\s*=\s*["']category["'][^>]*\bdefault\s*=\s*["'][^"']*all/im.test(content)) {
        pushCustomMatch(matches, "custom:commerceProductsFilter", 1, "All must omit the category condition; do not query category=all");
    }

    if (productUsesCategory && (!/<when\b/im.test(content) || !/<else\b/im.test(content))) {
        pushCustomMatch(matches, "custom:commerceProductsFilter", 1, "Category filtering needs an explicit filtered branch and an All branch without category");
    }

    var hasBoundActiveClass = /(?:k-attribute\s*=\s*"[^"]*\bclass\b[^"]*(?:active|selected)[^"]*"|k-attribute\s*=\s*'[^']*\bclass\b[^']*(?:active|selected)[^']*')/im.test(content)
        && /(?:selected[_-]?category|category[_-]?(?:active|selected)|is[_-]?active)/im.test(content);
    var hasClientActiveSync = /(?:URLSearchParams|location\.search)[\s\S]{0,2000}(?:classList\.(?:add|toggle)|aria-current)/im.test(content)
        || /(?:classList\.(?:add|toggle)|aria-current)[\s\S]{0,2000}(?:URLSearchParams|location\.search)/im.test(content);
    if (!hasBoundActiveClass && !hasClientActiveSync) {
        pushCustomMatch(matches, "custom:commerceProductsFilter", 1, "Category selector must bind its active state to the selected category instead of a static class");
    }

    return matches;
}

function scanCommerceProductsBundle(artifacts, findings, maxMatchesPerRule, rule, context) {
    if (!rule) return;
    var productsArtifacts = artifacts.filter(function (artifact) {
        return ruleApplies(rule, context, artifact) && commerceSurfaceIs(artifact, ["products"]);
    });
    if (productsArtifacts.length === 0) return;

    var combinedContent = productsArtifacts.map(function (artifact) {
        return "\n<!-- artifact:" + artifact.objectType + ":" + artifact.name + " -->\n" + String(artifact.content || "");
    }).join("\n");
    var bundleArtifact = {
        name: productsArtifacts.map(function (artifact) { return artifact.name; }).join(" + "),
        objectType: "PageBundle",
        commerceSurface: "products"
    };
    var matches = scanCommerceProductsFilter(combinedContent, bundleArtifact);

    var categoryQueryCount = (combinedContent.match(/<query\b(?=[^>]*\bsource\s*=\s*["']commerce["'])(?=[^>]*\bresource\s*=\s*["']category["'])[^>]*>/gim) || []).length;
    var categorySelectorCount = (combinedContent.match(/\bdata-category-filter\b/gim) || []).length;
    if (categoryQueryCount > 1 || categorySelectorCount > 1) {
        pushCustomMatch(matches, "cross:commerceProductsFilter", 1, "Products page contains duplicate category queries or category selectors");
    }

    if (matches.length === 0) return;
    findings.push({
        id: rule.id,
        title: rule.title || "",
        severity: String(rule.severity || "MEDIUM").toUpperCase(),
        requiredStage: normalizeDeliveryStage(rule.requiredStage) || "static-first",
        artifact: {
            name: bundleArtifact.name,
            objectType: bundleArtifact.objectType
        },
        tags: asArray(rule.tags),
        fix: rule.fix || "",
        matches: matches.slice(0, maxMatchesPerRule),
        matchCount: matches.length
    });
}

function scanCommerceProductDetailQuery(content, artifact) {
    var matches = [];
    if (!commerceSurfaceIs(artifact, ["product-detail"])) return matches;

    var queryStringLets = {};
    var letRe = /<let\b([^>]*)>/gim;
    var letMatch;
    while ((letMatch = letRe.exec(content)) !== null) {
        var letAttrs = letMatch[1] || "";
        if (readTagAttribute(letAttrs, "source").toLowerCase() !== "querystring") continue;
        var asName = readTagAttribute(letAttrs, "as");
        if (asName) queryStringLets[asName] = readTagAttribute(letAttrs, "from");
    }

    var foundProductItem = false;
    var queryRe = /<query\b([^>]*)>/gim;
    var queryMatch;
    while ((queryMatch = queryRe.exec(content)) !== null && matches.length < 20) {
        var attrs = queryMatch[1] || "";
        if (readTagAttribute(attrs, "source").toLowerCase() !== "commerce") continue;
        if (readTagAttribute(attrs, "resource").toLowerCase() !== "product") continue;
        if (readTagAttribute(attrs, "action").toLowerCase() !== "item") continue;
        foundProductItem = true;
        var idValue = readTagAttribute(attrs, "id");
        if (!idValue) {
            pushCustomMatch(matches, "custom:commerceProductDetailQuery", countLine(content, queryMatch.index || 0), "Commerce product item query must provide id");
            continue;
        }
        if (!queryStringLets[idValue]) {
            pushCustomMatch(matches, "custom:commerceProductDetailQuery", countLine(content, queryMatch.index || 0), "Product item id must reference a <let source=queryString> variable; unresolved id=" + idValue);
        }
    }

    if (foundProductItem && Object.keys(queryStringLets).length === 0) {
        pushCustomMatch(matches, "custom:commerceProductDetailQuery", 1, "Product detail item query must read its id or seoName from queryString");
    }
    return matches;
}

function scanCommerceAuthContract(content, artifact) {
    var matches = [];
    var surface = String((artifact && artifact.commerceSurface) || "").trim().toLowerCase();
    var objectType = String((artifact && artifact.objectType) || "").toLowerCase();

    if (surface === "login") {
        if (objectType === "api" && !/k\.commerce\.customer\.login\s*\(/im.test(content)) {
            pushCustomMatch(matches, "custom:commerceAuthContract", 1, "Commerce login API must call k.commerce.customer.login");
        }
        if (objectType === "view" && !/fetch\s*\([^)]*(?:login|auth)/im.test(content)) {
            pushCustomMatch(matches, "custom:commerceAuthContract", 1, "Login View must submit to the Commerce login API");
        }
    }

    if (surface === "register") {
        if (objectType === "api" && /k\.commerce\.customer\.create\s*\(/im.test(content) && !/k\.commerce\.customer\.login\s*\(/im.test(content)) {
            pushCustomMatch(matches, "custom:commerceAuthContract", 1, "Registration API must call customer.login after customer.create to establish kb_commerce");
        }
    }

    if (surface === "auth-status" && objectType === "api") {
        if (!/k\.commerce\.customer\.currentId\b/im.test(content)) {
            pushCustomMatch(matches, "custom:commerceAuthContract", 1, "Auth status API must read k.commerce.customer.currentId from kb_commerce");
        }
        if (!/k\.commerce\.customer\.get\s*\(/im.test(content)) {
            pushCustomMatch(matches, "custom:commerceAuthContract", 1, "Auth status API must read the current Commerce customer");
        }
    }

    if (surface === "logout" && objectType === "api" && !/k\.commerce\.customer\.logout\s*\(/im.test(content)) {
        pushCustomMatch(matches, "custom:commerceAuthContract", 1, "Commerce logout API must call k.commerce.customer.logout");
    }

    if (surface === "account" && (objectType === "view" || objectType === "script")) {
        if (!/fetch\s*\([^)]*(?:auth[^)]*me|\/api\/me|auth[^)]*status)/im.test(content)) {
            pushCustomMatch(matches, "custom:commerceAuthContract", 1, "Account must load the shared Commerce auth status endpoint");
        }
    }

    if (surface && /(?:localStorage\.(?:setItem|getItem)\s*\([^)]*commerce_token|k\.cookie\.get\s*\(\s*["']commerce_token["'])/im.test(content)) {
        pushCustomMatch(matches, "custom:commerceAuthContract", 1, "Commerce surfaces must not use commerce_token as a second login source; use customer.currentId and kb_commerce");
    }

    if (surface && /k\.cookie\.get\s*\(\s*["']kb_commerce["']/im.test(content)) {
        pushCustomMatch(matches, "custom:commerceAuthContract", 1, "Do not parse kb_commerce manually; use customer.currentId, cart.currentId and Commerce APIs");
    }

    return matches;
}

function scanCommerceCartLineContract(content, artifact) {
    var matches = [];
    if (!commerceSurfaceIs(artifact, ["cart", "checkout", "header"])) return matches;
    var re = /\b(?:item|line|cartLine)\.(unitPrice|totalPrice)\b/gim;
    var match;
    while ((match = re.exec(content)) !== null && matches.length < 20) {
        pushCustomMatch(
            matches,
            "custom:commerceCartLineContract",
            countLine(content, match.index || 0),
            match[0] + " is not a CartDetail line field; use price for unit price or amount for line total"
        );
    }
    return matches;
}

function hasCommerceCustomerComparison(content, objectNames, customerPath) {
    var currentIdNames = ["currentId", "customerId"];
    var declarationRe = /\b(?:const|let|var)\s+([A-Za-z_$][\w$]*)\s*=\s*k\.commerce\.customer\.currentId\b/gim;
    var declarationMatch;
    while ((declarationMatch = declarationRe.exec(content)) !== null) {
        if (currentIdNames.indexOf(declarationMatch[1]) < 0) currentIdNames.push(declarationMatch[1]);
    }

    var customerPathPattern = customerPath === "customerId"
        ? "\\??\\.customerId"
        : "\\??\\.customer\\??\\.id";
    var objectPattern = "(?:" + objectNames.join("|") + ")" + customerPathPattern;
    for (var i = 0; i < currentIdNames.length; i++) {
        var idPattern = currentIdNames[i].replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
        var comparison = new RegExp(
            "(?:" + objectPattern + "\\s*(?:===|==|!==|!=)\\s*" + idPattern +
            "|" + idPattern + "\\s*(?:===|==|!==|!=)\\s*" + objectPattern + ")",
            "im"
        );
        if (comparison.test(content)) return true;
    }

    var directCurrentId = new RegExp(
        "(?:" + objectPattern + "\\s*(?:===|==|!==|!=)\\s*k\\.commerce\\.customer\\.currentId" +
        "|k\\.commerce\\.customer\\.currentId\\s*(?:===|==|!==|!=)\\s*" + objectPattern + ")",
        "im"
    );
    return directCurrentId.test(content);
}

function scanCommerceOrderOwnership(content, artifact) {
    var matches = [];
    if (!commerceSurfaceIs(artifact, ["account", "orders", "order-result", "order-detail", "checkout"])) return matches;

    var readsOrders = /k\.commerce\.order\.(?:get|list)\s*\(/im.test(content);
    var startsPayment = /k\.payment\.[A-Za-z_$][\w$]*\.charge\s*\(/im.test(content);
    if (!readsOrders && !startsPayment) return matches;

    var hasCurrentCustomer = /k\.commerce\.customer\.currentId\b/im.test(content);
    if (!hasCurrentCustomer) {
        pushCustomMatch(matches, "custom:commerceOrderOwnership", 1, "Storefront order and payment APIs must derive identity from k.commerce.customer.currentId");
    }

    var readsSingleOrder = /k\.commerce\.order\.get\s*\(/im.test(content);
    if ((readsSingleOrder || startsPayment) && !hasCommerceCustomerComparison(content, ["order", "createdOrder", "newOrder", "result", "detail"])) {
        pushCustomMatch(matches, "custom:commerceOrderOwnership", 1, "Order detail/payment must compare order.customer.id with the current Commerce customer before returning data or charging");
    }

    if (/k\.commerce\.order\.list\s*\(/im.test(content) && /(?:body|query|params|request)\.customerId\b/im.test(content)) {
        pushCustomMatch(matches, "custom:commerceOrderOwnership", 1, "Order list must not accept an arbitrary customerId from the public request");
    }

    return matches;
}


function scanCommerceClientDataBoundary(content, artifact) {
    var matches = [];
    if (!commerceSurfaceIs(artifact, ["product-detail"])) return matches;
    var exports = {};
    var exportRe = /<(let|query)\b([^>]*)>/gim;
    var exportMatch;
    while ((exportMatch = exportRe.exec(content)) !== null) {
        var attrText = exportMatch[2] || "";
        if (!isEnabledBooleanTagAttribute(attrText, "export")) continue;
        var name = readTagAttribute(attrText, "as");
        if (name) exports[name] = true;
    }
    if (Object.keys(exports).length === 0) return matches;

    var scriptRe = /<script\b([^>]*)>([\s\S]*?)<\/script>/gim;
    var scriptMatch;
    while ((scriptMatch = scriptRe.exec(content)) !== null && matches.length < 20) {
        if (/\benv\s*=\s*(?:["']server["']|server)/im.test(scriptMatch[1] || "")) continue;
        var body = (scriptMatch[2] || "").replace(/(["'`])(?:\\.|(?!\1)[^\\])*?\1/g, "");
        for (var name in exports) {
            if (!Object.prototype.hasOwnProperty.call(exports, name)) continue;
            if (new RegExp("\\b(?:const|let|var)\\s+" + escapeRegExp(name) + "\\b").test(body)) continue;
            var directReference = new RegExp("\\b" + name.replace(/[.*+?^${}()|[\\]\\]/g, "\\$&") + "\\s*(?:\\.|\\[)", "m");
            var directMatch = directReference.exec(body);
            if (!directMatch) continue;
            pushCustomMatch(matches, "custom:commerceClientDataBoundary", countLine(content, (scriptMatch.index || 0) + (directMatch.index || 0)), name + " is a server-rendered k-data export; render required values into data-* or safe JSON before client use");
        }
    }
    return matches;
}

function scanCommerceAddToCartContract(content, artifact) {
    var matches = [];
    if (!/(?:addOrUpdateLine|fetch\s*\([^)]*(?:cart[^)]*add|add[^)]*cart))/i.test(content)) return matches;
    if (!/variantId|selectedVariant|data-variant-id/.test(content)) oneCheck(matches, "cartVariantReview", content, 0, "Verify the selected/default variant is included in the add-to-cart request.");
    if (!/finally|onSettled|onSuccess|dispatchEvent|invalidateQueries|refreshCart|cartStore|store\./.test(content)) oneCheck(matches, "cartStateReview", content, 0, "Verify control recovery and shared cart-state refresh through the application state mechanism.");
    return matches;
}

function scanCommercePaymentContract(content, artifact) {
    var matches = [];
    if (!commerceSurfaceIs(artifact, ["checkout", "order-result"])) return matches;
    if (/k\.payment\.alipay(?:Form|H5)\.charge\s*\(/im.test(content)) {
        if (!/\border\s*:/im.test(content)) {
            pushCustomMatch(matches, "custom:commercePaymentContract", 1, "Alipay charge must include the Commerce order field");
        }
        if (!/\brequestId\b/im.test(content)) {
            pushCustomMatch(matches, "custom:commercePaymentContract", 1, "Preserve charge response.requestId for pending/status recovery");
        }
        if (!/\breturnUrl\s*:/im.test(content)) {
            pushCustomMatch(matches, "custom:commercePaymentContract", 1, "Alipay charge must provide an order-result returnUrl; the return is not payment proof");
        }
        if (!hasCommerceCustomerComparison(content, ["order", "createdOrder", "newOrder", "result", "detail"])) {
            pushCustomMatch(matches, "custom:commercePaymentContract", 1, "Payment charge path must verify order.customer.id against the current Commerce customer");
        }
        if (!/\bcurrency\s*:\s*["']CNY["']/im.test(content)) {
            pushCustomMatch(matches, "custom:commercePaymentContract", 1, "Alipay charge must use a CNY Commerce order and pass currency: CNY");
        }
        if (/<script\b[^>]*\benv\s*=\s*(?:["']server["']|server)[^>]*>/im.test(content)) {
            pushCustomMatch(matches, "custom:commercePaymentContract", 1, "Do not put Alipay charge in <script env=server>; navigate to a Payment API/CodeBlock that writes renderHtml directly");
        }
        if (!/k\.response\.write\s*\([^)]*renderHtml/im.test(content)) {
            pushCustomMatch(matches, "custom:commercePaymentContract", 1, "Payment endpoint must write response.nextAction.renderHtml directly with k.response.write");
        }
    }
    if (/innerHTML\s*=\s*[^;]*renderHtml|insertAdjacentHTML\s*\([^,]+,[^)]*renderHtml/im.test(content)) {
        pushCustomMatch(matches, "custom:commercePaymentContract", 1, "Do not inject Alipay renderHtml with client innerHTML; render it in the server response");
    }
    if (/k\.payment\.[A-Za-z_$][\w$]*\.charge\s*\(/im.test(content) && /return\s*\{[\s\S]*renderHtml/im.test(content) && /k\.api\.(?:get|post)\s*\(/im.test(content)) {
        pushCustomMatch(matches, "custom:commercePaymentContract", 1, "Do not return provider renderHtml as JSON for client injection; navigate directly to the Payment endpoint and write renderHtml in its server response");
    }
    return matches;
}

function scanCommerceCheckoutContract(content, artifact) {
    var matches = [];
    if (!commerceSurfaceIs(artifact, ["checkout"])) return matches;
    if (!/k\.commerce\.order\.create\s*\(/im.test(content)) return matches;

    if (!/k\.commerce\.customer\.currentId\b/im.test(content)) {
        pushCustomMatch(matches, "custom:commerceCheckoutContract", 1, "Checkout order.create must require k.commerce.customer.currentId");
    }
    if (!/k\.commerce\.cart\.currentId\b/im.test(content)) {
        pushCustomMatch(matches, "custom:commerceCheckoutContract", 1, "Checkout order.create must use k.commerce.cart.currentId, not a stale client-only cart id");
    }
    if (!hasCommerceCustomerComparison(content, ["cart", "currentCart", "activeCart", "cartDetail"], "customerId")) {
        pushCustomMatch(matches, "custom:commerceCheckoutContract", 1, "Checkout must compare cart.customerId with the current Commerce customer before order.create");
    }
    if (!hasCommerceCustomerComparison(content, ["order", "createdOrder", "newOrder", "result"])) {
        pushCustomMatch(matches, "custom:commerceCheckoutContract", 1, "Checkout must compare the created order.customer.id with the current Commerce customer before starting payment");
    }
    if (/innerHTML\s*=\s*[^;]*renderHtml|insertAdjacentHTML\s*\([^,]+,[^)]*renderHtml/im.test(content)) {
        pushCustomMatch(matches, "custom:commerceCheckoutContract", 1, "Checkout must not inject payment renderHtml via client innerHTML");
    }
    return matches;
}

function scanCommerceOrderResultContract(content, artifact) {
    var matches = [];
    if (!commerceSurfaceIs(artifact, ["order-result", "account", "orders"])) return matches;

    if (commerceSurfaceIs(artifact, ["order-result"])) {
        var readsOrder =
            /k\.commerce\.order\.get\s*\(/im.test(content) ||
            /fetch\s*\([^)]*(?:order|orders|order-status|orderStatus)/im.test(content);
        if (!readsOrder) {
            pushCustomMatch(matches, "custom:commerceOrderResultContract", 1, "Order result must re-read the Commerce order or payment status by orderId; returnUrl alone is not proof of payment");
        }
        if (!/(?:paid|pending|canceled|cancelled|failed|支付|待支付|处理中)/im.test(content)) {
            pushCustomMatch(matches, "custom:commerceOrderResultContract", 1, "Order result must distinguish paid/pending/failed states from the re-read order");
        }
    }

    var accountHasOrderCapability = artifact._commercePaymentCapability ||
        /k\.commerce\.order\.list\s*\(|fetch\s*\([^)]*(?:orders|order\/list|customer\/orders)|order[-_]?detail|订单详情/im.test(content);
    if (commerceSurfaceIs(artifact, ["orders"]) || (commerceSurfaceIs(artifact, ["account"]) && accountHasOrderCapability)) {
        var listsOrders =
            /k\.commerce\.order\.list\s*\(/im.test(content) ||
            /fetch\s*\([^)]*(?:orders|order\/list|customer\/orders)/im.test(content);
        if (!listsOrders) {
            pushCustomMatch(matches, "custom:commerceOrderResultContract", 1, "Account/Orders must list the current customer's orders");
        }
        if (!/(?:order[-_]?detail|orderDetail|\/orders?\/[^\s"'?#]+|订单详情)/im.test(content)) {
            pushCustomMatch(matches, "custom:commerceOrderResultContract", 1, "Account/Orders must provide an order detail entry point");
        }
    }

    return matches;
}

function scanCommerceHeaderEntrypoints(content, artifact) {
    var matches = [];
    var model = String((artifact && artifact.commerceModel) || "").trim().toLowerCase();
    if (model !== "online-checkout" || !commerceSurfaceIs(artifact, ["header"])) return matches;
    if (!/(cart|basket|购物车)/im.test(content)) {
        pushCustomMatch(matches, "custom:commerceHeaderEntrypoints", 1, "Online-checkout Header is missing a cart entry");
    }
    if (!/(account|login|register|customer|orders|登录|注册|账户|账号|订单)/im.test(content)) {
        pushCustomMatch(matches, "custom:commerceHeaderEntrypoints", 1, "Online-checkout Header is missing login/account/order entry points");
    }
    if (!/(?:fetch\s*\([^)]*(?:auth[^)]*me|\/api\/me|auth[^)]*status))/im.test(content)) {
        pushCustomMatch(matches, "custom:commerceHeaderEntrypoints", 1, "Online-checkout Header must fetch the shared Commerce auth-status endpoint");
    }

    var guestNodes = content.match(/<[^>]*\bdata-auth-guest\b[^>]*>/gim) || [];
    var userNodes = content.match(/<[^>]*\bdata-auth-user\b[^>]*>/gim) || [];
    if (!guestNodes.length || !userNodes.length) {
        pushCustomMatch(matches, "custom:commerceHeaderEntrypoints", 1, "Header must provide data-auth-guest and data-auth-user groups for mutual exclusion");
    } else {
        var guestHidden = guestNodes.every(function (tag) { return /(?:^|\s)hidden(?:\s|$)/i.test(readTagAttribute(tag, "class")); });
        var userHidden = userNodes.every(function (tag) { return /(?:^|\s)hidden(?:\s|$)/i.test(readTagAttribute(tag, "class")); });
        if (!guestHidden || !userHidden) {
            pushCustomMatch(matches, "custom:commerceHeaderEntrypoints", 1, "Header data-auth-guest and data-auth-user must both start with hidden until auth-status resolves; otherwise login and account flash together");
        }

        var responsiveDisplay = /^(?:sm|md|lg|xl|2xl):(?:block|inline|inline-block|flex|inline-flex|grid|inline-grid|table|table-row|table-cell)$/i;
        var conflictingNodes = guestNodes.concat(userNodes).filter(function (tag) {
            return readTagAttribute(tag, "class").split(/\s+/).some(function (className) {
                return responsiveDisplay.test(className);
            });
        });
        if (conflictingNodes.length) {
            pushCustomMatch(matches, "custom:commerceHeaderEntrypoints", 1, "Auth visibility nodes must not combine hidden with responsive display classes such as md:flex; put breakpoint display on an outer wrapper and flex/grid on an inner wrapper");
        }

        var selectsGuest = /querySelectorAll\s*\(\s*["']\[data-auth-guest\]["']\s*\)/im.test(content);
        var selectsUser = /querySelectorAll\s*\(\s*["']\[data-auth-user\]["']\s*\)/im.test(content);
        var toggleCount = (content.match(/classList\.toggle\s*\(\s*["']hidden["']\s*,/gim) || []).length;
        if (!selectsGuest || !selectsUser || toggleCount < 2 || !/\bauthenticated\b/im.test(content)) {
            pushCustomMatch(matches, "custom:commerceHeaderEntrypoints", 1, "Header must update every guest and user group from the same authenticated state and toggle both sides mutually");
        }
    }

    if (!/(?:fetch\s*\([^)]*(?:cart|basket)|cart[-_]?count|cartCount|totalQuantity|itemCount|购物车数量|<query\b[^>]*resource\s*=\s*["']cart["'])/im.test(content)) {
        pushCustomMatch(matches, "custom:commerceHeaderEntrypoints", 1, "Online-checkout Header must consume live cart quantity state");
    }
    if (!/(?:addEventListener\s*\(\s*["']kooboo:cart-changed|watch\s*\([^)]*cart|subscribe\s*\([^)]*cart)/im.test(content)) {
        pushCustomMatch(matches, "custom:commerceHeaderEntrypoints", 1, "Header must refresh cart quantity when the shared cart-changed signal fires");
    }
    return matches;
}

function scanKDataQueryContract(content) {
    var matches = [];
    var queryRe = /<query\b([^>]*)>/gim;
    var supportedSources = {
        commerce: true,
        content: true,
        culture: true,
        indexeddb: true,
        mysql: true,
        sqlite: true,
        sqlserver: true
    };
    var supportedActions = { item: true, list: true, paged: true };
    var queryMatch;

    while ((queryMatch = queryRe.exec(content)) !== null && matches.length < 20) {
        var attrText = queryMatch[1] || "";
        var asName = readTagAttribute(attrText, "as");
        var source = readTagAttribute(attrText, "source").toLowerCase();
        var resource = readTagAttribute(attrText, "resource");
        var action = readTagAttribute(attrText, "action").toLowerCase();
        var issues = [];
        if (!asName) issues.push("missing as");
        if (!source) issues.push("missing source");
        if (!resource) issues.push("missing resource");
        if (!action) issues.push("missing action");
        if (source && !supportedSources[source]) issues.push("unsupported source=" + source);
        if (action && !supportedActions[action]) issues.push("unsupported action=" + action);
        if (source === "culture" && action === "paged") issues.push("unsupported source/action=culture:paged");
        if (issues.length === 0) continue;

        var index = queryMatch.index || 0;
        pushCustomMatch(
            matches,
            "custom:kDataQueryContract",
            countLine(content, index),
            issues.join(", ") + " | " + content.substring(index, Math.min(content.length, index + 180))
        );
    }

    return matches;
}

function extractKDataRegions(content) {
    var regions = [];
    var tagRe = /<(\/?)\s*k-data\b[^>]*>/gim;
    var depth = 0;
    var start = -1;
    var tagMatch;
    while ((tagMatch = tagRe.exec(content)) !== null) {
        var isClosing = tagMatch[1] === "/";
        if (!isClosing) {
            if (depth === 0) start = tagMatch.index || 0;
            depth++;
            continue;
        }
        depth = Math.max(0, depth - 1);
        if (depth === 0 && start >= 0) {
            regions.push({ start: start, end: tagRe.lastIndex });
            start = -1;
        }
    }
    if (start >= 0) regions.push({ start: start, end: content.length });
    return regions;
}

function isIndexInsideRegions(index, regions) {
    for (var i = 0; i < regions.length; i++) {
        if (index >= regions[i].start && index < regions[i].end) return true;
    }
    return false;
}

function scanKDataElementsInsideKData(content) {
    var matches = [];
    var regions = extractKDataRegions(content);
    var tagRe = /<(query|try|catch|redirect)\b[^>]*>/gim;
    var tagMatch;
    while ((tagMatch = tagRe.exec(content)) !== null && matches.length < 20) {
        var index = tagMatch.index || 0;
        if (isIndexInsideRegions(index, regions)) continue;
        pushCustomMatch(
            matches,
            "custom:kDataElementsInsideKData",
            countLine(content, index),
            content.substring(index, Math.min(content.length, index + 160))
        );
    }
    return matches;
}

function scanKDataExportContract(content) {
    var matches = [];
    var regions = extractKDataRegions(content);

    for (var r = 0; r < regions.length && matches.length < 20; r++) {
        var region = regions[r];
        var body = content.substring(region.start, region.end);
        var exportRe = /<(let|query)\b([^>]*)>/gim;
        var exportMatch;
        while ((exportMatch = exportRe.exec(body)) !== null && matches.length < 20) {
            var attrText = exportMatch[2] || "";
            if (!hasTagAttribute(attrText, "export")) continue;
            var asName = readTagAttribute(attrText, "as");
            var exportValue = readTagAttribute(attrText, "export").toLowerCase();
            var issue = "";
            if (!asName) issue = "export attribute requires non-empty as";
            else if (exportValue && exportValue !== "true" && exportValue !== "false") issue = "export must be valueless, true, or false";
            if (!issue) continue;

            var index = region.start + (exportMatch.index || 0);
            pushCustomMatch(
                matches,
                "custom:kDataExportContract",
                countLine(content, index),
                issue + " | " + content.substring(index, Math.min(content.length, index + 160))
            );
        }
    }

    return matches;
}

function extractLayoutBodyRegions(content) {
    var regions = [];
    var openRe = /<layout\b[^>]*>/gi;
    var closeRe = /<\/layout>/gi;
    var openMatch;
    while ((openMatch = openRe.exec(content)) !== null) {
        var bodyStart = (openMatch.index || 0) + openMatch[0].length;
        closeRe.lastIndex = bodyStart;
        var closeMatch = closeRe.exec(content);
        regions.push({
            bodyStart: bodyStart,
            bodyEnd: closeMatch ? closeMatch.index : content.length
        });
    }
    return regions;
}

function isIndexInsideLayoutBody(index, regions) {
    for (var i = 0; i < regions.length; i++) {
        var region = regions[i];
        if (index >= region.bodyStart && index < region.bodyEnd) return true;
    }
    return false;
}

function scanLayoutWrapsPlaceholder(content, artifact) {
    var matches = [];
    if (!getArtifactLayoutName(artifact)) return matches;
    if (!/<placeholder\b/i.test(content)) return matches;

    var layoutRegions = extractLayoutBodyRegions(content);
    if (layoutRegions.length === 0) return matches;

    var placeholderRe = /<placeholder\b/gi;
    var placeholderMatch;
    while ((placeholderMatch = placeholderRe.exec(content)) !== null) {
        var placeholderIndex = placeholderMatch.index || 0;
        if (isIndexInsideLayoutBody(placeholderIndex, layoutRegions)) continue;

        pushCustomMatch(
            matches,
            "custom:layoutWrapsPlaceholder",
            countLine(content, placeholderIndex),
            content.substring(placeholderIndex, Math.min(content.length, placeholderIndex + 140)).replace(/\s+/g, " ").trim()
        );
    }
    return matches;
}

function scanLayoutIdMatchesArtifactLayoutName(content, artifact) {
    var matches = [];
    var expectedLayoutName = String((artifact && artifact.layoutName) || "").trim();
    if (!expectedLayoutName) return matches;

    var layoutRe = /<layout\b([^>]*)>/im;
    var layoutMatch = layoutRe.exec(content);
    if (!layoutMatch) return matches;

    var attrText = String(layoutMatch[1] || "");
    var idMatch = /\bid\s*=\s*("([^"]*)"|'([^']*)')/im.exec(attrText);
    if (!idMatch) return matches;

    var actualLayoutId = String(idMatch[2] !== undefined ? idMatch[2] : idMatch[3] || "").trim();
    if (actualLayoutId === expectedLayoutName) return matches;

    pushCustomMatch(
        matches,
        "custom:layoutIdMatchesArtifactLayoutName",
        countLine(content, layoutMatch.index || 0),
        "<layout id=\"" + actualLayoutId + "\"> expected id=\"" + expectedLayoutName + "\""
    );
    return matches;
}

function escapeRegExp(value) { return String(value).replace(/[.*+?^${}()|[\]\\]/g, "\\$&"); }
// Conservative structural checks. Intent/performance/data-flow questions are
// returned as advisories; they cannot substitute for runtime/business evidence.
function oneCheck(matches, name, text, index, excerpt) {
    pushCustomMatch(matches, "custom:" + name, countLine(text, index || 0), excerpt || text.slice(index || 0, (index || 0) + 140));
}

function maskScriptAndStyle(text) {
    return String(text).replace(/<(script|style)\b[^>]*>[\s\S]*?<\/\1\s*>/gi, function (part) { return part.replace(/[^\r\n]/g, " "); });
}

function scanUnfinishedContent(text, artifact) {
    var matches = [];
    var html = includesAny(["Page", "Layout", "View"], [artifact.objectType]);
    var re = /<!--([\s\S]*?)-->|\/\*([\s\S]*?)\*\/|(?:^|\n)\s*\/\/([^\n]*)/g;
    var m;
    while ((m = re.exec(text)) !== null) {
        if (/\b(?:TODO|FIXME|TBD)\b|这里省略|待补充|未实现/i.test(m[0])) oneCheck(matches, "unfinishedComment", text, m.index);
    }
    if (html) {
        var visible = maskScriptAndStyle(text).replace(/<(pre|code)\b[^>]*>[\s\S]*?<\/\1\s*>/gi, "");
        var copy = />\s*(TODO|FIXME|TBD|coming soon|这里省略|待补充|未实现|\[?placeholder\]?|placeholder\s+(?:text|content|copy|image|title|description))\s*</gi;
        while ((m = copy.exec(visible)) !== null) oneCheck(matches, "unfinishedCopy", text, m.index, m[0]);
    }
    return matches;
}

function scanEmojiPolicy(text) {
    var matches = [];
    // Unicode properties via constructor keep this script parseable on older engines.
    var re;
    try { re = new RegExp("\\p{Extended_Pictographic}", "gu"); } catch (e) { return matches; }
    var m;
    while ((m = re.exec(maskScriptAndStyle(text))) !== null && matches.length < 5) oneCheck(matches, "emojiPolicy", text, m.index, "Review requested/brand-approved emoji: " + m[0]);
    return matches;
}

function scanFakeUnicodeIcons(text) {
    var matches = [];
    var glyphs = "▣▤○⌁⌕→←↑↓◆◇□■●★×⋮";
    var emojiRe = null;
    try { emojiRe = new RegExp("\\p{Extended_Pictographic}", "u"); } catch (e) { /* Older engines still check the explicit glyph list. */ }
    function isFakeIconValue(value) {
        var normalized = String(value || "").trim();
        return normalized.length <= 12 && (normalized.length === 1 && glyphs.indexOf(normalized) >= 0 || !!emojiRe && emojiRe.test(normalized));
    }
    var tagRe = /<(i|span|button|a)\b[^>]*>\s*([^<\r\n]{1,12})\s*<\/\1\s*>/gi;
    var propertyRe = /\b(?:icon|iconName|navigationIcon|menuIcon)\s*:\s*(["'])([^"'\r\n]{1,12})\1/gi;
    var m;
    while ((m = tagRe.exec(text)) !== null && matches.length < 10) {
        if (isFakeIconValue(m[2])) oneCheck(matches, "fakeUnicodeIcon", text, m.index, "Replace text icon '" + String(m[2]).trim() + "' with App Base Lucide markup such as <i data-lucide=\"circle\" aria-hidden=\"true\"></i>.");
    }
    while ((m = propertyRe.exec(text)) !== null && matches.length < 10) {
        if (isFakeIconValue(m[2])) oneCheck(matches, "fakeUnicodeIcon", text, m.index, "Replace text icon '" + String(m[2]).trim() + "' with a Lucide icon name such as 'circle'.");
    }
    return matches;
}

function scanFakeLinks(text) {
    var matches = [], re = /<a\b([^>]*)>/gi, m;
    while ((m = re.exec(text)) !== null) {
        if (!hasTagAttribute(m[1], "href")) continue;
        var href = readTagAttribute(m[1], "href").trim();
        if (!href || href === "#" || /^javascript:/i.test(href)) oneCheck(matches, "fakeLink", text, m.index);
    }
    return matches;
}

function scanCdataWrapper(text) {
    var matches = [];
    if (/^\s*<!\[CDATA\[/.test(text)) oneCheck(matches, "cdataWrapper", text, 0);
    return matches;
}

function scanImageSourcePolicy(text, artifact) {
    var matches = [];
    var candidates = artifact ? getArtifactResourceReferenceCandidates(artifact) : extractResourceReferenceCandidates(text);
    candidates.forEach(function (item) {
        if (item.kind === "media" && /(?:picsum\.photos|placehold\.co|placeholder\.com|loremflickr\.com|source\.unsplash\.com)/i.test(item.value)) {
            pushCustomMatch(matches, "custom:imageSourcePolicy", item.line, "Review placeholder asset: " + item.value);
        }
    });
    return matches;
}

function scanViewportHeight(text) {
    var matches = [], m;
    var tags = /\bclass\s*=\s*["']([^"']*)["']/gi;
    while ((m = tags.exec(text)) !== null) {
        if (m[1].split(/\s+/).some(function (c) { return c === "h-screen"; })) oneCheck(matches, "viewportHeight", text, m.index);
    }
    // Responsive CSS needs scope analysis; report as an advisory only.
    var css = /(?:^|[;{\s])height\s*:\s*100vh\b/gi;
    while ((m = css.exec(text)) !== null) oneCheck(matches, "viewportHeight", text, m.index);
    return matches;
}

function scanOverflowRisk(text) {
    var matches = [], re = /\b(?:min-)?width\s*:\s*100vw\b|(?:^|[\s"'])w-screen(?=[\s"'])/gi, m;
    while ((m = re.exec(text)) !== null) oneCheck(matches, "overflowReview", text, m.index);
    return matches;
}

function scanFormLabels(text) {
    var matches = [], html = maskScriptAndStyle(text), labels = {}, wrapped = [], ids = {};
    var tag, tags = /<([\w:-]+)\b([^>]*)>/g;
    while ((tag = tags.exec(html)) !== null) {
        var id = readTagAttribute(tag[2], "id");
        if (id) ids[id] = true;
    }
    var re = /<label\b([^>]*)>([\s\S]*?)<\/label\s*>/gi, m;
    while ((m = re.exec(html)) !== null) {
        var target = readTagAttribute(m[1], "for");
        if (m[2].replace(/<[^>]*>/g, "").trim()) {
            if (target) labels[target] = true;
            wrapped.push([m.index, re.lastIndex]);
        }
    }
    re = /<(input|textarea|select)\b([^>]*)>/gi;
    while ((m = re.exec(html)) !== null) {
        var attrs = m[2], type = readTagAttribute(attrs, "type").toLowerCase();
        if (/^(hidden|submit|reset|button|image)$/.test(type)) continue;
        var labelledBy = readTagAttribute(attrs, "aria-labelledby").trim().split(/\s+/).filter(Boolean);
        if (readTagAttribute(attrs, "aria-label").trim() ||
            (labelledBy.length && labelledBy.every(function (id) { return !!ids[id]; })) ||
            labels[readTagAttribute(attrs, "id")] ||
            wrapped.some(function (range) { return m.index >= range[0] && m.index < range[1]; })) continue;
        // Framework components/dynamic bindings may supply the accessible name at runtime.
        if (/\b(?:v-bind|:aria-label|:aria-labelledby|:id|k-attribute)\b/.test(attrs)) continue;
        oneCheck(matches, "formLabel", text, m.index);
    }
    return matches;
}

function scanFormFeedback(text) {
    var matches = [], re = /<form\b[^>]*>([\s\S]*?)<\/form\s*>/gi, m;
    while ((m = re.exec(text)) !== null) {
        if (/error|invalid|alert|aria-live|失败|错误|无效|必填|请重试|校验|验证|role\s*=\s*["']status/i.test(m[1])) continue;
        oneCheck(matches, "formFeedbackReview", text, m.index, "Verify localized or shared-component submit-error feedback for this form.");
    }
    return matches;
}

function scanTemplateDialect(text) {
    var matches = [], html = maskScriptAndStyle(text), re = /\{\{[#/]\s*[^}]+\}\}/g, m;
    // These delimiters are valid in a client template runtime. Do not infer that
    // every Page or View is exclusively a Kooboo server template.
    if (/Handlebars|Mustache|Vue\.|createApp|Alpine|angular|\bv-(?:if|for|html)\s*=|\bx-(?:data|html)\s*=/i.test(text)) return matches;
    while ((m = re.exec(html)) !== null) oneCheck(matches, "templateDialect", text, m.index, "Confirm the renderer for this block template: " + m[0]);
    return matches;
}

function scanTemplateServerContext(text) {
    var matches = [], re = /<script\b([^>]*)>([\s\S]*?)<\/script\s*>/gi, m;
    while ((m = re.exec(text)) !== null) {
        var env = readTagAttribute(m[1], "env").toLowerCase();
        var type = readTagAttribute(m[1], "type").toLowerCase();
        if (env === "server" || /json|template/.test(type)) continue;
        if (/\b(?:const|let|var|function)\s+k\b|\([^)]*\bk\b[^)]*\)\s*=>/.test(m[2])) continue;
        var code = m[2].replace(/(["'`])(?:\\.|(?!\1)[^\\])*?\1/g, "");
        if (/\bk\.(?:api|request|response|cookie|commerce|DB|site|content|payment)\b/.test(code)) oneCheck(matches, "serverContext", text, m.index);
    }
    return matches;
}

function scanLayoutPlaceholder(text) {
    var matches = [], re = /\bk-placeholder\s*=\s*(?:"([^"]*)"|'([^']*)'|([^\s>]+))/gi, m;
    while ((m = re.exec(text)) !== null) if ((m[1] || m[2] || m[3] || "").trim()) return matches;
    oneCheck(matches, "layoutPlaceholder", text, 0, "Layout requires a non-empty k-placeholder name.");
    return matches;
}

function scanPageSkeleton(text) {
    var matches = [], re = /<([\w:-]+)\b([^>]*\bdata-block\s*=\s*["'][^"']+["'][^>]*)>\s*<\/\1\s*>/gi, m;
    while ((m = re.exec(text)) !== null) {
        var id = readTagAttribute(m[2], "id");
        if (id && new RegExp("(?:mount|createRoot|hydrateRoot|getElementById)\\s*\\(\\s*[\"']#?" + escapeRegExp(id) + "[\"']").test(text)) continue;
        oneCheck(matches, "pageAssemblyReview", text, m.index, "Confirm unfinished Page Plan block versus intentional client mount: " + m[0]);
    }
    return matches;
}

function scanQueryCallbackContract(text) {
    var matches = [];
    var re = /k\.api\.get\s*\(\s*(?:async\s*)?\(\s*(\w+)\s*\)\s*=>\s*\{?([^;\n]{0,300})/g, m;
    while ((m = re.exec(text)) !== null) {
        if (new RegExp("\\b" + escapeRegExp(m[1]) + "\\.(?:queryString|query)\\b").test(m[2])) oneCheck(matches, "queryCallbackReview", text, m.index);
    }
    return matches;
}

function scanDataStrategyContract(text, artifact) {
    var matches = [];
    // Empty client state is not mock data. Inline API fixtures can be intentional
    // (including App mock-api); strategy verification is advisory, never a ban.
    if (!includesAny(["Api", "CodeBlock"], [artifact.objectType])) return matches;
    var re = /(?:return\s*\[\s*\{|(?:const|let|var)\s+\w+\s*=\s*\[\s*\{)/g, m;
    while ((m = re.exec(text)) !== null) oneCheck(matches, "dataStrategyReview", text, m.index, "Verify inline records against the approved mock-api/new-store/data-source strategy.");
    return matches;
}

function scanHardcodedSecrets(text) {
    var privateKey = /-----BEGIN (?:RSA |EC |OPENSSH )?PRIVATE KEY-----[\s\S]+?-----END (?:RSA |EC |OPENSSH )?PRIVATE KEY-----/g;
    var matches = [], keyMatch;
    while ((keyMatch = privateKey.exec(text)) !== null) matches.push({ pattern: "custom:privateKey", line: countLine(text, keyMatch.index), excerpt: "[redacted private key material]", definite: true });
    var re = /\b(api[_-]?key|secret|token|password)\s*[:=]\s*["']([^"'\n]{8,})["']/gi, m;
    while ((m = re.exec(text)) !== null) {
        var value = m[2];
        if (/^(?:your[_ -]|replace[_ -]|example|placeholder|\$\{|<|x{8,}|\*{8,})/i.test(value)) continue;
        // Never put the candidate secret in the model-visible finding.
        pushCustomMatch(matches, "custom:credentialReview", countLine(text, m.index), m[1] + " = [redacted credential candidate]");
    }
    return matches;
}

function extractModuleReferences(content, objectType) {
    var sources = [], scripts = /<script\b([^>]*)>([\s\S]*?)<\/script\s*>/gi, match;
    if (objectType ? includesAny(["Page", "Layout", "View"], [objectType]) : /^\s*</.test(content)) {
        while ((match = scripts.exec(content)) !== null) {
            if (readTagAttribute(match[1], "type").toLowerCase() === "module") sources.push({ text: match[2], offset: match.index + match[0].indexOf(">") + 1 });
        }
    } else sources.push({ text: content, offset: 0 });
    var refs = [];
    sources.forEach(function (source) {
        var tokens = [], re = /\/\*[\s\S]*?\*\/|\/\/[^\n]*|"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'|`(?:\\.|[^`\\])*`|[A-Za-z_$][\w$]*|[^\s]/g, token;
        while ((token = re.exec(source.text)) !== null) {
            if (/^\/[*\/]/.test(token[0])) continue;
            tokens.push({ text: token[0], index: source.offset + token.index, string: /^["'`]/.test(token[0]) });
        }
        function add(token) {
            if (!token || !token.string || (token.text[0] === "`" && /\$\{/.test(token.text))) {
                refs.push({ value: "computed module import", kind: "dynamic-module", index: token ? token.index : 0 }); return;
            }
            var value = token.text.slice(1, -1).replace(/\\([\\'"`])/g, "$1");
            refs.push({ value: value, kind: /^(?:\.?\.?\/|[a-z][a-z0-9+.-]*:|\/\/)/i.test(value) ? "script" : "module", index: token.index });
        }
        for (var i = 0; i < tokens.length; i++) {
            var current = tokens[i];
            if (current.string || (current.text !== "import" && current.text !== "export") || (i && tokens[i - 1].text === ".")) continue;
            var next = tokens[i + 1];
            if (!next || next.text === ".") continue;
            if (current.text === "import" && (next.string || next.text === "(")) { add(next.text === "(" ? tokens[i + 2] : next); continue; }
            for (var j = i + 1; j < tokens.length && tokens[j].text !== ";"; j++) {
                if (tokens[j].text === "from" && !tokens[j].string) { add(tokens[j + 1]); break; }
                if (j > i + 1 && /^(?:import|export|const|let|function|return)$/.test(tokens[j].text)) break;
            }
        }
    });
    return refs;
}

function isWholeSanitizerCall(value) {
    var prefix = /^(?:DOMPurify\.sanitize|sanitizeHtml)\s*\(/.exec(value);
    if (!prefix) return false;
    var depth = 1, quote = "", escaped = false;
    for (var i = prefix[0].length; i < value.length; i++) {
        var ch = value[i];
        if (quote) { if (escaped) escaped = false; else if (ch === "\\") escaped = true; else if (ch === quote) quote = ""; continue; }
        if (ch === "\"" || ch === "'" || ch === "`") { quote = ch; continue; }
        if (ch === "(") depth++;
        if (ch === ")" && --depth === 0) return !value.slice(i + 1).trim();
    }
    return false;
}

function scanHtmlInjection(text) {
    var matches = [], re = /(?:innerHTML\s*=\s*|insertAdjacentHTML\s*\(\s*["'][^"']*["']\s*,\s*|document\.write\s*\(\s*)([^;\n]+)/g, m;
    while ((m = re.exec(text)) !== null) {
        var rhs = m[1].trim();
        if (!/^innerHTML/.test(m[0])) rhs = rhs.replace(/\)\s*$/, "");
        if (/^(["'])(?:\\.|(?!\1)[^\\])*\1\s*$/.test(rhs)) continue;
        if (isWholeSanitizerCall(rhs)) continue;
        oneCheck(matches, "htmlInjectionReview", text, m.index, "Review untrusted input and sanitizer coverage at this HTML sink.");
    }
    return matches;
}

function scanWriteValidation(text) {
    var matches = [];
    if (/\bk\.request\./.test(text) && /\b(?:create|update|add|delete|remove)\s*\(/.test(text) &&
        !/\b(?:validate|safeParse|parse|authorize|assert|checkPermission)\s*\(|\bif\s*\([^)]*\)\s*(?:\{\s*)?(?:throw|return)/.test(text)) {
        oneCheck(matches, "writeValidationReview", text, 0, "Verify field validation and ownership in this write path and its called services.");
    }
    return matches;
}

function scanImportMapContract(text) {
    var matches = [], re = /<script\b([^>]*)>([\s\S]*?)<\/script\s*>/gi, m;
    while ((m = re.exec(text)) !== null) {
        if (readTagAttribute(m[1], "type").toLowerCase() !== "importmap") continue;
        if (hasTagAttribute(m[1], "src") || readTagAttribute(m[1], "env").toLowerCase() === "server") {
            oneCheck(matches, "importMap", text, m.index, "Import map must be inline client JSON, without src or env=server."); continue;
        }
        try {
            var map = JSON.parse(m[2]);
            if (!map || typeof map !== "object" || Array.isArray(map)) throw new Error();
            function mapping(value) {
                if (!value || typeof value !== "object" || Array.isArray(value)) return false;
                return Object.keys(value).every(function (key) { return typeof value[key] === "string" && /^(?:\.?\.?\/|[a-z][a-z0-9+.-]*:)/i.test(value[key]) && (key.slice(-1) !== "/" || value[key].slice(-1) === "/"); });
            }
            if (map.imports !== undefined && !mapping(map.imports)) throw new Error();
            if (map.scopes !== undefined && (!map.scopes || typeof map.scopes !== "object" || Array.isArray(map.scopes) || !Object.keys(map.scopes).every(function (key) { return mapping(map.scopes[key]); }))) throw new Error();
        } catch (e) { oneCheck(matches, "importMap", text, m.index, "Import map JSON or address mappings are invalid."); }
    }
    return matches;
}

function runCustomCheck(customCheck, content, artifact) {
    if (customCheck === "unfinishedContent") return scanUnfinishedContent(content, artifact);
    if (customCheck === "emojiPolicy") return scanEmojiPolicy(content, artifact);
    if (customCheck === "fakeUnicodeIcons") return scanFakeUnicodeIcons(content, artifact);
    if (customCheck === "fakeLinks") return scanFakeLinks(content, artifact);
    if (customCheck === "cdataWrapper") return scanCdataWrapper(content, artifact);
    if (customCheck === "imageSourcePolicy") return scanImageSourcePolicy(content, artifact);
    if (customCheck === "viewportHeight") return scanViewportHeight(content, artifact);
    if (customCheck === "overflowRisk") return scanOverflowRisk(content, artifact);
    if (customCheck === "formLabels") return scanFormLabels(content, artifact);
    if (customCheck === "formFeedback") return scanFormFeedback(content, artifact);
    if (customCheck === "templateDialect") return scanTemplateDialect(content, artifact);
    if (customCheck === "templateServerContext") return scanTemplateServerContext(content, artifact);
    if (customCheck === "layoutPlaceholder") return scanLayoutPlaceholder(content, artifact);
    if (customCheck === "pageSkeleton") return scanPageSkeleton(content, artifact);
    if (customCheck === "queryCallbackContract") return scanQueryCallbackContract(content, artifact);
    if (customCheck === "dataStrategyContract") return scanDataStrategyContract(content, artifact);
    if (customCheck === "hardcodedSecrets") return scanHardcodedSecrets(content, artifact);
    if (customCheck === "htmlInjection") return scanHtmlInjection(content, artifact);
    if (customCheck === "writeValidation") return scanWriteValidation(content, artifact);
    if (customCheck === "importMapContract") return scanImportMapContract(content, artifact);
    if (customCheck === "apiResponseContract") {
        var responseMatches = [];
        if (/return\s+undefined\b/.test(content)) oneCheck(responseMatches, "responseContractReview", content, 0, "Verify whether an undefined response is permitted by the endpoint contract.");
        return responseMatches;
    }

    if (customCheck === "visibleFocusIndicator") return scanVisibleFocusIndicator(content);
    if (customCheck === "selectArrowInset") return scanSelectArrowInset(content);
    if (customCheck === "fetchErrorHandling") return scanFetchErrorHandling(content);
    if (customCheck === "kAttributeNoMath") return scanKAttributeNoMath(content);
    if (customCheck === "duplicateKAttribute") return scanDuplicateKAttribute(content);
    if (customCheck === "kDataBranchTagsInsideKData") return scanKDataBranchTagsInsideKData(content);
    if (customCheck === "kDataBranchStructure") return scanKDataBranchStructure(content);
    if (customCheck === "kDataQueryNoSelfClosing") return scanKDataQueryNoSelfClosing(content);
    if (customCheck === "kDataQueryContract") return scanKDataQueryContract(content);
    if (customCheck === "kDataExportContract") return scanKDataExportContract(content);
    if (customCheck === "kDataElementsInsideKData") return scanKDataElementsInsideKData(content);
    if (customCheck === "duplicateKDataAttributes") return scanDuplicateKDataAttributes(content);
    if (customCheck === "clientScriptMustache") return scanClientScriptMustache(content);
    if (customCheck === "layoutIdMatchesArtifactLayoutName") return scanLayoutIdMatchesArtifactLayoutName(content, artifact);
    if (customCheck === "apiNamedActionRoute") return scanApiNamedActionRoute(content, artifact);
    if (customCheck === "layoutWrapsPlaceholder") return scanLayoutWrapsPlaceholder(content, artifact);
    if (customCheck === "envServerWrapper") return scanEnvServerWrapper(content);
    if (customCheck === "commerceSourceOfTruth") return scanCommerceSourceOfTruth(content, artifact);
    if (customCheck === "commerceListingPrice") return scanCommerceListingPrice(content, artifact);
    if (customCheck === "commerceProductsFilter") return scanCommerceProductsFilter(content, artifact);
    if (customCheck === "commerceProductDetailQuery") return scanCommerceProductDetailQuery(content, artifact);
    if (customCheck === "commerceAuthContract") return scanCommerceAuthContract(content, artifact);
    if (customCheck === "commerceCartLineContract") return scanCommerceCartLineContract(content, artifact);
    if (customCheck === "commerceHeaderEntrypoints") return scanCommerceHeaderEntrypoints(content, artifact);
    if (customCheck === "commerceOrderOwnership") return scanCommerceOrderOwnership(content, artifact);
    if (customCheck === "commerceClientDataBoundary") return scanCommerceClientDataBoundary(content, artifact);
    if (customCheck === "commerceAddToCartContract") return scanCommerceAddToCartContract(content, artifact);
    if (customCheck === "commercePaymentContract") return scanCommercePaymentContract(content, artifact);
    if (customCheck === "commerceCheckoutContract") return scanCommerceCheckoutContract(content, artifact);
    if (customCheck === "commerceOrderResultContract") return scanCommerceOrderResultContract(content, artifact);
    return [];
}

function getArtifactLayoutName(artifact) {
    return String((artifact && artifact.layoutName) || "").trim();
}

function extractPlaceholderBodies(content) {
    var bodies = [];
    var re = /<placeholder\b[^>]*>([\s\S]*?)<\/placeholder>/gim;
    var match;
    while ((match = re.exec(content)) !== null) {
        var body = match[1] || "";
        var bodyStart = (match.index || 0) + match[0].indexOf(body);
        bodies.push({ body: body, bodyStart: bodyStart });
    }
    return bodies;
}

function extractPagePlaceholderIdMatches(content) {
    var matches = [];
    var re = /<placeholder\b[^>]*\bid\s*=\s*(?:"([^"]*)"|'([^']*)'|([^\s>]+))/gim;
    var match;
    while ((match = re.exec(content)) !== null) {
        var value = String(match[1] !== undefined ? match[1] : (match[2] !== undefined ? match[2] : match[3] || "")).trim();
        if (!value) continue;
        var index = match.index || 0;
        matches.push({
            value: value,
            line: countLine(content, index),
            excerpt: content.substring(index, Math.min(content.length, index + 140)).replace(/\s+/g, " ").trim()
        });
    }
    return matches;
}

function extractLayoutPlaceholderMatches(content) {
    var matches = [];
    var re = /\bk-placeholder\s*=\s*(?:"([^"]*)"|'([^']*)'|([^\s>]+))/gim;
    var match;
    while ((match = re.exec(content)) !== null) {
        var value = String(match[1] !== undefined ? match[1] : (match[2] !== undefined ? match[2] : match[3] || "")).trim();
        if (!value) continue;
        matches.push({ value: value });
    }
    return matches;
}

function extractViewIdMatchesInRange(content, rangeStart, rangeBody) {
    var matches = [];
    var re = /<view\b[^>]*\bid\s*=\s*("([^"]*)"|'([^']*)')/gim;
    var match;
    while ((match = re.exec(rangeBody)) !== null) {
        var viewId = String(match[2] !== undefined ? match[2] : match[3] || "").trim();
        if (!viewId) continue;
        var absIndex = rangeStart + (match.index || 0);
        matches.push({
            id: viewId,
            line: countLine(content, absIndex),
            excerpt: content.substring(absIndex, Math.min(content.length, absIndex + 140)).replace(/\s+/g, " ").trim()
        });
    }
    return matches;
}

function createResourceReferenceKnownError(priority) {
    return {
        id: "KOOB-RESOURCE-REFERENCE",
        priority: priority || "normal",
        playbook: createDebugPlaybook("7-koob-resource-reference")
    };
}

function addSkippedStructuralCheck(skippedChecks, id, reason, evidenceRequirement) {
    for (var i = 0; i < skippedChecks.length; i++) {
        if (skippedChecks[i].id === id && skippedChecks[i].reason === reason) return;
    }
    skippedChecks.push({
        id: id,
        reason: reason,
        evidenceRequirement: evidenceRequirement
    });
}

function addArtifactObjectsToInventory(list, artifacts, objectType) {
    var result = list ? list.slice() : [];
    for (var i = 0; i < artifacts.length; i++) {
        var artifact = artifacts[i];
        if (artifact.objectType !== objectType) continue;
        result.push({
            id: String(artifact.id || artifact.nameOrId || "").trim(),
            name: String(artifact.name || artifact.nameOrId || "").trim(),
            url: String(artifact.url || artifact.Url || "").trim()
        });
    }
    return result;
}

function buildObjectInventory(artifacts, objectType) {
    var list = readSiteObjectList(objectType);
    if (list === null) return null;
    return addArtifactObjectsToInventory(list, artifacts, objectType);
}

function normalizeResourceRoute(value) {
    var url = String(value || "").trim();
    var splitAt = url.search(/[?#]/);
    if (splitAt >= 0) url = url.substring(0, splitAt);
    while (url.indexOf("./") === 0) url = url.substring(2);
    if (!url) return "";
    if (url[0] !== "/") url = "/" + url;
    return url.replace(/\/+/g, "/").toLowerCase();
}

function isIgnoredResourceReference(value) {
    var url = String(value || "").trim();
    if (!url) return true;
    if (/^(?:[a-z][a-z0-9+.-]*:|\/\/|#)/i.test(url)) return true;
    if (/[{}]|<%|%>/.test(url)) return true;
    var route = normalizeResourceRoute(url);
    return (
        route.indexOf("/_api/") === 0 ||
        route.indexOf("/api/") === 0 ||
        route.indexOf("/_admin/") === 0 ||
        route.indexOf("/__") === 0
    );
}

function hasStaticResourceExtension(value) {
    var route = normalizeResourceRoute(value);
    return /\.(?:avif|bmp|css|cur|gif|ico|jpe?g|js|mjs|cjs|map|mp4|ogg|pdf|png|svg|webm|webp|woff2?|ttf|otf|eot)$/.test(route);
}

function getRouteBaseName(value) {
    var route = normalizeResourceRoute(value);
    var parts = route.split("/");
    return parts.length > 0 ? parts[parts.length - 1] : route;
}

function buildRouteInventory(lists) {
    var routeMap = {};
    var nameMap = {};
    for (var i = 0; i < lists.length; i++) {
        var list = lists[i] || [];
        for (var j = 0; j < list.length; j++) {
            var item = list[j] || {};
            var route = normalizeResourceRoute(item.url);
            if (route) routeMap[route] = item;
            var name = String(item.name || "").trim().toLowerCase();
            if (name && !nameMap[name]) nameMap[name] = item;
            var basename = getRouteBaseName(item.url);
            if (basename && !nameMap[basename]) nameMap[basename] = item;
        }
    }
    return { routeMap: routeMap, nameMap: nameMap };
}

function addResourceCandidate(candidates, content, index, value, kind, excerpt) {
    if (!String(value || "").trim() || /^(?:data:|blob:|#)/i.test(String(value))) return;
    if (/[{}]|<%|%>/.test(String(value))) return;
    candidates.push({
        value: String(value || "").trim(),
        kind: kind,
        line: countLine(content, index),
        excerpt: String(excerpt || "").replace(/\s+/g, " ").trim()
    });
}

function extractResourceReferenceCandidates(content, objectType) {
    var candidates = [];
    var tagRe = /<(script|link|img|source|video)\b([^>]*)>/gim;
    var tagMatch;
    while ((tagMatch = tagRe.exec(content)) !== null && candidates.length < 500) {
        var tagName = String(tagMatch[1] || "").toLowerCase();
        var attrText = tagMatch[2] || "";
        var index = tagMatch.index || 0;
        var excerpt = content.substring(index, Math.min(content.length, index + 180));
        if (tagName === "script") {
            addResourceCandidate(candidates, content, index, readTagAttribute(attrText, "src"), "script", excerpt);
        } else if (tagName === "link") {
            var rel = readTagAttribute(attrText, "rel").toLowerCase();
            if (/(^|\s)stylesheet(\s|$)/.test(rel)) {
                addResourceCandidate(candidates, content, index, readTagAttribute(attrText, "href"), "style", excerpt);
            }
        } else {
            addResourceCandidate(candidates, content, index, readTagAttribute(attrText, "src"), "media", excerpt);
            if (tagName === "video") {
                addResourceCandidate(candidates, content, index, readTagAttribute(attrText, "poster"), "media", excerpt);
            }
            var srcset = readTagAttribute(attrText, "srcset");
            if (srcset) {
                var entries = srcset.split(",");
                for (var e = 0; e < entries.length && candidates.length < 500; e++) {
                    var srcsetUrl = String(entries[e] || "").trim().split(/\s+/)[0];
                    addResourceCandidate(candidates, content, index, srcsetUrl, "media", excerpt);
                }
            }
        }
    }

    // CSS URLs are meaningful only in CSS regions, never in JavaScript helper names.
    var cssSources = [];
    if (objectType === "Style") cssSources.push({ text: content, offset: 0 });
    else {
        var styleRe = /<style\b[^>]*>([\s\S]*?)<\/style\s*>/gi, styleMatch;
        while ((styleMatch = styleRe.exec(content)) !== null) {
            cssSources.push({ text: styleMatch[1], offset: styleMatch.index + styleMatch[0].indexOf(">") + 1 });
        }
        var htmlTagRe = /<[a-z][\w:-]*\b[^>]*>/gi, htmlTag;
        while ((htmlTag = htmlTagRe.exec(content)) !== null) {
            var styleAttr = /\sstyle\s*=\s*("[^"]*"|'[^']*')/i.exec(htmlTag[0]);
            if (!styleAttr) continue;
            cssSources.push({ text: styleAttr[1].slice(1, -1), offset: htmlTag.index + styleAttr.index + styleAttr[0].indexOf(styleAttr[1]) + 1 });
        }
    }
    cssSources.forEach(function (source) {
        var cssUrlRe = /\burl\(\s*(?:"([^"]+)"|'([^']+)'|([^)'"\s]+))\s*\)/gim;
        var cssMatch;
        while ((cssMatch = cssUrlRe.exec(source.text)) !== null && candidates.length < 500) {
            var cssValue = cssMatch[1] !== undefined ? cssMatch[1] : (cssMatch[2] !== undefined ? cssMatch[2] : cssMatch[3] || "");
            addResourceCandidate(
                candidates,
                content,
                source.offset + cssMatch.index,
                cssValue,
                "media",
                source.text.substring(cssMatch.index, cssMatch.index + 160)
            );
        }
    });
    extractModuleReferences(content, objectType).forEach(function (reference) {
        if (reference.kind === "dynamic-module") candidates.push({ value: reference.value, kind: reference.kind, line: countLine(content, reference.index), excerpt: reference.value });
        else addResourceCandidate(candidates, content, reference.index, reference.value, reference.kind, reference.value);
    });
    var maps = readImportMaps(content);
    maps.forEach(function (map) {
        var dictionaries = [map && map.imports];
        if (map && map.scopes) Object.keys(map.scopes).forEach(function (scope) { dictionaries.push(map.scopes[scope]); });
        dictionaries.forEach(function (dict) {
            if (!dict || typeof dict !== "object") return;
            Object.keys(dict).forEach(function (key) {
                // Prefix mappings denote a directory, not an individual resource.
                if (typeof dict[key] === "string" && key.slice(-1) !== "/") addResourceCandidate(candidates, content, 0, dict[key], "script", "import map: " + key);
            });
        });
    });
    return candidates;
}

function getArtifactResourceReferenceCandidates(artifact) {
    if (!artifact) return [];
    if (!Array.isArray(artifact._resourceReferenceCandidates)) {
        artifact._resourceReferenceCandidates = extractResourceReferenceCandidates(artifact.content, artifact.objectType);
    }
    return artifact._resourceReferenceCandidates;
}

function getArtifactImportMaps(artifact) {
    if (!artifact) return [];
    if (!Array.isArray(artifact._importMaps)) artifact._importMaps = readImportMaps(artifact.content);
    return artifact._importMaps;
}

function getResourceListsForKind(inventories, kind) {
    if (kind === "script") return [inventories.Script, inventories.CmsFile];
    if (kind === "style") return [inventories.Style, inventories.CmsFile];
    return [inventories.Image, inventories.CmsFile];
}

function findSuggestedResource(item, inventory) {
    var basename = getRouteBaseName(item.value);
    var suggestion = basename ? inventory.nameMap[basename] : null;
    if (!suggestion) return "";
    return String(suggestion.url || "").trim();
}

function pushMissingViewReferenceFinding(findings, artifact, matches, maxMatchesPerRule) {
    findings.push({
        id: VIEW_REFERENCE_RULE_ID,
        title: "引用的 View 必须存在",
        severity: "BLOCKER",
        artifact: { name: artifact.name, objectType: artifact.objectType },
        tags: ["template", "view", "resource", "cross-artifact"],
        knownError: createResourceReferenceKnownError("immediate"),
        fix: "先用 list_site_object(View) 确认真实名称。名称拼写错误时修正 <view id>；对象尚未创建时，只有在需求明确其内容和职责后才创建，不要生成同名空 View。",
        matches: matches.slice(0, maxMatchesPerRule),
        matchCount: matches.length
    });
}

function pushMissingLocalResourceFinding(findings, artifact, matches, maxMatchesPerRule, severity) {
    findings.push({
        id: LOCAL_RESOURCE_URL_RULE_ID,
        title: "本地静态资源引用必须使用已存在的 Route URL",
        severity: severity,
        severityFloor: severity === "BLOCKER" ? "BLOCKER" : undefined,
        artifact: { name: artifact.name, objectType: artifact.objectType },
        tags: ["resource", "route", "network", "cross-artifact"],
        knownError: createResourceReferenceKnownError(severity === "BLOCKER" ? "immediate" : "normal"),
        fix: "用 list_site_object 读取 Style/Script/Image/CmsFile 的真实 Url，并把引用改成根绝对 URL。静态扫描命中后还要用 Preview Network 确认失败请求；不要为猜测路径创建重复资源。",
        matches: matches.slice(0, maxMatchesPerRule),
        matchCount: matches.length
    });
}

function crossRuleApplies(rules, id, context, artifact) {
    var rule = findRuleById(rules, id);
    return !!rule && ruleApplies(rule, context, artifact);
}

function resolveLocalReference(value, base) {
    var ref = String(value || "").trim();
    if (/^(?:[a-z][a-z0-9+.-]*:|\/\/|#)/i.test(ref)) return { external: true };
    if (/[{}]|<%|%>/.test(ref)) return { unknown: true };
    if (ref[0] !== "/") {
        if (!base || /[{}]|<%|%>/.test(base)) return { unknown: true };
        if (/^(?:[a-z][a-z0-9+.-]*:|\/\/)/i.test(base)) return { external: true };
        ref = base.split(/[?#]/)[0].replace(/[^/]*$/, "") + ref;
    }
    var parts = ref.split(/[?#]/)[0].split("/"), out = [];
    parts.forEach(function (part) { if (part === "..") out.pop(); else if (part && part !== ".") out.push(part); });
    return { route: "/" + out.join("/") + (/\/$/.test(ref) ? "/" : "") };
}

function artifactResourceBase(artifact) {
    var base = artifact.url || artifact.Url || "";
    if (artifact.objectType === "Page") {
        var tag = /<base\b([^>]*)>/i.exec(String(artifact.content || ""));
        if (tag) {
            var href = readTagAttribute(tag[1], "href");
            if (/^(?:[a-z][a-z0-9+.-]*:|\/\/)/i.test(href)) return href;
            var resolved = resolveLocalReference(href, base);
            if (!resolved.route) return "";
            base = resolved.route;
        }
    }
    return base;
}

function readImportMaps(content) {
    var maps = [], re = /<script\b([^>]*)>([\s\S]*?)<\/script\s*>/gi, m;
    while ((m = re.exec(content)) !== null) {
        if (readTagAttribute(m[1], "type").toLowerCase() !== "importmap") continue;
        try { maps.push(JSON.parse(m[2])); } catch (e) { /* Structural rule reports malformed maps. */ }
    }
    return maps;
}

function mapModuleSpecifier(specifier, map, sourceUrl, pageBase) {
    var dictionaries = [];
    var scopes = map && map.scopes;
    if (scopes && typeof scopes === "object") {
        Object.keys(scopes).map(function (scope) { return { key: scope, resolved: resolveLocalReference(scope, pageBase).route }; })
            .filter(function (scope) { return scope.resolved && sourceUrl.indexOf(scope.resolved) === 0; })
            .sort(function (a, b) { return b.resolved.length - a.resolved.length; })
            .forEach(function (scope) { dictionaries.push(scopes[scope.key]); });
    }
    dictionaries.push(map && map.imports);
    for (var i = 0; i < dictionaries.length; i++) {
        var dict = dictionaries[i];
        if (!dict || typeof dict !== "object") continue;
        var keys = Object.keys(dict).filter(function (key) { return key === specifier || (key.slice(-1) === "/" && specifier.indexOf(key) === 0); }).sort(function (a, b) { return b.length - a.length; });
        if (keys.length && typeof dict[keys[0]] === "string") return dict[keys[0]] + specifier.slice(keys[0].length);
    }
    return null;
}

function resolveModuleForPage(candidate, source, page) {
    var maps = getArtifactImportMaps(page), address = null;
    for (var i = 0; i < maps.length; i++) {
        address = mapModuleSpecifier(candidate.value, maps[i], source.url || page.url || "", artifactResourceBase(page));
        if (address !== null) break;
    }
    return address === null ? { unknown: true } : resolveLocalReference(address, artifactResourceBase(page));
}

var MODULE_CHAIN_READ_LIMIT = 200;

function buildModulePageOwners(artifacts) {
    var scripts = Object.create(null), owners = Object.create(null), siteScripts = null, chainReads = 0;
    artifacts.forEach(function (artifact) {
        if (artifact.objectType === "Script" && artifact.url) scripts[normalizeResourceRoute(artifact.url)] = artifact;
    });
    // Unchanged platform entries such as App Base main.js/app.js are rarely listed as artifacts,
    // yet they link the Page import map to the changed Scripts. Read them only to trace ownership;
    // they are not scanned for findings.
    function chainScript(route) {
        if (route in scripts) return scripts[route];
        scripts[route] = null;
        if (chainReads >= MODULE_CHAIN_READ_LIMIT) return null;
        if (siteScripts === null) {
            siteScripts = Object.create(null);
            (readSiteObjectList("Script") || []).forEach(function (item) {
                if (item.url) siteScripts[normalizeResourceRoute(item.url)] = item;
            });
        }
        var record = siteScripts[route];
        if (!record) return null;
        chainReads++;
        var content = null;
        try { content = readObjectText(record.name || record.id, "Script"); } catch (e) { content = null; }
        if (typeof content !== "string") return null;
        return (scripts[route] = { objectType: "Script", name: record.name, url: record.url, content: content });
    }
    artifacts.filter(function (artifact) { return artifact.objectType === "Page"; }).forEach(function (page) {
        var queue = [], visited = Object.create(null);
        function enqueue(reference, source) {
            var target = reference.kind === "module" ? resolveModuleForPage(reference, source, page) :
                reference.kind === "script" ? resolveLocalReference(reference.value, artifactResourceBase(source)) : { unknown: true };
            if (!target.route) return;
            var route = normalizeResourceRoute(target.route);
            if (visited[route]) return;
            visited[route] = true;
            if (!owners[route]) owners[route] = [];
            owners[route].push(page);
            var next = chainScript(route);
            if (next) queue.push(next);
        }
        var tags = /<script\b([^>]*)>/gi, tag;
        while ((tag = tags.exec(page.content)) !== null) {
            if (readTagAttribute(tag[1], "type").toLowerCase() !== "module") continue;
            var src = readTagAttribute(tag[1], "src");
            if (src) enqueue({ kind: "script", value: src }, page);
        }
        extractModuleReferences(page.content, "Page").forEach(function (ref) { enqueue(ref, page); });
        // Each script is visited once per Page, preserving that Page's import map and scopes.
        for (var i = 0; i < queue.length; i++) {
            var source = queue[i];
            extractModuleReferences(source.content, "Script").forEach(function (ref) { enqueue(ref, source); });
        }
    });
    return owners;
}

function resolveModuleReference(candidate, artifact, artifacts, owners) {
    var pages = artifact.objectType === "Page" ? [artifact] :
        (owners || buildModulePageOwners(artifacts))[normalizeResourceRoute(artifact.url || "")] || [];
    if (!pages.length) return [{ unknown: true }];
    return pages.map(function (page) {
        return resolveModuleForPage(candidate, artifact, page);
    });
}

function scanCrossArtifactResourceReferences(artifacts, context, findings, skippedChecks, maxMatchesPerRule, rules) {
    var modulePageOwners = buildModulePageOwners(artifacts);
    var inventoryCache = {};
    var routeInventoryCache = {};
    function getInventory(objectType) {
        if (!Object.prototype.hasOwnProperty.call(inventoryCache, objectType)) {
            inventoryCache[objectType] = buildObjectInventory(artifacts, objectType);
        }
        return inventoryCache[objectType];
    }
    function getRouteInventory(types) {
        var key = types.join("|");
        if (!routeInventoryCache[key]) {
            routeInventoryCache[key] = buildRouteInventory(types.map(function (type) { return getInventory(type); }));
        }
        return routeInventoryCache[key];
    }
    var viewArtifacts = artifacts.filter(function (artifact) {
        return crossRuleApplies(rules, VIEW_REFERENCE_RULE_ID, context, artifact) && /<view\b[^>]*\bid\s*=/i.test(artifact.content);
    });
    if (viewArtifacts.length) {
        var views = getInventory("View");
        if (views === null) addSkippedStructuralCheck(skippedChecks, VIEW_REFERENCE_RULE_ID, "list_site_object(View) unavailable", "Read list_site_object(View) and verify referenced Views.");
        else {
            var names = {};
            views.forEach(function (view) { names[view.name.toLowerCase()] = true; names[view.id.toLowerCase()] = true; });
            viewArtifacts.forEach(function (artifact) {
                var refs = extractViewIdMatchesInRange(artifact.content, 0, artifact.content), missing = [];
                refs.forEach(function (ref) {
                    if (/[{}]|<%|%>/.test(ref.id) || names[ref.id.toLowerCase()]) return;
                    missing.push({ pattern: "cross:missingView:" + ref.id, line: ref.line, excerpt: ref.excerpt });
                });
                if (missing.length) pushMissingViewReferenceFinding(findings, artifact, missing, maxMatchesPerRule);
            });
        }
    }
    artifacts.forEach(function (artifact) {
        if (!crossRuleApplies(rules, LOCAL_RESOURCE_URL_RULE_ID, context, artifact)) return;
        var candidates = getArtifactResourceReferenceCandidates(artifact), missing = [], missingModules = [];
        if (candidates.length >= 500) addSkippedStructuralCheck(skippedChecks, LOCAL_RESOURCE_URL_RULE_ID, "Resource reference scan limit reached: " + artifact.name, "Split the resource verification scope and verify all references.");
        candidates.forEach(function (candidate) {
            var targets = candidate.kind === "dynamic-module" ? [{ unknown: true }] : candidate.kind === "module" ? resolveModuleReference(candidate, artifact, artifacts, modulePageOwners) : [resolveLocalReference(candidate.value, artifactResourceBase(artifact))];
            targets.forEach(function (target) {
                if (target.external) return;
                if (target.unknown) {
                    addSkippedStructuralCheck(skippedChecks, LOCAL_RESOURCE_URL_RULE_ID, "Unresolved resource base or module map: " + artifact.name + " -> " + candidate.value,
                        "Include the owning Page with its real Url/import map and the full import chain from its module entry to this Script. If already included, report the unresolved scanner check instead of renaming valid code or repeating an unchanged scan. Computed imports require targeted runtime verification; a generic UI audit does not resolve this structural check.");
                    return;
                }
                if (isIgnoredResourceReference(target.route)) return;
                var types = candidate.kind === "script" || candidate.kind === "module" ? ["Script", "CmsFile"] : candidate.kind === "style" ? ["Style", "CmsFile"] : ["Image", "CmsFile"];
                var inventories = types.map(function (type) { return getInventory(type); });
                var inventory = getRouteInventory(types);
                if (inventory.routeMap[normalizeResourceRoute(target.route)]) return;
                if (inventories.some(function (list) { return list === null; })) {
                    addSkippedStructuralCheck(skippedChecks, LOCAL_RESOURCE_URL_RULE_ID, "Resource inventory unavailable: " + types.join("/"), "Read the applicable resource inventories and verify the resolved URL.");
                    return;
                }
                // One failed static module or script request stops the whole module graph, so the page renders blank.
                (candidate.kind === "script" || candidate.kind === "module" ? missingModules : missing)
                    .push({ pattern: "cross:missingResourceRoute:" + target.route, line: candidate.line, excerpt: candidate.value + " resolves to missing route " + target.route });
            });
        });
        if (missingModules.length) pushMissingLocalResourceFinding(findings, artifact, missingModules, maxMatchesPerRule, "BLOCKER");
        if (missing.length) pushMissingLocalResourceFinding(findings, artifact, missing, maxMatchesPerRule, "HIGH");
    });
}

function buildLayoutContentMap(artifacts) {
    var map = {};
    for (var i = 0; i < artifacts.length; i++) {
        var artifact = artifacts[i];
        if (artifact.objectType !== "Layout" || !artifact.content) continue;
        var name = String(artifact.name || "").trim();
        if (!name) continue;
        map[name] = String(artifact.content);
    }
    return map;
}

function resolveLayoutContent(layoutName, layoutContentMap) {
    if (layoutContentMap[layoutName]) return layoutContentMap[layoutName];
    try {
        var content = readObjectText(layoutName, "Layout");
        if (content === null || content === undefined) return null;
        layoutContentMap[layoutName] = String(content);
        return layoutContentMap[layoutName];
    } catch (e) {
        return null;
    }
}

function createLayoutAssemblyKnownError() {
    return {
        id: "KOOB-LAYOUT-PAGE-ASSEMBLY",
        priority: "immediate",
        playbook: createDebugPlaybook("5-koob-layout-page-assembly")
    };
}

function pushCrossDuplicateFinding(findings, pageArtifact, layoutName, matches, maxMatchesPerRule) {
    findings.push({
        id: CROSS_RULE_ID,
        title: "Layout 与 Page 不得重复引用同一 View",
        severity: "BLOCKER",
        artifact: {
            name: pageArtifact.name,
            objectType: pageArtifact.objectType
        },
        tags: ["template", "layout", "cross-artifact"],
        knownError: createLayoutAssemblyKnownError(),
        fix: "默认采用 Layout 托管 chrome：若 Layout 已含 site_header/site_footer 等 view，从 Page 的 <placeholder> 删除同名 <view>。若坚持 Page 托管 chrome，则从 Layout 删除对应 view（次选）。",
        matches: matches.slice(0, maxMatchesPerRule),
        matchCount: matches.length,
        relatedLayout: layoutName
    });
}

function pushPlaceholderNameFinding(findings, pageArtifact, layoutName, matches, maxMatchesPerRule) {
    findings.push({
        id: PLACEHOLDER_NAME_RULE_ID,
        title: "Page placeholder 必须存在于绑定的 Layout",
        severity: "BLOCKER",
        artifact: {
            name: pageArtifact.name,
            objectType: pageArtifact.objectType
        },
        tags: ["template", "layout", "placeholder", "cross-artifact"],
        knownError: createLayoutAssemblyKnownError(),
        fix: "Page 的 <placeholder id> 必须与绑定 Layout 的 k-placeholder 值逐字一致且大小写敏感。优先修正 Page placeholder id；只有需求确实改变 Layout 区域时才修改 Layout。",
        matches: matches.slice(0, maxMatchesPerRule),
        matchCount: matches.length,
        relatedLayout: layoutName
    });
}

// 第二阶段：按 layoutName 配对 Layout 与 Page，检查 Layout 可读性、placeholder 名称
// 以及 Layout/Page 是否重复引用同一 view id。
function scanCrossArtifactDuplicateViewRefs(artifacts, context, findings, maxMatchesPerRule, rules) {

    var layoutContentMap = buildLayoutContentMap(artifacts);
    for (var i = 0; i < artifacts.length; i++) {
        var pageArtifact = artifacts[i];
        if (pageArtifact.objectType !== "Page" || !pageArtifact.content) continue;

        var layoutName = getArtifactLayoutName(pageArtifact);
        if (!layoutName) continue;
        var checkReadable = crossRuleApplies(rules, LAYOUT_READ_RULE_ID, context, pageArtifact);
        var checkPlaceholder = crossRuleApplies(rules, PLACEHOLDER_NAME_RULE_ID, context, pageArtifact);
        var checkDuplicate = crossRuleApplies(rules, CROSS_RULE_ID, context, pageArtifact);
        if (!checkReadable && !checkPlaceholder && !checkDuplicate) continue;

        var layoutContent = resolveLayoutContent(layoutName, layoutContentMap);
        if (layoutContent === null) {
            if (checkReadable) findings.push({
                id: LAYOUT_READ_RULE_ID,
                title: "Page 绑定的 Layout 必须存在且可读取",
                severity: "BLOCKER",
                artifact: {
                    name: pageArtifact.name,
                    objectType: pageArtifact.objectType
                },
                tags: ["template", "layout", "cross-artifact"],
                knownError: createLayoutAssemblyKnownError(),
                fix: "确认 Page 的 layoutName 指向已存在的 Layout，并确保 Page content 的 <layout id> 与该名称一致。不要为了通过扫描临时创建同名空 Layout。",
                matches: [{
                    pattern: "cross:layoutUnreadable",
                    line: 0,
                    excerpt: "无法读取 Layout \"" + layoutName + "\" 的内容"
                }],
                matchCount: 1,
                relatedLayout: layoutName
            });
            continue;
        }

        var layoutPlaceholderMatches = extractLayoutPlaceholderMatches(layoutContent);
        var layoutPlaceholderMap = {};
        for (var lp = 0; lp < layoutPlaceholderMatches.length; lp++) {
            layoutPlaceholderMap[layoutPlaceholderMatches[lp].value] = true;
        }
        var pagePlaceholderMatches = extractPagePlaceholderIdMatches(pageArtifact.content);
        var placeholderNameMatches = [];
        for (var pp = 0; pp < pagePlaceholderMatches.length; pp++) {
            var pagePlaceholder = pagePlaceholderMatches[pp];
            if (layoutPlaceholderMap[pagePlaceholder.value]) continue;
            placeholderNameMatches.push({
                pattern: "cross:placeholderMissingInLayout:" + pagePlaceholder.value,
                line: pagePlaceholder.line,
                excerpt: pagePlaceholder.excerpt + " (Layout \"" + layoutName + "\" 中不存在同名 k-placeholder)"
            });
        }
        if (checkPlaceholder && placeholderNameMatches.length > 0) {
            pushPlaceholderNameFinding(
                findings,
                pageArtifact,
                layoutName,
                placeholderNameMatches,
                maxMatchesPerRule
            );
        }

        if (!checkDuplicate) continue;
        var layoutViewMatches = extractViewIdMatchesInRange(layoutContent, 0, layoutContent);
        if (layoutViewMatches.length === 0) continue;

        var layoutIdMap = {};
        for (var lv = 0; lv < layoutViewMatches.length; lv++) {
            layoutIdMap[layoutViewMatches[lv].id.toLowerCase()] = layoutViewMatches[lv].id;
        }

        var placeholderBodies = extractPlaceholderBodies(pageArtifact.content);
        if (placeholderBodies.length === 0) continue;

        var duplicateMatches = [];
        for (var pb = 0; pb < placeholderBodies.length; pb++) {
            var bodyInfo = placeholderBodies[pb];
            var pageViewMatches = extractViewIdMatchesInRange(
                pageArtifact.content,
                bodyInfo.bodyStart,
                bodyInfo.body
            );
            for (var pv = 0; pv < pageViewMatches.length; pv++) {
                var pageView = pageViewMatches[pv];
                if (!layoutIdMap[pageView.id.toLowerCase()]) continue;
                duplicateMatches.push({
                    pattern: "cross:duplicateViewId:" + pageView.id,
                    line: pageView.line,
                    excerpt: pageView.excerpt + " (与 Layout \"" + layoutName + "\" 重复)"
                });
            }
        }

        if (duplicateMatches.length > 0) {
            pushCrossDuplicateFinding(findings, pageArtifact, layoutName, duplicateMatches, maxMatchesPerRule);
        }
    }
}

// 对单个 artifact 执行单条规则的所有 patterns。
// 每个命中会记录 pattern、行号和一小段 excerpt；单条规则最多保留 20 个原始命中。
function scanArtifact(rule, artifact) {
    var content = String(artifact.content || "");
    var patterns = asArray(rule.patterns);
    var requiredPatterns = asArray(rule.requiredPatterns);
    var customChecks = asArray(rule.customCheck).concat(asArray(rule.customChecks));
    var matches = [];
    for (var i = 0; i < patterns.length; i++) {
        var pattern = patterns[i];
        var re = compilePattern(pattern);
        if (!re) {
            matches.push({
                pattern: pattern,
                line: 0,
                excerpt: "Invalid regex pattern"
            });
            continue;
        }
        var m;
        var guard = 0;
        while ((m = re.exec(content)) !== null && guard < 20) {
            guard++;
            var idx = m.index || 0;
            var excerpt = content.substring(idx, Math.min(content.length, idx + 140)).replace(/\s+/g, " ").trim();
            matches.push({
                pattern: pattern,
                line: countLine(content, idx),
                excerpt: excerpt
            });
            if (m.index === re.lastIndex) re.lastIndex++;
        }
    }
    for (var j = 0; j < requiredPatterns.length; j++) {
        var requiredPattern = requiredPatterns[j];
        var requiredRe = compilePattern(requiredPattern);
        if (!requiredRe) {
            matches.push({
                pattern: requiredPattern,
                line: 0,
                excerpt: "Invalid required regex pattern"
            });
            continue;
        }
        if (!requiredRe.test(content)) {
            matches.push({
                pattern: requiredPattern,
                line: 1,
                excerpt: "Missing required pattern: " + requiredPattern
            });
        }
    }
    for (var k = 0; k < customChecks.length; k++) {
        var customMatches = runCustomCheck(customChecks[k], content, artifact);
        for (var mIndex = 0; mIndex < customMatches.length && matches.length < 20; mIndex++) {
            matches.push(customMatches[mIndex]);
        }
    }
    return matches;
}

// 准备 artifact 内容：
// - 如果调用方传了 content，直接使用，适合扫描尚未保存的临时代码。
// - 否则通过 read_object_text 按 nameOrId/objectType 读取 Kooboo 中的最新内容。
// - 读取失败会生成一个 BLOCKER finding，避免“没扫到内容却误以为通过”。
function hydrateArtifact(artifact) {
    artifact = artifact || {};
    artifact.objectType = normalizeObjectType(artifact.objectType);
    artifact.nameOrId = getArtifactNameOrId(artifact);
    artifact.name = artifact.name || artifact.nameOrId || "unnamed";

    if (typeof artifact.content === "string") {
        artifact.contentSource = artifact.contentSource || "argument";
        return { artifact: artifact, error: null };
    }

    if (!artifact.nameOrId || !artifact.objectType || artifact.objectType === "Unknown") {
        return {
            artifact: artifact,
            error: "artifact 缺少 name/nameOrId 或 objectType，且未提供 content，无法读取对象内容。"
        };
    }

    try {
        var content = readObjectText(artifact.nameOrId, artifact.objectType);
        if (content === null || content === undefined) {
            return {
                artifact: artifact,
                error: "read_object_text 未返回内容。"
            };
        }
        artifact.content = String(content);
        artifact.contentSource = "read_object_text";
        return { artifact: artifact, error: null };
    } catch (e) {
        return {
            artifact: artifact,
            error: e && e.message ? e.message : String(e)
        };
    }
}

// 按 severity 分组，方便最终生成 summary 和 result。
function groupFindings(findings) {
    var groups = { BLOCKER: [], HIGH: [], MEDIUM: [], LOW: [] };
    for (var i = 0; i < findings.length; i++) {
        var sev = findings[i].severity || "MEDIUM";
        if (!groups[sev]) groups[sev] = [];
        groups[sev].push(findings[i]);
    }
    return groups;
}

// 将规则中的已知错误路由聚合到顶层结果，方便 Debug 在看到 Preflight
// 返回值时立即进入专用 playbook，而不是继续开放式猜测。
function collectKnownErrors(findings) {
    var byId = {};
    for (var i = 0; i < findings.length; i++) {
        var finding = findings[i];
        if (!finding.knownError || !finding.knownError.id) continue;
        var id = finding.knownError.id;
        if (!byId[id]) {
            byId[id] = {
                id: id,
                priority: finding.knownError.priority || "normal",
                playbook: finding.knownError.playbook || null,
                ruleIds: [],
                artifacts: []
            };
        }
        if (byId[id].ruleIds.indexOf(finding.id) < 0) {
            byId[id].ruleIds.push(finding.id);
        }
        var artifactKey = finding.artifact.objectType + ":" + finding.artifact.name;
        if (byId[id].artifacts.indexOf(artifactKey) < 0) {
            byId[id].artifacts.push(artifactKey);
        }
    }

    var result = [];
    for (var key in byId) {
        if (Object.prototype.hasOwnProperty.call(byId, key)) result.push(byId[key]);
    }
    result.sort(function (a, b) {
        if (a.priority === b.priority) return a.id.localeCompare(b.id);
        if (a.priority === "immediate") return -1;
        if (b.priority === "immediate") return 1;
        return a.id.localeCompare(b.id);
    });
    return result;
}

// severity 排序和 minSeverity 过滤使用的权重。
function severityRank(severity) {
    var ranks = { BLOCKER: 4, HIGH: 3, MEDIUM: 2, LOW: 1 };
    return ranks[severity] || 0;
}

var DELIVERY_STAGE_RANK = {
    "static-first": 1,
    "commerce-runtime": 2,
    "payment-runtime": 3
};

function normalizeDeliveryStage(value) {
    var stage = String(value || "").trim().toLowerCase();
    return DELIVERY_STAGE_RANK[stage] ? stage : "";
}

function partitionFindingsByDeliveryStage(findings, deliveryStage) {
    var active = [];
    var deferred = [];
    var currentRank = DELIVERY_STAGE_RANK[deliveryStage] || DELIVERY_STAGE_RANK["static-first"];
    for (var i = 0; i < findings.length; i++) {
        var requiredStage = normalizeDeliveryStage(findings[i].requiredStage) || "static-first";
        if (DELIVERY_STAGE_RANK[requiredStage] <= currentRank) active.push(findings[i]);
        else deferred.push({ finding: findings[i], stage: requiredStage });
    }
    return { active: active, deferred: deferred };
}

function compactDeferredFindings(deferred) {
    var byStage = {};
    var order = ["commerce-runtime", "payment-runtime"];
    for (var i = 0; i < deferred.length; i++) {
        var stage = deferred[i].stage;
        if (!byStage[stage]) byStage[stage] = [];
        var id = deferred[i].finding.id;
        if (byStage[stage].indexOf(id) < 0) byStage[stage].push(id);
    }
    var result = [];
    for (var j = 0; j < order.length; j++) {
        var ids = byStage[order[j]];
        if (ids && ids.length) result.push({ stage: order[j], ruleIds: ids.sort() });
    }
    return result;
}

function groupFindingsForOutput(findings) {
    var grouped = {};
    var order = [];
    for (var i = 0; i < findings.length; i++) {
        var finding = findings[i];
        if (!grouped[finding.id]) {
            grouped[finding.id] = {
                id: finding.id,
                severity: finding.severity,
                title: finding.title,
                fix: finding.fix,
                occurrences: []
            };
            order.push(finding.id);
        }
        grouped[finding.id].occurrences.push({
            artifact: {
                objectType: finding.artifact.objectType,
                nameOrId: finding.artifact.name
            },
            matches: (finding.matches || []).map(function (match) {
                return { line: match.line, excerpt: match.excerpt };
            })
        });
    }
    return order.map(function (id) { return grouped[id]; });
}

function contentHash(content) {
    var text = String(content || "");
    var hash = 2166136261;
    for (var i = 0; i < text.length; i++) {
        hash ^= text.charCodeAt(i);
        hash = Math.imul(hash, 16777619);
    }
    return (hash >>> 0).toString(16).padStart(8, "0");
}

// —— 聚焦扫描 / 显式缓存 / Dirty Artifact Set 支持 ——
// capability 是安全默认值；focused 必须由调用方显式选择。
// 缓存也必须由可信 adapter 显式传入并启用，扫描器不会通过 k.file 读写站点资源。
// Page/Layout/View/Style 与 Commerce artifact 参与跨对象或能力级判断，始终重新读取内容。
function artifactCacheKey(artifact) {
    return (artifact.objectType || "Unknown") + ":" + (artifact.nameOrId || artifact.name || "");
}

function normalizeCacheArg(value) {
    if (value === null || value === undefined || value === "") return null;
    if (typeof value === "object") return value;
    try {
        var parsed = JSON.parse(String(value));
        return parsed && typeof parsed === "object" ? parsed : null;
    } catch (e) {
        return null;
    }
}

function stableStringList(values) {
    return asArray(values).map(function (value) { return String(value); }).sort();
}

// finding 复用必须绑定影响规则选择、适用性和输出裁剪的完整上下文。
// artifact 元数据也属于上下文，避免 layoutName / multilingual / Commerce surface 改变后复用旧 finding。
function buildCacheContextKey(options, artifact) {
    var metadata = {};
    var metadataFields = [
        "layoutName", "url", "multilingual", "commerceModel", "commerceSurface",
        "commerceCapability", "expectedContract", "compositionGroups"
    ];
    for (var i = 0; i < metadataFields.length; i++) {
        var field = metadataFields[i];
        if (artifact && artifact[field] !== undefined) metadata[field] = artifact[field];
    }
    return contentHash(JSON.stringify({
        multilingual: options.multilingual,
        profile: options.profile,
        appPlanId: options.appPlanId,
        scopeKey: options.scopeKey,
        taskType: options.taskType,
        layers: stableStringList(options.layers),
        minSeverity: options.minSeverity,
        maxMatchesPerRule: options.maxMatchesPerRule,
        deliveryStage: options.deliveryStage,
        commerceModel: options.commerceModel,
        commerceSurface: options.commerceSurface,
        commerceCapability: options.commerceCapability,
        metadata: metadata
    }));
}

function artifactRequiresLiveContent(artifact, entry) {
    var objectType = normalizeObjectType(artifact && artifact.objectType);
    if (includesAny(["Page", "Layout", "View", "Style", "Script"], [objectType])) return true;
    if (artifact && (artifact.commerceModel || artifact.commerceSurface || artifact.commerceCapability)) return true;
    var flags = entry && entry.contentFlags;
    return !!(flags && flags.commerce);
}

// 从 list_site_object（已缓存）按名称/ID 取对象 Size，用于低成本变化检测。
// 拿不到清单或名称不匹配时返回 null（调用方按“无法确认”处理）。
function getArtifactSize(artifact) {
    var list = readSiteObjectList(artifact.objectType);
    if (!list || !list.length) return null;
    var name = artifact.nameOrId || artifact.name || "";
    for (var i = 0; i < list.length; i++) {
        if (list[i].name === name || list[i].id === name) {
            return typeof list[i].size === "number" ? list[i].size : null;
        }
    }
    return null;
}

function normalizeDirtyKeys(dirtyList) {
    var keys = {};
    for (var i = 0; i < dirtyList.length; i++) {
        var item = dirtyList[i] || {};
        var objectType = normalizeObjectType(item.objectType);
        var nameOrId = getArtifactNameOrId(item);
        if (objectType && nameOrId) keys[objectType + ":" + nameOrId] = true;
    }
    return keys;
}

// focused 模式下决定某 artifact 是否可以直接复用缓存：
// - 必须是 focused 模式、启用缓存、存在规则版本一致的缓存项；
// - 调用方未标记 dirty；
// - 未显式传 content（临时内容一律重新扫描）；
// - 缓存条目与 list_site_object 均有可比 Size，且数值相等。
// Size 拿不到时禁止复用（无法确认未变化）；同 Size 仍可能漏掉同长改写，阶段收尾 capability 全量兜底。
function canReuseArtifact(cacheEnabled, mode, entry, dirtyKeys, cacheKey, artifact, contextKey) {
    if (!cacheEnabled || mode !== FOCUSED_MODE || !entry) return false;
    if (entry.ruleVersion !== RULE_VERSION) return false;
    if (entry.contextKey !== contextKey) return false;
    if (dirtyKeys[cacheKey]) return false;
    if (artifactRequiresLiveContent(artifact, entry)) return false;
    if (typeof artifact.content === "string") return false;
    if (!entry.capabilities || entry.complete === false) return false;
    if (typeof entry.size !== "number") return false;
    var currentSize = getArtifactSize(artifact);
    if (currentSize === null || currentSize !== entry.size) return false;
    return true;
}

// 复用对象没有 content：用缓存 contentFlags 布尔值恢复 Commerce 上下文（不再注入伪源码）。
function applyCachedContentFlags(entry, state) {
    var flags = entry && entry.contentFlags;
    if (!flags) return;
    if (flags.commerce) state.commerceContext = true;
    if (flags.payment) {
        state.commerceContext = true;
        state.commercePaymentCapability = true;
    }
}

// 从内容提取可被他对象引用的候选名（id/name/view/layout 属性），供复用路径建消费者图。
function extractRefCandidates(content) {
    var refs = [];
    var seen = {};
    var text = String(content || "");
    var attrRe = /\b(?:id|name|view|layout)\s*=\s*["']([^"']+)["']/gi;
    var m;
    while ((m = attrRe.exec(text)) !== null) {
        var name = String(m[1] || "").trim();
        if (!name || name.length < 2) continue;
        var key = name.toLowerCase();
        if (seen[key]) continue;
        seen[key] = true;
        refs.push(name);
    }
    return refs;
}

function referencesArtifactName(content, name) {
    if (!name) return false;
    var escaped = String(name).replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
    try {
        return new RegExp("\\b" + escaped + "\\b", "i").test(String(content || ""));
    } catch (e) {
        return false;
    }
}

function artifactReferencesName(artifact, name) {
    if (!artifact || !name) return false;
    if (artifact.content) {
        return referencesArtifactName(artifact.content, name);
    }
    var refs = artifact._refsOut;
    if (!Array.isArray(refs)) return false;
    var lower = String(name).toLowerCase();
    for (var i = 0; i < refs.length; i++) {
        if (String(refs[i] || "").toLowerCase() === lower) return true;
    }
    return false;
}

// 启发式依赖图：边从被引用 artifact 指向其直接消费者。
// 图只负责 scope 扩展提示；结构规则仍读取真实内容并独立给出结论。
function buildArtifactDependencyGraph(artifacts, targetKeys) {
    var consumersByArtifact = {};
    var edges = [];
    for (var i = 0; i < artifacts.length; i++) {
        var a = artifacts[i];
        if (!a || !a.name) continue;
        var aKey = (a.objectType || "Unknown") + ":" + a.name;
        var aIdKey = (a.objectType || "Unknown") + ":" + (a.nameOrId || a.name);
        if (targetKeys && !targetKeys[aKey] && !targetKeys[aIdKey]) continue;
        for (var j = 0; j < artifacts.length; j++) {
            if (i === j) continue;
            var b = artifacts[j];
            if (!b) continue;
            if (artifactReferencesName(b, a.name) || artifactReferencesName(b, a.nameOrId)) {
                if (!consumersByArtifact[aKey]) consumersByArtifact[aKey] = [];
                var bKey = (b.objectType || "Unknown") + ":" + (b.name || b.nameOrId || "");
                if (bKey && consumersByArtifact[aKey].indexOf(bKey) < 0) {
                    consumersByArtifact[aKey].push(bKey);
                    edges.push({ from: aKey, to: bKey });
                }
            }
        }
    }
    return { consumersByArtifact: consumersByArtifact, edges: edges };
}

function getCompletionGate(result, artifacts, runtimeAcceptance, mode, skippedChecks) {
    var structuralPass = result !== "FAIL";
    var structuralComplete = !skippedChecks || skippedChecks.length === 0;
    var runtimeRequired = !!(runtimeAcceptance && runtimeAcceptance.required);
    var focused = mode === FOCUSED_MODE;
    var gate = {
        claimAllowed: structuralPass && structuralComplete && !runtimeRequired && !focused,
        maxCompletionLevel: structuralPass && structuralComplete ? "structurally verified" : "modified",
        verifiedArtifacts: structuralPass ? artifacts.map(function (artifact) {
            return {
                objectType: artifact.objectType,
                nameOrId: artifact.nameOrId || artifact.name,
                contentHash: artifact._contentHash || contentHash(artifact.content)
            };
        }) : []
    };
    var missingEvidence = [];
    if (runtimeRequired) missingEvidence = missingEvidence.concat(runtimeAcceptance.checks);
    if (focused) {
        // focused 与 runtime 是两个独立证据缺口，必须同时报告，不能互相覆盖。
        missingEvidence.push("focused scan 未覆盖完整能力切片；完成当前阶段前需以 mode=capability 全量扫描");
    }
    if (!structuralComplete) {
        for (var i = 0; i < skippedChecks.length; i++) {
            missingEvidence.push(skippedChecks[i].id + ": " + skippedChecks[i].reason + ". " + skippedChecks[i].evidenceRequirement);
        }
    }
    if (missingEvidence.length) gate.missingEvidence = missingEvidence;
    if (result === "FAIL") gate.unresolvedErrors = ["Preflight contains unresolved BLOCKER findings"];
    if (focused) gate.focused = true;
    return gate;
}

// 主流程：
// 1. 读取入参，并按 artifacts 选择需要的规则文件。
// 2. 将 artifacts hydrate 成可扫描内容，优先从 Kooboo 对象读取，避免 content 参数消耗 token。
// 3. 对 artifacts 逐个过滤适用规则并扫描。
// 4. 根据是否存在 BLOCKER 决定 FAIL / PASS_WITH_WARNINGS / PASS。
function getNextActions(result, completionGate, profile, skippedChecks) {
    if (result === "FAIL") return null;
    var actions = {
        profile: profile,
        always: ["update_todos"],
        whenNaturalNextStep: ["show_next_recommended_action"]
    };
    if (skippedChecks && skippedChecks.length) {
        actions.beforeClaimCompletion = ["resolve_skipped_checks_before_rerunning_preflight"];
        actions.retryGuidance = "Structural verification is incomplete. Resolve the specific skippedChecks first. Rerun only after relevant source, scope, metadata, or scanner changes; if the required inputs are already present, report the scanner limitation and stop unchanged retries. Do not rename valid business code to evade a heuristic.";
    } else if (completionGate && completionGate.claimAllowed) {
        actions.whenStageComplete = ["update_site_milestones"];
    } else if (completionGate && completionGate.focused) {
        actions.beforeClaimCompletion = ["rerun_preflight_with_mode_capability"];
    }
    return actions;
}

function getRuntimeAcceptance(artifacts, deliveryStage, mode, profile) {
    var surfaces = {};
    var paymentCapability = false;
    var hasRenderableFrontend = false;
    for (var i = 0; i < artifacts.length; i++) {
        var objectType = String(artifacts[i].objectType || "").trim().toLowerCase();
        if (objectType === "page" || objectType === "layout" || objectType === "view" || objectType === "style" || objectType === "script" || objectType === "htmlblock") {
            hasRenderableFrontend = true;
        }
        var surface = String(artifacts[i].commerceSurface || "").trim().toLowerCase();
        if (surface) surfaces[surface] = true;
        ["products", "login", "register", "auth-status", "logout", "cart", "checkout", "orders", "order-detail"].forEach(function (name) {
            if (commerceSurfaceIs(artifacts[i], [name])) surfaces[name] = true;
        });
        if (artifacts[i]._commercePaymentCapability ||
            /k\.commerce\.order\.(?:create|list)\s*\(|k\.payment\.[A-Za-z_$][\w$]*\.charge\s*\(|fetch\s*\([^)]*(?:orders|order\/list|customer\/orders)/im.test(String(artifacts[i].content || ""))) {
            paymentCapability = true;
        }
    }

    var requirements = [];
    function requireEvidence(id, level, description) {
        requirements.push({ id: id, level: level, description: description });
    }
    if (hasRenderableFrontend && mode === CAPABILITY_MODE) {
        var uiRequirementId = profile === APP_PROFILE ? "app.ui-runtime" : "site.ui-runtime";
        requireEvidence(uiRequirementId, "runtime", "After the final preview refresh, call read_preview_diagnostics with mode=issues and verificationRequirementId=" + uiRequirementId + ". The computed-style UI audit must report passed=true with zero actionable errors; fix missing stylesheets, browser-default styling, serious horizontal overflow, and other errors before completion, and review clipped-content or overlap warnings. This is broad runtime regression evidence only: it does not prove any task-specific visual, layout, spacing, or style-repair requirement, which must be verified separately against the affected element and requested outcome.");
    }
    if (deliveryStage === "commerce-runtime" || deliveryStage === "payment-runtime") {
        if (surfaces.products) requireEvidence("commerce.products.category-runtime", "runtime", "Open Products: All and each active category show the expected associated products; refresh/back preserves the selected category.");
        if (surfaces["product-detail"]) requireEvidence("commerce.product-detail.runtime", "runtime", "Open a real product-detail URL: no query error text is rendered; image, price and selected variant are present.");
        if (surfaces.login || surfaces.register || surfaces["auth-status"] || surfaces.account || surfaces.header) requireEvidence("commerce.customer.shared-auth-runtime", "runtime", "Register/login, then open and refresh Account plus a Header page; both must show the same authenticated customer. Logout must restore guest state.");
        if (surfaces.cart || surfaces.header) requireEvidence("commerce.cart.shared-state-runtime", "runtime", "Add a real selected variant to cart; Header quantity and Cart contents must update and survive navigation/refresh.");
        if (surfaces.checkout || surfaces["order-result"] || surfaces["order-detail"] || surfaces.orders) requireEvidence("commerce.order.ownership-runtime", "runtime", "Create an order only after cart.customerId matches the logged-in customer; verify order.customer.id matches that customer and the same order appears in storefront Account/Orders.");
    }
    if (deliveryStage === "payment-runtime" && (surfaces.checkout || surfaces["order-result"] || surfaces["order-detail"] || surfaces.orders || (surfaces.account && paymentCapability))) {
        requireEvidence("commerce.payment.provider-business", "business", "Verify the server Payment endpoint produces PaymentRequest, provider callback or checkStatus confirms payment, and the same order is paid in storefront Account/Orders and Commerce admin.");
    }

    return {
        required: requirements.length > 0,
        checks: requirements.map(function (item) { return item.description; }),
        requirements: requirements
    };
}

function normalizeFunctionResult(value) {
    if (typeof value !== "string") return value;
    try {
        return JSON.parse(value);
    } catch (e) {
        return null;
    }
}

function validateAppProfileContext(appPlanId, scopeKey) {
    if (!appPlanId || !scopeKey) {
        return { error: { status: "error", code: "invalid_arguments", message: "Read the current server App Plan with read_app_plan({}) before App Preflight. The adapter injects appPlanId; do not supply it yourself." } };
    }
    try {
        var result = normalizeFunctionResult(k.ai.callFunction("read_app_plan", {}));
        if (!result || result.error || result.exists !== true || !result.plan || !result.plan.id) {
            return { error: { status: "error", code: "app_plan_read_unavailable", message: "App Preflight could not read the current server App Plan." } };
        }
        if (String(result.plan.id) !== appPlanId) {
            return { error: { status: "error", code: "app_plan_context_mismatch", message: "App Preflight read a different current App Plan. Read the Plan again before scanning." } };
        }
        return { value: { appPlanId: appPlanId, scopeKey: scopeKey } };
    } catch (e) {
        return { error: { status: "error", code: "app_plan_read_unavailable", message: "App Preflight could not call read_app_plan for the current server Plan." } };
    }
}

function main() {
    var profile = String(getArg("profile", SITE_PROFILE)).trim().toLowerCase();
    var taskType = String(getArg("taskType", "all"));
    var layers = asArray(getArg("layers", []));
    var artifacts = asArray(getArg("artifacts", []));
    var commerceModel = String(getArg("commerceModel", "")).trim();
    var commerceSurface = String(getArg("commerceSurface", "")).trim();
    var commerceCapability = String(getArg("commerceCapability", "")).trim().toLowerCase();
    var deliveryStageRaw = String(getArg("deliveryStage", "")).trim();
    var debug = getBoolArg("debug", false);
    var minSeverity = String(getArg("minSeverity", "LOW")).toUpperCase();
    var maxMatchesPerRule = Number(getArg("maxMatchesPerRule", 5));
    if (isNaN(maxMatchesPerRule) || maxMatchesPerRule < 1) maxMatchesPerRule = 5;
    var mode = String(getArg("mode", CAPABILITY_MODE)).toLowerCase();
    if (mode !== FOCUSED_MODE && mode !== CAPABILITY_MODE) mode = CAPABILITY_MODE;
    var dirtyList = asArray(getArg("dirty", []));
    var cacheEnabled = getBoolArg("useCache", false);

    if (profile !== SITE_PROFILE && profile !== APP_PROFILE) {
        return { status: "error", code: "invalid_arguments", message: "profile must be site or app." };
    }
    var appContext = null;
    if (profile === APP_PROFILE) {
        var appContextResult = validateAppProfileContext(
            String(getArg("appPlanId", "")).trim(),
            String(getArg("scopeKey", "")).trim()
        );
        if (appContextResult.error) return appContextResult.error;
        appContext = appContextResult.value;
    }

    if (!artifacts || artifacts.length === 0) {
        return { status: "error", code: "invalid_arguments", message: "No artifacts provided. Pass every Page/Layout/View/Style/Script/API/CodeBlock changed in this turn." };
    }
    if (commerceModel && commerceModel !== "catalog-contact" && commerceModel !== "online-checkout") {
        return { status: "error", code: "invalid_arguments", message: "commerceModel must be catalog-contact or online-checkout." };
    }

    // —— 显式缓存与 dirty 集合 ——
    // useCache 默认 false；只有可信 adapter 同时传 useCache=true 与 cache 对象时才启用。
    // 扫描器不会通过 k.file 创建或更新站点资源。
    var cache = cacheEnabled ? normalizeCacheArg(getArg("cache", null)) : null;
    if (!cache || !cache.entries || typeof cache.entries !== "object") {
        cache = null;
        cacheEnabled = false;
    }
    var dirtyKeys = normalizeDirtyKeys(dirtyList);
    var artifactScopeKeys = {};
    for (var scopeIndex = 0; scopeIndex < artifacts.length; scopeIndex++) {
        var scopeType = normalizeObjectType(artifacts[scopeIndex].objectType);
        var scopeName = getArtifactNameOrId(artifacts[scopeIndex]);
        if (scopeType && scopeName) artifactScopeKeys[scopeType + ":" + scopeName] = true;
    }
    var uncoveredDirty = [];
    for (var dirtyKey in dirtyKeys) {
        if (Object.prototype.hasOwnProperty.call(dirtyKeys, dirtyKey) && !artifactScopeKeys[dirtyKey]) uncoveredDirty.push(dirtyKey);
    }
    if (uncoveredDirty.length) {
        return {
            status: "error",
            code: "dirty_scope_incomplete",
            message: "Preflight artifacts do not cover every authoritative dirty artifact: " + uncoveredDirty.sort().join(", "),
            missingArtifacts: uncoveredDirty.sort()
        };
    }

    var inferredCommerceModel = commerceModel;
    if (!inferredCommerceModel) {
        for (var modelIndex = 0; modelIndex < artifacts.length; modelIndex++) {
            if (artifacts[modelIndex].commerceModel) {
                inferredCommerceModel = String(artifacts[modelIndex].commerceModel).trim();
                break;
            }
        }
    }
    for (var metadataIndex = 0; metadataIndex < artifacts.length; metadataIndex++) {
        if (!artifacts[metadataIndex].commerceModel && inferredCommerceModel) artifacts[metadataIndex].commerceModel = inferredCommerceModel;
        if (!artifacts[metadataIndex].commerceCapability && commerceCapability) artifacts[metadataIndex].commerceCapability = commerceCapability;
    }

    var platformSettings = readPlatformSettings();
    for (var settingsIndex = 0; settingsIndex < artifacts.length; settingsIndex++) {
        var settingsArtifact = artifacts[settingsIndex] || {};
        settingsArtifact.objectType = normalizeObjectType(settingsArtifact.objectType);
        settingsArtifact.nameOrId = getArtifactNameOrId(settingsArtifact);
        settingsArtifact.name = settingsArtifact.name || settingsArtifact.nameOrId || "unnamed";
        delete settingsArtifact._capabilities;
        delete settingsArtifact._layoutKnown;
        delete settingsArtifact._layoutUnknown;
        delete settingsArtifact._platformMetadataLoaded;
        settingsArtifact._multilingual = platformSettings ? platformSettings.multilingual : null;
        artifactPlatformMetadata(settingsArtifact);
    }
    var cacheContextOptions = {
        multilingual: platformSettings ? platformSettings.multilingual : null,
        profile: profile,
        appPlanId: appContext ? appContext.appPlanId : null,
        scopeKey: appContext ? appContext.scopeKey : null,
        taskType: taskType,
        layers: layers,
        minSeverity: minSeverity,
        maxMatchesPerRule: maxMatchesPerRule,
        deliveryStage: deliveryStageRaw || "static-first",
        commerceModel: inferredCommerceModel,
        commerceSurface: commerceSurface,
        commerceCapability: commerceCapability
    };

    // 先做不读内容的复用预判：focused 模式下，未标记 dirty、有同版本缓存、Size 可比且未变化、
    // 且未显式传 content 的对象可以直接复用上一次扫描结果，避免重复 read_object_text 和规则扫描。
    var reusePlan = [];
    for (var planIndex = 0; planIndex < artifacts.length; planIndex++) {
        var planSource = artifacts[planIndex] || {};
        var planArtifact = {};
        for (var planField in planSource) {
            if (Object.prototype.hasOwnProperty.call(planSource, planField)) planArtifact[planField] = planSource[planField];
        }
        planArtifact.objectType = normalizeObjectType(planArtifact.objectType);
        planArtifact.nameOrId = getArtifactNameOrId(planArtifact);
        if (!planArtifact.commerceSurface && commerceSurface) planArtifact.commerceSurface = commerceSurface;
        var planKey = artifactCacheKey(planArtifact);
        var planEntry = cacheEnabled && cache ? cache.entries[planKey] : null;
        var planContextKey = buildCacheContextKey(cacheContextOptions, planArtifact);
        reusePlan.push({
            key: planKey,
            entry: planEntry,
            contextKey: planContextKey,
            reusable: canReuseArtifact(cacheEnabled, mode, planEntry, dirtyKeys, planKey, planArtifact, planContextKey)
        });
    }

    var hydrationResults = [];
    for (var hydrateIndex = 0; hydrateIndex < artifacts.length; hydrateIndex++) {
        var sourceArtifact = artifacts[hydrateIndex];
        if (reusePlan[hydrateIndex].reusable) {
            sourceArtifact._capabilities = reusePlan[hydrateIndex].entry.capabilities || {};
            hydrationResults.push(null);
            continue;
        }
        var prepared = hydrateArtifact(sourceArtifact);
        if (prepared.error) sourceArtifact._hydrationError = prepared.error;
        artifactPlatformMetadata(sourceArtifact);
        hydrationResults.push(prepared);
    }

    var commerceContext = !!inferredCommerceModel || !!commerceSurface || !!commerceCapability;
    var commercePaymentCapability = /(?:online[-_]?checkout|checkout|payment|orders?)/.test(commerceCapability);
    for (var surfaceIndex = 0; surfaceIndex < artifacts.length; surfaceIndex++) {
        var surfaceRawContent = String(artifacts[surfaceIndex].content || "");
        var surfaceEntry = reusePlan[surfaceIndex] && reusePlan[surfaceIndex].entry;
        if (reusePlan[surfaceIndex] && reusePlan[surfaceIndex].reusable && surfaceEntry) {
            var flagState = { commerceContext: commerceContext, commercePaymentCapability: commercePaymentCapability };
            applyCachedContentFlags(surfaceEntry, flagState);
            commerceContext = flagState.commerceContext;
            commercePaymentCapability = flagState.commercePaymentCapability;
            surfaceRawContent = "";
        }
        if (artifacts[surfaceIndex].commerceModel || artifacts[surfaceIndex].commerceSurface || /(?:<query\b[^>]*\bsource\s*=\s*["']commerce["']|k\.commerce\.)/im.test(surfaceRawContent)) {
            commerceContext = true;
        }
        if (!artifacts[surfaceIndex].commerceModel && inferredCommerceModel) artifacts[surfaceIndex].commerceModel = inferredCommerceModel;
        if (!artifacts[surfaceIndex].commerceSurface) {
            artifacts[surfaceIndex].commerceSurface = commerceSurface ||
                (/k\.commerce\.|<query\b[^>]*\bsource\s*=\s*["']commerce["']/i.test(surfaceRawContent) ? inferCommerceSurface(artifacts[surfaceIndex]) : "");
        }
        var inferredSurface = String(artifacts[surfaceIndex].commerceSurface || "").trim().toLowerCase();
        var rawContent = surfaceRawContent;
        if (includesAny(["checkout", "order-result", "order-detail", "orders"], [inferredSurface]) ||
            /k\.commerce\.order\.create\s*\(|k\.payment\.[A-Za-z_$][\w$]*\.charge\s*\(|k\.commerce\.order\.list\s*\(/im.test(rawContent)) {
            commercePaymentCapability = true;
        }
    }
    for (var capabilityIndex = 0; capabilityIndex < artifacts.length; capabilityIndex++) {
        artifacts[capabilityIndex]._commercePaymentCapability = commercePaymentCapability;
    }

    if (commerceContext && !deliveryStageRaw) {
        return { status: "error", code: "invalid_arguments", message: "deliveryStage is required for Commerce Preflight." };
    }
    var deliveryStage = deliveryStageRaw ? normalizeDeliveryStage(deliveryStageRaw) : "static-first";
    if (!deliveryStage) {
        return { status: "error", code: "invalid_arguments", message: "deliveryStage must be static-first, commerce-runtime, or payment-runtime." };
    }

    var context = { profile: profile, taskType: taskType, layers: layers };
    for (var detectionIndex = 0; detectionIndex < artifacts.length; detectionIndex++) {
        if (!reusePlan[detectionIndex].reusable) artifacts[detectionIndex]._capabilities = detectCapabilities(artifacts[detectionIndex], context);
    }
    var selectedRuleResources = selectRuleResources(artifacts, profile);
    var rules;
    try { rules = readSelectedRules(selectedRuleResources); }
    catch (error) { return { status: "error", code: "rule_load_failed", message: String(error.message || error) }; }
    rules = rules.filter(function (rule) {
        var profiles = asArray(rule.profiles);
        return (profiles.length === 0 || includesAny(profiles, [profile])) &&
            severityRank(String(rule.severity || "MEDIUM").toUpperCase()) >= severityRank(minSeverity);
    });
    var findings = [];
    var skippedChecks = [];
    var consideredRules = 0;
    var hydratedArtifacts = [];
    var scanSummary = {
        profile: profile,
        mode: mode,
        ruleVersion: RULE_VERSION,
        cacheEnabled: cacheEnabled,
        artifacts: [],
        scanned: 0,
        reusedFromCache: 0,
        dirtyCount: Object.keys(dirtyKeys).length,
        artifactCount: artifacts.length
    };
    if (mode === FOCUSED_MODE && scanSummary.dirtyCount === 0 && artifacts.length >= 2) {
        scanSummary.warnings = ["focused_without_dirty"];
    }

    for (var a = 0; a < artifacts.length; a++) {
        var plan = reusePlan[a];
        if (plan && plan.reusable && plan.entry) {
            // —— 缓存命中：不读内容、不跑规则，直接复用上一次扫描结果 ——
            var reusedEntry = plan.entry;
            var reusedArtifact = {
                objectType: normalizeObjectType(artifacts[a].objectType),
                nameOrId: getArtifactNameOrId(artifacts[a]),
                name: getArtifactNameOrId(artifacts[a]) || "unnamed",
                content: "",
                _contentHash: reusedEntry.hash || "",
                _refsOut: Array.isArray(reusedEntry.refsOut) ? reusedEntry.refsOut : []
            };
            reusedArtifact._capabilities = reusedEntry.capabilities || {};
            if (artifacts[a].name) reusedArtifact.name = artifacts[a].name;
            if (artifacts[a].layoutName) reusedArtifact.layoutName = artifacts[a].layoutName;
            if (artifacts[a].commerceSurface) reusedArtifact.commerceSurface = artifacts[a].commerceSurface;
            reusedArtifact._commercePaymentCapability = commercePaymentCapability;
            hydratedArtifacts.push(reusedArtifact);
            scanSummary.reusedFromCache++;
            scanSummary.artifacts.push({
                name: reusedArtifact.name,
                objectType: reusedArtifact.objectType,
                scanned: false,
                reusedFromCache: true,
                dirty: false,
                changed: false
            });
            if (Array.isArray(reusedEntry.findings)) {
                for (var cf = 0; cf < reusedEntry.findings.length; cf++) {
                    var cachedFinding = reusedEntry.findings[cf];
                    if (!cachedFinding) continue;
                    var cachedSeverity = String(cachedFinding.severity || "MEDIUM").toUpperCase();
                    if (severityRank(cachedSeverity) < severityRank(minSeverity)) continue;
                    findings.push(cachedFinding);
                }
            }
            continue;
        }

        var hydrated = hydrationResults[a] || (artifacts[a]._hydrationError ? { artifact: artifacts[a], error: artifacts[a]._hydrationError } : hydrateArtifact(artifacts[a]));
        var artifact = hydrated.artifact;
        if (hydrated.error) {
            findings.push({
                id: "PF-SCAN-READ-001",
                title: "无法读取待扫描对象内容",
                severity: "BLOCKER",
                artifact: {
                    name: artifact.name || artifact.nameOrId || "unnamed",
                    objectType: artifact.objectType || "Unknown"
                },
                tags: ["scanner", "read-object"],
                fix: "确认 artifacts 中包含正确的 name/nameOrId 和 objectType；若对象尚未保存，可显式传 content。",
                matches: [{
                    pattern: "read_object_text",
                    line: 0,
                    excerpt: hydrated.error
                }],
                matchCount: 1
            });
            continue;
        }

        hydratedArtifacts.push(artifact);
        if (artifact._layoutUnknown) addSkippedStructuralCheck(skippedChecks, LAYOUT_READ_RULE_ID, "Page binding metadata unavailable: " + artifact.name, "Read the persisted Page LayoutName and rerun template assembly checks.");
        if (artifact._multilingual === null && /(?:location\.(?:href|assign|replace)|k\.response\.redirect|href\s*=\s*["']\/[a-z]{2}\/)/i.test(artifact.content)) addSkippedStructuralCheck(skippedChecks, "PF-KSCRIPT-004", "Site multilingual configuration unavailable", "Read get_site_info and verify whether platform multilingual routing is enabled.");
        if (artifact.objectType === "Api" && /k\.api\.(?:get|post|put|patch|delete)\s*\(\s*["']/.test(artifact.content) && !resolveArtifactUrl(artifact)) addSkippedStructuralCheck(skippedChecks, "PF-KSCRIPT-007", "API route unavailable: " + artifact.name, "Read the actual API Url and verify named action routing.");

        var artifactFindings = [];
        for (var r = 0; r < rules.length; r++) {
            var rule = rules[r];
            // Products is a page-level capability. Its category selector and
            // product grid may legitimately live in different Views.
            if (rule.id === "PF-COMMERCE-003") continue;
            var severity = String(rule.severity || "MEDIUM").toUpperCase();
            if (severityRank(severity) < severityRank(minSeverity)) continue;
            if (!ruleApplies(rule, context, artifact)) continue;
            consideredRules++;
            var matches = scanArtifact(rule, artifact);
            if (matches.length === 0) continue;
            var finding = {
                id: rule.id,
                advisory: rule.advisory === "true" && !matches.some(function (match) { return match.definite; }),
                title: rule.title || "",
                severity: severity,
                artifact: {
                    name: artifact.name,
                    objectType: artifact.objectType
                },
                requiredStage: normalizeDeliveryStage(rule.requiredStage) || "static-first",
                tags: asArray(rule.tags),
                fix: rule.fix || "",
                matches: matches.slice(0, maxMatchesPerRule),
                matchCount: matches.length
            };
            if (rule.knownErrorId) {
                finding.knownError = {
                    id: rule.knownErrorId,
                    priority: rule.debugPriority || "normal",
                    playbook: createDebugPlaybook(rule.debugPlaybookAnchor)
                };
            }
            artifactFindings.push(finding);
            findings.push(finding);
        }

        // 更新缓存条目：规则版本 + 内容哈希 + Size + findings + contentFlags + refsOut。
        var artifactHash = contentHash(artifact.content);
        artifact._contentHash = artifactHash;
        artifact._refsOut = extractRefCandidates(artifact.content);
        var cacheKey = plan ? plan.key : artifactCacheKey(artifact);
        var cachedSize = cacheEnabled && cache ? getArtifactSize(artifact) : null;
        var changed = !!cacheEnabled && !!cache && !!cache.entries[cacheKey] && cache.entries[cacheKey].hash !== artifactHash;
        var artifactDirty = !!dirtyKeys[cacheKey] || changed;
        scanSummary.scanned++;
        scanSummary.artifacts.push({
            name: artifact.name,
            objectType: artifact.objectType,
            scanned: true,
            reusedFromCache: false,
            dirty: artifactDirty,
            changed: changed
        });
        if (cacheEnabled && cache) {
            cache.entries[cacheKey] = {
                ruleVersion: RULE_VERSION,
                contextKey: plan ? plan.contextKey : buildCacheContextKey(cacheContextOptions, artifact),
                hash: artifactHash,
                size: cachedSize,
                findings: artifactFindings,
                refsOut: artifact._refsOut,
                complete: skippedChecks.length === 0,
                capabilities: artifact._capabilities,
                contentFlags: {
                    payment: /k\.commerce\.order\.(?:create|list)\s*\(|k\.payment\.[A-Za-z_$][\w$]*\.charge\s*\(|fetch\s*\([^)]*(?:orders|order\/list|customer\/orders)/im.test(String(artifact.content || "")),
                    commerce: /<query\b[^>]*\bsource\s*=\s*["']commerce["']|k\.commerce\./im.test(String(artifact.content || ""))
                }
            };
        }
    }

    scanCrossArtifactDuplicateViewRefs(hydratedArtifacts, context, findings, maxMatchesPerRule, rules);
    scanCrossArtifactResourceReferences(hydratedArtifacts, context, findings, skippedChecks, maxMatchesPerRule, rules);
    scanCommerceProductsBundle(
        hydratedArtifacts,
        findings,
        maxMatchesPerRule,
        findRuleById(rules, "PF-COMMERCE-003"),
        context
    );

    for (var findingIndex = 0; findingIndex < findings.length; findingIndex++) {
        var findingRule = findRuleById(rules, findings[findingIndex].id);
        if (findingRule) {
            findings[findingIndex].advisory = findingRule.advisory === "true" && !(findings[findingIndex].matches || []).some(function (match) { return match.definite; });
            var floor = findings[findingIndex].severityFloor;
            findings[findingIndex].severity = floor && severityRank(floor) > severityRank(findingRule.severity) ? floor : findingRule.severity;
            delete findings[findingIndex].severityFloor;
            findings[findingIndex].fix = findingRule.fix;
        }
    }
    findings.sort(function (a, b) {
        var rank = severityRank(b.severity) - severityRank(a.severity);
        if (rank !== 0) return rank;
        return String(a.id).localeCompare(String(b.id));
    });

    var advisories = findings.filter(function (finding) { return finding.advisory === true; });
    findings = findings.filter(function (finding) { return finding.advisory !== true; });
    var partitioned = partitionFindingsByDeliveryStage(findings, deliveryStage);
    var activeFindings = partitioned.active;
    var groups = groupFindings(activeFindings);
    var result = groups.BLOCKER.length > 0 ? "FAIL" : (activeFindings.length > 0 ? "PASS_WITH_WARNINGS" : "PASS");
    var runtimeAcceptance = getRuntimeAcceptance(hydratedArtifacts, deliveryStage, mode, profile);
    var completionGate = getCompletionGate(result, hydratedArtifacts, runtimeAcceptance, mode, skippedChecks);
    var output = {
        result: result,
        structuralStatus: result === "FAIL" ? "failed" : skippedChecks.length ? "incomplete" : "passed",
        summary: {
            blockers: groups.BLOCKER.length,
            high: groups.HIGH.length,
            medium: groups.MEDIUM.length,
            low: groups.LOW.length
        },
        structuralReceipt: {
            passed: result !== "FAIL",
            complete: skippedChecks.length === 0,
            profile: profile,
            mode: mode,
            ruleVersion: RULE_VERSION,
            verifiedArtifacts: completionGate.verifiedArtifacts,
            evidenceRequirements: runtimeAcceptance.requirements || []
        },
        completionGate: completionGate,
        findings: groupFindingsForOutput(activeFindings),
        scanSummary: scanSummary
    };
    if (skippedChecks.length) output.skippedChecks = skippedChecks;
    var deferred = compactDeferredFindings(partitioned.deferred);
    if (advisories.length) output.advisories = groupFindingsForOutput(advisories);
    if (deferred.length) output.deferred = deferred;
    var knownErrors = collectKnownErrors(activeFindings);
    if (knownErrors.length) output.knownErrors = knownErrors;
    if (appContext) output.structuralReceipt.appContext = appContext;
    var nextActions = getNextActions(result, completionGate, profile, skippedChecks);
    if (nextActions) output.nextActions = nextActions;

    // 直接消费者：只输出 dirty 对象在本批中的引用方，提示调用方下一次聚焦扫描应包含哪些对象。
    var dependencyGraph = buildArtifactDependencyGraph(hydratedArtifacts, dirtyKeys);
    var consumerMap = dependencyGraph.consumersByArtifact;
    var relevantConsumers = {};
    var consumerCount = 0;
    for (var s = 0; s < scanSummary.artifacts.length; s++) {
        var summaryArtifact = scanSummary.artifacts[s];
        if (!summaryArtifact.dirty) continue;
        var consumerKey = summaryArtifact.objectType + ":" + summaryArtifact.name;
        if (consumerMap[consumerKey] && consumerMap[consumerKey].length) {
            relevantConsumers[consumerKey] = consumerMap[consumerKey];
            consumerCount++;
        }
    }
    if (consumerCount) output.directConsumers = relevantConsumers;

    // 缓存归 adapter 所有；扫描器不写站点文件，也不把缓存主体塞进模型可见输出。
    output.cachePersisted = false;
    if (debug) {
        output.debug = {
            profile: profile,
            taskType: taskType,
            layers: layers,
            deliveryStage: deliveryStage,
            mode: mode,
            ruleVersion: RULE_VERSION,
            loadedRuleResources: selectedRuleResources,
            artifactCount: artifacts.length,
            ruleCount: rules.length,
            consideredRuleApplications: consideredRules,
            scannedArtifacts: scanSummary.scanned,
            reusedFromCache: scanSummary.reusedFromCache,
            dependencyEdgeCount: dependencyGraph.edges.length
        };
    }
    return output;
}

return main();
