import{d as i}from"./main.ea42d807.js";import{a as t}from"./replace-all.7cf5f327.js";import{d as p}from"./i18n.48bd28ac.js";const e=p.global.t,c=s=>i.post(t("digitalshipping/create"),s,void 0,{successMessage:e("common.saveSuccess")}),n=s=>i.post(t("digitalshipping/Edit"),s,void 0,{successMessage:e("common.saveSuccess")}),d=s=>i.get(t("digitalshipping/getEdit"),{id:s}),l=()=>i.get(t("digitalshipping/list")),h=s=>i.get(t("digitalshipping/get"),{id:s}),r=s=>i.post(t("digitalshipping/deletes"),{ids:s},void 0,{successMessage:e("common.deleteSuccess")}),m=s=>i.post(t("digitalshipping/SetDefault"),{id:s}),S=s=>i.post(t("digitalshipping/preview"),s);export{h as a,d as b,c,r as d,n as e,l as g,S as p,m as s};