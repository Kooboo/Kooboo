declare module "*.svg" {
  const content: any;
  export default content;
}

declare module "*.png" {
  const content: any;
  export default content;
}

declare module "@simonwep/pickr";

declare module "tinymce-declaration" {
  import tinymce from "tinymce";
  export default tinymce;
}
