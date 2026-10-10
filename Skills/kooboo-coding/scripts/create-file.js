// Write file to AI workspace (prefix: _ai/)
// Usage: run_skill_script('kooboo-coding', 'create-file.js', {path: 'plans/my-plan.md', content: '# Plan'})

var AI_PREFIX = "_ai/";

// Validate input
if (!k.request.path || typeof k.request.path !== "string") {
    return { success: false, error: "Missing or invalid 'path' parameter" };
}

if (!k.request.content || typeof k.request.content !== "string") {
    return { success: false, error: "Missing or invalid 'content' parameter" };
}

// Ensure path starts with _ai/
var fullPath = k.request.path.startsWith(AI_PREFIX)
    ? k.request.path
    : AI_PREFIX + k.request.path;

// Validate path (no parent directory traversal)
if (fullPath.indexOf("..") !== -1) {
    return { success: false, error: "Invalid path: parent directory traversal not allowed" };
}

try {
    // Write file
    var fileInfo = k.file.write(fullPath, k.request.content);

    return {
        success: true,
        path: fullPath,
        size: fileInfo.size,
        lastModified: fileInfo.lastModified
    };
} catch (error) {
    return {
        success: false,
        error: error.message || String(error)
    };
}
