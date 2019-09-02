import { init, Settings } from "tinymce";
import "tinymce/themes/silver";
import "tinymce/plugins/save";
import "tinymce/plugins/link";
import "tinymce/plugins/image";
(document as any).init = function(settings: Settings) {
  init(settings);
};
