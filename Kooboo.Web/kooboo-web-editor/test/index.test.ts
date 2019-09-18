import tinymce from "@/components/richEditor/tinymce";
(window as any).tinymce = tinymce;
const testsContext = (require as any).context(".", true, /\.test$/);
testsContext.keys().forEach(testsContext);
