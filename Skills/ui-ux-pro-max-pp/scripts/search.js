// Kooboo runtime entry for ui-ux-pro-max-pp (UUPM++).
// This script runs via run_skill_script and uses read_skill_resource for data access.

var SKILL_NAME = "ui-ux-pro-max-pp";
var MAX_RESULTS = 3;
var BOX_WIDTH = 90;
var _resourceCache = {};

var CSV_CONFIG = {
    style: { file: "styles.csv", searchCols: ["Style Category", "Keywords", "Best For", "Type", "AI Prompt Keywords"], outputCols: ["Style Category", "Type", "Keywords", "Primary Colors", "Effects & Animation", "Best For", "Performance", "Accessibility", "Framework Compatibility", "Complexity", "AI Prompt Keywords", "CSS/Technical Keywords", "Implementation Checklist", "Design System Variables"] },
    color: { file: "colors.csv", searchCols: ["Product Type", "Notes"], outputCols: ["Product Type", "Primary (Hex)", "Secondary (Hex)", "CTA (Hex)", "Background (Hex)", "Text (Hex)", "Notes", "Variant ID", "Tone Tags", "Palette Family", "Weight"] },
    chart: { file: "charts.csv", searchCols: ["Data Type", "Keywords", "Best Chart Type", "Accessibility Notes"], outputCols: ["Data Type", "Keywords", "Best Chart Type", "Secondary Options", "Color Guidance", "Accessibility Notes", "Library Recommendation", "Interactive Level"] },
    landing: { file: "landing.csv", searchCols: ["Pattern Name", "Keywords", "Conversion Optimization", "Section Order"], outputCols: ["Pattern Name", "Keywords", "Section Order", "Primary CTA Placement", "Color Strategy", "Conversion Optimization"] },
    product: { file: "products.csv", searchCols: ["Product Type", "Keywords", "Primary Style Recommendation", "Key Considerations"], outputCols: ["Product Type", "Keywords", "Primary Style Recommendation", "Secondary Styles", "Landing Page Pattern", "Dashboard Style (if applicable)", "Color Palette Focus"] },
    ux: { file: "ux-guidelines.csv", searchCols: ["Category", "Issue", "Description", "Platform"], outputCols: ["Category", "Issue", "Platform", "Description", "Do", "Don't", "Code Example Good", "Code Example Bad", "Severity"] },
    typography: { file: "typography.csv", searchCols: ["Font Pairing Name", "Category", "Mood/Style Keywords", "Best For", "Heading Font", "Body Font"], outputCols: ["Font Pairing Name", "Category", "Heading Font", "Body Font", "Mood/Style Keywords", "Best For", "Google Fonts URL", "CSS Import", "Tailwind Config", "Notes"] },
    icons: { file: "icons.csv", searchCols: ["Category", "Icon Name", "Keywords", "Best For"], outputCols: ["Category", "Icon Name", "Keywords", "Library", "Import Code", "Usage", "Best For", "Style"] },
    web: { file: "web-interface.csv", searchCols: ["Category", "Issue", "Keywords", "Description"], outputCols: ["Category", "Issue", "Platform", "Description", "Do", "Don't", "Code Example Good", "Code Example Bad", "Severity"] },
    slop: { file: "anti-slop.csv", searchCols: ["Category", "Issue", "Description", "Do", "Don't"], outputCols: ["Category", "Issue", "Description", "Do", "Don't", "Code Example Good", "Code Example Bad", "Severity"] },
    prompt: { file: "ui-prompts.csv", searchCols: ["Section Type", "Prompt ID", "Name", "Keywords", "Style Tags", "Best For", "Layout Archetype", "Content Hierarchy", "Density", "Conversion Goal", "Visual Treatment", "Compatible Styles", "Variant Brief", "Prompt ZH", "Prompt EN", "Kooboo Constraints", "Layout Intent", "Avoid Patterns"], outputCols: ["Section Type", "Prompt ID", "Name", "Keywords", "Style Tags", "Required Content", "Best For", "Layout Archetype", "Content Hierarchy", "Density", "Conversion Goal", "Visual Treatment", "Compatible Styles", "Variant Brief", "Prompt ZH", "Prompt EN", "Kooboo Constraints", "Preflight Tags", "Scope", "Create Phase", "Image Policy", "Data Mode", "Interaction Policy", "Layout Intent", "Avoid Patterns"] }
};

// Kooboo 站点统一使用原生 HTML + Tailwind（脚本引入或后台开关内置）+ Kooboo 服务端绑定语法
// (k-data / env="server" / k-for / k-if)。其它前端框架的指南对 Kooboo 场景是噪音，已下线。
var STACK_CONFIG = {
    "html-tailwind": "stacks/html-tailwind.csv"
};

var STACK_COLS = {
    searchCols: ["Category", "Guideline", "Description", "Do", "Don't"],
    outputCols: ["Category", "Guideline", "Description", "Do", "Don't", "Code Good", "Code Bad", "Severity", "Docs URL"]
};

var SEARCH_CONFIG = {
    product: { maxResults: 1 },
    style: { maxResults: 3 },
    color: { maxResults: 2 },
    landing: { maxResults: 2 },
    typography: { maxResults: 2 }
};

var PROMPT_SCORE_THRESHOLD_RATIO = 0.65;
var HERO_SCORE_THRESHOLD_RATIO = 0.10;
var PRODUCT_SCORE_THRESHOLD_RATIO = 0.65;
var PRODUCT_SCORE_GAP = 0.35;
var SEED_PICK_RANK_DECAY = 0.62;
var OPEN_BRIEF_SEED_PICK_RANK_DECAY = 0.86;
var OPEN_BRIEF_CONTENT_SEED_PICK_RANK_DECAY = 0.78;
var DIVERSITY_RESCUE_TARGET = 3;
var DIVERSITY_RESCUE_RATIO = 0.40;
// Shared chrome (header/footer) candidates already passed a strict structural
// filter (brandName/navItems, legalLinks/navGroups). BM25 gaps between valid
// chrome variants are mostly noise, so use a lower rescue floor to keep the
// distinct-archetype variants in the diversity pool.
var SHARED_CHROME_RESCUE_RATIO = 0.10;
// Hero candidates are hard-filtered to the conversion goal, which can leave only
// 1-2 desktop topologies for a vertical. To keep the hero visually diverse, when
// the goal-matched set covers fewer distinct topologies than this target, backfill
// the best off-goal hero of each missing topology. The goal bonus keeps on-goal
// heroes ranked first, so the off-goal topology lands last with a low seed weight.
var HERO_TOPOLOGY_TARGET = 5;
var COMMERCE_HERO_QUERY_TERMS = "ecommerce commerce shop store product catalog category bento gallery search overlay campaign purchase featured products shelf curated masonry origin sourcing subscription comparison local pickup lookbook marketplace proof guided finder sensory fullbleed photography overlay editorial runway quiet luxury craft counter";
var COLOR_VARIANT_POOL_TARGET = 3;
var COMMERCE_COLOR_FALLBACK_TYPES = [
    "Commerce Palette Pool",
    "E-commerce",
    "Marketplace (P2P)",
    "Digital Products/Downloads",
    "Subscription Box Service"
];
var PREMIUM_COLOR_FALLBACK_TYPES = [
    "Premium Palette Pool",
    "Luxury/Premium Brand",
    "E-commerce Luxury",
    "Fashion Editorial Retail"
];
var PLAYFUL_COLOR_FALLBACK_TYPES = [
    "Playful Palette Pool",
    "Pet Tech App",
    "Educational App",
    "Creator Economy Platform"
];
var NATURAL_COLOR_FALLBACK_TYPES = [
    "Natural Palette Pool",
    "Sustainability/ESG Platform",
    "Agriculture/Farm Tech"
];
var _parsedCsvCache = {};
var _uiPromptIndex = null;
var DISABLED_PROMPT_IDS = {
    "footer.prompt.cta-footer": true
};

var STYLE_KEY_ALIASES = {
    minimalism: "minimalism",
    minimal: "minimalism",
    "flat-design": "flat-design",
    flat: "flat-design",
    glassmorphism: "glassmorphism",
    neumorphism: "neumorphism",
    brutalism: "brutalism",
    editorial: "editorial",
    luxury: "luxury",
    playful: "playful",
    professional: "professional",
    modern: "modern",
    organic: "organic",
    retro: "retro",
    "dark-mode": "dark-mode"
};

function readRequestArg(name) {
    try {
        if (k.request && k.request.model && k.request.model[name] !== undefined) {
            return k.request.model[name];
        }
    } catch (error) {
        // Fall back to the string-based request accessor.
    }
    return k.request.get(name);
}
function getArg(name, defaultValue) {
    var value = readRequestArg(name);
    return (value === null || value === undefined || value === "") ? defaultValue : value;
}
function hasArg(name) {
    var value = readRequestArg(name);
    return value !== null && value !== undefined && value !== "";
}
function getNumberArg(name, defaultValue) {
    var n = Number(getArg(name, defaultValue));
    return isNaN(n) ? defaultValue : n;
}

function resultError(code, message) {
    return { status: "error", code: code, message: message };
}

var VALID_MODES = {
    "page-plan": true,
    "design-system": true,
    "page-item": true,
    "view-batch": true,
    "prompt-recommendations": true,
    search: true,
    stack: true,
    help: true
};

function normalizeMode(value) {
    var mode = String(value || "").trim().toLowerCase();
    return VALID_MODES[mode] ? mode : "";
}

var MODE_ARGUMENTS = {
    "page-plan": ["mode", "brief", "sessionId", "designVariantSeed"],
    "design-system": ["mode", "brief", "sessionId", "designVariantSeed"],
    "page-item": ["mode", "brief", "pageName", "sessionId", "designVariantSeed"],
    "view-batch": ["mode", "brief", "blockRefs", "sessionId", "designVariantSeed"],
    "prompt-recommendations": ["mode", "brief", "query", "sectionType", "pageName", "limit", "excludePromptIds", "projectName", "sessionId", "designVariantSeed", "contentLocale"],
    search: ["mode", "query", "domain", "limit"],
    stack: ["mode", "query", "stack", "limit"],
    help: ["mode"]
};

function getRequestArgumentNames() {
    try {
        if (k.request && k.request.model && typeof k.request.model === "object" && !Array.isArray(k.request.model)) {
            return Object.keys(k.request.model);
        }
    } catch (error) {
        // The string accessor fallback cannot enumerate arbitrary request fields.
    }
    var names = {};
    var modes = Object.keys(MODE_ARGUMENTS);
    for (var i = 0; i < modes.length; i++) {
        var fields = MODE_ARGUMENTS[modes[i]];
        for (var j = 0; j < fields.length; j++) names[fields[j]] = true;
    }
    return Object.keys(names).filter(function (name) { return hasArg(name); });
}

function validateModeArguments(mode) {
    var allowed = {};
    var fields = MODE_ARGUMENTS[mode] || [];
    for (var i = 0; i < fields.length; i++) allowed[fields[i]] = true;
    var argumentNames = getRequestArgumentNames();
    for (var j = 0; j < argumentNames.length; j++) {
        var field = argumentNames[j];
        if (!allowed[field]) return field + " is not valid for mode " + mode;
    }
    return "";
}

function readSkillResource(resourceName) {
    var key = "data/" + resourceName;
    if (_resourceCache[key]) return _resourceCache[key];
    var content = k.ai.callFunction("read_skill_resource", { skillName: SKILL_NAME, resourceName: key });
    _resourceCache[key] = String(content || "");
    return _resourceCache[key];
}

function readSkillJson(resourceName, fallbackValue) {
    var key = "json:data/" + resourceName;
    if (_resourceCache.hasOwnProperty(key)) return _resourceCache[key];
    var content = readSkillResource(resourceName);
    try {
        _resourceCache[key] = JSON.parse(String(content || ""));
    } catch (e) {
        _resourceCache[key] = fallbackValue;
    }
    return _resourceCache[key];
}

function parseCSVLine(line) {
    var result = [];
    var current = "";
    var inQuotes = false;
    var i;
    for (i = 0; i < line.length; i++) {
        var ch = line[i];
        if (ch === "\"") {
            if (inQuotes && line[i + 1] === "\"") {
                current += "\"";
                i++;
            } else {
                inQuotes = !inQuotes;
            }
        } else if (ch === "," && !inQuotes) {
            result.push(current.trim());
            current = "";
        } else {
            current += ch;
        }
    }
    result.push(current.trim());
    return result;
}

function parseCSV(content) {
    var lines = String(content || "").replace(/^\uFEFF/, "").split("\n");
    var clean = [];
    for (var i = 0; i < lines.length; i++) {
        if (lines[i].trim()) clean.push(lines[i]);
    }
    if (clean.length < 1) return [];
    var headers = parseCSVLine(clean[0]);
    var rows = [];
    for (var j = 1; j < clean.length; j++) {
        var values = parseCSVLine(clean[j]);
        var row = {};
        for (var kx = 0; kx < headers.length; kx++) row[headers[kx]] = values[kx] || "";
        rows.push(row);
    }
    return rows;
}

function tokenize(text) {
    var raw = String(text || "").toLowerCase();
    var out = [];
    var ascii = raw.replace(/[^\x00-\x7f]+/g, " ");
    var parts = ascii.replace(/[^\w\s]/g, " ").split(/\s+/);
    var i;
    for (i = 0; i < parts.length; i++) {
        if (parts[i].length > 2) out.push(parts[i]);
    }
    var cjkRe = /[\u4e00-\u9fff\u3040-\u30ff\uac00-\ud7af]+/g;
    var match;
    while ((match = cjkRe.exec(raw)) !== null) {
        var seg = match[0];
        for (i = 0; i < seg.length; i++) out.push(seg.charAt(i));
        for (i = 0; i < seg.length - 1; i++) out.push(seg.charAt(i) + seg.charAt(i + 1));
    }
    return out;
}

function getParsedCsvRows(fileName) {
    if (_parsedCsvCache[fileName]) return _parsedCsvCache[fileName];
    _parsedCsvCache[fileName] = parseCSV(readSkillResource(fileName));
    return _parsedCsvCache[fileName];
}

function getUiPromptIndex() {
    if (_uiPromptIndex) return _uiPromptIndex;
    var sourceRows = getParsedCsvRows("ui-prompts.csv");
    var rows = [];
    var bySection = {};
    var i;
    for (i = 0; i < sourceRows.length; i++) {
        var promptId = String(sourceRows[i]["Prompt ID"] || "");
        if (DISABLED_PROMPT_IDS[promptId]) continue;
        rows.push(sourceRows[i]);
        var sectionKey = normalizeKey(sourceRows[i]["Section Type"]);
        if (!bySection[sectionKey]) bySection[sectionKey] = [];
        bySection[sectionKey].push(sourceRows[i]);
    }
    _uiPromptIndex = { rows: rows, bySection: bySection };
    return _uiPromptIndex;
}

function findPromptRowById(promptId) {
    var target = String(promptId || "");
    if (!target) return null;
    var rows = getUiPromptIndex().rows;
    for (var i = 0; i < rows.length; i++) {
        if (String(rows[i]["Prompt ID"] || "") === target) return rows[i];
    }
    return null;
}

function normalizeStyleKey(value) {
    var key = normalizeKey(value);
    if (!key) return "";
    if (STYLE_KEY_ALIASES[key]) return STYLE_KEY_ALIASES[key];
    if (/minimal|swiss|e-ink|zero-interface/.test(key)) return "minimalism";
    if (/flat/.test(key)) return "flat-design";
    if (/glass|liquid-glass|spatial-ui/.test(key)) return "glassmorphism";
    if (/neumorph|soft-ui/.test(key)) return "neumorphism";
    if (/brutal|raw|anti-polish/.test(key)) return "brutalism";
    if (/editorial|magazine|storytelling|paper/.test(key)) return "editorial";
    if (/luxury|premium|skeuomorph|tactile/.test(key)) return "luxury";
    if (/playful|vibrant|memphis|clay|gen-z|maximal/.test(key)) return "playful";
    if (/accessible|inclusive|trust|authority|executive/.test(key)) return "professional";
    if (/organic|nature|biomimetic|biophilic/.test(key)) return "organic";
    if (/retro|vintage|y2k|vaporwave|pixel/.test(key)) return "retro";
    if (/dark|oled|cyberpunk|hud|sci-fi/.test(key)) return "dark-mode";
    if (/modern|bento|aurora|gradient|dimensional|product|conversion|feature|motion|interactive|ai-native/.test(key)) return "modern";
    return key;
}

function layoutArchetypeOf(row) {
    return normalizeKey(row["Layout Archetype"] || "");
}

// Visual desktop skeleton only — distinct from layoutArchetype / contentHierarchy.
var DESKTOP_TOPOLOGY_BY_ARCHETYPE = {
    "split-left": "split-left",
    "split-reverse": "split-reverse",
    "fullbleed": "fullbleed",
    "typography-stack": "typography-stack",
    "stacked-mobile-first": "stacked-mobile-first",
    "proof-band": "split-left",
    "booking-panel": "split-left",
    // Retail product heroes are product-stage compositions, not marketing copy|media splits.
    "product-spotlight": "featured-item-stage",
    "announcement-banner": "typography-stack",
    "demo-frame": "split-left",
    "editorial-margin": "typography-stack",
    "search-forward": "search-overlay",
    "dashboard-mockup-split": "product-mock-stage",
    "ai-isometric-split": "illustration-stage",
    "dark-fintech-proof": "product-mock-stage",
    "restaurant-reservation-photo": "photo-reservation-stage",
    "category-bento-commerce": "category-bento",
    "monochrome-editorial-fullbleed": "fullbleed",
    "web3-marketplace-3d": "illustration-stage",
    "gallery-sidebar-product": "gallery-sidebar",
    "wedding-collage-booking": "media-collage",
    "property-visual-service": "service-visual-stage",
    "health-app-preview": "product-mock-stage",
    "vehicle-booking-spotlight": "featured-item-stage",
    "green-service-frame": "service-frame",
    "search-led-boutique-commerce": "search-overlay",
    "dark-delivery-illustration": "illustration-stage",
    "vertical-split-carousel": "vertical-carousel",
    "cinematic-portfolio-reel": "cinematic-reel",
    "fullbleed-carousel-stage": "fullbleed",
    "ai-prompt-composer": "interactive-composer",
    "interior-search-overlay": "search-overlay",
    "proof-card-service-stage": "proof-card-stage",
    "legal-consultation-carousel": "media-overlay-stage",
    "dark-law-media-strip": "media-strip",
    "furniture-cost-campaign-panel": "campaign-panel",
    "architecture-monochrome-slider": "typography-slider",
    "product-launch-shelf": "product-shelf-stage",
    "craft-story-product-journal": "editorial-product-journal",
    "collection-runway-strip": "collection-strip",
    "deal-countdown-command": "campaign-panel",
    "sensory-fullbleed-product": "fullbleed",
    "origin-map-product": "origin-map-stage",
    "curator-picks-masonry": "masonry-gallery",
    "subscription-builder-hero": "interactive-composer",
    "comparison-proof-commerce": "proof-card-stage",
    "local-pickup-market": "local-market-stage",
    "price-assurance-split": "assurance-split",
    "lookbook-spread-commerce": "media-collage",
    "service-booking-dock": "booking-dock",
    "calendar-proof-appointment": "calendar-stage",
    "marketplace-vendor-grid": "marketplace-grid",
    "cinematic-product-reel": "cinematic-reel",
    "social-proof-product-wall": "product-proof-wall",
    "guided-finder-commerce": "search-overlay"
};

// Coarser visual family used only for hero diversity backfill: split-left and
// split-reverse are distinct topologies but share a two-column skeleton, so they
// must not count as "different" when reaching for a genuinely distinct 3rd hero.
function heroTopologyFamilyOf(topology) {
    if (topology === "split-left" || topology === "split-reverse") return "split";
    if (topology === "product-mock-stage" || topology === "featured-item-stage" || topology === "product-shelf-stage" || topology === "product-proof-wall") return "product-stage";
    if (topology === "search-overlay" || topology === "category-bento" || topology === "collection-strip" || topology === "masonry-gallery" || topology === "marketplace-grid" || topology === "local-market-stage") return "commerce-discovery";
    if (topology === "media-overlay-stage" || topology === "fullbleed" || topology === "cinematic-reel") return "immersive-media";
    if (topology === "typography-stack" || topology === "typography-slider" || topology === "editorial-product-journal") return "typography";
    if (topology === "proof-card-stage" || topology === "service-visual-stage" || topology === "service-frame" || topology === "origin-map-stage" || topology === "assurance-split" || topology === "booking-dock" || topology === "calendar-stage") return "service-proof";
    if (topology === "illustration-stage") return "illustration";
    return topology;
}

function desktopTopologyOf(row) {
    var archetype = layoutArchetypeOf(row);
    if (DESKTOP_TOPOLOGY_BY_ARCHETYPE[archetype]) return DESKTOP_TOPOLOGY_BY_ARCHETYPE[archetype];
    if (/split-reverse|reverse/.test(archetype)) return "split-reverse";
    if (/fullbleed|immersive|background/.test(archetype)) return "fullbleed";
    if (/bento|category/.test(archetype)) return "category-bento";
    if (/dashboard|mockup|app-preview|product-spotlight/.test(archetype)) return "product-mock-stage";
    if (/gallery|sidebar/.test(archetype)) return "gallery-sidebar";
    if (/collage|mosaic/.test(archetype)) return "media-collage";
    if (/search|filter|overlay/.test(archetype)) return "search-overlay";
    if (/campaign|promo|deal|countdown/.test(archetype)) return "campaign-panel";
    if (/marketplace|vendor-grid/.test(archetype)) return "marketplace-grid";
    if (/masonry|curator/.test(archetype)) return "masonry-gallery";
    if (/product-shelf|launch-shelf/.test(archetype)) return "product-shelf-stage";
    if (/origin-map|source-map/.test(archetype)) return "origin-map-stage";
    if (/booking-dock/.test(archetype)) return "booking-dock";
    if (/calendar/.test(archetype)) return "calendar-stage";
    if (/assurance/.test(archetype)) return "assurance-split";
    if (/proof-wall|social-proof/.test(archetype)) return "product-proof-wall";
    if (/proof-card|proof.*stage/.test(archetype)) return "proof-card-stage";
    if (/media-strip|strip/.test(archetype)) return "media-strip";
    if (/inline-message|narrow-prose|headline-list/.test(archetype)) return "stacked-mobile-first";
    if (/carousel|slider/.test(archetype)) return "vertical-carousel";
    if (/isometric|3d|illustration/.test(archetype)) return "illustration-stage";
    if (/reel|cinematic/.test(archetype)) return "cinematic-reel";
    if (/stacked|mobile-first/.test(archetype)) return "stacked-mobile-first";
    if (/typography|editorial|margin|announcement|statement|centered-notice|magazine-pullquote/.test(archetype)) return "typography-stack";
    if (/split|grid|aside|panel|two-col|dual|prose-with|form-details|items-totals|filter-sidebar|chart-insight|lead-story/.test(archetype)) return "split-left";
    return "typography-stack";
}

function responsiveStrategyOf(row) {
    var topology = desktopTopologyOf(row);
    if (topology === "stacked-mobile-first") return "mobile-copy-first";
    if (topology === "fullbleed") return "mobile-overlay-collapse";
    if (topology === "split-reverse") return "mobile-visual-first";
    if (topology === "split-left") return "mobile-copy-first";
    if (topology === "typography-stack") return "mobile-copy-first";
    if (topology === "category-bento" || topology === "gallery-sidebar" || topology === "media-collage" || topology === "collection-strip" || topology === "masonry-gallery" || topology === "marketplace-grid" || topology === "product-proof-wall") return "mobile-visual-grid-collapse";
    if (topology === "search-overlay") return "mobile-search-first";
    if (topology === "campaign-panel" || topology === "proof-card-stage" || topology === "origin-map-stage" || topology === "assurance-split" || topology === "booking-dock" || topology === "calendar-stage") return "mobile-panel-first";
    if (topology === "product-mock-stage" || topology === "featured-item-stage" || topology === "product-shelf-stage" || topology === "interactive-composer" || topology === "local-market-stage") return "mobile-product-first";
    if (topology === "illustration-stage" || topology === "service-visual-stage" || topology === "service-frame" || topology === "editorial-product-journal") return "mobile-copy-first";
    if (topology === "media-overlay-stage" || topology === "media-strip" || topology === "cinematic-reel" || topology === "vertical-carousel" || topology === "typography-slider") return "mobile-overlay-collapse";
    return "mobile-adaptive";
}

function buildSelectionContractFields(row) {
    return {
        layoutArchetype: layoutArchetypeOf(row),
        desktopTopology: desktopTopologyOf(row),
        contentHierarchy: normalizeKey(row["Content Hierarchy"] || ""),
        responsiveStrategy: responsiveStrategyOf(row),
        visualTreatment: normalizeKey(row["Visual Treatment"] || "")
    };
}

function buildVisualDiversityDirective(row, sectionType) {
    var contract = buildSelectionContractFields(row);
    var parts = [
        "Visibly implement this selected component variant, not a generic fallback.",
        "layoutArchetype=" + (contract.layoutArchetype || "unspecified"),
        "desktopTopology=" + (contract.desktopTopology || "unspecified"),
        "contentHierarchy=" + (contract.contentHierarchy || "unspecified"),
        "visualTreatment=" + (contract.visualTreatment || "unspecified")
    ];
    if (sectionType === "hero") {
        parts.push("Hero first viewport must make the selected archetype obvious; do not invent a left-copy/right-image marketing split unless desktopTopology is explicitly split-left or split-reverse.");
        if (contract.desktopTopology === "fullbleed" || contract.desktopTopology === "media-overlay-stage" || contract.desktopTopology === "cinematic-reel") {
            parts.push("Photography may fill the viewport as background, but brand/headline and/or CTA and/or in-viewport product/category path must overlay the same plane — never a silent image wall.");
        }
        if (contract.desktopTopology === "search-overlay") {
            parts.push("Lead with wayfinding/search/filters in the first viewport, not a slogan marketing hero.");
        }
    }
    return parts.join(" ");
}

function buildGridFillDirective(row, sectionType) {
    var signature = [
        sectionType,
        layoutArchetypeOf(row),
        desktopTopologyOf(row)
    ].join(" ").toLowerCase();
    if (!/(grid|bento|masonry|catalog|shelf|wall|featured-products|related-products)/.test(signature)) return "";
    return "Grid fill: Spanning or stretched cards need an h-full inner wrapper with growing media/content. If content is short, reduce the span or use self-start. No blank card interior; drop mismatched spans/fixed heights on mobile.";
}

function normalizePlanDetail(value) {
    var v = String(value || "").toLowerCase();
    if (v === "full") return "full";
    return "summary";
}

function recommendedViewNameForBlock(pageName, block) {
    var raw = block.blockRef || blockRefFor(pageName, block.type, block, 0);
    var name = normalizeKey(raw).replace(/-/g, "_") || "view";
    if (name.length <= 64) return name;
    return name.slice(0, 64).replace(/_+$/g, "") || "view";
}

var CREATIVE_DIRECTION_BUCKETS = [
    { id: "romantic-editorial", paletteFamily: "blush" },
    { id: "premium-dramatic", paletteFamily: "dark-gold" },
    { id: "natural-product-led", paletteFamily: "natural" },
    { id: "minimal-gallery", paletteFamily: "monochrome" },
    { id: "playful-seasonal", paletteFamily: "high-contrast" }
];

function isCanonicalDesignVariantSeed(seed) {
    var value = String(seed || "").trim();
    return /^dvs-\d+$/i.test(value) || /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i.test(value);
}

function isRepeatedUuid(uuid) {
    return /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(uuid)
        && /^(.)\1{7}-(.)\2{3}-(.)\3{3}-(.)\4{3}-(.)\5{11}$/i.test(uuid);
}

function isWeakSessionId(sessionId) {
    var value = String(sessionId || "").trim();
    var key = normalizeKey(value);
    if (!key) return true;
    if (value.length < 16) return true;
    if (isRepeatedUuid(value)) return true;
    return /^(session|chat|conversation|test|demo|example|placeholder|current-chat-conversation-id)(-|_)?\d*$/i.test(key)
        || /current|example|placeholder/.test(key);
}

function resolveDesignVariantSeed(brief, projectName) {
    if (brief && brief.designVariantSeed) {
        var briefSeed = String(brief.designVariantSeed).trim();
        return isCanonicalDesignVariantSeed(briefSeed) ? briefSeed : "";
    }
    var argSeed = getArg("designVariantSeed", "");
    if (argSeed) {
        var seed = String(argSeed).trim();
        return isCanonicalDesignVariantSeed(seed) ? seed : "";
    }
    var sessionId = getArg("sessionId", "");
    if (sessionId && !isWeakSessionId(sessionId)) return "dvs-" + stableHash(String(sessionId));
    return "";
}

function validateDesignVariantSeedValue(seed, fieldName) {
    var value = String(seed || "").trim();
    if (!value || isCanonicalDesignVariantSeed(value)) return "";
    return fieldName + " must be an opaque UUID or a script-returned dvs-* seed. Put natural-language design intent in creativeDirection.";
}

function pickCreativeDirectionFromSeed(seed) {
    var idx = stableHash(seed) % CREATIVE_DIRECTION_BUCKETS.length;
    return CREATIVE_DIRECTION_BUCKETS[idx].id;
}

function paletteFamilyForCreativeDirection(directionId) {
    var i;
    for (i = 0; i < CREATIVE_DIRECTION_BUCKETS.length; i++) {
        if (CREATIVE_DIRECTION_BUCKETS[i].id === directionId) {
            return CREATIVE_DIRECTION_BUCKETS[i].paletteFamily;
        }
    }
    return "";
}

function enrichSiteBrief(brief, projectName) {
    if (!brief) return brief;
    var enriched = Object.assign({}, brief);
    enriched.designVariantSeed = resolveDesignVariantSeed(enriched, projectName);
    if (enriched.designVariantSeed && !enriched.creativeDirection && (!enriched.tone || !enriched.tone.length)) {
        enriched.creativeDirection = pickCreativeDirectionFromSeed(enriched.designVariantSeed);
    }
    if (enriched.creativeDirection && !enriched.palettePreference) {
        enriched.palettePreference = paletteFamilyForCreativeDirection(enriched.creativeDirection);
    }
    return enriched;
}

function requireDesignVariantSeed(enriched) {
    if (enriched && enriched.designVariantSeed) return null;
    return "designVariantSeed is required. On the first call, pass a real sessionId or an explicit UUID outside AI Chat; in AI Chat, omit designVariantSeed so the current session can be injected. Later reuse the script-returned dvs-* seed in resolvedBrief.";
}

function getColorRowsForProductType(productType) {
    var rows = getParsedCsvRows("colors.csv");
    var matched = [];
    var i;
    for (i = 0; i < rows.length; i++) {
        if (rows[i]["Product Type"] === productType) matched.push(rows[i]);
    }
    return matched;
}

function rowsWithColorVariantId(rows) {
    var out = [];
    for (var i = 0; i < rows.length; i++) {
        if (String(rows[i]["Variant ID"] || "").trim()) out.push(rows[i]);
    }
    return out;
}

function getColorVariantsForProductType(productType) {
    var matched = getColorRowsForProductType(productType);
    var withVariant = matched.filter(function (r) { return String(r["Variant ID"] || "").trim(); });
    return withVariant.length ? withVariant : matched;
}

function colorRowIdentity(row) {
    var variantId = String(row["Variant ID"] || "").trim();
    if (variantId) return "variant:" + normalizeKey(variantId);
    return [
        row["Product Type"] || "",
        row["Primary (Hex)"] || "",
        row["Secondary (Hex)"] || "",
        row["CTA (Hex)"] || "",
        row["Background (Hex)"] || "",
        row["Notes"] || ""
    ].map(normalizeKey).join("|");
}

function addColorRows(target, seen, rows) {
    if (!rows || !rows.length) return;
    for (var i = 0; i < rows.length; i++) {
        var row = rows[i];
        if (!row || !row["Product Type"]) continue;
        var key = colorRowIdentity(row);
        if (!key || seen[key]) continue;
        seen[key] = true;
        target.push(row);
    }
}

function uniqueColorChoiceCount(rows) {
    var seen = {};
    var count = 0;
    for (var i = 0; i < rows.length; i++) {
        var key = colorRowIdentity(rows[i]);
        if (!key || seen[key]) continue;
        seen[key] = true;
        count++;
    }
    return count;
}

function addColorFallbackTypes(target, seen, types) {
    for (var i = 0; i < types.length; i++) {
        var type = types[i];
        if (!type || seen[type]) continue;
        seen[type] = true;
        target.push(type);
    }
}

function colorFallbackContextText(category, brief, queryTerms) {
    return [
        category || "",
        brief && brief.verticalText ? brief.verticalText : "",
        brief && brief.verticalKeywords ? brief.verticalKeywords : "",
        brief && brief.siteType ? brief.siteType : "",
        brief && brief.commerceModel ? brief.commerceModel : "",
        brief && brief.tone ? brief.tone.join(" ") : "",
        queryTerms && queryTerms.raw ? queryTerms.raw : "",
        queryTerms && queryTerms.verticalTerms ? queryTerms.verticalTerms : "",
        queryTerms && queryTerms.siteTypeTerms ? queryTerms.siteTypeTerms : "",
        queryTerms && queryTerms.toneTerms ? queryTerms.toneTerms : ""
    ].join(" ").toLowerCase();
}

function buildColorFallbackProductTypes(category, brief, queryTerms, classification) {
    var types = [];
    var seen = {};
    addColorFallbackTypes(types, seen, [category]);

    var topCandidates = classification && classification.topCandidates ? classification.topCandidates : [];
    for (var i = 0; i < topCandidates.length; i++) {
        addColorFallbackTypes(types, seen, [topCandidates[i].productType]);
    }

    var contextText = colorFallbackContextText(category, brief, queryTerms);
    var commerceContext = (brief && /^ecommerce-/.test(String(brief.siteType || "")))
        || isCommerceLike(contextText, category);
    var premiumContext = /(premium|luxury|exclusive|elegant|sophisticated|high-end|高端|高级|奢|精致)/.test(contextText);
    var playfulContext = /(playful|vibrant|pet|dog|cat|toy|cute|宠物|狗|猫|玩具|可爱)/.test(contextText);
    var naturalContext = /(natural|organic|fresh|botanical|green|garden|鲜花|花店|花束|植物|自然|清新|有机)/.test(contextText);

    if (commerceContext) addColorFallbackTypes(types, seen, COMMERCE_COLOR_FALLBACK_TYPES);
    if (premiumContext) addColorFallbackTypes(types, seen, PREMIUM_COLOR_FALLBACK_TYPES);
    if (playfulContext) addColorFallbackTypes(types, seen, PLAYFUL_COLOR_FALLBACK_TYPES);
    if (naturalContext) addColorFallbackTypes(types, seen, NATURAL_COLOR_FALLBACK_TYPES);

    return types;
}

function buildColorCandidatePool(category, brief, queryTerms, colorResults, classification) {
    var exact = getColorVariantsForProductType(category);
    if (uniqueColorChoiceCount(exact) >= COLOR_VARIANT_POOL_TARGET) {
        var exactFiltered = filterColorCandidates(exact, brief);
        return uniqueColorChoiceCount(exactFiltered) >= 2 ? exactFiltered : exact;
    }

    var candidates = [];
    var seen = {};
    addColorRows(candidates, seen, exact);

    var fallbackTypes = buildColorFallbackProductTypes(category, brief, queryTerms, classification);
    for (var i = 0; i < fallbackTypes.length; i++) {
        addColorRows(candidates, seen, getColorVariantsForProductType(fallbackTypes[i]));
    }

    var filtered = filterColorCandidates(candidates, brief);
    var filteredVariants = rowsWithColorVariantId(filtered);
    if (uniqueColorChoiceCount(filteredVariants) >= 2) {
        var preferred = [];
        var preferredSeen = {};
        addColorRows(preferred, preferredSeen, exact);
        addColorRows(preferred, preferredSeen, filteredVariants);
        return preferred;
    }
    if (uniqueColorChoiceCount(filtered) >= 2) return filtered;

    addColorRows(candidates, seen, colorResults || []);
    filtered = filterColorCandidates(candidates, brief);
    filteredVariants = rowsWithColorVariantId(filtered);
    if (uniqueColorChoiceCount(filteredVariants) >= 2) {
        var searchedPreferred = [];
        var searchedPreferredSeen = {};
        addColorRows(searchedPreferred, searchedPreferredSeen, exact);
        addColorRows(searchedPreferred, searchedPreferredSeen, filteredVariants);
        return searchedPreferred;
    }
    if (uniqueColorChoiceCount(filtered) >= 2) return filtered;
    return candidates.length ? candidates : exact;
}

function filterColorCandidates(rows, brief) {
    if (!rows || !rows.length) return [];
    var preference = brief && brief.palettePreference ? normalizeKey(brief.palettePreference) : "";
    var familyCompatible = [];
    var i;
    for (i = 0; i < rows.length; i++) {
        var row = rows[i];
        if (preference && normalizeKey(row["Palette Family"] || "") !== preference) continue;
        familyCompatible.push(row);
    }
    return familyCompatible.length ? familyCompatible : rows.slice();
}

function colorToneMatchCount(row, brief) {
    var tone = brief && brief.tone ? brief.tone : [];
    if (!tone.length) return 0;
    var toneTags = splitList(row["Tone Tags"] || "").map(normalizeKey);
    var matches = 0;
    for (var i = 0; i < tone.length; i++) {
        var toneKey = normalizeKey(tone[i]);
        if (toneKey && toneTags.indexOf(toneKey) >= 0) {
            matches++;
        }
    }
    return matches;
}

function colorVariantWeight(row, brief) {
    var base = Number(row.Weight) || 1;
    var toneMatches = colorToneMatchCount(row, brief);
    if (!brief || !brief.tone || !brief.tone.length) return base;
    return base * (toneMatches ? (2 + toneMatches) : 0.75);
}

function pickColorVariant(candidates, seed, brief) {
    if (!candidates || !candidates.length) return {};
    if (brief && brief.palettePreference) {
        var pref = normalizeKey(brief.palettePreference);
        var i;
        for (i = 0; i < candidates.length; i++) {
            if (normalizeKey(candidates[i]["Palette Family"] || "") === pref) return candidates[i];
        }
    }
    if (candidates.length === 1) return candidates[0];
    var totalWeight = 0;
    var weighted = [];
    var i;
    for (i = 0; i < candidates.length; i++) {
        var w = colorVariantWeight(candidates[i], brief);
        weighted.push({ row: candidates[i], weight: w });
        totalWeight += w;
    }
    var target = ((stableHash(String(seed || "") + "|color-palette") % 1000000) / 1000000) * totalWeight;
    var cumulative = 0;
    for (i = 0; i < weighted.length; i++) {
        cumulative += weighted[i].weight;
        if (target < cumulative) return weighted[i].row;
    }
    return weighted[0].row;
}

function pickColorForDesignSystem(category, brief, queryTerms, colorResults, seed, classification) {
    var candidates = buildColorCandidatePool(category, brief, queryTerms, colorResults, classification);
    var row = pickColorVariant(candidates, seed, brief);
    if (!row || !row["Product Type"]) {
        row = queryTerms.toneTerms ? selectColorForTone(colorResults, queryTerms.toneTerms) : (colorResults[0] || {});
    }
    return {
        row: row,
        paletteVariantId: String(row["Variant ID"] || "").trim()
    };
}

function buildHeroFacets(brief, ds, pageName) {
    var query = ds && ds.queryTerms ? ds.queryTerms.raw : "";
    var category = ds ? ds.category : "";
    var siteType = brief ? brief.siteType : inferSiteTypeFromLegacyQuery(query, category);
    var commerceModel = brief ? String(brief.commerceModel || "") : "";
    var page = String(pageName || "").toLowerCase();
    var commerceLike = /^ecommerce-/.test(siteType) || isCommerceLike(query, category);
    var conversionGoal = "awareness";
    if (/checkout|online.checkout|online-checkout|cart|下单/.test(commerceModel)) conversionGoal = "purchase";
    else if (commerceLike && page === "home") conversionGoal = "purchase";
    else if (/saas|landing|leadgen/.test(siteType)) conversionGoal = "signup";
    return {
        pagePurpose: /^ecommerce-/.test(siteType) && page === "home" ? "commerce-home" : (page + "-page"),
        conversionGoal: conversionGoal,
        contentEmphasis: commerceLike ? "product" : "story",
        mediaEmphasis: brief && brief.creativeDirection && /editorial|romantic|premium/.test(brief.creativeDirection) ? "photography" : "photography",
        visualTone: brief && brief.tone ? brief.tone.slice(0, 2) : [],
        creativeDirection: brief && brief.creativeDirection ? brief.creativeDirection : "",
        density: "medium"
    };
}

function buildHeroSearchQueryFromFacets(facets, ds, sectionType, pageName, query) {
    var parts = [sectionType, pageName, query, ds.category, ds.pattern.name, ds.style.name, ds.style.keywords];
    if (facets) {
        parts.push(facets.pagePurpose, facets.conversionGoal, facets.contentEmphasis, facets.mediaEmphasis, facets.creativeDirection);
        parts = parts.concat(facets.visualTone || []);
        if (facets.conversionGoal === "purchase" || facets.contentEmphasis === "product" || facets.pagePurpose === "commerce-home") {
            parts.push(COMMERCE_HERO_QUERY_TERMS);
        }
    }
    return parts.filter(function (p) { return !!p; }).join(" ");
}

function dedupeHeroCandidatesByTopology(rescored, maxTopologies) {
    var byTopology = {};
    var i;
    for (i = 0; i < rescored.length; i++) {
        var topo = desktopTopologyOf(rescored[i].row);
        if (!byTopology[topo] || byTopology[topo].score < rescored[i].score) {
            byTopology[topo] = Object.assign({}, rescored[i], { desktopTopology: topo });
        }
    }
    var list = [];
    for (var k in byTopology) {
        if (Object.prototype.hasOwnProperty.call(byTopology, k)) list.push(byTopology[k]);
    }
    list.sort(function (a, b) { return b.score - a.score; });
    return list.slice(0, maxTopologies || 3);
}

function buildHeroTopologyRescoredPool(ranked, filtered, context, topBm25, filteredOut) {
    var topologyBest = {};
    var usedArchetypes = context.usedArchetypesOnPage || [];
    var usedTopologies = context.usedDesktopTopologiesOnPage || [];
    var i;
    for (i = 0; i < ranked.length; i++) {
        if (ranked[i][1] <= 0) continue;
        var idx = ranked[i][0];
        var row = filtered[idx];
        var bm25 = ranked[i][1];
        var topology = desktopTopologyOf(row);
        if (!topologyBest[topology] || topologyBest[topology].bm25 < bm25) {
            topologyBest[topology] = { idx: idx, bm25: bm25, row: row, topology: topology };
        }
    }
    var winners = [];
    for (var k in topologyBest) {
        if (Object.prototype.hasOwnProperty.call(topologyBest, k)) winners.push(topologyBest[k]);
    }
    winners.sort(function (a, b) { return b.bm25 - a.bm25; });
    winners = winners.slice(0, HERO_TOPOLOGY_TARGET);

    var rescored = [];
    for (i = 0; i < winners.length; i++) {
        var w = winners[i];
        var row = w.row;
        var bm25 = w.bm25;
        var minScore = topBm25 * HERO_SCORE_THRESHOLD_RATIO;
        if (bm25 < minScore) {
            filteredOut.push({ promptId: row["Prompt ID"] || "", reason: "below-hero-topology-floor" });
            continue;
        }
        var bonus = scorePromptRowBonus(row, context.ds, context.pageName, "hero", context.heroFacets, context);
        var archetype = layoutArchetypeOf(row);
        var diversityPenalty = 0;
        var diversityReason = "";
        var u;
        for (u = 0; u < usedTopologies.length; u++) {
            if (usedTopologies[u] && usedTopologies[u] === w.topology) {
                diversityPenalty = 1.2;
                diversityReason = "duplicate-topology-on-page";
                break;
            }
        }
        if (!diversityPenalty) {
            for (u = 0; u < usedArchetypes.length; u++) {
                if (usedArchetypes[u] && usedArchetypes[u] === archetype) {
                    diversityPenalty = 0.8;
                    diversityReason = "duplicate-archetype-on-page";
                    break;
                }
            }
        }
        rescored.push({
            idx: w.idx,
            score: bm25 + bonus - diversityPenalty,
            bm25: bm25,
            bonus: bonus,
            diversityPenalty: diversityPenalty,
            diversityReason: diversityReason,
            archetype: archetype,
            desktopTopology: w.topology,
            row: row
        });
    }
    return rescored;
}

function appendSelectionContractMeta(meta, row) {
    return Object.assign({}, meta, buildSelectionContractFields(row));
}

function isSharedBlockScope(scope) {
    return scope === "shared";
}

function normalizeLocaleKey(locale) {
    return String(locale || "").trim().toLowerCase();
}

function validateBriefContentLocale(brief) {
    var argLocale = getArg("contentLocale", "");
    if (argLocale && normalizeLocaleKey(argLocale) !== normalizeLocaleKey(brief.contentLocale)) {
        return "contentLocale argument (" + argLocale + ") conflicts with brief.contentLocale (" + brief.contentLocale + ")";
    }
    return "";
}

function resolveContentLocale() {
    var locale = getArg("contentLocale", "");
    if (!locale) return null;
    return normalizeContentLocale(locale) || null;
}

function resolvePromptText(row, contentLocale) {
    var isEn = /^en/i.test(String(contentLocale || ""));
    var primary = isEn ? row["Prompt EN"] : row["Prompt ZH"];
    var fallback = isEn ? row["Prompt ZH"] : row["Prompt EN"];
    var fallbackUsed = false;
    if (!String(primary || "").trim()) {
        primary = fallback;
        fallbackUsed = true;
    }
    return {
        locale: isEn ? "en" : "zh",
        requestedLocale: contentLocale,
        text: String(primary || "").trim(),
        promptLocaleFallback: fallbackUsed
    };
}

function buildPromptSearchDoc(row) {
    return [
        row["Section Type"],
        row["Prompt ID"],
        row["Name"],
        row["Keywords"],
        row["Style Tags"],
        row["Best For"],
        row["Layout Archetype"],
        row["Content Hierarchy"],
        row["Density"],
        row["Conversion Goal"],
        row["Visual Treatment"]
    ].join(" ");
}

function filterCompatibleCandidates(rows, ds) {
    if (!rows || !rows.length) return [];
    var styleKey = normalizeStyleKey((ds && ds.style && ds.style.name) || "");
    if (!styleKey) return rows.slice();
    var compatible = [];
    var i;
    for (i = 0; i < rows.length; i++) {
        var styles = splitList(rows[i]["Compatible Styles"]);
        if (!styles.length) {
            compatible.push(rows[i]);
            continue;
        }
        var ok = false;
        var j;
        for (j = 0; j < styles.length; j++) {
            if (normalizeStyleKey(styles[j]) === styleKey) {
                ok = true;
                break;
            }
        }
        if (ok) compatible.push(rows[i]);
    }
    // Style compatibility is a soft preference. A tiny compatible set can
    // otherwise collapse an ecommerce header/hero into a local-service prompt.
    return compatible.length >= Math.min(3, rows.length) ? compatible : rows.slice();
}

function requiredContentKeys(row) {
    return splitList(row["Required Content"] || "").map(normalizeKey);
}

function hasRequiredContentKey(keys, key) {
    return keys.indexOf(normalizeKey(key)) >= 0;
}

function filterSharedChromeCandidates(rows, sectionType, ds) {
    if (!rows || !rows.length) return [];
    var filtered = [];
    var i;
    for (i = 0; i < rows.length; i++) {
        var keys = requiredContentKeys(rows[i]);
        if (sectionType === "header") {
            if (hasRequiredContentKey(keys, "brandName") && hasRequiredContentKey(keys, "navItems")) {
                filtered.push(rows[i]);
            }
            continue;
        }
        if (sectionType === "footer") {
            var hasLegal = hasRequiredContentKey(keys, "legalLinks");
            var hasNavigation = hasRequiredContentKey(keys, "navGroups")
                || hasRequiredContentKey(keys, "productLinks")
                || hasRequiredContentKey(keys, "companyLinks");
            if (!hasLegal || !hasNavigation) continue;
            if (
                rows[i]["Prompt ID"] === "footer.prompt.saas-links"
                && !isSaasLike(ds && ds.queryTerms ? ds.queryTerms.raw : "", ds ? ds.category : "")
            ) {
                continue;
            }
            filtered.push(rows[i]);
        }
    }
    return filtered.length ? filtered : rows.slice();
}

function filterHeroCandidatesByGoal(rows, heroFacets) {
    if (!rows || !rows.length || !heroFacets || !heroFacets.conversionGoal) return rows || [];
    var expectedGoal = normalizeKey(heroFacets.conversionGoal);
    var matched = [];
    var topologies = {};
    var i;
    for (i = 0; i < rows.length; i++) {
        if (normalizeKey(rows[i]["Conversion Goal"] || "") !== expectedGoal) continue;
        matched.push(rows[i]);
        topologies[desktopTopologyOf(rows[i])] = true;
    }
    // Keep goal matching only when it still leaves a real diversity pool.
    return matched.length >= 2 && Object.keys(topologies).length >= 2 ? matched : rows;
}

function buildPromptSeedKey(context) {
    if (context.isShared) {
        return [context.variantSeed || "", context.sectionType || ""].join("|");
    }
    return [
        context.variantSeed || "",
        context.pageName || "",
        context.sectionType || "",
        String(context.occurrence || 0)
    ].join("|");
}

function seedPickRankDecayForContext(context) {
    var brief = context && context.brief;
    var isOpenBrief = brief && (!brief.tone || !brief.tone.length);
    if (!isOpenBrief) return SEED_PICK_RANK_DECAY;
    return (context.isShared || context.sectionType === "hero")
        ? OPEN_BRIEF_SEED_PICK_RANK_DECAY
        : OPEN_BRIEF_CONTENT_SEED_PICK_RANK_DECAY;
}

var SITE_TYPE_TERMS = {
    ecommerce: true, commerce: true, shop: true, store: true, retail: true,
    "电商": true, "商城": true, "商店": true, "店铺": true, "网店": true,
    "售卖": true, "销售": true, "卖货": true, "商品": true, "购物": true,
    "购买": true, "下单": true, "零售": true,
    saas: true, portfolio: true, service: true, marketplace: true, catalog: true,
    dashboard: true, blog: true, landing: true
};

var TONE_TERMS = {
    elegant: true, romantic: true, luxury: true, premium: true, minimal: true,
    modern: true, fresh: true, natural: true, professional: true, playful: true,
    bold: true, warm: true, clean: true, exclusive: true, sophisticated: true,
    gifts: true, gift: true
};

function parseQueryTerms(query) {
    var tokens = tokenize(query);
    var siteType = [];
    var tone = [];
    var vertical = [];
    var i;
    for (i = 0; i < tokens.length; i++) {
        var t = tokens[i];
        if (SITE_TYPE_TERMS[t]) siteType.push(t);
        else if (TONE_TERMS[t]) tone.push(t);
        else vertical.push(t);
    }
    return {
        raw: String(query || ""),
        verticalTerms: vertical.join(" "),
        siteTypeTerms: siteType.join(" "),
        toneTerms: tone.slice(0, 2).join(" ")
    };
}

function buildProductSearchQuery(terms) {
    var parts = [terms.verticalTerms, terms.toneTerms].filter(function (p) { return !!p; });
    return parts.join(" ").trim() || terms.raw;
}

function buildColorSearchQuery(terms, reasoning, product) {
    return [
        terms.verticalTerms,
        terms.toneTerms,
        toneColorMood(terms.toneTerms),
        reasoning.colorMood,
        product["Color Palette Focus"] || "",
        product["Product Type"] || ""
    ].filter(function (p) { return !!p; }).join(" ");
}

function buildTypographySearchQuery(terms, reasoning, product, styleName) {
    return [
        terms.verticalTerms,
        terms.toneTerms,
        toneTypographyMood(terms.toneTerms),
        reasoning.typographyMood,
        product["Product Type"] || "",
        styleName || ""
    ].filter(function (p) { return !!p; }).join(" ");
}

function toneColorMood(toneTerms) {
    var t = String(toneTerms || "").toLowerCase();
    if (/premium|luxury|exclusive|sophisticated/.test(t)) return "Black + Gold premium elegant";
    if (/natural|organic|fresh|warm/.test(t)) return "Natural Green earth floral tones";
    if (/modern|clean|minimal|professional/.test(t)) return "Professional clean neutral";
    if (/playful|vibrant|bold/.test(t)) return "Vibrant energetic accent";
    return "";
}

function toneTypographyMood(toneTerms) {
    var t = String(toneTerms || "").toLowerCase();
    if (/premium|luxury|exclusive|elegant/.test(t)) return "Elegant refined luxury serif";
    if (/natural|organic|fresh/.test(t)) return "Natural humanist friendly";
    if (/modern|clean|minimal/.test(t)) return "Modern geometric sans";
    if (/playful|bold/.test(t)) return "Expressive display friendly";
    return "";
}

var VALID_SITE_TYPES = {
    "ecommerce-catalog": true,
    "ecommerce-luxury": true,
    "ecommerce-florist": true,
    "local-service": true,
    "portfolio-creative": true,
    "saas-product": true,
    "content-publication": true,
    "landing-leadgen": true,
    "general-multipage": true
};

var CONTENT_LOCALE_ALIASES = {
    "zh": "zh",
    "zh-cn": "zh",
    "en": "en",
    "en-us": "en"
};

var PRODUCT_CLASSIFY_SEARCH_COLS = ["Product Type", "Keywords"];
var MIN_KEYWORD_HIT_RATIO = 0.34;

var CLASSIFICATION_STOP_WORDS = {
    modern: true, elegant: true, romantic: true, luxury: true, premium: true,
    minimal: true, natural: true, fresh: true, professional: true, playful: true,
    bold: true, warm: true, clean: true, exclusive: true, sophisticated: true
};

function normalizeContentLocale(locale) {
    var normalized = normalizeLocaleKey(locale);
    return CONTENT_LOCALE_ALIASES[normalized] || "";
}

function isValidContentLocale(locale) {
    return !!normalizeContentLocale(locale);
}

function normalizeClassificationToken(token) {
    // Normalize ordinary plurals without accepting arbitrary substrings.
    return /^[a-z]{4,}s$/.test(token) && !/ss$/.test(token)
        ? token.slice(0, -1) : token;
}

function verticalTokensForClassification(query) {
    var tokens = tokenize(query);
    var vertical = [];
    for (var i = 0; i < tokens.length; i++) {
        var t = normalizeClassificationToken(tokens[i]);
        if (SITE_TYPE_TERMS[t] || CLASSIFICATION_STOP_WORDS[t] || t.length < 2) continue;
        if (vertical.indexOf(t) < 0) vertical.push(t);
    }
    return vertical;
}

function keywordListFromProductRow(row) {
    return verticalTokensForClassification(String(row.Keywords || "") + " " + String(row["Product Type"] || ""));
}

function countKeywordHits(queryTokens, productRow) {
    var keywordList = keywordListFromProductRow(productRow);
    var hits = 0;
    for (var i = 0; i < queryTokens.length; i++) {
        if (keywordList.indexOf(queryTokens[i]) >= 0) hits++;
    }
    return hits;
}

function keywordHitRatio(queryTokens, productRow) {
    if (!queryTokens.length) return 0;
    return countKeywordHits(queryTokens, productRow) / queryTokens.length;
}

function findProductByType(productType) {
    var rows = getParsedCsvRows("products.csv");
    var i;
    for (i = 0; i < rows.length; i++) {
        if (String(rows[i]["Product Type"]).trim().toLowerCase() === String(productType).trim().toLowerCase()) return rows[i];
    }
    return null;
}

function normalizeBriefPageName(pageName) {
    var value = String(pageName || "").trim();
    if (!value) return "";
    var lower = value.toLowerCase().replace(/^https?:\/\/[^/]+/i, "");
    lower = lower.split("?")[0].split("#")[0].replace(/^\/+|\/+$/g, "");
    if (!lower || lower === "index" || lower === "home" || lower === "homepage") return "home";
    var aliases = {
        product: "products",
        shop: "products",
        store: "products",
        catalog: "products",
        collection: "collections",
        collections: "collections",
        detail: "product-detail",
        "product-detail": "product-detail",
        "product-details": "product-detail",
        basket: "cart",
        cart: "cart",
        checkout: "checkout",
        account: "account",
        "my-account": "account",
        "customer-account": "account",
        orders: "orders",
        "order-history": "orders",
        "order-detail": "order-detail",
        "order-details": "order-detail",
        "order-result": "order-result",
        "order-success": "order-result",
        "order-confirmation": "order-result",
        "thank-you": "order-result",
        login: "login",
        signin: "login",
        register: "register",
        signup: "register",
        about: "about",
        contact: "contact",
        services: "services",
        service: "services",
        pricing: "pricing",
        portfolio: "portfolio",
        articles: "articles",
        blog: "articles",
        article: "article-detail",
        "article-detail": "article-detail",
        "article-details": "article-detail",
        "blog-detail": "article-detail",
        "post-detail": "article-detail",
        subscribe: "subscribe"
    };
    return aliases[lower] || normalizeKey(lower);
}

function hasExplicitCheckoutIntent(raw, pages) {
    var requestedPages = Array.isArray(pages) ? pages : [];
    if (requestedPages.some(function (page) {
        return page === "cart" || page === "checkout" || page === "order-result";
    })) return true;

    var intentText = [
        raw && raw.verticalText,
        raw && raw.verticalKeywords
    ].filter(Boolean).join(" ");
    return /(?:购物车|结算|在线下单|在线结账|在线支付|立即下单|直接下单|\bshopping\s+cart\b|\bcart\b|\bcheckout\b|\bonline\s+(?:order|checkout|payment)\b|\bpayment\b)/i.test(intentText);
}

function resolveBriefCommerceModel(raw, siteType, pages, submittedModel) {
    if (!/^ecommerce-/.test(siteType)) return submittedModel;
    if (hasExplicitCheckoutIntent(raw, pages)) return "online-checkout";

    var isResolvedBriefReplay = !!String(raw.designVariantSeed || "").trim()
        && !!String(raw.projectName || "").trim();
    if (isResolvedBriefReplay && (submittedModel === "catalog-contact" || submittedModel === "online-checkout")) {
        return submittedModel;
    }
    return "catalog-contact";
}

function getCommerceModelDecisionSource(raw, parsed) {
    if (!raw || !parsed || !/^ecommerce-/.test(parsed.siteType)) return "";
    var submittedModel = String(raw.commerceModel || "").trim();
    if (submittedModel === parsed.commerceModel) return "";
    return parsed.commerceModel === "online-checkout"
        ? "explicit-transaction-intent"
        : "catalog-default-policy";
}

// References select existing plan items by identity. Object metadata never
// overrides the plan. Each list uses one representation and preserves order.
function parsePlanReferences(value, field, key, normalize, allowEmpty) {
    if (!Array.isArray(value) || (!allowEmpty && !value.length)) {
        return { error: field + " must be " + (allowEmpty ? "an" : "a non-empty") + " array of strings or plan reference objects" };
    }
    var refs = [];
    var objectList = value.length && typeof value[0] === "object";
    for (var i = 0; i < value.length; i++) {
        var item = value[i];
        var ref = objectList && item && typeof item === "object" && !Array.isArray(item)
            ? item[key] : (objectList ? null : item);
        if (typeof ref !== "string" || !ref.trim()) {
            return { error: field + "[" + i + "] must be " + (objectList ? "an object with a non-empty " + key : "a non-empty string") + "; use one representation per list" };
        }
        ref = normalize(ref);
        if (!ref || refs.indexOf(ref) >= 0) {
            return { error: field + "[" + i + "] must identify a unique non-empty reference" };
        }
        refs.push(ref);
    }
    return { values: refs };
}

function parseSiteBrief(brief) {
    if (!brief) return null;
    if (typeof brief !== "object" || Array.isArray(brief)) return { error: "brief must be an object" };
    var raw = brief;

    var pageRefs = parsePlanReferences(raw.pages === undefined ? [] : raw.pages,
        "Site Brief pages", "name", normalizeBriefPageName, true);
    if (pageRefs.error) return { error: pageRefs.error + ". Reuse page-plan.resolvedBrief, including its pages and designVariantSeed." };

    var verticalText = String(raw.verticalText || "").trim();
    if (typeof raw.verticalKeywords !== "string") return { error: "Site Brief verticalKeywords must be a string" };
    var verticalKeywords = raw.verticalKeywords.trim();
    var siteType = String(raw.siteType || "").trim();
    var tone = Array.isArray(raw.tone)
        ? raw.tone.map(function (v) { return String(v).trim(); }).filter(Boolean)
        : (raw.tone ? [String(raw.tone).trim()] : []);
    var themeMode = raw.themeMode === "dark" ? "dark" : (raw.themeMode === "light" ? "light" : "");
    var pages = pageRefs.values;
    var contentLocale = normalizeContentLocale(raw.contentLocale);
    var confirmedCategory = String(raw.confirmedCategory || raw.categoryOverride || "").trim();
    var designVariantSeed = String(raw.designVariantSeed || "").trim();
    var creativeDirection = String(raw.creativeDirection || "").trim();
    var palettePreference = String(raw.palettePreference || "").trim();
    var submittedCommerceModel = String(raw.commerceModel || "").trim();
    if (/^ecommerce-/.test(siteType) && submittedCommerceModel &&
        submittedCommerceModel !== "catalog-contact" && submittedCommerceModel !== "online-checkout") {
        return { error: "Commerce Site Brief requires commerceModel (catalog-contact|online-checkout)" };
    }
    var commerceModel = resolveBriefCommerceModel(raw, siteType, pages, submittedCommerceModel);

    if (!verticalText) return { error: "Site Brief requires verticalText" };
    if (!verticalKeywords) return { error: "Site Brief requires verticalKeywords" };
    if (!siteType || !VALID_SITE_TYPES[siteType]) return { error: "Site Brief requires valid siteType" };
    if (tone.length > 2) return { error: "Site Brief requires tone (0-2 items)" };
    if (!themeMode) return { error: "Site Brief requires themeMode (dark|light)" };
    if (!contentLocale) return { error: "Site Brief requires contentLocale (zh|en)" };
    if (confirmedCategory) {
        var confirmedProduct = findProductByType(confirmedCategory);
        if (!confirmedProduct) {
            return { error: "Site Brief confirmedCategory must use a Product Type returned in classification.category or topCandidates[].productType, not siteType. Received: " + confirmedCategory + ". Reuse resolvedBrief unchanged unless the user explicitly confirmed a category." };
        }
        confirmedCategory = confirmedProduct["Product Type"];
    }
    if (/^ecommerce-/.test(siteType) && commerceModel !== "catalog-contact" && commerceModel !== "online-checkout") {
        return { error: "Commerce Site Brief requires commerceModel (catalog-contact|online-checkout)" };
    }
    if (!/^ecommerce-/.test(siteType) && commerceModel) {
        return { error: "commerceModel is only valid for ecommerce siteType" };
    }

    return {
        verticalText: verticalText,
        verticalKeywords: verticalKeywords,
        siteType: siteType,
        tone: tone,
        themeMode: themeMode,
        audience: String(raw.audience || "").trim(),
        projectName: String(raw.projectName || "").trim(),
        commerceModel: commerceModel,
        pages: pages,
        contentLocale: contentLocale,
        confirmedCategory: confirmedCategory,
        designVariantSeed: designVariantSeed,
        creativeDirection: creativeDirection,
        palettePreference: palettePreference
    };
}

var PROJECT_NAME_ZH_LEFT = ["澄", "栖", "岚", "沐", "叙", "砚", "予", "汐", "弦", "朔", "棠", "隽"];
var PROJECT_NAME_ZH_RIGHT = ["屿", "川", "序", "庭", "光", "野", "集", "舍", "岸", "谷", "原", "间"];
var PROJECT_NAME_EN_LEFT = ["Ave", "Eli", "Luma", "Nori", "Sora", "Vela", "Ceri", "Mira"];
var PROJECT_NAME_EN_RIGHT = ["lune", "vara", "nora", "velle", "rien", "sio", "mera", "tide"];

function resolveProjectName(explicitName, brief, seed) {
    var explicit = String(explicitName || "").trim();
    if (explicit) return explicit;
    var hash = stableHash(String(seed || "project"));
    var locale = brief && brief.contentLocale ? String(brief.contentLocale).toLowerCase() : "";
    if (locale === "zh") {
        return PROJECT_NAME_ZH_LEFT[hash % PROJECT_NAME_ZH_LEFT.length]
            + PROJECT_NAME_ZH_RIGHT[Math.floor(hash / PROJECT_NAME_ZH_LEFT.length) % PROJECT_NAME_ZH_RIGHT.length];
    }
    return PROJECT_NAME_EN_LEFT[hash % PROJECT_NAME_EN_LEFT.length]
        + PROJECT_NAME_EN_RIGHT[Math.floor(hash / PROJECT_NAME_EN_LEFT.length) % PROJECT_NAME_EN_RIGHT.length];
}

function buildBriefSearchContext(brief) {
    return [
        brief.verticalKeywords,
        (brief.tone || []).slice(0, 2).join(" "),
        brief.creativeDirection || ""
    ].filter(function (p) { return !!p; }).join(" ").trim();
}

function buildBriefQueryTerms(brief) {
    var toneTerms = (brief.tone || []).slice(0, 2).join(" ");
    return {
        raw: buildBriefSearchContext(brief),
        verticalTerms: brief.verticalKeywords,
        siteTypeTerms: brief.siteType,
        toneTerms: toneTerms
    };
}

function stableHash(str) {
    var h = 0;
    var s = String(str || "");
    var i;
    for (i = 0; i < s.length; i++) {
        h = ((h << 5) - h) + s.charCodeAt(i);
        h |= 0;
    }
    return Math.abs(h);
}

function scoreDiscoveryHeroTopologyBias(row, pageName, heroFacets, brief, ds) {
    var page = String(pageName || "").toLowerCase();
    if (page && page !== "home") return 0;

    var siteType = String(
        (brief && brief.siteType) ||
        (ds && ds.siteType) ||
        (heroFacets && heroFacets.pagePurpose === "commerce-home" ? "ecommerce-catalog" : "") ||
        ""
    ).toLowerCase();
    var bias = "";
    if (/^ecommerce-/.test(siteType) || (heroFacets && heroFacets.pagePurpose === "commerce-home")) bias = "shop";
    else if (siteType === "content-publication") bias = "mag";
    else if (siteType === "local-service") bias = "hub";
    if (!bias) return 0;

    var topology = desktopTopologyOf(row);
    var family = heroTopologyFamilyOf(topology);

    if (bias === "shop") {
        if (
            family === "immersive-media" ||
            family === "commerce-discovery" ||
            topology === "fullbleed" ||
            topology === "media-collage" ||
            topology === "masonry-gallery" ||
            topology === "collection-strip" ||
            topology === "product-shelf-stage" ||
            topology === "editorial-product-journal" ||
            topology === "featured-item-stage" ||
            topology === "local-market-stage" ||
            topology === "category-bento"
        ) {
            return 2.4;
        }
        if (family === "split") return -2.8;
        if (topology === "typography-stack" || topology === "product-mock-stage" || topology === "illustration-stage") {
            return -1.0;
        }
        return 0;
    }

    if (bias === "mag") {
        if (
            family === "immersive-media" ||
            family === "typography" ||
            topology === "masonry-gallery" ||
            topology === "media-collage" ||
            topology === "cinematic-reel" ||
            topology === "media-strip"
        ) {
            return 2.2;
        }
        if (family === "split") return -2.4;
        return 0;
    }

    // hub / local-service discovery
    if (
        family === "commerce-discovery" ||
        topology === "search-overlay" ||
        topology === "calendar-stage" ||
        topology === "booking-dock" ||
        topology === "marketplace-grid" ||
        topology === "proof-card-stage"
    ) {
        return 2.4;
    }
    if (family === "split") return -2.2;
    if (topology === "typography-stack") return -1.0;
    return 0;
}

function scorePromptRowBonus(row, ds, pageName, sectionType, heroFacets, context) {
    var bonus = 0;
    var tags = String(row["Style Tags"] || "").toLowerCase();
    var keywords = String(row["Keywords"] || "").toLowerCase();
    var styleName = normalizeStyleKey((ds && ds.style && ds.style.name) || "");
    var category = String((ds && ds.category) || "").toLowerCase();
    var page = String(pageName || "").toLowerCase();
    var compat = splitList(row["Compatible Styles"]);
    var i;

    if (styleName) {
        for (i = 0; i < compat.length; i++) {
            if (normalizeStyleKey(compat[i]) === styleName) {
                bonus += 1.8;
                break;
            }
        }
        if (tags.indexOf(styleName) >= 0) bonus += 1.2;
    }
    if (category && keywords.indexOf(category.split("/")[0]) >= 0) bonus += 0.8;
    if (page === "home" && sectionType === "hero" && tags.indexOf("hero-centric") >= 0) bonus += 0.5;
    if (page === "products" && sectionType === "product-grid" && tags.indexOf("catalog") >= 0) bonus += 0.5;
    if (isCommerceLike(ds.queryTerms ? ds.queryTerms.raw : "", category) && tags.indexOf("ecommerce") >= 0) bonus += 0.4;
    if (isSaasLike(ds.queryTerms ? ds.queryTerms.raw : "", category) && tags.indexOf("saas") >= 0) bonus += 0.4;
    if (heroFacets && sectionType === "hero") {
        var goal = String(heroFacets.conversionGoal || "").toLowerCase();
        var rowGoal = normalizeKey(row["Conversion Goal"] || "");
        if (goal && rowGoal && rowGoal.indexOf(goal) >= 0) bonus += 0.6;
        if (heroFacets.contentEmphasis === "product" && keywords.indexOf("product") >= 0) bonus += 0.5;
        if (heroFacets.creativeDirection && keywords.indexOf(String(heroFacets.creativeDirection).split("-")[0]) >= 0) bonus += 0.4;
        bonus += scoreDiscoveryHeroTopologyBias(
            row,
            pageName,
            heroFacets,
            context && context.brief,
            ds
        );
    }
    return bonus;
}

function buildRescoredPromptPool(ranked, filtered, context, options) {
    options = options || {};
    var forRecommendations = !!options.forRecommendations;
    var usedArchetypes = context.usedArchetypesOnPage || [];
    var usedTopologies = context.usedDesktopTopologiesOnPage || [];
    var rescueRatio = context.isShared ? SHARED_CHROME_RESCUE_RATIO : DIVERSITY_RESCUE_RATIO;
    var topBm25 = ranked.length ? ranked[0][1] : 0;
    var rescored = [];
    var belowThreshold = [];
    var filteredOut = [];
    var i;

    if (context.sectionType === "hero" && !forRecommendations) {
        rescored = buildHeroTopologyRescoredPool(ranked, filtered, context, topBm25, filteredOut);
    }

    if (!rescored.length) {
        for (i = 0; i < ranked.length; i++) {
            if (ranked[i][1] <= 0) continue;
            var idx = ranked[i][0];
            var row = filtered[idx];
            var bm25 = ranked[i][1];
            var bonus = scorePromptRowBonus(row, context.ds, context.pageName, context.sectionType, context.heroFacets, context);
            var archetype = layoutArchetypeOf(row);
            var topology = desktopTopologyOf(row);
            var diversityPenalty = 0;
            var diversityReason = "";
            var u;
            for (u = 0; u < usedTopologies.length; u++) {
                if (usedTopologies[u] && usedTopologies[u] === topology) {
                    diversityPenalty = 1.2;
                    diversityReason = "duplicate-topology-on-page";
                    break;
                }
            }
            if (!diversityPenalty) {
                for (u = 0; u < usedArchetypes.length; u++) {
                    if (usedArchetypes[u] && usedArchetypes[u] === archetype) {
                        diversityPenalty = 0.8;
                        diversityReason = "duplicate-archetype-on-page";
                        break;
                    }
                }
            }
            var thresholdRatio;
            if (forRecommendations) {
                thresholdRatio = context.sectionType === "hero" ? HERO_SCORE_THRESHOLD_RATIO : DIVERSITY_RESCUE_RATIO;
            } else if (context.sectionType === "hero") {
                thresholdRatio = HERO_SCORE_THRESHOLD_RATIO;
            } else {
                thresholdRatio = PROMPT_SCORE_THRESHOLD_RATIO;
            }
            if (bm25 < topBm25 * thresholdRatio) {
                filteredOut.push({ promptId: row["Prompt ID"] || "", reason: "below-score-threshold" });
                if (!forRecommendations && bm25 >= topBm25 * rescueRatio) {
                    belowThreshold.push({
                        idx: idx,
                        score: bm25 + bonus - diversityPenalty,
                        bm25: bm25,
                        bonus: bonus,
                        diversityPenalty: diversityPenalty,
                        diversityReason: diversityReason,
                        archetype: archetype,
                        desktopTopology: topology,
                        row: row
                    });
                }
                continue;
            }
            rescored.push({
                idx: idx,
                score: bm25 + bonus - diversityPenalty,
                bm25: bm25,
                bonus: bonus,
                diversityPenalty: diversityPenalty,
                diversityReason: diversityReason,
                archetype: archetype,
                desktopTopology: topology,
                row: row
            });
        }
    }

    if (!forRecommendations && context.sectionType !== "hero" && rescored.length < DIVERSITY_RESCUE_TARGET && belowThreshold.length) {
        belowThreshold.sort(function (a, b) { return b.score - a.score; });
        var r;
        for (r = 0; r < belowThreshold.length && rescored.length < DIVERSITY_RESCUE_TARGET; r++) {
            var rescueCandidate = belowThreshold[r];
            var duplicateArchetype = false;
            for (u = 0; u < rescored.length; u++) {
                if (rescored[u].archetype && rescored[u].archetype === rescueCandidate.archetype) {
                    duplicateArchetype = true;
                    break;
                }
            }
            if (!duplicateArchetype) {
                rescueCandidate.rescued = true;
                rescored.push(rescueCandidate);
            }
        }
    }

    rescored.sort(function (a, b) { return b.score - a.score; });
    if (!forRecommendations && context.sectionType === "hero" && rescored.length > 1) {
        rescored = dedupeHeroCandidatesByTopology(rescored, HERO_TOPOLOGY_TARGET);
    }

    return {
        rescored: rescored,
        belowThreshold: belowThreshold,
        filteredOut: filteredOut,
        topBm25: topBm25,
        rescueRatio: rescueRatio
    };
}

function pickDiverseRecommendationCandidates(rescored, sectionType, limit, excludeSet) {
    var out = [];
    var seenTopology = {};
    var seenPrompt = {};
    var i;

    for (i = 0; i < rescored.length && out.length < limit; i++) {
        var candidate = rescored[i];
        var row = candidate.row || {};
        var promptId = row["Prompt ID"] || "";
        if (!promptId || excludeSet[promptId] || seenPrompt[promptId]) continue;
        if (sectionType === "hero") {
            var topology = candidate.desktopTopology || desktopTopologyOf(row);
            if (seenTopology[topology]) continue;
            seenTopology[topology] = true;
        }
        seenPrompt[promptId] = true;
        out.push(Object.assign({}, candidate, { rank: out.length + 1 }));
    }

    if (sectionType === "hero" && out.length < limit) {
        for (i = 0; i < rescored.length && out.length < limit; i++) {
            var fill = rescored[i];
            var fillId = (fill.row || {})["Prompt ID"] || "";
            if (!fillId || excludeSet[fillId] || seenPrompt[fillId]) continue;
            seenPrompt[fillId] = true;
            out.push(Object.assign({}, fill, { rank: out.length + 1 }));
        }
    }

    return out;
}

function parseExcludePromptIds(raw) {
    if (!raw) return [];
    var text = String(raw).trim();
    if (!text) return [];
    try {
        var parsed = JSON.parse(text);
        if (Array.isArray(parsed)) {
            return parsed.map(function (item) { return String(item || "").trim(); }).filter(Boolean);
        }
    } catch (e1) {}
    return text.split(/[,;]/).map(function (part) { return part.trim(); }).filter(Boolean);
}

function pickPromptCandidate(ranked, filtered, context) {
    var seedKey = buildPromptSeedKey(context);
    var baseMeta = {
        scope: context.scope || "",
        isShared: !!context.isShared,
        seedKey: seedKey
    };

    if (!filtered.length) {
        return {
            row: {},
            selectionReason: "empty-pool",
            candidateCount: 0,
            selectionMeta: baseMeta
        };
    }

    if (!ranked.length || ranked[0][1] <= 0) {
        return {
            row: filtered[0],
            selectionReason: "bm25-fallback",
            candidateCount: filtered.length,
            selectionMeta: Object.assign({}, baseMeta, {
                candidateCount: filtered.length,
                qualifiedCount: 1,
                bm25Score: 0,
                bonus: 0,
                poolSize: 1,
                pickIndex: 0,
                promptId: filtered[0]["Prompt ID"] || "",
                layoutArchetype: layoutArchetypeOf(filtered[0]),
                filteredReasons: ["bm25-zero"]
            })
        };
    }

    var pool = buildRescoredPromptPool(ranked, filtered, context, { forRecommendations: false });
    var rescored = pool.rescored;
    var filteredOut = pool.filteredOut;
    var topBm25 = pool.topBm25;
    var rescueRatio = pool.rescueRatio;

    if (!rescored.length) {
        var fallbackIdx = ranked[0][0];
        return {
            row: filtered[fallbackIdx],
            selectionReason: "threshold-fallback",
            candidateCount: filtered.length,
            selectionMeta: appendSelectionContractMeta(Object.assign({}, baseMeta, {
                candidateCount: filtered.length,
                qualifiedCount: 1,
                bm25Score: ranked[0][1],
                bonus: scorePromptRowBonus(filtered[fallbackIdx], context.ds, context.pageName, context.sectionType, context.heroFacets, context),
                poolSize: 1,
                pickIndex: 0,
                promptId: filtered[fallbackIdx]["Prompt ID"] || "",
                filteredReasons: ["threshold-empty"]
            }), filtered[fallbackIdx])
        };
    }

    var poolSize = rescored.length;
    var totalWeight = 0;
    var rankDecay = seedPickRankDecayForContext(context);
    var i;
    for (i = 0; i < rescored.length; i++) {
        rescored[i].weight = Math.pow(rankDecay, i);
        totalWeight += rescored[i].weight;
    }
    var target = poolSize > 1 ? ((stableHash(seedKey) % 1000000) / 1000000) * totalWeight : 0;
    var pick = 0;
    var cumulativeWeight = 0;
    for (i = 0; i < rescored.length; i++) {
        cumulativeWeight += rescored[i].weight;
        if (target < cumulativeWeight) {
            pick = i;
            break;
        }
    }
    var chosen = rescored[pick];

    return {
        row: chosen.row,
        selectionReason: poolSize > 1 ? "threshold-seed-pick" : "single-qualified-candidate",
        candidateCount: rescored.length,
        selectionMeta: appendSelectionContractMeta(Object.assign({}, baseMeta, {
            candidateCount: filtered.length,
            qualifiedCount: rescored.length,
            topBm25Score: topBm25,
            bm25Score: chosen.bm25,
            bonus: chosen.bonus,
            diversityPenalty: chosen.diversityPenalty,
            diversityReason: chosen.diversityReason || "",
            poolSize: poolSize,
            pickIndex: pick,
            promptId: chosen.row["Prompt ID"] || "",
            rankDecay: rankDecay,
            scoreThresholdRatio: context.sectionType === "hero"
                ? HERO_SCORE_THRESHOLD_RATIO
                : (chosen.rescued ? rescueRatio : PROMPT_SCORE_THRESHOLD_RATIO),
            selectionWeight: chosen.weight,
            totalWeight: totalWeight,
            filteredReasons: filteredOut.slice(0, 5),
            heroFacets: context.heroFacets || null,
            topologyDiversityPool: context.sectionType === "hero"
        }), chosen.row)
    };
}

function bm25Rank(query, documents) {
    var corpus = [];
    var docLengths = [];
    var docFreq = {};
    var idf = {};
    var N = documents.length;
    var i;
    for (i = 0; i < N; i++) {
        var tok = tokenize(documents[i]);
        corpus.push(tok);
        docLengths.push(tok.length);
        var seen = {};
        for (var t = 0; t < tok.length; t++) seen[tok[t]] = true;
        for (var key in seen) docFreq[key] = (docFreq[key] || 0) + 1;
    }
    if (N === 0) return [];
    var avgdl = 0;
    for (i = 0; i < docLengths.length; i++) avgdl += docLengths[i];
    avgdl = avgdl / N;
    for (var w in docFreq) idf[w] = Math.log((N - docFreq[w] + 0.5) / (docFreq[w] + 0.5) + 1);

    var q = tokenize(query);
    var k1 = 1.5, b = 0.75;
    var scored = [];
    for (i = 0; i < corpus.length; i++) {
        var tf = {};
        for (var ti = 0; ti < corpus[i].length; ti++) tf[corpus[i][ti]] = (tf[corpus[i][ti]] || 0) + 1;
        var score = 0;
        for (var qi = 0; qi < q.length; qi++) {
            var term = q[qi];
            if (idf[term] === undefined) continue;
            var f = tf[term] || 0;
            var den = f + k1 * (1 - b + (b * docLengths[i]) / (avgdl || 1));
            score += idf[term] * ((f * (k1 + 1)) / (den || 1));
        }
        scored.push([i, score]);
    }
    scored.sort(function (a, b2) { return b2[1] - a[1]; });
    return scored;
}

function detectDomain(query) {
    var q = String(query || "").toLowerCase();
    var map = {
        color: ["color", "palette", "hex", "#", "rgb"],
        chart: ["chart", "graph", "visualization", "trend", "bar", "pie", "scatter", "heatmap", "funnel"],
        landing: ["landing", "page", "cta", "conversion", "hero", "testimonial", "pricing", "section"],
        product: ["saas", "ecommerce", "e-commerce", "fintech", "healthcare", "gaming", "portfolio", "crypto", "dashboard", "电商", "商城", "售卖", "商品", "购物"],
        prompt: ["prompt", "prompts", "提示词", "ui prompt", "section prompt", "generate html", "building prompt", "block", "blocks", "building block", "component", "section", "header", "footer", "hero", "pricing", "contact", "module"],
        style: ["style", "design", "ui", "minimalism", "glassmorphism", "neumorphism", "brutalism", "dark mode", "flat", "aurora", "css", "implementation", "variable", "checklist", "tailwind"],
        ux: ["ux", "usability", "accessibility", "wcag", "touch", "scroll", "animation", "keyboard", "navigation", "mobile"],
        typography: ["font", "typography", "heading", "serif", "sans"],
        icons: ["icon", "icons", "lucide", "heroicons", "symbol", "glyph", "svg icon"],
        web: ["aria", "focus", "outline", "semantic", "virtualize", "autocomplete", "form", "input type", "preconnect"],
        slop: ["slop", "anti-slop", "antislop", "ai tell", "avoid", "forbidden", "ban", "banned", "cliche", "generic"]
    };
    var best = "style", bestScore = 0;
    for (var d in map) {
        var score = 0;
        for (var i = 0; i < map[d].length; i++) if (q.indexOf(map[d][i]) >= 0) score++;
        if (score > bestScore) { bestScore = score; best = d; }
    }
    return best;
}

function searchInRows(rows, searchCols, outputCols, query, maxResults) {
    var docs = [];
    for (var i = 0; i < rows.length; i++) {
        var text = [];
        for (var c = 0; c < searchCols.length; c++) text.push(String(rows[i][searchCols[c]] || ""));
        docs.push(text.join(" "));
    }
    var ranked = bm25Rank(query, docs);
    var out = [];
    for (var r = 0; r < ranked.length && out.length < maxResults; r++) {
        var idx = ranked[r][0], score = ranked[r][1];
        if (score <= 0) continue;
        var item = {};
        for (var oc = 0; oc < outputCols.length; oc++) {
            var col = outputCols[oc];
            if (rows[idx].hasOwnProperty(col)) item[col] = rows[idx][col];
        }
        out.push(item);
    }
    return out;
}

function rankProductCandidates(query, maxResults, searchCols) {
    var cfg = CSV_CONFIG.product;
    var rows = getParsedCsvRows(cfg.file);
    var cols = searchCols || PRODUCT_CLASSIFY_SEARCH_COLS;
    var docs = [];
    var i;
    for (i = 0; i < rows.length; i++) {
        var text = [];
        for (var c = 0; c < cols.length; c++) text.push(String(rows[i][cols[c]] || ""));
        docs.push(verticalTokensForClassification(text.join(" ")).join(" "));
    }
    var ranked = bm25Rank(verticalTokensForClassification(query).join(" "), docs);
    var out = [];
    for (var r = 0; r < ranked.length && out.length < maxResults; r++) {
        var idx = ranked[r][0];
        var score = ranked[r][1];
        if (score <= 0) continue;
        var item = { score: score };
        for (var oc = 0; oc < cfg.outputCols.length; oc++) {
            var col = cfg.outputCols[oc];
            if (rows[idx].hasOwnProperty(col)) item[col] = rows[idx][col];
        }
        out.push(item);
    }
    return out;
}

function classifyProductWithConfidence(verticalKeywords, confirmedCategory) {
    if (confirmedCategory) {
        var confirmedProduct = findProductByType(confirmedCategory);
        if (!confirmedProduct) {
            return {
                error: "Unknown confirmedCategory: " + confirmedCategory
            };
        }
        return {
            category: confirmedProduct["Product Type"],
            product: Object.assign({ score: 1 }, confirmedProduct),
            categoryConfidence: "confirmed",
            topCandidates: [{ productType: confirmedProduct["Product Type"], score: 1, keywords: confirmedProduct.Keywords || "" }],
            topScore: 1,
            scoreGap: 1,
            keywordHitRatio: 1,
            source: "confirmedCategory"
        };
    }

    var verticalTokens = verticalTokensForClassification(verticalKeywords);
    var ranked = rankProductCandidates(verticalKeywords, getParsedCsvRows("products.csv").length, PRODUCT_CLASSIFY_SEARCH_COLS);
    var scored = [];
    var i;
    for (i = 0; i < ranked.length; i++) {
        var hitRatio = keywordHitRatio(verticalTokens, ranked[i]);
        scored.push({
            row: ranked[i],
            hitRatio: hitRatio
        });
    }
    scored.sort(function (a, b) {
        if (b.hitRatio !== a.hitRatio) return b.hitRatio - a.hitRatio;
        return b.row.score - a.row.score;
    });

    var topCandidates = scored.slice(0, 3).map(function (item) {
        return {
            productType: item.row["Product Type"] || "General",
            score: item.row.score,
            keywords: item.row.Keywords || "",
            keywordHitRatio: item.hitRatio
        };
    });

    if (!scored.length || scored[0].row.score <= 0) {
        return {
            category: "General",
            product: {},
            categoryConfidence: "low",
            topCandidates: topCandidates,
            topScore: 0,
            scoreGap: 0,
            keywordHitRatio: 0
        };
    }

    var top1 = scored[0];
    var top2 = scored[1];
    var productType = top1.row["Product Type"] || "General";
    var scoreGap = top2 ? top1.row.score - top2.row.score : top1.row.score;
    var hitCount = countKeywordHits(verticalTokens, top1.row);
    // A sparse incidental match must not select an unrelated industry's design.
    // Keep candidates for inspection, but let normal page planning use its
    // existing generic fallback rather than inventing a confirmed category.
    if (hitCount < 2 && top1.hitRatio < MIN_KEYWORD_HIT_RATIO) {
        return {
            category: "General",
            product: {},
            categoryConfidence: "low",
            topCandidates: topCandidates,
            topScore: top1.row.score,
            scoreGap: scoreGap,
            keywordHitRatio: top1.hitRatio,
            keywordHitCount: hitCount,
            source: "insufficient-industry-evidence"
        };
    }
    var confidence = "high";

    if (hitCount < 1) {
        confidence = "low";
    } else if (productType === "General") {
        confidence = "low";
    } else if (top1.hitRatio < MIN_KEYWORD_HIT_RATIO ||
        (top2 && top1.hitRatio === top2.hitRatio &&
            (scoreGap < PRODUCT_SCORE_GAP || top2.row.score >= top1.row.score * PRODUCT_SCORE_THRESHOLD_RATIO))) {
        // Weak coverage or tied evidence is diagnostic, not a mandatory user question.
        confidence = "medium";
    }

    return {
        category: productType,
        product: top1.row,
        categoryConfidence: confidence,
        topCandidates: topCandidates,
        topScore: top1.row.score,
        scoreGap: scoreGap,
        keywordHitRatio: top1.hitRatio,
        keywordHitCount: hitCount
    };
}

function search(query, domain, maxResults) {
    var d = domain || detectDomain(query);
    if (d === "block") d = "prompt";
    var cfg = CSV_CONFIG[d] || CSV_CONFIG.style;
    var rows = d === "prompt" ? getUiPromptIndex().rows : getParsedCsvRows(cfg.file);
    var results = searchInRows(rows, cfg.searchCols, cfg.outputCols, query, maxResults || MAX_RESULTS);
    return { domain: d, query: query, file: cfg.file, count: results.length, results: results };
}

function searchStack(query, stack, maxResults) {
    if (!STACK_CONFIG[stack]) return { domain: "stack", stack: stack, query: query, file: "", count: 0, results: [], error: "Unknown stack: " + stack };
    var rows = parseCSV(readSkillResource(STACK_CONFIG[stack]));
    var results = searchInRows(rows, STACK_COLS.searchCols, STACK_COLS.outputCols, query, maxResults || MAX_RESULTS);
    return { domain: "stack", stack: stack, query: query, file: STACK_CONFIG[stack], count: results.length, results: results };
}

function findReasoningRule(category, rules) {
    var c = String(category || "").toLowerCase();
    var i;
    for (i = 0; i < rules.length; i++) if (String(rules[i].UI_Category || "").toLowerCase() === c) return rules[i];
    for (i = 0; i < rules.length; i++) {
        var ui = String(rules[i].UI_Category || "").toLowerCase();
        if (ui.indexOf(c) >= 0 || c.indexOf(ui) >= 0) return rules[i];
    }
    return null;
}

function applyReasoning(category, rules) {
    var rule = findReasoningRule(category, rules);
    if (!rule) return { pattern: "Hero + Features + CTA", stylePriority: ["Minimalism", "Flat Design"], colorMood: "Professional", typographyMood: "Clean", keyEffects: "Subtle hover transitions", antiPatterns: "", decisionRules: {}, severity: "MEDIUM" };
    var parsed = {};
    try { parsed = rule.Decision_Rules ? JSON.parse(rule.Decision_Rules) : {}; } catch (e) { parsed = {}; }
    return {
        pattern: rule.Recommended_Pattern || "",
        stylePriority: String(rule.Style_Priority || "").split("+").map(function (s) { return s.trim(); }),
        colorMood: rule.Color_Mood || "",
        typographyMood: rule.Typography_Mood || "",
        keyEffects: rule.Key_Effects || "",
        antiPatterns: rule.Anti_Patterns || "",
        decisionRules: parsed,
        severity: rule.Severity || "MEDIUM"
    };
}

// Collect anti-slop rules for designSystem output.
// Strategy:
//   1) Include EVERY HIGH-severity rule, grouped by Category (compact one-liners).
//   2) Add up to `extra` query-relevant MEDIUM/LOW rules (BM25 ranked).
// Returns { high: { Category: [{issue,do,don't}, ...] }, extra: [...] }
function gatherAntiSlop(query, extra) {
    var rows = parseCSV(readSkillResource("anti-slop.csv"));
    if (!rows || rows.length === 0) return { high: {}, extra: [] };

    var maxExtra = typeof extra === "number" ? extra : 3;

    var high = {};
    var others = [];
    for (var i = 0; i < rows.length; i++) {
        var r = rows[i];
        var sev = String(r["Severity"] || "").toUpperCase();
        var entry = {
            issue: r["Issue"] || "",
            cat: r["Category"] || "",
            doRule: r["Do"] || "",
            dontRule: r["Don't"] || "",
            sev: sev || "MEDIUM"
        };
        if (sev === "HIGH") {
            if (!high[entry.cat]) high[entry.cat] = [];
            high[entry.cat].push(entry);
        } else {
            others.push(entry);
        }
    }

    var extras = [];
    if (others.length > 0 && maxExtra > 0) {
        var docs = [];
        for (var j = 0; j < others.length; j++) {
            var o = others[j];
            docs.push([o.cat, o.issue, o.dontRule, o.doRule].join(" "));
        }
        var ranked = bm25Rank(query || "", docs);
        for (var k = 0; k < ranked.length && extras.length < maxExtra; k++) {
            if (ranked[k][1] <= 0) continue;
            extras.push(others[ranked[k][0]]);
        }
    }
    return { high: high, extra: extras };
}

function renderAntiSlopMarkdown(slop) {
    var lines = [];
    lines.push("");
    lines.push("## Anti-Slop Rules");
    lines.push("> 默认强制项；偏离需在设计稿中给出明确理由。");
    var cats = [];
    for (var c in slop.high) cats.push(c);
    cats.sort();
    for (var i = 0; i < cats.length; i++) {
        var cat = cats[i];
        lines.push("");
        lines.push("**[HIGH] " + cat + "**");
        var items = slop.high[cat];
        for (var j = 0; j < items.length; j++) {
            lines.push("- " + items[j].issue + " — Do: " + items[j].doRule);
        }
    }
    if (slop.extra.length > 0) {
        lines.push("");
        lines.push("**Context-Relevant**");
        for (var k = 0; k < slop.extra.length; k++) {
            var e = slop.extra[k];
            lines.push("- [" + e.sev + "] " + e.cat + " · " + e.issue + " — Do: " + e.doRule);
        }
    }
    return lines.join("\n");
}

function renderAntiSlopAscii(slop) {
    var lines = [];
    lines.push("");
    lines.push("+" + Array(BOX_WIDTH).join("-") + "+");
    lines.push(rpad("|  ANTI-SLOP RULES (must avoid)", BOX_WIDTH) + "|");
    lines.push("+" + Array(BOX_WIDTH).join("-") + "+");
    var cats = [];
    for (var c in slop.high) cats.push(c);
    cats.sort();
    for (var i = 0; i < cats.length; i++) {
        var cat = cats[i];
        lines.push(rpad("|  [HIGH] " + cat, BOX_WIDTH) + "|");
        var items = slop.high[cat];
        for (var j = 0; j < items.length; j++) {
            var wrap = wrapText(items[j].issue + " -> " + items[j].doRule, "|     - ", BOX_WIDTH);
            for (var w = 0; w < wrap.length; w++) lines.push(rpad(wrap[w], BOX_WIDTH) + "|");
        }
    }
    if (slop.extra.length > 0) {
        lines.push("|" + Array(BOX_WIDTH).join(" ") + "|");
        lines.push(rpad("|  CONTEXT-RELEVANT", BOX_WIDTH) + "|");
        for (var k = 0; k < slop.extra.length; k++) {
            var e = slop.extra[k];
            var wrap2 = wrapText("[" + e.sev + "] " + e.cat + " - " + e.issue + " -> " + e.doRule, "|     ", BOX_WIDTH);
            for (var w2 = 0; w2 < wrap2.length; w2++) lines.push(rpad(wrap2[w2], BOX_WIDTH) + "|");
        }
    }
    lines.push("+" + Array(BOX_WIDTH).join("-") + "+");
    return lines.join("\n");
}

function selectBestMatch(results, priorities) {
    if (!results || results.length === 0) return {};
    if (!priorities || priorities.length === 0) return results[0];
    for (var i = 0; i < priorities.length; i++) {
        var p = String(priorities[i]).toLowerCase().trim();
        for (var j = 0; j < results.length; j++) {
            var n = String(results[j]["Style Category"] || "").toLowerCase();
            if (n.indexOf(p) >= 0 || p.indexOf(n) >= 0) return results[j];
        }
    }
    return results[0];
}

function selectColorForTone(colorResults, toneTerms) {
    if (!colorResults || !colorResults.length) return {};
    var t = String(toneTerms || "").toLowerCase();
    var i;
    for (i = 0; i < colorResults.length; i++) {
        var notes = String(colorResults[i].Notes || "").toLowerCase();
        var productType = String(colorResults[i]["Product Type"] || "").toLowerCase();
        if (/premium|luxury|exclusive|elegant/.test(t) && (notes.indexOf("gold") >= 0 || notes.indexOf("black") >= 0 || productType.indexOf("luxury") >= 0)) {
            return colorResults[i];
        }
        if (/natural|organic|fresh|warm/.test(t) && (notes.indexOf("green") >= 0 || notes.indexOf("natural") >= 0 || notes.indexOf("floral") >= 0)) {
            return colorResults[i];
        }
    }
    return colorResults[0];
}

function selectTypographyForTone(typoResults, toneTerms) {
    if (!typoResults || !typoResults.length) return {};
    var t = String(toneTerms || "").toLowerCase();
    var moodNeedle = toneTypographyMood(toneTerms).toLowerCase();
    var i;
    for (i = 0; i < typoResults.length; i++) {
        var mood = String(typoResults[i]["Mood/Style Keywords"] || "").toLowerCase();
        if (/premium|luxury|elegant/.test(t) && (mood.indexOf("elegant") >= 0 || mood.indexOf("luxury") >= 0 || mood.indexOf("refined") >= 0)) {
            return typoResults[i];
        }
        if (/natural|organic|fresh/.test(t) && (mood.indexOf("natural") >= 0 || mood.indexOf("humanist") >= 0 || mood.indexOf("friendly") >= 0)) {
            return typoResults[i];
        }
        if (moodNeedle && mood.indexOf(moodNeedle.split(" ")[0]) >= 0) return typoResults[i];
    }
    return typoResults[0];
}

function wrapText(text, prefix, width) {
    if (!text) return [];
    var words = String(text).split(/\s+/);
    var lines = [];
    var cur = prefix;
    for (var i = 0; i < words.length; i++) {
        if ((cur + " " + words[i]).length <= width - 2) cur += (cur === prefix ? "" : " ") + words[i];
        else { if (cur !== prefix) lines.push(cur); cur = prefix + words[i]; }
    }
    if (cur !== prefix) lines.push(cur);
    return lines;
}

function rpad(s, len, ch) {
    var out = String(s || "");
    var fill = ch || " ";
    while (out.length < len) out += fill;
    return out;
}

function formatASCIIBox(ds) {
    var lines = [];
    var w = BOX_WIDTH - 1;
    lines.push("+" + Array(w + 1).join("-") + "+");
    lines.push(rpad("|  TARGET: " + ds.project_name + " - RECOMMENDED DESIGN SYSTEM", BOX_WIDTH) + "|");
    lines.push("+" + Array(w + 1).join("-") + "+");
    lines.push("|" + Array(BOX_WIDTH + 1).join(" ") + "|");
    lines.push(rpad("|  PATTERN: " + ds.pattern.name, BOX_WIDTH) + "|");
    lines.push(rpad("|     CTA: " + ds.pattern.cta_placement, BOX_WIDTH) + "|");
    lines.push(rpad("|     Sections: " + ds.pattern.sections, BOX_WIDTH) + "|");
    lines.push("|" + Array(BOX_WIDTH + 1).join(" ") + "|");
    lines.push(rpad("|  STYLE: " + ds.style.name, BOX_WIDTH) + "|");
    var kw = wrapText("Keywords: " + (ds.style.keywords || ""), "|     ", BOX_WIDTH);
    for (var i = 0; i < kw.length; i++) lines.push(rpad(kw[i], BOX_WIDTH) + "|");
    lines.push("|" + Array(BOX_WIDTH + 1).join(" ") + "|");
    lines.push(rpad("|  COLORS:", BOX_WIDTH) + "|");
    lines.push(rpad("|     Primary:    " + ds.colors.primary, BOX_WIDTH) + "|");
    lines.push(rpad("|     Secondary:  " + ds.colors.secondary, BOX_WIDTH) + "|");
    lines.push(rpad("|     CTA:        " + ds.colors.cta, BOX_WIDTH) + "|");
    lines.push(rpad("|     Background: " + ds.colors.background, BOX_WIDTH) + "|");
    lines.push(rpad("|     Text:       " + ds.colors.text, BOX_WIDTH) + "|");
    lines.push("|" + Array(BOX_WIDTH + 1).join(" ") + "|");
    lines.push(rpad("|  TYPOGRAPHY: " + ds.typography.heading + " / " + ds.typography.body, BOX_WIDTH) + "|");
    lines.push("|" + Array(BOX_WIDTH + 1).join(" ") + "|");
    lines.push("+" + Array(w + 1).join("-") + "+");
    return lines.join("\n");
}

function formatMarkdown(ds) {
    return [
        "## Design System: " + ds.project_name, "",
        "### Pattern", "- **Name:** " + ds.pattern.name, "- **Sections:** " + ds.pattern.sections, "- **CTA Placement:** " + ds.pattern.cta_placement, "",
        "### Style", "- **Name:** " + ds.style.name, "- **Keywords:** " + (ds.style.keywords || ""), "- **Best For:** " + (ds.style.best_for || ""), "",
        "### Colors", "| Role | Hex |", "|------|-----|",
        "| Primary | " + ds.colors.primary + " |",
        "| Secondary | " + ds.colors.secondary + " |",
        "| CTA | " + ds.colors.cta + " |",
        "| Background | " + ds.colors.background + " |",
        "| Text | " + ds.colors.text + " |", "",
        "### Typography", "- **Heading:** " + ds.typography.heading, "- **Body:** " + ds.typography.body, "- **Mood:** " + (ds.typography.mood || ""), ""
    ].join("\n");
}

function formatMasterMD(ds) {
    var lines = [];
    lines.push("# Design System Master File");
    lines.push("");
    lines.push("> **LOGIC:** Check `design-system/pages/[page-name].md` first; page file overrides this master.");
    lines.push("");
    lines.push("**Project:** " + ds.project_name);
    lines.push("**Category:** " + (ds.category || "General"));
    lines.push("");
    lines.push("## Global Rules");
    lines.push("");
    lines.push("### Color Palette");
    lines.push("| Role | Hex |");
    lines.push("|------|-----|");
    lines.push("| Primary | `" + ds.colors.primary + "` |");
    lines.push("| Secondary | `" + ds.colors.secondary + "` |");
    lines.push("| CTA/Accent | `" + ds.colors.cta + "` |");
    lines.push("| Background | `" + ds.colors.background + "` |");
    lines.push("| Text | `" + ds.colors.text + "` |");
    lines.push("");
    lines.push("### Typography");
    lines.push("- **Heading Font:** " + ds.typography.heading);
    lines.push("- **Body Font:** " + ds.typography.body);
    if (ds.typography.google_fonts_url) lines.push("- **Google Fonts:** " + ds.typography.google_fonts_url);
    lines.push("");
    lines.push("## Style Guidelines");
    lines.push("- **Style:** " + ds.style.name);
    lines.push("- **Pattern:** " + ds.pattern.name);
    lines.push("- **Sections:** " + ds.pattern.sections);
    lines.push("- **CTA Placement:** " + ds.pattern.cta_placement);
    lines.push("");
    lines.push("## Anti-Patterns");
    var anti = String(ds.anti_patterns || "").split("+");
    for (var i = 0; i < anti.length; i++) {
        var a = anti[i].trim();
        if (a) lines.push("- ❌ " + a);
    }
    lines.push("- ❌ Emojis as icons");
    lines.push("- ❌ Missing `cursor-pointer` on interactive elements");
    lines.push("- ❌ Layout-shifting hover effects");
    return lines.join("\n");
}

function detectPageType(context) {
    var c = String(context || "").toLowerCase();
    var patterns = [
        [["dashboard", "admin", "analytics", "metrics", "stats"], "Dashboard / Data View"],
        [["checkout", "payment", "cart", "billing"], "Checkout / Payment"],
        [["settings", "profile", "account"], "Settings / Profile"],
        [["landing", "marketing", "hero", "homepage"], "Landing / Marketing"],
        [["login", "signin", "signup", "auth"], "Authentication"],
        [["pricing", "plans", "subscription"], "Pricing / Plans"]
    ];
    for (var i = 0; i < patterns.length; i++) {
        var kws = patterns[i][0];
        for (var j = 0; j < kws.length; j++) if (c.indexOf(kws[j]) >= 0) return patterns[i][1];
    }
    return "General";
}

function generateIntelligentOverrides(pageName, pageQuery) {
    var context = String(pageName || "") + " " + String(pageQuery || "");
    var styleSearch = search(context, "style", 1);
    var uxSearch = search(context, "ux", 3);
    var landingSearch = search(context, "landing", 1);
    var layout = {}, spacing = {}, typography = {}, colors = {}, components = [], uniqueComponents = [], recommendations = [];
    var style = styleSearch.results[0] || {};
    var kws = String(style["Keywords"] || "").toLowerCase();
    if (kws.match(/data|dense|dashboard|grid/)) {
        layout["Max Width"] = "1400px or full-width";
        layout["Grid"] = "12-column grid";
        spacing["Content Density"] = "High";
    } else if (kws.match(/minimal|simple|clean|single/)) {
        layout["Max Width"] = "800px";
        layout["Layout"] = "Single column centered";
        spacing["Content Density"] = "Low";
    } else {
        layout["Max Width"] = "1200px";
        layout["Layout"] = "Responsive sections";
    }
    for (var i = 0; i < uxSearch.results.length; i++) {
        var ux = uxSearch.results[i];
        if (ux["Do"]) recommendations.push((ux["Category"] || "UX") + ": " + ux["Do"]);
        if (ux["Don't"]) components.push("Avoid: " + ux["Don't"]);
    }
    var landing = landingSearch.results[0] || {};
    if (landing["Section Order"]) layout["Sections"] = landing["Section Order"];
    if (landing["Primary CTA Placement"]) recommendations.push("CTA Placement: " + landing["Primary CTA Placement"]);
    if (landing["Color Strategy"]) colors["Strategy"] = landing["Color Strategy"];
    if (recommendations.length === 0) recommendations.push("Refer to MASTER.md for default rules");
    return {
        pageType: detectPageType(context),
        layout: layout,
        spacing: spacing,
        typography: typography,
        colors: colors,
        components: components,
        uniqueComponents: uniqueComponents,
        recommendations: recommendations
    };
}

function formatPageOverrideMD(ds, pageName, pageQuery) {
    var ov = generateIntelligentOverrides(pageName, pageQuery);
    var lines = [];
    lines.push("# " + pageName + " Page Overrides");
    lines.push("");
    lines.push("> **PROJECT:** " + ds.project_name);
    lines.push("> **Page Type:** " + ov.pageType);
    lines.push("> Rules in this file override `MASTER.md`.");
    lines.push("");
    lines.push("## Layout Overrides");
    for (var lk in ov.layout) lines.push("- **" + lk + ":** " + ov.layout[lk]);
    if (Object.keys(ov.layout).length === 0) lines.push("- No overrides");
    lines.push("");
    lines.push("## Spacing Overrides");
    for (var sk in ov.spacing) lines.push("- **" + sk + ":** " + ov.spacing[sk]);
    if (Object.keys(ov.spacing).length === 0) lines.push("- No overrides");
    lines.push("");
    lines.push("## Typography Overrides");
    for (var tk in ov.typography) lines.push("- **" + tk + ":** " + ov.typography[tk]);
    if (Object.keys(ov.typography).length === 0) lines.push("- No overrides");
    lines.push("");
    lines.push("## Color Overrides");
    for (var ck in ov.colors) lines.push("- **" + ck + ":** " + ov.colors[ck]);
    if (Object.keys(ov.colors).length === 0) lines.push("- No overrides");
    lines.push("");
    lines.push("## Component Overrides");
    for (var ci = 0; ci < ov.components.length; ci++) lines.push("- " + ov.components[ci]);
    if (ov.components.length === 0) lines.push("- No overrides");
    lines.push("");
    lines.push("## Recommendations");
    for (var ri = 0; ri < ov.recommendations.length; ri++) lines.push("- " + ov.recommendations[ri]);
    return lines.join("\n");
}

function formatOutput(result) {
    if (result.error) return "Error: " + result.error;
    var out = [];
    if (result.stack) {
        out.push("## UI Pro Max Stack Guidelines");
        out.push("**Stack:** " + result.stack + " | **Query:** " + result.query);
    } else {
        out.push("## UI Pro Max Search Results");
        out.push("**Domain:** " + result.domain + " | **Query:** " + result.query);
    }
    out.push("**Source:** " + result.file + " | **Found:** " + result.count + " results\n");
    for (var i = 0; i < result.results.length; i++) {
        out.push("### Result " + (i + 1));
        for (var key in result.results[i]) {
            var raw = result.results[i][key];
            var v = (raw && typeof raw === "object") ? JSON.stringify(raw) : String(raw);
            if (v.length > 300) v = v.slice(0, 300) + "...";
            out.push("- **" + key + ":** " + v);
        }
        out.push("");
    }
    return out.join("\n");
}

function splitList(value) {
    var raw = String(value || "");
    if (!raw) return [];
    var parts = raw.split(/[;,|]/);
    var out = [];
    for (var i = 0; i < parts.length; i++) {
        var item = parts[i].trim();
        if (item) out.push(item);
    }
    return out;
}

function defaultScopeFor(sectionType) {
    return (sectionType === "header" || sectionType === "footer") ? "shared" : "page";
}

function defaultCreatePhaseFor(sectionType) {
    return defaultScopeFor(sectionType) === "shared" ? "site-level" : "page-level";
}

function defaultImagePolicyFor(sectionType) {
    if (/hero|featured-products|product-grid|product-gallery|related-products|blog-list|case-studies|demo-preview|cart-items/.test(sectionType)) return "required";
    if (/header|footer|faq|pricing|auth-form|data-table|chart-panel|announcement-bar|trust-bar|category-filter|checkout-summary|checkout-form|pagination|order-status/.test(sectionType)) return "none";
    return "optional";
}

function defaultDataModeFor(sectionType) {
    if (/product-grid|featured-products|data-table|blog-list|case-studies|pricing|faq|pagination|cart-items|checkout-form|order-status|order-history/.test(sectionType)) return "bindable-static";
    return "static-first";
}

function defaultInteractionPolicyFor(sectionType) {
    if (/auth-form|contact|category-filter|data-table|checkout-summary|checkout-form|pagination|cart-items|order-status|order-history/.test(sectionType)) return "static";
    if (/faq|chart-panel|product-gallery|demo-preview/.test(sectionType)) return "progressive";
    return "none";
}

function normalizeKey(value) {
    return String(value || "").toLowerCase().replace(/[^a-z0-9]+/g, "-").replace(/^-+|-+$/g, "");
}

function blockLabel(type) {
    var labels = {
        header: "Header 导航",
        hero: "Hero 主视觉",
        "featured-products": "Featured Products 推荐内容",
        "category-filter": "Category Filter 分类筛选",
        "product-grid": "Product Grid 列表网格",
        "product-gallery": "Product Gallery 媒体展示",
        "product-summary": "Product Summary 购买摘要",
        "related-products": "Related Products 相关推荐",
        pagination: "Pagination 分页导航",
        "cart-items": "Cart Items 购物车商品",
        "checkout-form": "Checkout Form 结算信息",
        "order-status": "Order Status 订单状态",
        "order-history": "Order History 订单记录",
        "profile-account": "Profile Account 账户资料",
        "auth-form": "Auth Form 登录注册",
        "checkout-summary": "Checkout Summary 金额摘要",
        "care-guide": "Care Guide 内容指南",
        "article-content": "Article Content 文章正文",
        features: "Features 功能/卖点",
        "trust-bar": "Trust Bar 信任背书",
        pricing: "Pricing 价格方案",
        testimonials: "Testimonials 用户评价",
        contact: "Contact 联系方式",
        map: "Map 位置/区域",
        faq: "FAQ 常见问题",
        cta: "CTA 行动召唤",
        footer: "Footer 页脚"
    };
    return labels[type] || type;
}

function pageTitle(name) {
    var titles = {
        home: "首页",
        products: "商品列表页",
        "product-detail": "商品详情页",
        cart: "购物车页",
        checkout: "结算页",
        "order-result": "订单结果页",
        account: "账户中心页",
        orders: "订单列表页",
        "order-detail": "订单详情页",
        login: "登录页",
        register: "注册页",
        contact: "联系页",
        services: "服务页",
        about: "关于页",
        pricing: "价格页",
        portfolio: "作品页",
        collections: "系列页",
        occasions: "场合选购页",
        "care-guide": "养护指南页",
        story: "品牌故事页",
        articles: "文章列表页",
        "article-detail": "文章详情页",
        subscribe: "订阅页"
    };
    return titles[name] || name;
}

function blockRefFor(pageName, sectionType, block, localOccurrence) {
    var scope = block && block.scope ? block.scope : defaultScopeFor(sectionType);
    if (isSharedBlockScope(scope)) return "site-" + normalizeKey(sectionType);
    var base = normalizeKey(pageName || "page") + "-" + normalizeKey(sectionType || "block");
    return localOccurrence > 0 ? base + "-" + (localOccurrence + 1) : base;
}

function compactBlockForPlan(block) {
    return {
        blockRef: block.blockRef || "",
        label: block.label,
        promptId: block.promptId
    };
}

function compactPagesForPlan(pages) {
    var outPages = [];
    var sharedByRef = {};
    var sharedRefs = [];
    for (var i = 0; i < pages.length; i++) {
        var page = pages[i];
        var blocks = [];
        for (var j = 0; j < page.blocks.length; j++) {
            var sourceBlock = page.blocks[j];
            var summary = compactBlockForPlan(sourceBlock);
            if (isSharedBlockScope(sourceBlock.scope)) {
                if (!sharedByRef[summary.blockRef]) {
                    sharedByRef[summary.blockRef] = summary;
                    sharedRefs.push(summary.blockRef);
                }
            } else {
                blocks.push(summary);
            }
        }
        outPages.push({
            name: page.name,
            title: page.title,
            purpose: page.purpose,
            blocks: blocks
        });
    }

    var sharedBlocks = [];
    for (var k = 0; k < sharedRefs.length; k++) sharedBlocks.push(sharedByRef[sharedRefs[k]]);
    return { pages: outPages, sharedBlocks: sharedBlocks };
}

function summarizePagePlan(plan) {
    var compact = compactPagesForPlan(plan.pages || []);
    return {
        version: 3,
        resolvedBrief: plan.resolvedBrief,
        decisionReceipts: plan.decisionReceipts || [],
        classification: plan.classification,
        sharedBlocks: compact.sharedBlocks,
        pages: compact.pages
    };
}

function isCommerceLike(query, category) {
    var c = (String(query || "") + " " + String(category || "")).toLowerCase();
    return /(commerce|ecommerce|e-commerce|shop|store|retail|buy|sell|product|catalog|fish|aquarium|marketplace|电商|商城|商店|店铺|网店|售卖|销售|卖货|商品|购物|购买|下单|零售)/.test(c);
}

function isServiceLike(query, category) {
    var c = (String(query || "") + " " + String(category || "")).toLowerCase();
    return /(service|booking|appointment|consultation|clinic|restaurant|spa|agency|firm|local)/.test(c);
}

function isSaasLike(query, category) {
    var c = (String(query || "") + " " + String(category || "")).toLowerCase();
    return /(saas|software|platform|app|tool|subscription|dashboard)/.test(c);
}

function isPortfolioLike(query, category) {
    var c = (String(query || "") + " " + String(category || "")).toLowerCase();
    return /(portfolio|gallery|studio|photography|agency|personal|creative)/.test(c);
}

function sitePage(name, purpose, blocks, deliveryStage, covers) {
    return {
        name: name,
        purpose: purpose,
        blocks: blocks,
        deliveryStage: deliveryStage,
        covers: covers || []
    };
}

var SITE_BLUEPRINTS = {
    "ecommerce-catalog": [
        sitePage("home", "用摄影主导的首屏建立店铺识别，并在同屏露出商品浏览路径", ["header", "hero", "featured-products", "footer"], "entry", ["trust"]),
        sitePage("products", "展示商品目录，支持用户浏览和筛选", ["header", "category-filter", "product-grid", "footer"], "explore"),
        sitePage("product-detail", "说明单个商品的图片、价格、购买理由和养护信息", ["header", "product-gallery", "product-summary", "care-guide", "related-products", "footer"], "evaluate", ["act"]),
        sitePage("contact", "提供联系、门店、服务范围和常见问题", ["header", "contact", "map", "faq", "footer"], "act")
    ],
    "ecommerce-luxury": [
        sitePage("home", "用全幅或编辑式首屏建立高端品牌印象，并直接接入系列商品", ["header", "hero", "featured-products", "footer"], "entry", ["trust"]),
        sitePage("collections", "按系列展示高端商品与策展叙事", ["header", "category-filter", "product-grid", "testimonials", "footer"], "explore"),
        sitePage("product-detail", "突出工艺、材质与购买理由", ["header", "product-gallery", "product-summary", "testimonials", "related-products", "footer"], "evaluate", ["act"]),
        sitePage("story", "讲述品牌故事、工艺与可信背书", ["header", "features", "testimonials", "cta", "footer"], "trust"),
        sitePage("contact", "提供预约、咨询与门店信息", ["header", "contact", "faq", "footer"], "act")
    ],
    "ecommerce-florist": [
        sitePage("home", "用花礼摄影首屏建立品牌气质，并同屏露出当季可购组合", ["header", "hero", "featured-products", "footer"], "entry", ["trust"]),
        sitePage("occasions", "按送礼场景浏览花礼组合", ["header", "category-filter", "featured-products", "cta", "footer"], "explore"),
        sitePage("products", "展示花束、礼盒与植物目录", ["header", "category-filter", "product-grid", "trust-bar", "footer"], "explore"),
        sitePage("product-detail", "说明花礼内容、规格、配送信息与订购行动", ["header", "product-gallery", "product-summary", "care-guide", "footer"], "evaluate", ["act"]),
        sitePage("care-guide", "提供养护建议与保鲜说明", ["header", "care-guide", "faq", "footer"], "trust"),
        sitePage("about", "介绍花艺理念、团队与品牌故事", ["header", "features", "testimonials", "footer"], "trust"),
        sitePage("contact", "提供订购、配送与门店联系入口", ["header", "contact", "map", "faq", "footer"], "act")
    ],
    "local-service": [
        sitePage("home", "以服务场景或预约路径开场，快速进入项目与联系，而不是营销口号落地页", ["header", "hero", "features", "contact", "footer"], "entry", ["act", "trust"]),
        sitePage("services", "说明服务项目、流程与价格区间", ["header", "features", "pricing", "faq", "cta", "footer"], "evaluate"),
        sitePage("about", "补充团队资质、服务区域与品牌故事", ["header", "features", "testimonials", "footer"], "trust"),
        sitePage("contact", "提供预约、地址、营业时间与常见问题", ["header", "contact", "map", "faq", "footer"], "act")
    ],
    "portfolio-creative": [
        sitePage("home", "展示视觉风格、代表作品与联系入口", ["header", "hero", "featured-products", "testimonials", "cta", "footer"], "entry"),
        sitePage("portfolio", "集中展示作品分类和项目列表", ["header", "category-filter", "product-grid", "cta", "footer"], "evaluate", ["trust"]),
        sitePage("about", "说明背景、能力与创作方式", ["header", "features", "testimonials", "footer"], "trust"),
        sitePage("contact", "提供合作咨询和联系方式", ["header", "contact", "faq", "footer"], "act")
    ],
    "saas-product": [
        sitePage("home", "说明产品价值、核心能力和转化入口", ["header", "hero", "features", "trust-bar", "pricing", "cta", "footer"], "entry", ["act", "trust"]),
        sitePage("pricing", "展示套餐、差异和购买顾虑解答", ["header", "pricing", "faq", "cta", "footer"], "evaluate"),
        sitePage("contact", "提供咨询、演示预约和企业联系入口", ["header", "contact", "faq", "footer"], "act")
    ],
    "content-publication": [
        sitePage("home", "用封面或主稿摄影建立刊物识别，并同屏进入文章河/目录", ["header", "hero", "blog-list", "footer"], "entry", ["act"]),
        sitePage("articles", "展示文章列表与主题分类", ["header", "category-filter", "blog-list", "footer"], "explore"),
        sitePage("article-detail", "提供完整文章阅读、来源信息和后续阅读入口", ["header", "article-content", "cta", "footer"], "evaluate"),
        sitePage("about", "说明出版定位、作者与编辑方向", ["header", "features", "testimonials", "footer"], "trust"),
        sitePage("subscribe", "引导订阅、会员或通讯注册", ["header", "cta", "faq", "footer"], "act")
    ],
    "landing-leadgen": [
        sitePage("home", "单页承接流量、说明价值并收集线索", ["header", "hero", "features", "testimonials", "cta", "faq", "footer"], "entry", ["explore", "evaluate", "act", "trust"])
    ],
    "general-multipage": [
        sitePage("home", "说明站点主题、核心内容和主要行动入口", ["header", "hero", "features", "cta", "footer"], "entry"),
        sitePage("about", "说明背景、能力和可信信息", ["header", "features", "testimonials", "footer"], "evaluate", ["trust"]),
        sitePage("contact", "提供联系入口和常见问题", ["header", "contact", "faq", "footer"], "act")
    ]
};

// Compact storefront recipes are source guidance, not per-site memory. The
// generated Page Plan remains the only long-lived summary artifact.
var COMMERCE_STOREFRONT_CATALOG = {
    "catalog-contact": {
        pages: {
            home: {
                purpose: "用摄影主导首屏建立品牌，并同屏露出重点商品与购买路径",
                blocks: ["header", "hero", "featured-products", "footer"],
                deliveryStage: "entry",
                covers: ["trust"]
            },
            products: {
                purpose: "支持分类、搜索、排序和分页浏览商品目录",
                blocks: ["header", "category-filter", "product-grid", "footer"],
                deliveryStage: "explore",
                covers: []
            },
            "product-detail": {
                purpose: "展示商品媒体、规格、价格和联系购买入口",
                blocks: ["header", "product-gallery", "product-summary", "contact", "footer"],
                deliveryStage: "evaluate",
                covers: ["act"]
            }
        }
    },
    "online-checkout": {
        pages: {
            home: {
                purpose: "用摄影主导首屏建立店铺识别，并同屏露出可购买商品路径",
                blocks: ["header", "hero", "featured-products", "footer"],
                deliveryStage: "entry",
                covers: ["trust"]
            },
            products: {
                purpose: "支持分类、搜索、排序和分页浏览可购买商品",
                blocks: ["header", "category-filter", "product-grid", "footer"],
                deliveryStage: "explore",
                covers: []
            },
            "product-detail": {
                purpose: "完成商品媒体、规格、变体、数量和加购决策",
                blocks: ["header", "product-gallery", "product-summary", "related-products", "footer"],
                deliveryStage: "evaluate",
                covers: ["act"]
            },
            cart: {
                purpose: "查看并修改购物车商品、优惠和结算金额",
                blocks: ["header", "cart-items", "checkout-summary", "trust-bar", "footer"],
                deliveryStage: "task",
                covers: []
            },
            checkout: {
                purpose: "填写联系、地址、配送和支付信息并确认订单",
                blocks: ["header", "checkout-form", "checkout-summary", "footer"],
                deliveryStage: "task",
                covers: []
            },
            "order-result": {
                purpose: "反馈订单或支付结果并提供后续行动",
                blocks: ["header", "order-status", "related-products", "footer"],
                deliveryStage: "task",
                covers: []
            },
            account: {
                purpose: "管理顾客资料并进入订单记录",
                blocks: ["header", "profile-account", "order-history", "footer"],
                deliveryStage: "task",
                covers: []
            },
            orders: {
                purpose: "浏览当前顾客的订单记录和状态",
                blocks: ["header", "order-history", "footer"],
                deliveryStage: "task",
                covers: []
            },
            "order-detail": {
                purpose: "查看单笔订单状态、商品和金额明细",
                blocks: ["header", "order-status", "checkout-summary", "footer"],
                deliveryStage: "task",
                covers: []
            },
            login: {
                purpose: "建立 Commerce 顾客登录状态",
                blocks: ["header", "auth-form", "trust-bar", "footer"],
                deliveryStage: "task",
                covers: []
            },
            register: {
                purpose: "创建 Commerce 顾客账号并进入购买流程",
                blocks: ["header", "auth-form", "trust-bar", "footer"],
                deliveryStage: "task",
                covers: []
            }
        }
    }
};

var COMMERCE_BLOCK_REQUIRED_CONTENT = {
    "category-filter": ["categoryLinks", "searchField", "sortOptions", "activeFilters", "resultCount"],
    "product-grid": ["detailLink", "emptyState", "paginationControls"],
    "product-summary": ["variantOptions", "quantity", "availability", "addToCartAction"],
    pagination: ["currentPage", "pageCount", "previousLink", "nextLink", "preservedQuery"],
    "cart-items": ["cartLines", "quantityControls", "removeAction", "lineTotals", "emptyState"],
    "checkout-form": ["contactFields", "addressFields", "shippingOptions", "paymentOptions", "validationSummary"],
    "checkout-summary": ["editableItems", "priceBreakdown", "primaryAction"],
    "order-status": ["orderNumber", "orderState", "paymentState", "nextActions"],
    "order-history": ["orders", "orderState", "orderTotal", "detailLink", "emptyState"]
};

function mergeUniqueValues(base, additions) {
    var result = (base || []).slice();
    var seen = {};
    var i;
    for (i = 0; i < result.length; i++) seen[result[i]] = true;
    for (i = 0; i < (additions || []).length; i++) {
        if (seen[additions[i]]) continue;
        result.push(additions[i]);
        seen[additions[i]] = true;
    }
    return result;
}

function applyCommerceBlockRequirements(block, siteType) {
    if (!/^ecommerce-/.test(siteType)) return block;
    var additions = COMMERCE_BLOCK_REQUIRED_CONTENT[block.type] || [];
    if (!additions.length) return block;
    block.requiredContent = mergeUniqueValues(block.requiredContent, additions);
    if (block.prompt && block.prompt.constraints) {
        block.prompt.constraints += " Storefront required content: " + additions.join(", ") + ".";
    }
    return block;
}

function storefrontRecipeFor(pageName, commerceModel) {
    var model = normalizeCommerceModel(commerceModel);
    var catalog = COMMERCE_STOREFRONT_CATALOG[model];
    return catalog && catalog.pages[pageName] ? catalog.pages[pageName] : null;
}

function defaultBlocksForPageName(pageName, siteType, commerceModel) {
    var commerce = /^ecommerce-/.test(siteType);
    var storefrontRecipe = commerce ? storefrontRecipeFor(pageName, commerceModel) : null;
    if (storefrontRecipe) return storefrontRecipe.blocks.slice();
    if (pageName === "home") return ["header", "hero", "features", "cta", "footer"];
    if (pageName === "about") return ["header", "features", "testimonials", "footer"];
    if (pageName === "contact") return ["header", "contact", "faq", "footer"];
    if (pageName === "article-detail") return ["header", "article-content", "cta", "footer"];
    if (commerce && pageName === "products") return ["header", "category-filter", "product-grid", "footer"];
    if (commerce && pageName === "product-detail") return ["header", "product-gallery", "product-summary", "related-products", "footer"];
    return ["header", "features", "cta", "footer"];
}

function defaultDeliveryMetaForPageName(pageName, siteType, commerceModel) {
    var storefrontRecipe = /^ecommerce-/.test(siteType) ? storefrontRecipeFor(pageName, commerceModel) : null;
    if (storefrontRecipe) {
        return {
            deliveryStage: storefrontRecipe.deliveryStage || "evaluate",
            covers: (storefrontRecipe.covers || []).slice()
        };
    }
    if (pageName === "home") return { deliveryStage: "entry", covers: [] };
    if (/^(products|collections|occasions|articles)$/.test(pageName)) return { deliveryStage: "explore", covers: [] };
    if (/^(contact|subscribe)$/.test(pageName)) return { deliveryStage: "act", covers: [] };
    if (/^(cart|checkout|order-result|account|orders|order-detail|login|register)$/.test(pageName)) {
        return { deliveryStage: "task", covers: [] };
    }
    if (pageName === "about") return { deliveryStage: "evaluate", covers: ["trust"] };
    return { deliveryStage: "evaluate", covers: [] };
}

function buildGenericPageBlueprint(pageName, siteType, commerceModel) {
    var storefrontRecipe = /^ecommerce-/.test(siteType) ? storefrontRecipeFor(pageName, commerceModel) : null;
    var deliveryMeta = defaultDeliveryMetaForPageName(pageName, siteType, commerceModel);
    return {
        name: pageName,
        purpose: storefrontRecipe ? storefrontRecipe.purpose : "展示 " + pageName + " 相关内容并承接下一步行动",
        blocks: defaultBlocksForPageName(pageName, siteType, commerceModel),
        deliveryStage: deliveryMeta.deliveryStage,
        covers: deliveryMeta.covers
    };
}

function inferSiteTypeFromLegacyQuery(query, category) {
    var c = (String(query || "") + " " + String(category || "")).toLowerCase();
    if (isSaasLike(query, category)) return "saas-product";
    if (isPortfolioLike(query, category)) return "portfolio-creative";
    if (isServiceLike(query, category)) return "local-service";
    if (isCommerceLike(query, category)) {
        if (/(luxury|premium|exclusive|high-end|奢华|高端|精品)/.test(c)) return "ecommerce-luxury";
        if (/(florist|flower|plant|bouquet|floral|鲜花|花店|花束|绿植)/.test(c)) return "ecommerce-florist";
        return "ecommerce-catalog";
    }
    if (/(newsletter|publication|subscribe|content|blog)/.test(c)) return "content-publication";
    if (/(landing|lead|waitlist|coming soon)/.test(c)) return "landing-leadgen";
    return "general-multipage";
}

function normalizeCommerceModel(value) {
    var v = String(value || "").toLowerCase();
    if (v === "catalog-contact" || v === "online-checkout") return v;
    if (/展示联系|联系购买|catalog.contact|catalog-contact|询价|咨询购买/.test(v)) return "catalog-contact";
    if (/在线下单|online.checkout|online-checkout|购物车|在线购买/.test(v)) return "online-checkout";
    if (/联系|contact|catalog|展示|询价|咨询/.test(v)) return "catalog-contact";
    if (/在线|checkout|cart|下单|order/.test(v)) return "online-checkout";
    return v ? "catalog-contact" : "";
}

function applyCommerceModelToPage(page, commerceModel, siteType) {
    if (!/^ecommerce-/.test(siteType)) return page;
    var model = normalizeCommerceModel(commerceModel);
    if (!model) return page;
    var storefrontRecipe = storefrontRecipeFor(page.name, model);
    var blocks = storefrontRecipe ? storefrontRecipe.blocks.slice() : page.blocks.slice();
    return {
        name: page.name,
        purpose: storefrontRecipe ? storefrontRecipe.purpose : page.purpose,
        blocks: blocks,
        deliveryStage: storefrontRecipe && storefrontRecipe.deliveryStage
            ? storefrontRecipe.deliveryStage
            : page.deliveryStage,
        covers: storefrontRecipe && storefrontRecipe.covers
            ? storefrontRecipe.covers.slice()
            : (page.covers || []).slice()
    };
}

function clonePageBlueprint(page) {
    return {
        name: page.name,
        purpose: page.purpose,
        blocks: (page.blocks || []).slice(),
        deliveryStage: page.deliveryStage || "evaluate",
        covers: (page.covers || []).slice()
    };
}

function ensureCommercePlanningPages(pages, commerceModel, siteType) {
    if (!/^ecommerce-/.test(siteType)) return pages;
    var result = pages.slice();
    var hasExplore = false;
    var hasEvaluate = false;
    for (var i = 0; i < result.length; i++) {
        hasExplore = hasExplore || result[i].deliveryStage === "explore";
        hasEvaluate = hasEvaluate || result[i].deliveryStage === "evaluate";
    }
    if (!hasExplore) result.push(buildGenericPageBlueprint("products", siteType, commerceModel));
    if (!hasEvaluate) result.push(buildGenericPageBlueprint("product-detail", siteType, commerceModel));
    return result;
}

var INITIAL_DELIVERY_POLICY = {
    maxPages: 3,
    maxPageBlocks: 9,
    stages: ["entry", "explore", "evaluate", "act", "trust"]
};

function pageLevelBlockCount(page) {
    var count = 0;
    var blocks = page.blocks || [];
    for (var i = 0; i < blocks.length; i++) {
        if (blocks[i] !== "header" && blocks[i] !== "footer") count++;
    }
    return count;
}

function addPageCoverage(coverage, page) {
    coverage[page.deliveryStage || "evaluate"] = true;
    var covers = page.covers || [];
    for (var i = 0; i < covers.length; i++) coverage[covers[i]] = true;
}

function selectInitialDeliveryPages(pages) {
    if (!pages.length) return pages;
    var selected = [];
    var selectedNames = {};
    var coverage = {};
    var pageBlockCount = 0;

    function add(page, required) {
        if (!page || selectedNames[page.name] || selected.length >= INITIAL_DELIVERY_POLICY.maxPages) return false;
        var nextBlockCount = pageBlockCount + pageLevelBlockCount(page);
        if (!required && nextBlockCount > INITIAL_DELIVERY_POLICY.maxPageBlocks) return false;
        selected.push(page);
        selectedNames[page.name] = true;
        pageBlockCount = nextBlockCount;
        addPageCoverage(coverage, page);
        return true;
    }

    function firstCandidateForStage(stage) {
        for (var i = 0; i < pages.length; i++) {
            if (!selectedNames[pages[i].name] && pages[i].deliveryStage === stage) return pages[i];
        }
        return null;
    }

    var entry = firstCandidateForStage("entry");
    if (!entry) {
        for (var i = 0; i < pages.length; i++) {
            if (pages[i].deliveryStage !== "task") {
                entry = pages[i];
                break;
            }
        }
    }
    add(entry, true);

    for (var stageIndex = 1; stageIndex < INITIAL_DELIVERY_POLICY.stages.length; stageIndex++) {
        var stage = INITIAL_DELIVERY_POLICY.stages[stageIndex];
        if (coverage[stage]) continue;
        var candidate = firstCandidateForStage(stage);
        if (!candidate) continue;
        var requiredJourneyStage = stage === "explore" || stage === "evaluate";
        add(candidate, requiredJourneyStage);
    }

    if (selected.length === 1) {
        for (var fallbackIndex = 0; fallbackIndex < pages.length; fallbackIndex++) {
            var fallback = pages[fallbackIndex];
            if (fallback.deliveryStage !== "task" && add(fallback, false)) break;
        }
    }
    return selected;
}

function pageBlueprintsForSiteType(siteType, requestedPages, commerceModel, category) {
    var template = SITE_BLUEPRINTS[siteType] || SITE_BLUEPRINTS["general-multipage"];
    var cloned = [];
    var i;
    for (i = 0; i < template.length; i++) {
        var page = clonePageBlueprint(template[i]);
        cloned.push(applyCommerceModelToPage(page, commerceModel, siteType));
    }
    cloned = ensureCommercePlanningPages(cloned, commerceModel, siteType);
    if (!requestedPages || !requestedPages.length) return selectInitialDeliveryPages(cloned);
    var byName = {};
    for (i = 0; i < cloned.length; i++) byName[cloned[i].name] = cloned[i];
    var out = [];
    var j;
    for (j = 0; j < requestedPages.length; j++) {
        var pageName = requestedPages[j];
        if (byName[pageName]) out.push(byName[pageName]);
        else out.push(applyCommerceModelToPage(buildGenericPageBlueprint(pageName, siteType, commerceModel), commerceModel, siteType));
    }
    return out.length ? out : selectInitialDeliveryPages(cloned);
}

function pageBlueprintsFor(query, category) {
    return pageBlueprintsForSiteType(inferSiteTypeFromLegacyQuery(query, category), null, null, category);
}

function augmentPromptSearchQuery(sectionType, pageName, query, ds) {
    var parts = [sectionType, query, pageName, ds.category, ds.pattern.name, ds.style.name, ds.style.keywords];
    var c = (String(query || "") + " " + String(ds.category || "")).toLowerCase();
    if (sectionType === "hero") {
        if (isCommerceLike(query, ds.category)) parts.push(COMMERCE_HERO_QUERY_TERMS);
        if (isSaasLike(query, ds.category)) parts.push("saas demo software product preview app");
        if (isServiceLike(query, ds.category)) parts.push("service local booking consultation spa clinic");
        if (isPortfolioLike(query, ds.category)) parts.push("portfolio studio agency case creative gallery");
        if (/(newsletter|publication|subscribe|content site)/.test(c)) parts.push("newsletter paper publication subscribe editorial");
        if (/(restaurant|dining|menu|food)/.test(c)) parts.push("restaurant menu dining reservation");
        if (/(event|conference|ticket|schedule)/.test(c)) parts.push("event date conference schedule ticket");
        if (/(marketplace|directory|vendors)/.test(c)) parts.push("marketplace search directory categories");
        if (/(real estate|property|listing|home search)/.test(c)) parts.push("real estate listing property");
    }
    return parts.join(" ");
}

function prepareSectionPromptCandidates(sectionType, pageName, query, ds, context) {
    var sectionKey = normalizeKey(sectionType);
    var heroFacets = (context && context.heroFacets) || (sectionType === "hero" ? buildHeroFacets(context && context.brief, ds, pageName) : null);
    var searchQuery = sectionType === "hero" && heroFacets
        ? buildHeroSearchQueryFromFacets(heroFacets, ds, sectionType, pageName, query)
        : augmentPromptSearchQuery(sectionType, pageName, query, ds);

    var index = getUiPromptIndex();
    var sectionPool = (index.bySection[sectionKey] || []).slice();
    if (!sectionPool.length) sectionPool = index.rows.slice();
    var filtered = sectionPool.slice();
    if (sectionType === "header" || sectionType === "footer") {
        filtered = filterSharedChromeCandidates(filtered, sectionType, ds);
    }
    filtered = filterCompatibleCandidates(filtered, ds);
    if (sectionType === "hero") {
        var heroCompatiblePool = filtered.slice();
        var heroBroadPool = sectionPool.slice();
        filtered = filterHeroCandidatesByGoal(filtered, heroFacets);
        var topoSet = {};
        var t;
        for (t = 0; t < filtered.length; t++) {
            topoSet[desktopTopologyOf(filtered[t])] = true;
        }
        if (Object.keys(topoSet).length < 2) {
            filtered = (index.bySection[sectionKey] || []).slice();
            if (!filtered.length) filtered = index.rows.slice();
            filtered = filterHeroCandidatesByGoal(filtered, heroFacets);
            topoSet = {};
            for (t = 0; t < filtered.length; t++) {
                topoSet[desktopTopologyOf(filtered[t])] = true;
            }
        }
        if (Object.keys(topoSet).length < HERO_TOPOLOGY_TARGET) {
            var goalFamilySet = {};
            var gt;
            for (gt in topoSet) {
                if (Object.prototype.hasOwnProperty.call(topoSet, gt)) goalFamilySet[heroTopologyFamilyOf(gt)] = true;
            }
            var haveHeroIds = {};
            for (t = 0; t < filtered.length; t++) {
                haveHeroIds[filtered[t]["Prompt ID"]] = true;
            }
            var backfillPools = [heroCompatiblePool, heroBroadPool];
            var bp;
            for (bp = 0; bp < backfillPools.length && Object.keys(goalFamilySet).length < HERO_TOPOLOGY_TARGET; bp++) {
                var c;
                for (c = 0; c < backfillPools[bp].length && Object.keys(goalFamilySet).length < HERO_TOPOLOGY_TARGET; c++) {
                    var heroCand = backfillPools[bp][c];
                    var candFamily = heroTopologyFamilyOf(desktopTopologyOf(heroCand));
                    if (goalFamilySet[candFamily]) continue;
                    if (haveHeroIds[heroCand["Prompt ID"]]) continue;
                    filtered.push(heroCand);
                    haveHeroIds[heroCand["Prompt ID"]] = true;
                    goalFamilySet[candFamily] = true;
                }
            }
        }
    }

    var docs = [];
    var j;
    for (j = 0; j < filtered.length; j++) {
        docs.push(buildPromptSearchDoc(filtered[j]));
    }
    var ranked = bm25Rank(searchQuery, docs);
    return {
        filtered: filtered,
        ranked: ranked,
        searchQuery: searchQuery,
        heroFacets: heroFacets
    };
}

function rankSectionPromptCandidates(pool, context, options) {
    options = options || {};
    var limit = options.limit || 5;
    var excludeSet = {};
    var excludeIds = options.excludePromptIds || [];
    var e;
    for (e = 0; e < excludeIds.length; e++) excludeSet[excludeIds[e]] = true;

    var scored = buildRescoredPromptPool(pool.ranked, pool.filtered, context, { forRecommendations: true });
    var candidates = pickDiverseRecommendationCandidates(scored.rescored, context.sectionType, limit, excludeSet);
    return {
        candidates: candidates,
        topBm25: scored.topBm25,
        filteredOut: scored.filteredOut,
        searchQuery: pool.searchQuery,
        heroFacets: pool.heroFacets
    };
}

function recommendationFromScoredCandidate(candidate, sectionType, contentLocale, rank) {
    var row = candidate.row || {};
    var promptId = row["Prompt ID"] || "";
    var resolved = resolvePromptText(row, contentLocale);
    var scope = row["Scope"] || defaultScopeFor(sectionType);
    var createPhase = row["Create Phase"] || defaultCreatePhaseFor(sectionType);
    var imagePolicy = row["Image Policy"] || defaultImagePolicyFor(sectionType);
    var dataMode = row["Data Mode"] || defaultDataModeFor(sectionType);
    var interactionPolicy = row["Interaction Policy"] || defaultInteractionPolicyFor(sectionType);
    var variantBrief = row["Variant Brief"] || "";
    return {
        rank: rank,
        score: candidate.score,
        bm25Score: candidate.bm25,
        bonus: candidate.bonus,
        promptId: promptId,
        promptName: row["Name"] || blockLabel(sectionType),
        layoutArchetype: row["Layout Archetype"] || "",
        contentHierarchy: row["Content Hierarchy"] || "",
        density: row["Density"] || "",
        conversionGoal: row["Conversion Goal"] || "",
        visualTreatment: row["Visual Treatment"] || "",
        compatibleStyles: splitList(row["Compatible Styles"]),
        requiredContent: splitList(row["Required Content"]),
        variantBrief: variantBrief,
        layoutIntent: row["Layout Intent"] || "",
        avoidPatterns: splitList(row["Avoid Patterns"]),
        prompt: {
            kind: "section-generation",
            locale: resolved.locale,
            text: resolved.text,
            constraints: row["Kooboo Constraints"] || "Plain HTML + Tailwind; no external JS; no emoji."
        },
        generationContract: {
            output: "Return one Kooboo View HTML section only, without Page/Layout wrappers.",
            scope: scope,
            createPhase: createPhase,
            imagePolicy: imagePolicy,
            dataMode: dataMode,
            interactionPolicy: interactionPolicy,
            variantBrief: variantBrief,
            selectionContract: buildSelectionContractFields(row),
            visualDiversityDirective: buildVisualDiversityDirective(row, sectionType),
            layoutIntent: row["Layout Intent"] || "",
            avoidPatterns: splitList(row["Avoid Patterns"]),
            refinement: "Generate from the prompt first, then run Preflight and refine the View."
        },
        selectionMeta: appendSelectionContractMeta({
            bm25Score: candidate.bm25,
            bonus: candidate.bonus,
            diversityPenalty: candidate.diversityPenalty,
            diversityReason: candidate.diversityReason || "",
            desktopTopology: candidate.desktopTopology || desktopTopologyOf(row),
            layoutArchetype: candidate.archetype || layoutArchetypeOf(row),
            promptLocaleFallback: resolved.promptLocaleFallback,
            requestedLocale: resolved.requestedLocale
        }, row),
        reason: candidate.diversityReason
            ? "score-ranked;" + candidate.diversityReason
            : "score-ranked"
    };
}

function generateRecommendSectionPromptsData(options) {
    var sectionType = options.sectionType || "hero";
    var pageName = options.pageName || "home";
    var limit = options.limit || 5;
    var brief = options.brief || null;
    var query = options.query || "";
    var projectName = options.projectName || "";
    var excludePromptIds = options.excludePromptIds || [];
    var contentLocale = options.contentLocale || (brief && brief.contentLocale) || resolveContentLocale() || "zh";
    var ds;

    if (brief) {
        var briefLocaleError = validateBriefContentLocale(brief);
        if (briefLocaleError) return { error: briefLocaleError };
        ds = buildDesignSystemFromBrief(brief, projectName);
        if (ds.error) return { error: ds.error };
        query = buildBriefSearchContext(brief);
    } else {
        ds = buildDesignSystemData(query, projectName);
    }

    var scope = defaultScopeFor(sectionType);
    var context = {
        variantSeed: options.variantSeed || ds.designVariantSeed || projectName || query,
        occurrence: 0,
        scope: scope,
        isShared: isSharedBlockScope(scope),
        sectionType: sectionType,
        pageName: pageName,
        ds: ds,
        brief: brief,
        usedArchetypesOnPage: options.usedArchetypesOnPage || [],
        usedDesktopTopologiesOnPage: options.usedDesktopTopologiesOnPage || []
    };

    var pool = prepareSectionPromptCandidates(sectionType, pageName, query, ds, context);
    var ranked = rankSectionPromptCandidates(pool, context, {
        limit: limit,
        excludePromptIds: excludePromptIds
    });
    var recommendations = [];
    var i;
    for (i = 0; i < ranked.candidates.length; i++) {
        recommendations.push(recommendationFromScoredCandidate(
            ranked.candidates[i],
            sectionType,
            contentLocale,
            ranked.candidates[i].rank || (i + 1)
        ));
    }

    var compactedClassification = compactClassification(ds.classification || null);
    return {
        artifact: "section-prompt-recommendations",
        schemaVersion: 1,
        sectionType: sectionType,
        pageName: pageName,
        query: query,
        searchQuery: ranked.searchQuery,
        siteBrief: brief || ds.siteBrief || null,
        category: ds.category,
        categoryConfidence: compactedClassification ? compactedClassification.categoryConfidence : "unknown",
        contentLocale: contentLocale,
        designSystem: {
            style: ds.style.name,
            colorMood: ds.colors.notes || ds.colors.primary,
            paletteVariantId: ds.paletteVariantId || "",
            typography: ds.typography.heading + " / " + ds.typography.body,
            themeMode: brief ? brief.themeMode : "light",
            creativeDirection: brief ? brief.creativeDirection : ""
        },
        excludePromptIds: excludePromptIds,
        recommendations: recommendations,
        selectionMeta: {
            candidatePoolSize: pool.filtered.length,
            qualifiedCount: ranked.candidates.length,
            topBm25Score: ranked.topBm25,
            heroFacets: ranked.heroFacets || null,
            filteredReasons: (ranked.filteredOut || []).slice(0, 5)
        }
    };
}

function selectSectionPrompt(sectionType, pageName, query, ds, context) {
    var sectionKey = normalizeKey(sectionType);
    var scope = (context && context.scope) || defaultScopeFor(sectionType);
    var isShared = isSharedBlockScope(scope);
    var sharedCache = context && context.sharedSelectionCache;

    if (isShared && sharedCache && sharedCache[sectionKey]) {
        return sharedCache[sectionKey];
    }

    var pool = prepareSectionPromptCandidates(sectionType, pageName, query, ds, context);
    var picked = pickPromptCandidate(pool.ranked, pool.filtered, {
        ds: ds,
        pageName: pageName,
        sectionType: sectionType,
        scope: scope,
        isShared: isShared,
        variantSeed: (context && context.variantSeed) || ds.designVariantSeed || ds.project_name || query,
        occurrence: isShared ? 0 : ((context && context.occurrence) || 0),
        usedArchetypesOnPage: (context && context.usedArchetypesOnPage) || [],
        usedDesktopTopologiesOnPage: (context && context.usedDesktopTopologiesOnPage) || [],
        heroFacets: pool.heroFacets,
        brief: (context && context.brief) || null
    });

    if (isShared && sharedCache) {
        sharedCache[sectionKey] = picked;
    }
    return picked;
}

function blockFromPromptRow(row, sectionType, pageName, contentLocale, picked) {
    picked = picked || {};
    row = picked.row || row || {};
    var promptId = row["Prompt ID"] || (sectionType + ".prompt.generic");
    var requiredContent = splitList(row["Required Content"]);
    var preflightTags = splitList(row["Preflight Tags"]);
    var scope = row["Scope"] || defaultScopeFor(sectionType);
    var createPhase = row["Create Phase"] || defaultCreatePhaseFor(sectionType);
    var imagePolicy = row["Image Policy"] || defaultImagePolicyFor(sectionType);
    var dataMode = row["Data Mode"] || defaultDataModeFor(sectionType);
    var interactionPolicy = row["Interaction Policy"] || defaultInteractionPolicyFor(sectionType);
    var variantBrief = row["Variant Brief"] || "";
    var selectionContract = buildSelectionContractFields(row);
    var visualDiversityDirective = buildVisualDiversityDirective(row, sectionType);
    var gridFillDirective = buildGridFillDirective(row, sectionType);
    var layoutIntent = row["Layout Intent"] || "";
    var avoidPatterns = splitList(row["Avoid Patterns"]);
    var block = {
        type: sectionType,
        label: blockLabel(sectionType),
        promptId: promptId,
        promptName: row["Name"] || blockLabel(sectionType),
        scope: scope,
        createPhase: createPhase,
        requiredContent: requiredContent,
        styleTags: splitList(row["Style Tags"]),
        selectionReason: picked.selectionReason || "default",
        generationContract: {
            output: "Return one Kooboo View HTML section only, without Page/Layout wrappers.",
            scope: scope,
            createPhase: createPhase,
            imagePolicy: imagePolicy,
            dataMode: dataMode,
            interactionPolicy: interactionPolicy,
            variantBrief: variantBrief,
            selectionContract: selectionContract,
            visualDiversityDirective: visualDiversityDirective,
            layoutIntent: layoutIntent,
            avoidPatterns: avoidPatterns,
            refinement: "Generate from the prompt first, then run Preflight and refine the View."
        },
        preflightTags: preflightTags,
        bestFor: row["Best For"] || "",
        layoutSelection: selectionContract
    };

    var resolved = resolvePromptText(row, contentLocale);
    var constraints = row["Kooboo Constraints"] || "Plain HTML + Tailwind; no external JS; no emoji.";
    constraints += " Visual contract: " + visualDiversityDirective;
    if (layoutIntent) constraints += " Layout intent: " + layoutIntent;
    if (gridFillDirective) constraints += " " + gridFillDirective;
    if (avoidPatterns.length) constraints += " Avoid patterns: " + avoidPatterns.join("; ");
    block.prompt = {
        kind: "section-generation",
        locale: resolved.locale,
        text: resolved.text,
        constraints: constraints
    };
    return block;
}

function blockFromPrompt(sectionType, pageName, query, ds, context) {
    var picked = selectSectionPrompt(sectionType, pageName, query, ds, context);
    return blockFromPromptRow(picked.row || {}, sectionType, pageName, context && context.contentLocale, picked);
}

function assemblePagePlan(ds, blueprints, searchContext, projectName, brief, options) {
    options = options || {};
    var detail = normalizePlanDetail(options.detail || "summary");
    var enrichedBrief = brief ? enrichSiteBrief(brief, projectName) : null;
    var variantSeed = enrichedBrief
        ? enrichedBrief.designVariantSeed
        : (getArg("designVariantSeed", "") || projectName || searchContext);
    var contentLocale = enrichedBrief ? enrichedBrief.contentLocale : (resolveContentLocale() || "");
    var siteType = enrichedBrief ? enrichedBrief.siteType : inferSiteTypeFromLegacyQuery(searchContext, ds.category);
    var pages = [];
    var sectionOccurrence = {};
    var sharedSelectionCache = {};
    var i;

    for (i = 0; i < blueprints.length; i++) {
        var bp = blueprints[i];
        var blocks = [];
        var usedArchetypesOnPage = [];
        var usedDesktopTopologiesOnPage = [];
        var localBlockOccurrence = {};
        var j;
        for (j = 0; j < bp.blocks.length; j++) {
            var sectionType = bp.blocks[j];
            var scope = defaultScopeFor(sectionType);
            var isShared = isSharedBlockScope(scope);
            var occurrence = sectionOccurrence[sectionType] || 0;
            if (!isShared) sectionOccurrence[sectionType] = occurrence + 1;
            var localOccurrence = localBlockOccurrence[sectionType] || 0;
            localBlockOccurrence[sectionType] = localOccurrence + 1;
            var block = blockFromPrompt(sectionType, bp.name, searchContext, ds, {
                variantSeed: variantSeed,
                occurrence: isShared ? 0 : occurrence,
                scope: scope,
                sharedSelectionCache: sharedSelectionCache,
                usedArchetypesOnPage: usedArchetypesOnPage,
                usedDesktopTopologiesOnPage: usedDesktopTopologiesOnPage,
                contentLocale: contentLocale,
                brief: enrichedBrief
            });
            applyCommerceBlockRequirements(block, siteType);
            block.blockRef = blockRefFor(bp.name, sectionType, block, localOccurrence);
            blocks.push(block);
            var layoutSelection = block.layoutSelection || {};
            if (layoutSelection.layoutArchetype) {
                usedArchetypesOnPage.push(layoutSelection.layoutArchetype);
            }
            if (layoutSelection.desktopTopology) {
                usedDesktopTopologiesOnPage.push(layoutSelection.desktopTopology);
            }
        }
        pages.push({
            name: bp.name,
            title: pageTitle(bp.name),
            purpose: bp.purpose,
            blocks: blocks
        });
    }

    var classification = ds.classification || null;
    var compactedClassification = compactClassification(classification);
    var decisionReceipts = options.decisionReceipts || [];
    var plan = {
        version: 3,
        projectName: ds.project_name,
        resolvedBrief: enrichedBrief ? Object.assign({}, enrichedBrief, { projectName: ds.project_name }) : null,
        decisionReceipts: decisionReceipts,
        siteType: siteType,
        designVariantSeed: variantSeed,
        contentLocale: contentLocale,
        category: ds.category,
        categoryConfidence: compactedClassification ? compactedClassification.categoryConfidence : "unknown",
        classification: compactedClassification,
        pattern: ds.pattern.name,
        designSystem: {
            style: ds.style.name,
            colorMood: ds.colors.notes || ds.colors.primary,
            paletteVariantId: ds.paletteVariantId || "",
            typography: ds.typography.heading + " / " + ds.typography.body,
            themeMode: enrichedBrief ? enrichedBrief.themeMode : "light",
            creativeDirection: enrichedBrief ? enrichedBrief.creativeDirection : ""
        },
        pages: pages
    };
    if (detail === "full") return plan;
    return summarizePagePlan(plan);
}

function generatePagePlanData(query, projectName, options) {
    var ds = buildDesignSystemData(query, projectName);
    var blueprints = pageBlueprintsFor(query, ds.category);
    return assemblePagePlan(ds, blueprints, query, projectName, null, options);
}

function generatePagePlanDataFromBrief(brief, projectName, options) {
    options = options || {};
    var enriched = enrichSiteBrief(brief, projectName);
    var seedError = requireDesignVariantSeed(enriched);
    if (seedError) return { error: seedError };
    var decisionReceipts = [];
    if (options.commerceModelDecisionSource) {
        decisionReceipts.push({
            field: "commerceModel",
            source: options.commerceModelDecisionSource
        });
    }
    if (!brief.designVariantSeed) decisionReceipts.push({ field: "designVariantSeed", source: hasArg("designVariantSeed") ? "designVariantSeed" : "sessionId" });
    if (!brief.creativeDirection && enriched.creativeDirection) decisionReceipts.push({ field: "creativeDirection", source: "designVariantSeed" });
    if (!brief.palettePreference && enriched.palettePreference) decisionReceipts.push({ field: "palettePreference", source: "creativeDirection" });
    if (!brief.projectName) decisionReceipts.push({ field: "projectName", source: "designVariantSeed" });
    var ds = buildDesignSystemFromBrief(enriched, projectName);
    if (ds.error) return ds;
    var blueprints = pageBlueprintsForSiteType(enriched.siteType, enriched.pages, enriched.commerceModel, ds.category);
    var resolvedBrief = Object.assign({}, enriched, {
        pages: blueprints.map(function (page) { return page.name; })
    });
    if (!brief.pages || !brief.pages.length) {
        decisionReceipts.push({ field: "pages", source: "initial-delivery-policy" });
    }
    options.decisionReceipts = decisionReceipts;
    return assemblePagePlan(ds, blueprints, buildBriefSearchContext(resolvedBrief), projectName, resolvedBrief, options);
}

function generateFullPlanForRequest(query, projectName, brief) {
    if (brief) return generatePagePlanDataFromBrief(brief, projectName, { detail: "full" });
    return generatePagePlanData(query, projectName, { detail: "full" });
}

function findPageInPlan(plan, pageName) {
    var target = normalizeKey(pageName || "home");
    for (var i = 0; i < plan.pages.length; i++) {
        if (normalizeKey(plan.pages[i].name) === target) return plan.pages[i];
    }
    return null;
}

function findBlockInPlan(plan, options) {
    options = options || {};
    var blockRef = String(options.blockRef || "");
    var promptId = String(options.promptId || "");
    var pageName = String(options.pageName || "");
    var sectionType = String(options.sectionType || "");
    for (var i = 0; i < plan.pages.length; i++) {
        var page = plan.pages[i];
        if (pageName && normalizeKey(page.name) !== normalizeKey(pageName)) continue;
        for (var j = 0; j < page.blocks.length; j++) {
            var block = page.blocks[j];
            if (blockRef && block.blockRef === blockRef) return { page: page, block: block };
            if (promptId && block.promptId === promptId) {
                if (sectionType && normalizeKey(block.type) !== normalizeKey(sectionType)) continue;
                return { page: page, block: block };
            }
        }
    }
    return null;
}

function pageViewManifest(page) {
    var blockRefs = [];
    for (var i = 0; i < page.blocks.length; i++) {
        var block = page.blocks[i];
        if (!isSharedBlockScope(block.scope)) blockRefs.push(block.blockRef || "");
    }
    return blockRefs;
}

var PAGE_COMPOSITION_PATTERNS = [
    {
        groupKey: "product-decision",
        primaryType: "product-gallery",
        asideType: "product-summary",
        layoutIntent: "Place product media and the purchase summary side by side on desktop, with media first and the purchase summary immediately adjacent. Stack media before the summary on smaller screens."
    },
    {
        groupKey: "cart-decision",
        primaryType: "cart-items",
        asideType: "checkout-summary",
        layoutIntent: "Place editable cart items in the main desktop column and the totals or checkout summary in an adjacent aside. Stack items before the summary on smaller screens."
    },
    {
        groupKey: "checkout-decision",
        primaryType: "checkout-form",
        asideType: "checkout-summary",
        layoutIntent: "Place the checkout form in the main desktop column and the order summary in an adjacent review aside. Stack the form before the summary on smaller screens."
    },
    {
        groupKey: "order-review",
        primaryType: "order-status",
        asideType: "checkout-summary",
        layoutIntent: "Place order status and detail content in the main desktop column and the amount summary in an adjacent review aside. Stack status before the summary on smaller screens."
    }
];

function findPageBlockByType(page, blockType) {
    for (var i = 0; i < page.blocks.length; i++) {
        var block = page.blocks[i];
        if (!isSharedBlockScope(block.scope) && block.type === blockType) return block;
    }
    return null;
}

function pageCompositionGroups(page) {
    var groups = [];
    for (var i = 0; i < PAGE_COMPOSITION_PATTERNS.length; i++) {
        var pattern = PAGE_COMPOSITION_PATTERNS[i];
        var primary = findPageBlockByType(page, pattern.primaryType);
        var aside = findPageBlockByType(page, pattern.asideType);
        if (!primary || !aside) continue;
        groups.push({
            groupRef: normalizeKey(page.name) + "-" + pattern.groupKey,
            blockRefs: [primary.blockRef, aside.blockRef],
            primaryBlockRef: primary.blockRef,
            asideBlockRef: aside.blockRef,
            desktopTopology: "primary-aside",
            responsiveStrategy: "stack-primary-first",
            layoutIntent: pattern.layoutIntent
        });
    }
    return groups;
}

function findCompositionGroupForBlock(page, block) {
    var groups = pageCompositionGroups(page);
    for (var i = 0; i < groups.length; i++) {
        if (groups[i].blockRefs.indexOf(block.blockRef) >= 0) return groups[i];
    }
    return null;
}

function effectiveRequiredContent(block, page) {
    var required = (block.requiredContent || []).slice();
    var group = findCompositionGroupForBlock(page, block);
    if (!group || block.type !== "product-summary") return required;
    return required.filter(function (item) {
        var key = String(item || "").toLowerCase().replace(/[^a-z]/g, "");
        return key !== "productimage" && key !== "productmedia";
    });
}

function effectiveImagePolicy(block, page) {
    var policy = (block.generationContract || {}).imagePolicy || defaultImagePolicyFor(block.type);
    var group = findCompositionGroupForBlock(page, block);
    if (group && block.type === "product-summary" && policy === "required") return "optional";
    return policy;
}

function composePagePlacementHint(page, block) {
    var group = findCompositionGroupForBlock(page, block);
    if (!group) return "";
    var role = group.primaryBlockRef === block.blockRef ? "primary" : "aside";
    var lines = [
        "Page placement contract:",
        "- This View is the " + role + " member of Page composition group " + group.groupRef + ".",
        "- The Page, not this View, owns the cross-View wrapper and will place both members side by side on desktop.",
        "- Keep the View fluid within its assigned column. Do not add page-wide outer layout, duplicate the sibling View, or assume this View is a full-width standalone section.",
        "- " + group.layoutIntent
    ];
    if (block.type === "product-gallery") {
        lines.push("- Own the primary product media. Do not turn the gallery into a long full-width editorial sequence that pushes the purchase summary below the fold.");
    } else if (block.type === "product-summary") {
        lines.push("- Own product identity, price, options, availability, quantity, and the primary action. The sibling gallery owns the dominant product media; omit a second dominant product image even if the selected variant normally includes one. This page placement rule overrides any productImage or productMedia request inside the selected standalone variant.");
    } else if (block.type === "cart-items") {
        lines.push("- Own editable line items. Keep final totals and the checkout action in the sibling summary aside.");
    } else if (block.type === "checkout-form") {
        lines.push("- Own contact, address, shipping, payment choice, consent, and validation fields. Do not embed a second full order-summary rail.");
    } else if (block.type === "checkout-summary") {
        lines.push("- Render as a compact review aside with clear totals and action visibility, not as another full-width page section.");
    }
    return lines.join("\n");
}

function generatePageItemData(query, projectName, brief, pageName) {
    var plan = generateFullPlanForRequest(query, projectName, brief);
    if (plan.error) return plan;
    var page = findPageInPlan(plan, pageName || "home");
    if (!page) return { error: "Page not found in pagePlan: " + (pageName || "home") };
    var result = {
        version: 3,
        pageName: page.name,
        blockRefs: pageViewManifest(page)
    };
    var compositionGroups = pageCompositionGroups(page);
    if (compositionGroups.length) result.compositionGroups = compositionGroups;
    return result;
}

function composeViewPrompt(block, page) {
    var prompt = block.prompt || {};
    var contract = block.generationContract || {};
    var parts = [];
    parts.push(contract.output || "Return one Kooboo View HTML section only, without Page/Layout wrappers.");
    if (prompt.text) parts.push(prompt.text);
    if (prompt.constraints) parts.push("Constraints:\n" + prompt.constraints);
    var requiredContent = effectiveRequiredContent(block, page);
    if (requiredContent.length) {
        parts.push("Required content:\n- " + requiredContent.join("\n- "));
    }
    if (contract.variantBrief) parts.push("Variant direction:\n" + contract.variantBrief);
    var placementHint = composePagePlacementHint(page, block);
    if (placementHint) parts.push(placementHint);
    parts.push([
        "Implementation policies:",
        "- imagePolicy=" + effectiveImagePolicy(block, page),
        "- dataMode=" + (contract.dataMode || defaultDataModeFor(block.type)),
        "- interactionPolicy=" + (contract.interactionPolicy || defaultInteractionPolicyFor(block.type))
    ].join("\n"));
    return parts.join("\n\n");
}

function composeViewPromptDelta(block, page) {
    var full = composeViewPrompt(block, page);
    var base = (block.generationContract || {}).output || "Return one Kooboo View HTML section only, without Page/Layout wrappers.";
    if (full.indexOf(base + "\n\n") === 0) return full.slice(base.length + 2);
    return full;
}

function generateViewBatchData(query, projectName, brief, blockRefs) {
    var refs = parsePlanReferences(blockRefs, "blockRefs", "blockRef", function (ref) { return ref.trim(); }, false);
    if (refs.error) return refs;
    blockRefs = refs.values;
    var plan = generateFullPlanForRequest(query, projectName, brief);
    if (plan.error) return plan;
    var items = [];
    for (var i = 0; i < blockRefs.length; i++) {
        var blockRef = blockRefs[i];
        var found = findBlockInPlan(plan, { blockRef: blockRef });
        if (!found) return { error: "Unknown blockRef: " + blockRef };
        items.push({
            blockRef: blockRef,
            recommendedViewName: recommendedViewNameForBlock(found.page.name, found.block),
            prompt: composeViewPromptDelta(found.block, found.page)
        });
    }
    return {
        version: 3,
        baseContract: "Return one Kooboo View HTML section only, without Page/Layout wrappers.",
        items: items
    };
}

function compactClassification(classification) {
    if (!classification) return null;

    var topCandidates = (classification.topCandidates || []).map(function (item) {
        return {
            productType: item.productType || "",
            score: item.score || 0,
            keywordHitRatio: item.keywordHitRatio || 0
        };
    });

    return {
        category: classification.category || "General",
        categoryConfidence: classification.categoryConfidence || "unknown",
        keywordHitCount: classification.keywordHitCount || 0,
        keywordHitRatio: classification.keywordHitRatio || 0,
        topCandidates: topCandidates,
        source: classification.source || ""
    };
}

function compactDesignSystemV3(ds) {
    return {
        version: 3,
        projectName: ds.project_name,
        designVariantSeed: ds.designVariantSeed || "",
        classification: compactClassification(ds.classification || null),
        pattern: {
            name: ds.pattern.name,
            sections: ds.pattern.sections,
            ctaPlacement: ds.pattern.cta_placement
        },
        style: {
            name: ds.style.name,
            effects: ds.style.effects
        },
        colors: ds.colors,
        typography: {
            heading: ds.typography.heading,
            body: ds.typography.body,
            googleFontsUrl: ds.typography.google_fonts_url,
            cssImport: ds.typography.css_import
        },
        keyEffects: ds.key_effects,
        antiPatterns: ds.anti_patterns
    };
}

function formatPagePlanMarkdown(plan) {
    var lines = [];
    lines.push("# Page Plan: " + plan.projectName);
    lines.push("");
    lines.push("首版站点计划：" + plan.pages.length + " 个页面");
    lines.push("");
    lines.push("- **Category:** " + plan.category);
    lines.push("- **Pattern:** " + plan.pattern);
    lines.push("- **确认方式:** " + plan.confirmationPrompt);
    lines.push("");
    for (var i = 0; i < plan.pages.length; i++) {
        var page = plan.pages[i];
        lines.push("## " + (i + 1) + ". " + page.title + " (`" + page.name + "`)");
        lines.push("");
        lines.push(page.purpose);
        lines.push("");
        for (var j = 0; j < page.blocks.length; j++) {
            var block = page.blocks[j];
            lines.push("- " + block.label + " — `" + block.promptId + "`");
        }
        lines.push("");
    }
    return lines.join("\n");
}

function shouldLandingOverridePattern(query) {
    var q = String(query || "").toLowerCase();
    return /\b(pricing|plans|price|faq|documentation|webinar|conference|event|newsletter|waitlist|coming soon|portfolio|marketplace|directory|comparison)\b/.test(q);
}

function applyDarkThemeOverride(ds) {
    if (!ds || !ds.colors) return;
    ds.colors.background = "#0F172A";
    ds.colors.text = "#F8FAFC";
    ds.colors.notes = String(ds.colors.notes || "").trim();
    ds.colors.notes = ds.colors.notes ? ds.colors.notes + " | dark theme requested" : "dark theme requested";
}

function buildDesignSystemCore(options) {
    var queryTerms = options.queryTerms;
    var query = options.searchQuery;
    var projectName = options.projectName;
    var product = options.product || {};
    var category = options.category || product["Product Type"] || "General";
    var classification = options.classification || null;
    var reasoningRules = parseCSV(readSkillResource("ui-reasoning.csv"));
    var reasoning = applyReasoning(category, reasoningRules);

    var styleQuery = [
        queryTerms.verticalTerms,
        queryTerms.toneTerms,
        reasoning.stylePriority.slice(0, 2).join(" ")
    ].filter(function (p) { return !!p; }).join(" ");
    var styleResults = search(styleQuery || query, "style", SEARCH_CONFIG.style.maxResults).results || [];
    var colorMax = queryTerms.toneTerms ? 4 : SEARCH_CONFIG.color.maxResults;
    var typoMax = queryTerms.toneTerms ? 4 : SEARCH_CONFIG.typography.maxResults;
    var colorResults = search(buildColorSearchQuery(queryTerms, reasoning, product), "color", colorMax).results || [];
    var typoResults = search(buildTypographySearchQuery(queryTerms, reasoning, product, ""), "typography", typoMax).results || [];
    var landingResults = search(query, "landing", SEARCH_CONFIG.landing.maxResults).results || [];

    var bestStyle = selectBestMatch(styleResults, reasoning.stylePriority);
    var colorPick = pickColorForDesignSystem(
        category,
        options.brief || null,
        queryTerms,
        colorResults,
        options.designVariantSeed || options.projectName || query,
        classification
    );
    var bestColor = colorPick.row || {};
    var bestTypography = queryTerms.toneTerms ? selectTypographyForTone(typoResults, queryTerms.toneTerms) : (typoResults[0] || {});
    var bestLanding = landingResults[0] || {};
    var preferredPattern = reasoning.pattern || product["Landing Page Pattern"] || "";
    var patternName = (shouldLandingOverridePattern(query) && bestLanding["Pattern Name"])
        ? bestLanding["Pattern Name"]
        : (preferredPattern || bestLanding["Pattern Name"] || "Hero + Features + CTA");
    var siteTypeKey = String(options.siteType || "").toLowerCase();
    // Discovery homes should not inherit SaaS/marketing "Hero + Features + CTA" as the site pattern.
    if (/^ecommerce-/.test(siteTypeKey) && /hero\s*\+?\s*features\s*\+?\s*cta/i.test(patternName)) {
        patternName = "Photography-led Hero + Featured Products";
    } else if (siteTypeKey === "content-publication" && /hero\s*\+?\s*features\s*\+?\s*cta/i.test(patternName)) {
        patternName = "Cover or Lead Story + Article Index";
    } else if (siteTypeKey === "local-service" && /hero\s*\+?\s*features\s*\+?\s*cta/i.test(patternName)) {
        patternName = "Service Scene or Booking Path + Proof";
    }

    var ds = {
        project_name: projectName || String(query || "").toUpperCase(),
        query: query,
        queryTerms: queryTerms,
        category: category,
        pattern: { name: patternName, sections: bestLanding["Section Order"] || "Hero > Features > CTA", cta_placement: bestLanding["Primary CTA Placement"] || "Above fold", color_strategy: bestLanding["Color Strategy"] || "", conversion: bestLanding["Conversion Optimization"] || "" },
        style: { name: bestStyle["Style Category"] || "Minimalism", type: bestStyle["Type"] || "General", effects: bestStyle["Effects & Animation"] || reasoning.keyEffects, keywords: bestStyle["Keywords"] || "", best_for: bestStyle["Best For"] || "", performance: bestStyle["Performance"] || "", accessibility: bestStyle["Accessibility"] || "" },
        colors: { primary: bestColor["Primary (Hex)"] || "#2563EB", secondary: bestColor["Secondary (Hex)"] || "#3B82F6", cta: bestColor["CTA (Hex)"] || "#F97316", background: bestColor["Background (Hex)"] || "#F8FAFC", text: bestColor["Text (Hex)"] || "#1E293B", notes: bestColor["Notes"] || "" },
        typography: { heading: bestTypography["Heading Font"] || "Cabinet Grotesk", body: bestTypography["Body Font"] || "Geist", mood: bestTypography["Mood/Style Keywords"] || reasoning.typographyMood, best_for: bestTypography["Best For"] || "", google_fonts_url: bestTypography["Google Fonts URL"] || "", css_import: bestTypography["CSS Import"] || "" },
        key_effects: bestStyle["Effects & Animation"] || reasoning.keyEffects,
        anti_patterns: reasoning.antiPatterns,
        decision_rules: reasoning.decisionRules,
        severity: reasoning.severity,
        bundleQueries: {
            product: buildProductSearchQuery(queryTerms),
            color: buildColorSearchQuery(queryTerms, reasoning, product),
            typography: buildTypographySearchQuery(queryTerms, reasoning, product, bestStyle["Style Category"] || "")
        }
    };
    if (classification) ds.classification = classification;
    if (options.siteType) ds.siteType = options.siteType;
    if (options.designVariantSeed) ds.designVariantSeed = options.designVariantSeed;
    if (colorPick.paletteVariantId) ds.paletteVariantId = colorPick.paletteVariantId;
    if (options.themeMode === "dark") applyDarkThemeOverride(ds);
    return ds;
}

function buildDesignSystemData(query, projectName) {
    var queryTerms = parseQueryTerms(query);
    var classification = classifyProductWithConfidence(buildProductSearchQuery(queryTerms));
    return buildDesignSystemCore({
        queryTerms: queryTerms,
        searchQuery: query,
        projectName: projectName,
        product: classification.product,
        category: classification.category,
        classification: classification,
        themeMode: "light",
        designVariantSeed: getArg("designVariantSeed", "") || projectName
    });
}

function buildDesignSystemFromBrief(brief, projectName) {
    var enriched = enrichSiteBrief(brief, projectName);
    var seedError = requireDesignVariantSeed(enriched);
    if (seedError) return { error: seedError };
    var queryTerms = buildBriefQueryTerms(enriched);
    var classification = classifyProductWithConfidence(enriched.verticalKeywords, enriched.confirmedCategory);
    if (classification.error) return { error: classification.error };
    var ds = buildDesignSystemCore({
        queryTerms: queryTerms,
        searchQuery: buildBriefSearchContext(enriched),
        projectName: projectName,
        product: classification.product,
        category: classification.category,
        classification: classification,
        siteType: enriched.siteType,
        themeMode: enriched.themeMode,
        brief: enriched,
        designVariantSeed: enriched.designVariantSeed
    });
    ds.siteBrief = enriched;
    return ds;
}

function generateDesignSystem(query, projectName, brief) {
    var ds = brief ? buildDesignSystemFromBrief(brief, projectName) : buildDesignSystemData(query, projectName);
    if (ds.error) return ds;
    return compactDesignSystemV3(ds);
}

function buildUsage() {
    return {
        version: 3,
        modes: ["page-plan", "design-system", "page-item", "view-batch", "prompt-recommendations", "search", "stack", "help"],
        contract: "Pass one mode. Use brief as an object. Structured modes return objects.",
        seedContract: "For the first Page Plan in AI Chat, omit designVariantSeed and let AI Chat inject sessionId. Outside AI Chat, pass a real sessionId or an explicit UUID. Later reuse resolvedBrief.designVariantSeed, which is an opaque UUID or dvs-* value; put descriptive design intent in creativeDirection."
    };
}

function main() {
    var rawMode = getArg("mode", "");
    var mode = normalizeMode(rawMode);
    if (!mode) return resultError("invalid_arguments", "mode must be one of: page-plan, design-system, page-item, view-batch, prompt-recommendations, search, stack, help");
    var modeArgumentError = validateModeArguments(mode);
    if (modeArgumentError) return resultError("invalid_arguments", modeArgumentError);
    if (mode === "help") return buildUsage();

    var briefRaw = getArg("brief", null);
    var briefParsed = parseSiteBrief(briefRaw);
    if (briefParsed && briefParsed.error) return resultError("invalid_arguments", briefParsed.error);
    var brief = briefParsed || null;
    var commerceModelDecisionSource = getCommerceModelDecisionSource(briefRaw, brief);
    var briefSeedError = validateDesignVariantSeedValue(brief && brief.designVariantSeed, "brief.designVariantSeed");
    if (briefSeedError) return resultError("invalid_arguments", briefSeedError);
    var argumentSeedError = validateDesignVariantSeedValue(getArg("designVariantSeed", ""), "designVariantSeed");
    if (argumentSeedError) return resultError("invalid_arguments", argumentSeedError);
    if (brief && brief.designVariantSeed && (hasArg("sessionId") || hasArg("designVariantSeed"))) {
        return resultError("invalid_arguments", "brief.designVariantSeed makes sessionId/designVariantSeed redundant");
    }
    if ((!brief || !brief.designVariantSeed) && hasArg("sessionId") && hasArg("designVariantSeed")) {
        return resultError("invalid_arguments", "Pass sessionId or designVariantSeed, not both");
    }

    var query = getArg("query", "");
    var domain = getArg("domain", "");
    var stack = getArg("stack", "");
    var limit = getNumberArg("limit", 3);
    if (isNaN(limit) || limit % 1 !== 0 || limit < 1 || limit > 20) return resultError("invalid_arguments", "limit must be an integer from 1 to 20");

    var enrichedBrief = brief ? enrichSiteBrief(brief, "") : null;
    if (enrichedBrief) {
        var seedError = requireDesignVariantSeed(enrichedBrief);
        if (seedError) return resultError("invalid_arguments", seedError);
    }
    var projectName = resolveProjectName(enrichedBrief && enrichedBrief.projectName, enrichedBrief, enrichedBrief && enrichedBrief.designVariantSeed);

    if (mode === "prompt-recommendations") {
        if (!brief && !query) return resultError("invalid_arguments", "brief or query is required for prompt-recommendations");
        if (brief && query) return resultError("invalid_arguments", "Pass brief or query, not both");
        if (brief && hasArg("contentLocale")) return resultError("invalid_arguments", "brief.contentLocale makes contentLocale redundant");
        if (!brief && hasArg("contentLocale") && !isValidContentLocale(getArg("contentLocale", ""))) {
            return resultError("invalid_arguments", "contentLocale must be zh or en");
        }
        var sectionTypeArg = getArg("sectionType", "hero");
        var pageNameArg = getArg("pageName", "home");
        var excludeRaw = getArg("excludePromptIds", "");
        var recommendResult = generateRecommendSectionPromptsData({
            sectionType: sectionTypeArg,
            pageName: pageNameArg,
            limit: limit,
            excludePromptIds: parseExcludePromptIds(excludeRaw),
            brief: brief,
            query: query,
            projectName: projectName,
            contentLocale: resolveContentLocale() || (brief && brief.contentLocale) || ""
        });
        if (recommendResult.error) return resultError("invalid_arguments", recommendResult.error);
        return recommendResult;
    }
    if (mode === "page-item") {
        if (!brief) return resultError("invalid_arguments", "brief is required for page-item");
        var pageItemLocaleError = validateBriefContentLocale(brief);
        if (pageItemLocaleError) return resultError("invalid_arguments", pageItemLocaleError);
        var pageItemResult = generatePageItemData("", projectName, brief, getArg("pageName", "home"));
        if (pageItemResult.error) return resultError("invalid_arguments", pageItemResult.error);
        return pageItemResult;
    }
    if (mode === "view-batch") {
        if (!brief) return resultError("invalid_arguments", "brief is required for view-batch");
        var batchLocaleError = validateBriefContentLocale(brief);
        if (batchLocaleError) return resultError("invalid_arguments", batchLocaleError);
        var batchResult = generateViewBatchData("", projectName, brief, getArg("blockRefs", []));
        if (batchResult.error) return resultError("invalid_arguments", batchResult.error);
        return batchResult;
    }
    if (mode === "page-plan") {
        if (!brief) return resultError("invalid_arguments", "brief is required for page-plan");
        var briefLocaleError = validateBriefContentLocale(brief);
        if (briefLocaleError) return resultError("invalid_arguments", briefLocaleError);
        var planFromBrief = generatePagePlanDataFromBrief(brief, projectName, {
            detail: "summary",
            commerceModelDecisionSource: commerceModelDecisionSource
        });
        if (planFromBrief.error) return resultError("invalid_arguments", planFromBrief.error);
        return planFromBrief;
    }
    if (mode === "design-system") {
        if (!brief) return resultError("invalid_arguments", "brief is required for design-system");
        var dsLocaleError = validateBriefContentLocale(brief);
        if (dsLocaleError) return resultError("invalid_arguments", dsLocaleError);
        var designResult = generateDesignSystem("", projectName, brief);
        if (designResult.error) return resultError("invalid_arguments", designResult.error);
        return designResult;
    }

    if (!query) return resultError("invalid_arguments", "query is required for " + mode);
    if (mode === "stack") {
        if (!stack) return resultError("invalid_arguments", "stack is required for stack mode");
        return searchStack(query, stack, limit);
    }
    return search(query, domain || "", limit);
}

return main();
