import{a}from"./replace-all.d441bf14.js";import{d as s}from"./main.582f9de6.js";import{d as o}from"./i18n.bcd18f8a.js";const t=o.global.t,m=e=>s.post("/Template/Share",e,void 0,{successMessage:t("common.shareSuccess")}),p=e=>s.get("/Template/Detail",{Id:e}),n=e=>s.post("/Template/FullTextSearch",e),i=e=>s.post("/Template/Use",e,void 0,{successMessage:t("common.createSuccess")}),g=()=>s.get(a("/Template/type")),T=e=>s.get("/Template/shareValidate",{siteId:e}),u=()=>s.get("/Template/Personal");export{u as a,m as b,n as c,p as d,g,T as s,i as u};