// Read file from AI workspace (prefix: _ai/)
// Usage: run_skill_script('kooboo-coding', 'read-file.js', {path: 'plans/my-plan.md'})

var AI_PREFIX = "_ai/";

// Validate input
if (!k.request.path || typeof k.request.path !== "string") {
    return { success: false, error: "Missing or invalid 'path' parameter" };
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
    // Check if file exists
    if (!k.file.exists(fullPath)) {
        return {
            success: false,
            error: "File not found: " + fullPath
        };
    }

    // Read file
    var content = k.file.read(fullPath);
    var fileInfo = k.file.get(fullPath);

    return {
        success: true,
        path: fullPath,
        content: content,
        size: fileInfo.size,
        lastModified: fileInfo.lastModified
    };
} catch (error) {
    return {
        success: false,
        error: error.message || String(error)
    };
}
