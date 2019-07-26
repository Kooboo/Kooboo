export class Background {
  constructor(public value: string) {}
  get image() {
    let result = this.value.match(/url\((.+?)\)/);
    if (!result || result.length == 0) return "";
    return result[0];
  }
  set image(imageUrl: string) {
    this.value = this.value.replace(/url\((.+?)\)/, `url(${imageUrl})`);
  }
  clearImage() {
    this.value = this.value.replace(/url\((.+?)\)/, "url(none)");
  }
}
