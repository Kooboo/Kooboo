import{_ as S,a as $}from"./kmail-button.2651497e.js";import{_ as C}from"./site-button.cb1c5bb0.js";import{d,M as N,c as u,N as b,o as r,a as i,b as o,w as e,u as l,F as B,b6 as M,n as y,f as a,t as A,j as f}from"./url.8f5ec20c.js";import{g as I}from"./i18n.bcd18f8a.js";import{E as z}from"./windi.19264205.js";import{a as D,b as F}from"./index.0786c82d.js";import{E as v}from"./index.79f78425.js";import{b as V}from"./main.582f9de6.js";import{E as j}from"./index.cdccec76.js";import"./light-switch.228a2b15.js";import"./dark.166fd971.js";import"./plugin-vue_export-helper.41ffa612.js";import"./index.649f6c77.js";import"./index.8bc4b1d6.js";import"./index.aec72f69.js";import"./index.05e21f33.js";import"./index.ec6ad7db.js";import"./focus-trap.eafcfd1f.js";import"./isNil.98bb3b88.js";import"./event.53b2ad83.js";import"./dropdown.f6378640.js";import"./index.c80f5028.js";import"./refs.d2253dd4.js";import"./avatar.3578cce6.js";import"./logo-transparent.1566007e.js";import"./index.2341329b.js";import"./email.95ae8451.js";import"./index.aef630be.js";import"./aria.75ec5909.js";import"./index.2cfdc153.js";import"./error.7e8331f1.js";import"./index.50c16ae5.js";import"./preload-helper.13a99eb0.js";import"./replace-all.d441bf14.js";const L={class:"w-202px h-full relative bg-fff z-10 dark:bg-[#252526]"},P=d({setup(h){const n=N(),{t:s}=I(),m=u(()=>n.meta.activeMenu||n.name),c=u(()=>[{label:s("common.server"),name:"partner",show:!0,icon:"icon-Server"},{label:"DNS",name:"dns",show:!0,icon:"icon-dns"},{label:s("common.subAccount"),name:"subAccount",show:!0,icon:"icon-subaccount"}]);return(p,_)=>{const w=z,x=D,g=b("router-link"),k=F,E=v;return r(),i("aside",L,[o(E,null,{default:e(()=>[o(k,{"default-active":l(m)},{default:e(()=>[(r(!0),i(B,null,M(l(c).filter(t=>t.show),t=>(r(),i("div",{key:t.name},[o(g,{to:{name:t.name}},{default:e(()=>[o(x,{index:t.name},{default:e(()=>[o(w,{class:y(["iconfont",t.icon])},null,8,["class"]),a("span",null,A(t.label),1)]),_:2},1032,["index"])]),_:2},1032,["to"])]))),128))]),_:1},8,["default-active"])]),_:1})])}}}),R={class:"h-full flex flex-col bg-[#f3f5f5] dark:bg-[#1e1e1e]"},q={class:"flex-1 overflow-hidden relative"},G={class:"absolute inset-0 left-202px"},So=d({setup(h){const n=V();return(s,m)=>{const c=b("router-view"),p=v,_=j;return r(),f(_,{locale:l(n).locale},{default:e(()=>[a("div",R,[o(l($),{class:"pl-40px"},{left:e(()=>[o(C),o(S)]),right:e(()=>[]),_:1}),a("div",q,[o(P),a("div",G,[o(p,{id:"main-scrollbar",class:"w-full"},{default:e(()=>[(r(),f(c,{key:s.$route.name}))]),_:1})])])])]),_:1},8,["locale"])}}});export{So as default};