var g=(y,a,s)=>new Promise((_,n)=>{var c=t=>{try{u(s.next(t))}catch(m){n(m)}},p=t=>{try{u(s.throw(t))}catch(m){n(m)}},u=t=>t.done?_(t.value):Promise.resolve(t.value).then(c,p);u((s=s.apply(y,a)).next())});import{d as D,g as w,o as v,j as C,w as r,b as i,u as o,cd as T,N as U,b7 as q,a as H,f as b,e as M,k as N,aH as $,t as k,cg as K}from"./url.8f5ec20c.js";import{u as S}from"./replace-all.d441bf14.js";import{_ as F}from"./k-table.7d05e82e.js";import{_ as P}from"./relations-tag.7a82bbd0.js";import{u as j}from"./use-date.a1321c18.js";import{_ as B}from"./icon-button.315b6443.js";import{g as I,w as A,o as L}from"./i18n.bcd18f8a.js";import{T as R,i as z,U as G}from"./dev-mode.1f379952.js";import{r as J,a as O,s as Q,i as W}from"./validate.238a9986.js";import{_ as X}from"./index.7a75fca3.js";import{E as Y}from"./index.5cbbc5d7.js";import{E as Z,a as ee}from"./index.6d63eb53.js";import{E as oe}from"./index.d4a6b2d5.js";import{B as te}from"./breadcrumb.edaec300.js";import{u as ie}from"./main.582f9de6.js";import{b as me}from"./confirm.eadb49f1.js";import{E as re}from"./windi.19264205.js";import{E as ae}from"./index.aec72f69.js";import{E as se}from"./index.b83a1079.js";import"./sortable.esm.a99254e8.js";import"./index.564bc658.js";import"./index.6d22f937.js";import"./index.ec6ad7db.js";import"./focus-trap.eafcfd1f.js";import"./isNil.98bb3b88.js";import"./event.53b2ad83.js";import"./index.05e21f33.js";import"./index.79f78425.js";import"./error.7e8331f1.js";import"./index.c80f5028.js";import"./event.776e7e11.js";import"./scroll.4888a9e9.js";import"./isEqual.11d86bcc.js";import"./_baseIsEqual.547729d3.js";import"./debounce.730e1961.js";import"./toNumber.6efebd6a.js";import"./index.9a83ee01.js";import"./validator.b73911a9.js";import"./index.daafc4da.js";import"./index.59b1471f.js";import"./page.e927203a.js";import"./index.10f642b2.js";import"./index.a3d8335f.js";import"./aria.75ec5909.js";import"./guid.c1a40312.js";import"./style.9c8f6403.js";import"./dayjs.min.59f10137.js";import"./index.3a977dfb.js";import"./index.72ccc088.js";import"./classCompletion.a22e38a6.js";import"./userWorker.b3a6730b.js";import"./editor.main.d2800f63.js";import"./preload-helper.13a99eb0.js";import"./vuedraggable.umd.7929a3b6.js";import"./cloneDeep.060340c1.js";import"./_baseClone.eeff2792.js";import"./config.812575d1.js";import"./dark.166fd971.js";import"./index.8bc4b1d6.js";import"./dropdown.f6378640.js";import"./refs.d2253dd4.js";import"./use-copy-text.c117d066.js";import"./index.73c941f5.js";import"./toggleComment.5b29ca87.js";import"./use-save-tip.8f44d6c0.js";import"./index.373c837a.js";import"./index.4dc50d47.js";import"./index.3be79fd5.js";import"./plugin-vue_export-helper.21dcd24c.js";import"./index.79dc05e0.js";import"./index.ff00fbd6.js";import"./index.57a23e6d.js";import"./alert.8fe5500a.js";import"./index.d84e2378.js";/* empty css                                                               */import"./media-list.vue_vue_type_style_index_0_scoped_true_lang.c0b84b74.js";/* empty css                                                          */import"./file.734aeed6.js";/* empty css                                                          *//* empty css                                                                 */import"./use-file-upload.c73251ef.js";/* empty css                                                         */import"./image-editor.vue_vue_type_style_index_0_scoped_true_lang.53a9e945.js";import"./image-editor.f97cfa84.js";import"./main.esm.6441082b.js";import"./index.7633560d.js";import"./index.2341329b.js";import"./index.bda83f28.js";import"./index.bff48780.js";import"./index.649f6c77.js";import"./index.a32fb6e5.js";import"./plugin-vue_export-helper.41ffa612.js";import"./index.50c16ae5.js";import"./logo-transparent.1566007e.js";const le=D({props:{modelValue:{type:Boolean},selected:null},emits:["update:modelValue"],setup(y,{emit:a}){const s=y,_=R(),{t:n}=I(),c=w(!0),p=w(),u={name:[J(1,50),O(n("common.nameRequiredTips")),Q(),W(z,n("common.viewNameExistsTips"))]},t=w({id:s.selected.id,name:s.selected.name+"_Copy"}),m=()=>g(this,null,function*(){yield p.value.validate(),yield G(t.value.id,t.value.name),c.value=!1,_.load()});return(d,l)=>{const V=Y,h=Z,f=ee,E=oe;return v(),C(E,{"model-value":c.value,width:"600px","close-on-click-modal":!1,title:o(n)("common.copyView"),onClosed:l[3]||(l[3]=e=>a("update:modelValue",!1))},{footer:r(()=>[i(X,{onConfirm:m,onCancel:l[2]||(l[2]=e=>c.value=!1)})]),default:r(()=>[i(f,{ref_key:"form",ref:p,"label-position":"top",model:t.value,rules:o(u),onSubmit:l[1]||(l[1]=A(()=>{},["prevent"])),onKeydown:L(m,["enter"])},{default:r(()=>[i(h,{label:o(n)("common.name"),prop:"name"},{default:r(()=>[i(V,{modelValue:t.value.name,"onUpdate:modelValue":l[0]||(l[0]=e=>t.value.name=e)},null,8,["modelValue"])]),_:1},8,["label"])]),_:1},8,["model","rules","onKeydown"])]),_:1},8,["model-value","title"])}}}),ne={class:"p-24"},pe={class:"flex items-center py-24 space-x-16"},ce={class:"flex items-center"},ue=["title"],de=["title","onClick"],tt=D({setup(y){const{t:a}=I(),s=R(),_=ie(),n=T(),c=w(),p=w(!1),u=m=>{_.hasAccess("view","edit")&&(c.value=m,p.value=!0)},t=m=>g(this,null,function*(){yield me(m.length),s.deleteViews(m.map(d=>d.id))});return s.load(),(m,d)=>{const l=re,V=ae,h=U("router-link"),f=se,E=q("hasPermission");return v(),H("div",ne,[i(te,{name:o(a)("common.views")},null,8,["name"]),b("div",pe,[M((v(),C(V,{round:"","data-cy":"new-view",onClick:d[0]||(d[0]=e=>o(n).push(o(S)({name:"view-edit"})))},{default:r(()=>[b("div",ce,[i(l,{class:"iconfont icon-a-addto"}),$(" "+k(o(a)("common.newView")),1)])]),_:1})),[[E,{feature:"view",action:"edit"}]])]),i(o(F),{data:o(s).list,"show-check":"",permission:{feature:"view",action:"delete"},onDelete:t},{bar:r(({selected:e})=>[e.length===1?(v(),C(B,{key:0,permission:{feature:"view",action:"edit"},icon:"icon-copy",tip:o(a)("common.copy"),circle:"","data-cy":"copy",onClick:x=>u(e[0])},null,8,["tip","onClick"])):N("",!0)]),default:r(()=>[i(f,{label:o(a)("common.name"),prop:"name"},{default:r(({row:e})=>[i(h,{to:o(S)({name:"view-edit",query:{id:e.id}}),"data-cy":"name"},{default:r(()=>[b("span",{title:e.name,class:"ellipsis text-blue cursor-pointer"},k(e.name),9,ue)]),_:2},1032,["to"])]),_:1},8,["label"]),i(f,{label:o(a)("common.usedBy")},{default:r(({row:e})=>[i(P,{id:e.id,relations:e.relations,type:"View"},null,8,["id","relations"])]),_:1},8,["label"]),i(f,{label:o(a)("common.preview")},{default:r(({row:e})=>[b("a",{class:"text-blue ellipsis cursor-pointer",title:e.preview,onClick:x=>o(K)(e.preview)},k(e.relativeUrl),9,de)]),_:1},8,["label"]),i(f,{label:o(a)("common.lastModified"),prop:"lastModified"},{default:r(({row:e})=>[$(k(o(j)(e.lastModified)),1)]),_:1},8,["label"]),i(f,{width:"60px",align:"center"},{default:r(({row:e})=>[i(B,{permission:{feature:"site",action:"log"},icon:"icon-time rounded-full",tip:o(a)("common.version"),"data-cy":"versions",onClick:x=>m.$router.goLogVersions(e.keyHash,e.storeNameHash,e.tableNameHash)},null,8,["tip","onClick"])]),_:1})]),_:1},8,["data"]),p.value?(v(),C(le,{key:0,modelValue:p.value,"onUpdate:modelValue":d[1]||(d[1]=e=>p.value=e),selected:c.value},null,8,["modelValue","selected"])):N("",!0)])}}});export{tt as default};