var b=Object.defineProperty,y=Object.defineProperties;var B=Object.getOwnPropertyDescriptors;var d=Object.getOwnPropertySymbols;var $=Object.prototype.hasOwnProperty,P=Object.prototype.propertyIsEnumerable;var h=(e,t,n)=>t in e?b(e,t,{enumerable:!0,configurable:!0,writable:!0,value:n}):e[t]=n,E=(e,t)=>{for(var n in t||(t={}))$.call(t,n)&&h(e,n,t[n]);if(d)for(var n of d(t))P.call(t,n)&&h(e,n,t[n]);return e},k=(e,t)=>y(e,B(t));import{E as w}from"./windi.19264205.js";import{E as x}from"./index.aec72f69.js";import{E as D}from"./index.ec6ad7db.js";import{E as T}from"./index.3a977dfb.js";import{d as g,b7 as j,o as c,j as s,w as a,e as m,aT as f,b as u,n as z,a as A}from"./url.8f5ec20c.js";const I={key:1,class:"px-8"},N={inheritAttrs:!1},K=g(k(E({},N),{props:{circle:{type:Boolean},icon:null,tip:null,permission:null},setup(e){return(t,n)=>{const r=w,C=x,p=D,v=T,l=j("hasPermission");return e.circle?(c(),s(p,{key:0,placement:"top",content:e.tip},{default:a(()=>{var o,i;return[m((c(),s(C,f({circle:""},t.$attrs),{default:a(()=>[u(r,{class:z(["iconfont",e.icon])},null,8,["class"])]),_:1},16)),[[l,{feature:(o=e.permission)==null?void 0:o.feature,action:(i=e.permission)==null?void 0:i.action,effect:"circle"}]])]}),_:1},8,["content"])):(c(),A("span",I,[t.$attrs.onConfirm?(c(),s(v,{key:0,title:e.tip,onConfirm:t.$attrs.onConfirm},{reference:a(()=>{var o,i;return[m(u(r,f({class:["iconfont text-l cursor-pointer hover:text-blue",e.icon]},t.$attrs),null,16,["class"]),[[l,{feature:(o=e.permission)==null?void 0:o.feature,action:(i=e.permission)==null?void 0:i.action,effect:"icon"}]])]}),_:1},8,["title","onConfirm"])):(c(),s(p,{key:1,placement:"top",content:e.tip},{default:a(()=>{var o,i;return[m(u(r,f({class:["iconfont text-l cursor-pointer hover:text-blue",e.icon]},t.$attrs),null,16,["class"]),[[l,{feature:(o=e.permission)==null?void 0:o.feature,action:(i=e.permission)==null?void 0:i.action,effect:"icon"}]])]}),_:1},8,["content"]))]))}}}));export{K as _};