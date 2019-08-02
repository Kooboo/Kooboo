import { MenuItem, createItem } from "../basic";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { Menu } from "../menu";

export default abstract class BaseMenuItem implements MenuItem{
    constructor(public parentMenu: Menu)
    {

    }

    abstract el: HTMLElement;

    abstract setVisiable: (visiable: boolean)=>void;
    
    abstract update(comments: KoobooComment[]): void;
}