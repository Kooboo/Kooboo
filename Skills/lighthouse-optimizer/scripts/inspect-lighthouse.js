// Compact the existing Kooboo Lighthouse/PageSpeed report for AI analysis.
// Normal usage:
// run_skill_script("lighthouse-optimizer", "scripts/inspect-lighthouse.js", {
//   url: "https://example.com/",
//   mobileDevice: true,
//   maxIssues: 15,
//   maxItemsPerAudit: 5,
//   createReportUrl: false
// })
// Test/fixture usage can pass reportJson.

var DEFAULT_MAX_ISSUES = 15;
var DEFAULT_MAX_ITEMS = 5;
var MAX_TEXT_LENGTH = 600;
var PRIORITY_ORDER = { P1: 0, P2: 1, P3: 2 };

function getArg(name, fallback) {
    var value = null;
    try {
        if (k.request && k.request.model && k.request.model[name] !== undefined) value = k.request.model[name];
    } catch (error) {
        value = null;
    }
    if ((value === null || value === undefined || value === "") && k.request && typeof k.request.get === "function") {
        value = k.request.get(name);
    }
    return value === null || value === undefined || value === "" ? fallback : value;
}

function getBooleanArg(name, fallback) {
    var value = getArg(name, fallback);
    if (typeof value === "boolean") return value;
    if (typeof value === "number") return value !== 0;
    return /^(?:true|1|yes)$/i.test(String(value || ""));
}

function getIntegerArg(name, fallback, min, max) {
    var value = parseInt(getArg(name, fallback), 10);
    if (!isFinite(value)) value = fallback;
    if (value < min) value = min;
    if (value > max) value = max;
    return value;
}

function parseUrlHost(value) {
    var match = /^(https?):\/\/([^\/?#]+)(?:[\/?#]|$)/i.exec(String(value || "").trim());
    if (!match) return null;
    var authority = match[2];
    var atIndex = authority.lastIndexOf("@");
    if (atIndex !== -1) authority = authority.substring(atIndex + 1);
    var host = authority;
    if (host.charAt(0) === "[") {
        var closeBracket = host.indexOf("]");
        if (closeBracket === -1) return null;
        host = host.substring(1, closeBracket);
    } else {
        var colonIndex = host.indexOf(":");
        if (colonIndex !== -1) host = host.substring(0, colonIndex);
    }
    host = host.toLowerCase().replace(/\.$/, "");
    return host ? { scheme: match[1].toLowerCase(), host: host } : null;
}

function parseIpv4(host) {
    if (!/^\d{1,3}(?:\.\d{1,3}){3}$/.test(host)) return null;
    var parts = host.split(".");
    var numbers = [];
    for (var index = 0; index < parts.length; index++) {
        if (parts[index].length > 1 && parts[index].charAt(0) === "0") return null;
        var value = parseInt(parts[index], 10);
        if (value < 0 || value > 255) return null;
        numbers.push(value);
    }
    return numbers;
}

function isNonPublicIpv4(parts) {
    var first = parts[0];
    var second = parts[1];
    if (first === 0 || first === 10 || first === 127) return true;
    if (first === 100 && second >= 64 && second <= 127) return true;
    if (first === 169 && second === 254) return true;
    if (first === 172 && second >= 16 && second <= 31) return true;
    if (first === 192 && (second === 0 || second === 168)) return true;
    if (first === 198 && (second === 18 || second === 19)) return true;
    if (first === 198 && second === 51 && parts[2] === 100) return true;
    if (first === 203 && second === 0 && parts[2] === 113) return true;
    if (first >= 224) return true;
    return false;
}

function validatePublicUrl(value) {
    var parsed = parseUrlHost(value);
    if (!parsed) return { valid: false, code: "invalid_url", message: "An absolute http:// or https:// URL is required." };
    var host = parsed.host;
    if (host === "localhost" || /\.localhost$/.test(host) ||
        host === "localkooboo.com" || /\.localkooboo\.com$/.test(host) ||
        /\.(?:local|lan|internal)$/.test(host) || (host.indexOf(".") === -1 && host.indexOf(":") === -1)) {
        return { valid: false, code: "local_url_not_allowed", message: "Local or internal URLs cannot be inspected by the remote Lighthouse provider.", host: host };
    }
    var ipv4 = parseIpv4(host);
    if (/^[0-9.]+$/.test(host) && !ipv4) {
        return { valid: false, code: "local_url_not_allowed", message: "Non-canonical or invalid numeric IP URLs cannot be inspected by the remote Lighthouse provider.", host: host };
    }
    if (ipv4 && isNonPublicIpv4(ipv4)) {
        return { valid: false, code: "local_url_not_allowed", message: "Private, loopback, link-local, or reserved IP URLs cannot be inspected by the remote Lighthouse provider.", host: host };
    }
    if (host.indexOf(":") !== -1) {
        var ipv6 = host.toLowerCase();
        if (ipv6 === "::" || ipv6 === "::1" || /^(?:fc|fd)/.test(ipv6) || /^fe[89ab]/.test(ipv6) || /^2001:db8(?::|$)/.test(ipv6) || /^::ffff:(?:0\.|10\.|127\.|169\.254\.|172\.(?:1[6-9]|2\d|3[01])\.|192\.(?:0\.|168\.))/.test(ipv6)) {
            return { valid: false, code: "local_url_not_allowed", message: "Private, loopback, or link-local IPv6 URLs cannot be inspected by the remote Lighthouse provider.", host: host };
        }
    }
    return { valid: true, host: host };
}

function hasOwn(value, name) {
    return value && Object.prototype.hasOwnProperty.call(value, name);
}

function truncate(value, maxLength) {
    if (value === null || value === undefined) return null;
    var text = String(value);
    var limit = maxLength || MAX_TEXT_LENGTH;
    return text.length <= limit ? text : text.substring(0, limit) + "…";
}

function utf8ByteLength(value) {
    var text = String(value || "");
    var bytes = 0;
    for (var index = 0; index < text.length; index++) {
        var code = text.charCodeAt(index);
        if (code < 0x80) bytes += 1;
        else if (code < 0x800) bytes += 2;
        else if (code >= 0xd800 && code <= 0xdbff && index + 1 < text.length) {
            var next = text.charCodeAt(index + 1);
            if (next >= 0xdc00 && next <= 0xdfff) {
                bytes += 4;
                index += 1;
            } else bytes += 3;
        } else bytes += 3;
    }
    return bytes;
}

function errorResult(code, message, details) {
    return { status: "error", error: { code: code, message: truncate(message, 1000), details: details || null } };
}

function callLighthouseJson(url, mobileDevice) {
    try {
        return k.utils.google.LHRJson(url, mobileDevice);
    } catch (firstError) {
        try {
            return k.utils.google.lHRJson(url, mobileDevice);
        } catch (secondError) {
            throw firstError;
        }
    }
}

function callShortReportUrl(json) {
    try {
        return k.utils.google.ShortLhrReportUrl(json);
    } catch (firstError) {
        try {
            return k.utils.google.shortLhrReportUrl(json);
        } catch (secondError) {
            throw firstError;
        }
    }
}

function parseJson(value) {
    if (typeof value !== "string") return value;
    return JSON.parse(value.replace(/^\uFEFF/, ""));
}

function unwrapLighthouse(payload) {
    var current = payload;
    for (var depth = 0; depth < 4; depth++) {
        if (typeof current === "string") current = parseJson(current);
        if (!current || typeof current !== "object") return null;
        if (current.audits && current.categories) return current;
        if (current.lighthouseResult) {
            current = current.lighthouseResult;
            continue;
        }
        if (current.lhr) {
            current = current.lhr;
            continue;
        }
        return null;
    }
    return current && current.audits && current.categories ? current : null;
}

function scorePercent(score) {
    return typeof score === "number" && isFinite(score) ? Math.round(score * 100) : null;
}

function getAudit(lhr, id) {
    return lhr && lhr.audits && lhr.audits[id] ? lhr.audits[id] : null;
}

function metricOutput(lhr, id) {
    var audit = getAudit(lhr, id);
    if (!audit) return null;
    return {
        id: id,
        value: typeof audit.numericValue === "number" ? audit.numericValue : null,
        unit: audit.numericUnit || null,
        displayValue: truncate(audit.displayValue, 160),
        score: scorePercent(audit.score)
    };
}

function buildCategoryData(lhr) {
    var summaries = {};
    var auditMeta = {};
    var categories = lhr.categories || {};
    for (var categoryId in categories) {
        if (!hasOwn(categories, categoryId)) continue;
        var category = categories[categoryId] || {};
        summaries[categoryId] = { title: category.title || categoryId, score: scorePercent(category.score) };
        var refs = Array.isArray(category.auditRefs) ? category.auditRefs : [];
        for (var index = 0; index < refs.length; index++) {
            var ref = refs[index] || {};
            if (!ref.id) continue;
            if (!auditMeta[ref.id]) auditMeta[ref.id] = { categories: [], weight: 0, group: null };
            if (auditMeta[ref.id].categories.indexOf(categoryId) === -1) auditMeta[ref.id].categories.push(categoryId);
            if (typeof ref.weight === "number" && ref.weight > auditMeta[ref.id].weight) auditMeta[ref.id].weight = ref.weight;
            if (!auditMeta[ref.id].group && ref.group) auditMeta[ref.id].group = ref.group;
        }
    }
    return { summaries: summaries, auditMeta: auditMeta };
}

function numeric(value) {
    return typeof value === "number" && isFinite(value) ? value : null;
}

function getSavings(audit) {
    var details = audit && audit.details ? audit.details : {};
    var savingsMs = numeric(details.overallSavingsMs);
    var savingsBytes = numeric(details.overallSavingsBytes);
    if (savingsMs === null && audit && audit.metricSavings && typeof audit.metricSavings === "object") {
        var maxMetricSaving = 0;
        for (var key in audit.metricSavings) {
            if (!hasOwn(audit.metricSavings, key)) continue;
            var metricSaving = numeric(audit.metricSavings[key]);
            if (metricSaving !== null && metricSaving > maxMetricSaving) maxMetricSaving = metricSaving;
        }
        if (maxMetricSaving > 0) savingsMs = maxMetricSaving;
    }
    return { ms: savingsMs === null ? 0 : Math.round(savingsMs), bytes: savingsBytes === null ? 0 : Math.round(savingsBytes) };
}

function compactNode(node) {
    if (!node || typeof node !== "object") return null;
    var compact = {};
    if (node.selector) compact.selector = truncate(node.selector, 500);
    if (node.snippet) compact.snippet = truncate(node.snippet, 700);
    if (node.nodeLabel) compact.nodeLabel = truncate(node.nodeLabel, 300);
    if (node.path) compact.path = truncate(node.path, 400);
    if (node.lhId) compact.lhId = truncate(node.lhId, 120);
    return Object.keys(compact).length ? compact : null;
}

function copyScalar(source, target, name, maxLength) {
    if (!source || !hasOwn(source, name)) return;
    var value = source[name];
    if (typeof value === "number" && isFinite(value)) target[name] = value;
    else if (typeof value === "boolean") target[name] = value;
    else if (typeof value === "string" && value) target[name] = truncate(value, maxLength || 400);
}

function compactItem(item) {
    if (!item || typeof item !== "object") return null;
    var compact = {};
    var fields = ["url", "origin", "entity", "label", "source", "protocol", "mimeType", "failureReason", "reason", "totalBytes", "wastedBytes", "wastedMs", "duration", "navStartToEndTime", "transferSize", "resourceSize", "cacheLifetimeMs", "cacheHitProbability", "startTime", "endTime", "statusCode", "score", "value", "isLongest"];
    for (var index = 0; index < fields.length; index++) copyScalar(item, compact, fields[index], fields[index] === "url" ? 700 : 400);
    var directNode = item.node || ((item.selector || item.snippet || item.nodeLabel) ? item : null);
    var node = compactNode(directNode);
    if (node) compact.node = node;
    if (item.sourceLocation && typeof item.sourceLocation === "object") {
        var location = {};
        copyScalar(item.sourceLocation, location, "url", 700);
        copyScalar(item.sourceLocation, location, "line", 40);
        copyScalar(item.sourceLocation, location, "column", 40);
        copyScalar(item.sourceLocation, location, "lineNumber", 40);
        copyScalar(item.sourceLocation, location, "columnNumber", 40);
        if (Object.keys(location).length) compact.sourceLocation = location;
    }
    return Object.keys(compact).length ? compact : null;
}

function pushUniqueEvidence(result, item, maxItems) {
    if (!item || result.length >= maxItems) return;
    var serialized = JSON.stringify(item);
    for (var index = 0; index < result.length; index++) {
        if (JSON.stringify(result[index]) === serialized) return;
    }
    result.push(item);
}

function collectEvidence(value, result, maxItems, depth) {
    if (value === null || value === undefined || result.length >= maxItems || depth > 6) return;
    if (Array.isArray(value)) {
        for (var arrayIndex = 0; arrayIndex < value.length && result.length < maxItems; arrayIndex++) {
            collectEvidence(value[arrayIndex], result, maxItems, depth + 1);
        }
        return;
    }
    if (typeof value !== "object") return;

    var compact = compactItem(value);
    if (compact) pushUniqueEvidence(result, compact, maxItems);

    for (var key in value) {
        if (!hasOwn(value, key) || key === "node" || key === "sourceLocation" || key === "headings" || key === "boundingRect" || key === "debugData") continue;
        var child = value[key];
        if (child && typeof child === "object") collectEvidence(child, result, maxItems, depth + 1);
        if (result.length >= maxItems) return;
    }
}

function compactDetailItems(audit, maxItems) {
    var details = audit && audit.details ? audit.details : null;
    if (!details || !details.items || maxItems <= 0) return [];
    var result = [];
    collectEvidence(details.items, result, maxItems, 0);
    return result;
}

function isMetricAudit(id) {
    return id === "first-contentful-paint" || id === "largest-contentful-paint" || id === "cumulative-layout-shift" || id === "total-blocking-time" || id === "speed-index" || id === "interactive";
}

function shouldIncludeAudit(id, audit, savings) {
    if (!audit || isMetricAudit(id)) return false;
    var mode = audit.scoreDisplayMode || "numeric";
    if (mode === "notApplicable" || mode === "not-applicable" || mode === "manual") return false;
    if (mode === "error" || audit.errorMessage) return true;
    if (savings.ms > 0 || savings.bytes > 0) return true;
    return typeof audit.score === "number" && audit.score < 0.9;
}

function containsCategory(meta, name) {
    return meta && Array.isArray(meta.categories) && meta.categories.indexOf(name) !== -1;
}

function calculateImpact(audit, meta, savings) {
    var penalty = typeof audit.score === "number" ? Math.max(0, 1 - audit.score) * 100 : 20;
    var timeImpact = Math.min(120, savings.ms / 10);
    var byteImpact = Math.min(100, savings.bytes / 8192);
    var weightImpact = meta && typeof meta.weight === "number" ? meta.weight * 4 : 0;
    var errorImpact = audit.errorMessage || audit.scoreDisplayMode === "error" ? 100 : 0;
    return Math.round(penalty + timeImpact + byteImpact + weightImpact + errorImpact);
}

function determinePriority(audit, meta, savings) {
    if (audit.errorMessage || audit.scoreDisplayMode === "error") return "P1";
    if (savings.ms >= 500 || savings.bytes >= 250000) return "P1";
    if (audit.score === 0 && meta && meta.weight >= 5) return "P1";
    if (audit.score === 0 && (containsCategory(meta, "accessibility") || containsCategory(meta, "seo"))) return "P1";
    if (savings.ms >= 100 || savings.bytes >= 50000) return "P2";
    if (typeof audit.score === "number" && audit.score < 0.9) return "P2";
    return "P3";
}

function buildIssues(lhr, auditMeta, maxIssues, maxItems) {
    var issues = [];
    var audits = lhr.audits || {};
    for (var id in audits) {
        if (!hasOwn(audits, id)) continue;
        var audit = audits[id];
        var savings = getSavings(audit);
        if (!shouldIncludeAudit(id, audit, savings)) continue;
        var meta = auditMeta[id] || { categories: [], weight: 0, group: null };
        issues.push({
            id: id,
            title: truncate(audit.title || id, 300),
            description: truncate(audit.description, 700),
            score: scorePercent(audit.score),
            scoreDisplayMode: audit.scoreDisplayMode || null,
            displayValue: truncate(audit.displayValue, 240),
            categories: meta.categories,
            group: meta.group || null,
            weight: meta.weight || 0,
            priority: determinePriority(audit, meta, savings),
            impact: calculateImpact(audit, meta, savings),
            savings: savings,
            errorMessage: truncate(audit.errorMessage, 500),
            items: compactDetailItems(audit, maxItems)
        });
    }
    issues.sort(function (left, right) {
        var priorityDifference = PRIORITY_ORDER[left.priority] - PRIORITY_ORDER[right.priority];
        if (priorityDifference !== 0) return priorityDifference;
        if (left.impact !== right.impact) return right.impact - left.impact;
        return left.id < right.id ? -1 : left.id > right.id ? 1 : 0;
    });
    return { total: issues.length, items: issues.slice(0, maxIssues) };
}

function firstEvidenceNode(lhr) {
    var preferredFragments = ["largest-contentful-paint", "lcp-", "cls-", "layout-shift"];
    var audits = lhr.audits || {};

    function findNode(value, depth) {
        if (value === null || value === undefined || depth > 6) return null;
        if (Array.isArray(value)) {
            for (var arrayIndex = 0; arrayIndex < value.length; arrayIndex++) {
                var arrayNode = findNode(value[arrayIndex], depth + 1);
                if (arrayNode) return arrayNode;
            }
            return null;
        }
        if (typeof value !== "object") return null;
        var directNode = compactNode(value.node || ((value.selector || value.snippet || value.nodeLabel) ? value : null));
        if (directNode) return directNode;
        for (var key in value) {
            if (!hasOwn(value, key) || key === "headings" || key === "boundingRect" || key === "debugData") continue;
            var childNode = findNode(value[key], depth + 1);
            if (childNode) return childNode;
        }
        return null;
    }

    for (var fragmentIndex = 0; fragmentIndex < preferredFragments.length; fragmentIndex++) {
        var fragment = preferredFragments[fragmentIndex];
        for (var id in audits) {
            if (!hasOwn(audits, id) || id.indexOf(fragment) === -1) continue;
            var details = audits[id] && audits[id].details;
            var node = findNode(details && details.items, 0);
            if (node) return { auditId: id, node: node };
        }
    }
    return null;
}

function compactWarnings(lhr, extraWarnings) {
    var warnings = [];
    var runWarnings = Array.isArray(lhr.runWarnings) ? lhr.runWarnings : [];
    for (var index = 0; index < runWarnings.length && warnings.length < 10; index++) warnings.push(truncate(runWarnings[index], 500));
    for (var extraIndex = 0; extraIndex < extraWarnings.length && warnings.length < 10; extraIndex++) warnings.push(truncate(extraWarnings[extraIndex], 500));
    return warnings;
}

var url = String(getArg("url", "") || "").trim();
var mobileDevice = getBooleanArg("mobileDevice", true);
var maxIssues = getIntegerArg("maxIssues", DEFAULT_MAX_ISSUES, 1, 30);
var maxItemsPerAudit = getIntegerArg("maxItemsPerAudit", DEFAULT_MAX_ITEMS, 0, 10);
var createReportUrl = getBooleanArg("createReportUrl", false);
var providedJson = getArg("reportJson", null);
var rawJson = null;

if (url) {
    var inputUrlValidation = validatePublicUrl(url);
    if (!inputUrlValidation.valid) return errorResult(inputUrlValidation.code, inputUrlValidation.message, { url: url, host: inputUrlValidation.host || null });
} else if (!providedJson) {
    return errorResult("invalid_url", "An absolute public http:// or https:// URL is required.");
}

try {
    rawJson = providedJson ? String(providedJson) : callLighthouseJson(url, mobileDevice);
} catch (error) {
    return errorResult("lighthouse_request_failed", error && error.message ? error.message : String(error), { url: url || null });
}

if (!rawJson || typeof rawJson !== "string") return errorResult("empty_report", "The existing Lighthouse provider returned an empty report.");

var payload = null;
var lhr = null;
try {
    payload = parseJson(rawJson);
    if (payload && payload.error) {
        return errorResult("provider_error", payload.error.message || "The Lighthouse provider returned an error.", { code: payload.error.code || null, status: payload.error.status || null });
    }
    lhr = unwrapLighthouse(payload);
} catch (error) {
    return errorResult("invalid_report_json", error && error.message ? error.message : String(error));
}

if (!lhr) return errorResult("unsupported_report", "The response does not contain a Lighthouse result with audits and categories.");

var measuredUrl = lhr.finalUrl || lhr.requestedUrl || url || null;
if (measuredUrl) {
    var measuredUrlValidation = validatePublicUrl(measuredUrl);
    if (!measuredUrlValidation.valid) return errorResult(measuredUrlValidation.code, measuredUrlValidation.message, { url: measuredUrl, host: measuredUrlValidation.host || null });
}

var extraWarnings = [];
var reportUrl = String(getArg("reportUrl", "") || "").trim() || null;
if (!reportUrl && createReportUrl) {
    try {
        reportUrl = callShortReportUrl(rawJson);
    } catch (error) {
        extraWarnings.push("The full Lighthouse report link could not be created: " + (error && error.message ? error.message : String(error)));
    }
}

var categoryData = buildCategoryData(lhr);
var issueData = buildIssues(lhr, categoryData.auditMeta, maxIssues, maxItemsPerAudit);
var config = lhr.configSettings || {};

return {
    status: "success",
    reportUrl: reportUrl,
    requestedUrl: lhr.requestedUrl || url || null,
    finalUrl: lhr.finalUrl || null,
    fetchTime: lhr.fetchTime || null,
    lighthouseVersion: lhr.lighthouseVersion || null,
    formFactor: config.formFactor || config.emulatedFormFactor || null,
    rawReportBytes: utf8ByteLength(rawJson),
    auditCount: Object.keys(lhr.audits || {}).length,
    categories: categoryData.summaries,
    metrics: {
        firstContentfulPaint: metricOutput(lhr, "first-contentful-paint"),
        largestContentfulPaint: metricOutput(lhr, "largest-contentful-paint"),
        cumulativeLayoutShift: metricOutput(lhr, "cumulative-layout-shift"),
        totalBlockingTime: metricOutput(lhr, "total-blocking-time"),
        speedIndex: metricOutput(lhr, "speed-index"),
        interactive: metricOutput(lhr, "interactive")
    },
    evidence: { primaryNode: firstEvidenceNode(lhr) },
    issueCount: issueData.total,
    returnedIssueCount: issueData.items.length,
    issues: issueData.items,
    warnings: compactWarnings(lhr, extraWarnings)
};
