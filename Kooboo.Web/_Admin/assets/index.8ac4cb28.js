var g=(y,a,s)=>new Promise((_,n)=>{var c=t=>{try{u(s.next(t))}catch(m){n(m)}},p=t=>{try{u(s.throw(t))}catch(m){n(m)}},u=t=>t.done?_(t.value):Promise.resolve(t.value).then(c,p);u((s=s.apply(y,a)).next())});import{d as I,g as w,o as v,j as k,w as r,b as i,u as o,cd as T,O as U,b8 as q,a as M,f as b,e as H,k as $,aI as N,t as V,cg as K}from"./url.4cb47a7a.js";import{u as S}from"./replace-all.56d2c1f7.js";import{_ as F}from"./k-table.b1a54b80.js";import{_ as P}from"./relations-tag.b9cbe606.js";import{u as j}from"./use-date.f1258183.js";import{_ as B}from"./icon-button.74ccc970.js";import{g as D,w as A,o as L}from"./i18n.a87944a4.js";import{U as R,i as O,V as z}from"./dev-mode.acf56c3e.js";import{r as G,a as J,s as Q,i as W}from"./validate.bb56f9e3.js";import{_ as X}from"./index.f0d5f031.js";import{E as Y}from"./index.178b9e08.js";import{E as Z,a as ee}from"./index.9fd79879.js";import{E as oe}from"./index.c003dfa1.js";import{B as te}from"./breadcrumb.f4d80c95.js";import{u as ie}from"./main.9fdfea92.js";import{b as me}from"./confirm.b60b5d2f.js";import{E as re}from"./windi.6360286d.js";import{E as ae}from"./index.361e2245.js";import{E as se}from"./index.b38edc72.js";import"./sortable.esm.a99254e8.js";import"./index.c6da2706.js";import"./index.7c862087.js";import"./index.d96452a6.js";import"./focus-trap.50a00b7f.js";import"./isNil.98bb3b88.js";import"./event.53b2ad83.js";import"./index.7b8c5aef.js";import"./index.fd96ea36.js";import"./error.7e8331f1.js";import"./index.4d568055.js";import"./event.776e7e11.js";import"./scroll.16983984.js";import"./isEqual.4cab74e3.js";import"./_baseIsEqual.dc9e64c4.js";import"./debounce.4e0c106e.js";import"./toNumber.47eea968.js";import"./index.66319a92.js";import"./validator.b5df2420.js";import"./index.42922617.js";import"./index.f15f695e.js";import"./page.881ec888.js";import"./index.f2c1b903.js";import"./index.c10802d9.js";import"./aria.75ec5909.js";import"./guid.c1a40312.js";import"./style.c350d462.js";import"./dayjs.min.ccc40d4f.js";import"./index.4f90a917.js";import"./use-shortcuts.ff4b1f52.js";import"./userWorker.b3a6730b.js";import"./editor.main.d2800f63.js";import"./preload-helper.13a99eb0.js";import"./config.1e73f9c3.js";import"./dark.9459c47b.js";import"./index.c86a187b.js";import"./dropdown.86d3a7fb.js";import"./refs.3132cbdb.js";import"./use-copy-text.ec245a13.js";import"./index.269bf65e.js";import"./monaco.c080bc0a.js";import"./classCompletion.a22e38a6.js";import"./extraLib.d94bb2b4.js";import"./use-save-tip.e3dc783a.js";import"./index.6918f0a5.js";import"./vuedraggable.umd.af323a53.js";import"./index.f154d010.js";import"./index.b9a74c44.js";import"./plugin-vue_export-helper.21dcd24c.js";import"./index.79632b7d.js";import"./index.99e769ef.js";import"./index.d878e15f.js";import"./alert.d2815716.js";import"./index.24947ed8.js";/* empty css                                                               */import"./media-list.vue_vue_type_style_index_0_scoped_true_lang.d235f69b.js";/* empty css                                                          */import"./file.a821eac5.js";/* empty css                                                          *//* empty css                                                                 */import"./use-file-upload.78bd75f1.js";/* empty css                                                         */import"./image-editor.vue_vue_type_style_index_0_scoped_true_lang.ec5f0705.js";import"./image-editor.a4ae94b1.js";import"./index.0e5d639d.js";import"./cloneDeep.c5011aa2.js";import"./_baseClone.b8903935.js";import"./main.esm.e26794cb.js";import"./index.16f227d6.js";import"./index.d24a40b9.js";import"./index.f23f1e38.js";import"./index.a24f458a.js";import"./index.1fd72ea8.js";import"./index.abe211fa.js";import"./plugin-vue_export-helper.41ffa612.js";import"./index.50c16ae5.js";import"./logo-transparent.1566007e.js";const le=I({props:{modelValue:{type:Boolean},selected:null},emits:["update:modelValue"],setup(y,{emit:a}){const s=y,_=R(),{t:n}=D(),c=w(!0),p=w(),u={name:[G(1,50),J(n("common.nameRequiredTips")),Q(),W(O,n("common.viewNameExistsTips"))]},t=w({id:s.selected.id,name:s.selected.name+"_Copy"}),m=()=>g(this,null,function*(){yield p.value.validate(),yield z(t.value.id,t.value.name),c.value=!1,_.load()});return(d,l)=>{const C=Y,h=Z,f=ee,E=oe;return v(),k(E,{"model-value":c.value,width:"600px","close-on-click-modal":!1,title:o(n)("common.copyView"),onClosed:l[3]||(l[3]=e=>a("update:modelValue",!1))},{footer:r(()=>[i(X,{onConfirm:m,onCancel:l[2]||(l[2]=e=>c.value=!1)})]),default:r(()=>[i(f,{ref_key:"form",ref:p,"label-position":"top",model:t.value,rules:o(u),onSubmit:l[1]||(l[1]=A(()=>{},["prevent"])),onKeydown:L(m,["enter"])},{default:r(()=>[i(h,{label:o(n)("common.name"),prop:"name"},{default:r(()=>[i(C,{modelValue:t.value.name,"onUpdate:modelValue":l[0]||(l[0]=e=>t.value.name=e)},null,8,["modelValue"])]),_:1},8,["label"])]),_:1},8,["model","rules","onKeydown"])]),_:1},8,["model-value","title"])}}}),ne={class:"p-24"},pe={class:"flex items-center py-24 space-x-16"},ce={class:"flex items-center"},ue=["title"],de=["title","onClick"],mt=I({setup(y){const{t:a}=D(),s=R(),_=ie(),n=T(),c=w(),p=w(!1),u=m=>{_.hasAccess("view","edit")&&(c.value=m,p.value=!0)},t=m=>g(this,null,function*(){yield me(m.length),s.deleteViews(m.map(d=>d.id))});return s.load(),(m,d)=>{const l=re,C=ae,h=U("router-link"),f=se,E=q("hasPermission");return v(),M("div",ne,[i(te,{name:o(a)("common.views")},null,8,["name"]),b("div",pe,[H((v(),k(C,{round:"","data-cy":"new-view",onClick:d[0]||(d[0]=e=>o(n).push(o(S)({name:"view-edit"})))},{default:r(()=>[b("div",ce,[i(l,{class:"iconfont icon-a-addto"}),N(" "+V(o(a)("common.newView")),1)])]),_:1})),[[E,{feature:"view",action:"edit"}]])]),i(o(F),{data:o(s).list,"show-check":"",permission:{feature:"view",action:"delete"},onDelete:t},{bar:r(({selected:e})=>[e.length===1?(v(),k(B,{key:0,permission:{feature:"view",action:"edit"},icon:"icon-copy",tip:o(a)("common.copy"),circle:"","data-cy":"copy",onClick:x=>u(e[0])},null,8,["tip","onClick"])):$("",!0)]),default:r(()=>[i(f,{label:o(a)("common.name"),prop:"name"},{default:r(({row:e})=>[i(h,{to:o(S)({name:"view-edit",query:{id:e.id}}),"data-cy":"name"},{default:r(()=>[b("span",{title:e.name,class:"ellipsis text-blue cursor-pointer"},V(e.name),9,ue)]),_:2},1032,["to"])]),_:1},8,["label"]),i(f,{label:o(a)("common.usedBy")},{default:r(({row:e})=>[i(P,{id:e.id,relations:e.relations,type:"View"},null,8,["id","relations"])]),_:1},8,["label"]),i(f,{label:o(a)("common.preview")},{default:r(({row:e})=>[b("a",{class:"text-blue ellipsis cursor-pointer",title:e.preview,onClick:x=>o(K)(e.preview)},V(e.relativeUrl),9,de)]),_:1},8,["label"]),i(f,{label:o(a)("common.lastModified"),prop:"lastModified"},{default:r(({row:e})=>[N(V(o(j)(e.lastModified)),1)]),_:1},8,["label"]),i(f,{width:"60px",align:"center"},{default:r(({row:e})=>[i(B,{permission:{feature:"site",action:"log"},icon:"icon-time rounded-full",tip:o(a)("common.version"),"data-cy":"versions",onClick:x=>m.$router.goLogVersions(e.keyHash,e.storeNameHash,e.tableNameHash)},null,8,["tip","onClick"])]),_:1})]),_:1},8,["data"]),p.value?(v(),k(le,{key:0,modelValue:p.value,"onUpdate:modelValue":d[1]||(d[1]=e=>p.value=e),selected:c.value},null,8,["modelValue","selected"])):$("",!0)])}}});export{mt as default};
