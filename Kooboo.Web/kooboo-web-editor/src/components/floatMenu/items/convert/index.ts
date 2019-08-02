import { MenuItem, createItem } from "../../basic";
import { MenuActions } from "@/events/FloatMenuClickEvent";
import { TEXT } from "@/common/lang";
import { KoobooComment } from "@/kooboo/KoobooComment";
import context from "@/common/context";
import { isDynamicContent, isDirty, setGuid, clearKoobooInfo } from "@/kooboo/utils";
import { OBJECT_TYPE, KOOBOO_ID } from "@/common/constants";

import "./index.css"
import { clearContent } from "../../utils";
import { operationRecord } from "@/operation/Record";
import { ConverterLog } from "@/operation/recordLogs/ConverterLog";
import { setInlineEditor } from "@/components/richEditor";
import { ConvertUnit } from "@/operation/recordUnits/ConvertUnit";
import { newGuid } from "@/kooboo/outsideInterfaces";
import { isBody } from "@/dom/utils";
import { addDisableEditShade } from "./disableEditShade";

export function createConvert(): MenuItem {
    const convertItem = createItem(TEXT.CONVERT, MenuActions.convert);
    convertItem.el.classList.add("floatmenu-convert");

    const update = (comments: KoobooComment[]) => {
        convertItem.setVisiable(true);
        let args = context.lastSelectedDomEventArgs;

        if (comments.length == 0 ||
            comments[0].objecttype != OBJECT_TYPE.page ||
            isDynamicContent(args.element) ||               // 如果元素下面有其他kooboo类型
            !args.element.getAttribute(KOOBOO_ID) ||
            isBody(args.element) ||
            isDirty(args.element)                           // 如果元素是脏的，则说明该元素已被其他项操作过
        ) {
            return convertItem.setVisiable(false);
        }
    };

    let childsItemContainerObject = createChildMenus();
    convertItem.el.appendChild(childsItemContainerObject.el);

    convertItem.el.addEventListener("mouseover", () => {
        childsItemContainerObject.show();
    });
    convertItem.el.addEventListener("mouseout", () => {
        childsItemContainerObject.hidden();
    });

    return { el: convertItem.el, update };
}

function createChildMenus() {
    let childsItemContainer = document.createElement("div");

    let div = document.createElement("div");

    let htmlBlockItem = createConvertToHtmlBlock();
    div.appendChild(htmlBlockItem.el);

    let viewItem = createConvertToView();
    div.appendChild(viewItem.el);

    childsItemContainer.appendChild(div);

    childsItemContainer.classList.add("floatmenu-convert-childscontainer");
    let show = () => {
        childsItemContainer.classList.add("floatmenu-convert-childscontainer-hover");

        let rect = div.getBoundingClientRect();
        // 如果元素最右边的位置大于body宽度
        if (rect.right > document.body.clientWidth) {
            childsItemContainer.classList.add("floatmenu-convert-childscontainer-floatleft");
        }
    };
    let hidden = () => {
        childsItemContainer.classList.remove("floatmenu-convert-childscontainer-hover");
        childsItemContainer.classList.remove("floatmenu-convert-childscontainer-floatleft");
    }


    return { el: childsItemContainer, show, hidden };
}

function createConvertToHtmlBlock() {
    const item = createItem(`${TEXT.CONVERTTOHTMLBLOCK}`, MenuActions.convertToHtmlBlock);

    item.el.addEventListener("click", () => {
    });

    return item;
}

function createConvertToView() {
    const item = createItem(`${TEXT.CONVERTTOVIEW}`, MenuActions.convertToView);

    item.el.addEventListener("click", () => {
        let { element, koobooId } = context.lastSelectedDomEventArgs;
        let guid = setGuid(element);
        let startContent = element.outerHTML;
        const onSave = () => 
        {
            let viewNameorid = `${(new Date()).valueOf()}`;  // 以时间戳作为naneorid
            // 生成log
            let convertResult = {
                convertToType: "View",
                name: viewNameorid,
                koobooId: element.getAttribute(KOOBOO_ID),
                htmlBody: clearKoobooInfo(element.outerHTML)
            };
            let log = ConverterLog.create(JSON.stringify(convertResult));
            console.log(log);

            // 生成unit
            let units = [ConvertUnit.CreateViewConvertUnit(startContent)];

            // 为元素添加禁止编辑遮罩
            addDisableEditShade(element, (e) => {});

            let operation = new operationRecord(units, [log], guid);
            context.operationManager.add(operation);
        };

        const onCancel = () => {
            element.innerHTML = startContent;
        };
        setInlineEditor({ selector: element, onSave, onCancel });
    });

    return item;
}