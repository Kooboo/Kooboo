export function createImgPreview() {
  let el = document.createElement("div");
  el.style.outline = "5px solid #eee";
  el.style.margin = "5px";
  el.style.height = "300px";
  el.style.width = "400px";
  el.style.backgroundPosition = "center";
  el.style.backgroundRepeat = "no-repeat";
  el.style.backgroundSize = "contain";

  const setImage = (src: string) => {
    if (src.indexOf("url(") > -1) el.style.backgroundImage = src;
    else el.style.backgroundImage = `url('${src}')`;
  };

  return {
    imagePreview: el,
    setImage
  };
}
