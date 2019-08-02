import { MenuItem, createItem } from "../basic";
import { KoobooComment } from "@/kooboo/KoobooComment";

export default abstract class BaseMenuItem implements MenuItem{
    abstract el: HTMLElement;

    abstract setVisiable: (visiable: boolean)=>void;
    
    abstract update(comments: KoobooComment[]): void;
}