import type {
  MediaFileItem as MediaFileItemAPI,
  MediaFolderItem as MediaFolderItemAPI,
} from "@/api/content/media";

import MediaDialog from "./media-dialog.vue";

export { default as ImageEditor } from "./image-editor.vue";

export default MediaDialog;

export type MediaFileItem = MediaFileItemAPI & {
  selected?: boolean;
};

export type MediaFolderItem = MediaFolderItemAPI & {
  selected?: boolean;
};

export type DialogInfo = {
  multiple: boolean;
};
