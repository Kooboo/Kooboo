var w=(k,d,m)=>new Promise((v,n)=>{var u=e=>{try{c(m.next(e))}catch(s){n(s)}},p=e=>{try{c(m.throw(e))}catch(s){n(s)}},c=e=>e.done?v(e.value):Promise.resolve(e.value).then(u,p);c((m=m.apply(k,d)).next())});import{g as S,w as F}from"./i18n.bcd18f8a.js";import{u as U}from"./use-operation-dialog.175b540a.js";import{m as q,a as C,r as T,d as A}from"./validate.238a9986.js";import{f as M,m as O,n as j,o as G,q as H}from"./index.aef630be.js";import{d as B,M as Q,g as b,i as z,o as x,j as E,w as l,b as i,u as t,f as D,t as y,a as h,b6 as J,F as K,G as P,E as W,aH as X,cg as Y,x as Z}from"./url.8f5ec20c.js";import{_ as ee}from"./index.7a75fca3.js";import{R as oe}from"./main.582f9de6.js";import{_ as te}from"./plugin-vue_export-helper.21dcd24c.js";import{E as ae}from"./index.5cbbc5d7.js";import{E as le,a as ie}from"./index.6d63eb53.js";import{E as ne,a as se}from"./index.6d22f937.js";import{E as me}from"./index.d4a6b2d5.js";import{_ as re}from"./k-table.7d05e82e.js";import{b as de}from"./confirm.eadb49f1.js";import{E as ue}from"./index.aec72f69.js";import{E as pe}from"./index.b83a1079.js";import"./index.2341329b.js";import"./index.bda83f28.js";import"./replace-all.d441bf14.js";import"./index.bff48780.js";import"./index.649f6c77.js";import"./index.a32fb6e5.js";import"./style.9c8f6403.js";import"./toNumber.6efebd6a.js";import"./_baseIsEqual.547729d3.js";import"./index.50c16ae5.js";import"./windi.19264205.js";import"./preload-helper.13a99eb0.js";import"./event.776e7e11.js";import"./index.05e21f33.js";import"./error.7e8331f1.js";import"./isNil.98bb3b88.js";import"./_baseClone.eeff2792.js";import"./isEqual.11d86bcc.js";import"./index.ec6ad7db.js";import"./focus-trap.eafcfd1f.js";import"./event.53b2ad83.js";import"./index.79f78425.js";import"./index.c80f5028.js";import"./scroll.4888a9e9.js";import"./debounce.730e1961.js";import"./index.9a83ee01.js";import"./validator.b73911a9.js";import"./index.a3d8335f.js";import"./refs.d2253dd4.js";import"./sortable.esm.a99254e8.js";import"./icon-button.315b6443.js";import"./index.3a977dfb.js";import"./index.564bc658.js";import"./index.daafc4da.js";import"./index.59b1471f.js";import"./logo-transparent.1566007e.js";import"./index.10f642b2.js";import"./aria.75ec5909.js";const ce=["title"],fe=B({props:{modelValue:{type:Boolean},domain:null},emits:["create-success"],setup(k,{emit:d}){const m=k,v=Q(),{t:n}=S(),{visible:u,handleClose:p}=U(m,d),c=b(),e=b(),s=b(),o=b({subDomain:"",rootDomain:"",SiteId:""}),g={subDomain:[q,C(n("common.inputValue")),T(1,63),A(o.value)],SiteId:[C(n("common.inputValue"))]};z(()=>u.value,_=>w(this,null,function*(){var a,I;_&&(c.value=yield oe(),e.value=yield M(),(a=s.value)==null||a.resetFields(),o.value.subDomain="",o.value.rootDomain=e.value.filter(V=>V.id===v.query.id)[0].domainName,o.value.SiteId=(I=c.value[0])==null?void 0:I.siteId)}));function r(){var _;(_=s.value)==null||_.validate(a=>w(this,null,function*(){a&&(yield O(o.value.subDomain,o.value.rootDomain,o.value.SiteId),p(),d("create-success"))}))}return(_,a)=>{const I=ae,V=le,N=ne,R=se,L=ie,$=me;return x(),E($,{modelValue:t(u),"onUpdate:modelValue":a[3]||(a[3]=f=>P(u)?u.value=f:null),width:"600px","close-on-click-modal":!1,title:t(n)("common.newBinding"),onClose:t(p)},{footer:l(()=>[i(ee,{"confirm-label":t(n)("common.create"),onConfirm:r,onCancel:t(p)},null,8,["confirm-label","onCancel"])]),default:l(()=>[i(L,{ref_key:"form",ref:s,class:"el-form--label-normal",model:o.value,rules:t(g),"label-position":"top",onSubmit:a[2]||(a[2]=F(()=>{},["prevent"]))},{default:l(()=>[i(V,{prop:"subDomain",label:t(n)("common.domain")},{default:l(()=>[i(I,{modelValue:o.value.subDomain,"onUpdate:modelValue":a[0]||(a[0]=f=>o.value.subDomain=f),placeholder:"www","data-cy":"subdomain"},{append:l(()=>[D("span",{disabled:"",class:"ellipsis max-w-240px px-16 dark:bg-[#222] dark:rounded-tr-normal dark:rounded-br-normal dark:border-[#4c4d4f] dark:border-1 border-solid dark:border-l-0",title:o.value.rootDomain,"data-cy":"root-domain"},"."+y(o.value.rootDomain),9,ce)]),_:1},8,["modelValue"])]),_:1},8,["label"]),i(V,{prop:"SiteId",label:t(n)("common.site")},{default:l(()=>[i(R,{modelValue:o.value.SiteId,"onUpdate:modelValue":a[1]||(a[1]=f=>o.value.SiteId=f),class:"w-full"},{default:l(()=>[(x(!0),h(K,null,J(c.value,f=>(x(),E(N,{key:f.siteId,label:f.siteDisplayName,value:f.siteId,"data-cy":"site-opt"},null,8,["label","value"]))),128))]),_:1},8,["modelValue"])]),_:1},8,["label"])]),_:1},8,["model","rules"])]),_:1},8,["modelValue","title","onClose"])}}});var _e=te(fe,[["__scopeId","data-v-7327bb0f"]]);const be={class:"p-24"},ve={class:"flex items-center pb-24 space-x-16"},ge={class:"flex items-center"},we=["href"],ko=B({setup(k){const{t:d}=S(),m=W({status:!1,open(){m.status=!0}}),v=e=>{Y("//"+e)},n=b(),u=b();function p(){return w(this,null,function*(){const e=Z("id");e&&(u.value=yield j(e),n.value=yield G(e))})}p();function c(e){return w(this,null,function*(){e.length&&(yield de(e.length),yield H(e.map(s=>s.id)),p())})}return(e,s)=>{const o=ue,g=pe;return x(),h("div",be,[D("div",ve,[i(o,{round:"",onClick:t(m).open},{default:l(()=>[D("div",ge,y(t(d)("common.new")),1)]),_:1},8,["onClick"])]),i(t(re),{data:u.value,"show-check":"",onDelete:c},{default:l(()=>[i(g,{label:t(d)("common.domain")},{default:l(({row:r})=>[D("span",null,y(r.fullName),1)]),_:1},8,["label"]),i(g,{label:t(d)("common.site")},{default:l(({row:r})=>[D("a",{class:"cursor-pointer text-blue",href:`/_Admin/site?SiteId=${r.webSiteId}`,target:"_blank","data-cy":"site-name"},y(r.siteName),9,we)]),_:1},8,["label"]),i(g,{width:"150px",align:"center"},{default:l(({row:r})=>[i(o,{type:"primary",round:"",onClick:_=>v(r.fullName)},{default:l(()=>[X(y(t(d)("common.preview")),1)]),_:2},1032,["onClick"])]),_:1})]),_:1},8,["data"]),i(_e,{modelValue:t(m).status,"onUpdate:modelValue":s[0]||(s[0]=r=>t(m).status=r),domain:n.value,onCreateSuccess:p},null,8,["modelValue","domain"])])}}});export{ko as default};