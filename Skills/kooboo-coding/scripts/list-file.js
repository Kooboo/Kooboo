// List files in AI workspace (prefix: _ai/)
// Usage: run_skill_script('kooboo-coding', 'list-file.js', {folder: 'plans'})
// Usage: run_skill_script('kooboo-coding', 'list-file.js', {folder: 'plans', recursive: true})

var AI_PREFIX = "_ai/";

// Get folder path (default to root _ai/)
var folder = k.request.folder || "";
var fullPath = folder.startsWith(AI_PREFIX)
    ? folder
    : AI_PREFIX + folder;

// Validate path (no parent directory traversal)
if (fullPath.indexOf("..") !== -1) {
    return { success: false, error: "Invalid path: parent directory traversal not allowed" };
}

// Get recursive flag
var recursive = k.request.recursive === true || k.request.recursive === "true";

try {
    // Check if folder exists
    if (!k.file.folderExists(fullPath)) {
        return {
            success: false,
            error: "Folder not found: " + fullPath
        };
    }

    var files = [];
    var folders = [];

    if (recursive) {
        // Recursive: get all files under this folder
        var allFiles = k.file.folderFiles(fullPath, { recursive: true });
        for (var i = 0; i < allFiles.length; i++) {
            var file = allFiles[i];
            files.push({
                name: file.name,
                path: file.name,
                size: file.size,
                lastModified: file.lastModified
            });
        }
    } else {
        // Non-recursive: get files and subfolders
        var folderFiles = k.file.folderFiles(fullPath);
        for (var i = 0; i < folderFiles.length; i++) {
            var file = folderFiles[i];
            files.push({
                name: file.name,
                path: file.name,
                size: file.size,
                lastModified: file.lastModified
            });
        }

        var subFolders = k.file.subFolders(fullPath);
        for (var i = 0; i < subFolders.length; i++) {
            var subFolder = subFolders[i];
            folders.push({
                name: subFolder.name,
                path: subFolder.name
            });
        }
    }

    return {
        success: true,
        folder: fullPath,
        files: files,
        folders: folders,
        totalFiles: files.length,
        totalFolders: folders.length
    };
} catch (error) {
    return {
        success: false,
        error: error.message || String(error)
    };
}
