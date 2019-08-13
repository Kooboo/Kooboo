export class Background {
  image?: string;
  color?: string;
  position?: string;
  size?: string;
  repeat?: string;
  origin?: string;
  clip?: string;
  attachment?: string;

  constructor(public value: string) {
    let temp = document.createElement("div");
    temp.style.background = value;
    this.image = temp.style.backgroundImage!;
    this.color = temp.style.backgroundColor!;
    this.position = temp.style.backgroundPosition!;
    this.size = temp.style.backgroundSize!;
    this.repeat = temp.style.backgroundRepeat!;
    this.origin = temp.style.backgroundOrigin!;
    this.clip = temp.style.backgroundClip!;
    this.attachment = temp.style.backgroundAttachment!;
  }

  toString() {
    let result = "";
    if (this.color && this.color != "initial") result += `${this.color} `;
    if (this.image && this.image != "initial") result += `${this.image.replace(/"/g, "'")} `;
    if (this.position && this.position != "initial") result += `${this.position} `;
    if (this.size && this.size != "initial") result += `${this.size} `;
    if (this.repeat && this.repeat != "initial") result += `${this.repeat} `;
    if (this.origin && this.origin != "initial") result += `${this.origin} `;
    if (this.clip && this.clip != "initial") result += `${this.clip} `;
    if (this.attachment && this.attachment != "initial") result += `${this.attachment} `;
    return result;
  }
}
