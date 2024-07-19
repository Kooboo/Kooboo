export interface ScriptModule {
  id: string;
  name: string;
  universalIdentifier: string;
  relativeFolder: string;
  settings: Record<string, string>;
}

export interface ResourceType {
  name: string;
  displayName: string;
  isText: boolean;
  isBinary: boolean;
  defaultExtension: string;
  extensions: { type: string; name: string; display: string }[];
}

export interface ModuleFileInfo {
  id: string;
  name: string;
  isText: boolean;
  isBinary: boolean;
  fullName: string;
  folderName: string;
  extension: string;
  size: number;
  stringSize: number;
  lastModified: string;
  objectType: string;
  previewUrl: string;
}
