var x=(b,r,v)=>new Promise((s,c)=>{var R=d=>{try{t(v.next(d))}catch(u){c(u)}},y=d=>{try{t(v.throw(d))}catch(u){c(u)}},t=d=>d.done?s(d.value):Promise.resolve(d.value).then(R,y);t((v=v.apply(b,r)).next())});import{_ as z}from"./k-table.7d05e82e.js";import{d as J,g as j,v as I,w as Q,o as W}from"./i18n.bcd18f8a.js";import{u as X}from"./use-operation-dialog.175b540a.js";import{a as w}from"./replace-all.d441bf14.js";import{d as U}from"./main.582f9de6.js";import{a as E,r as Y,i as Z}from"./validate.238a9986.js";import{_ as ee}from"./index.7a75fca3.js";import{E as le}from"./index.5cbbc5d7.js";import{E as te,a as oe}from"./index.6d63eb53.js";import{E as ae,a as ne}from"./index.6d22f937.js";import{E as ie}from"./index.d4a6b2d5.js";import{d as K,g as h,E as se,c as $,i as me,o as m,j as A,w as o,b as i,u as e,a as k,b6 as D,aH as C,t as p,F,e as S,k as O,G as re,h as de,b7 as ue,f as g}from"./url.8f5ec20c.js";import{B as pe}from"./breadcrumb.edaec300.js";import{b as ce}from"./confirm.eadb49f1.js";import{E as fe}from"./windi.19264205.js";import{E as be}from"./index.aec72f69.js";import{E as _e}from"./index.b83a1079.js";import"./sortable.esm.a99254e8.js";import"./icon-button.315b6443.js";import"./index.ec6ad7db.js";import"./focus-trap.eafcfd1f.js";import"./isNil.98bb3b88.js";import"./event.53b2ad83.js";import"./index.05e21f33.js";import"./index.3a977dfb.js";import"./index.c80f5028.js";import"./index.564bc658.js";import"./isEqual.11d86bcc.js";import"./_baseIsEqual.547729d3.js";import"./error.7e8331f1.js";import"./index.daafc4da.js";import"./event.776e7e11.js";import"./index.59b1471f.js";import"./index.50c16ae5.js";import"./preload-helper.13a99eb0.js";import"./index.2341329b.js";import"./index.bda83f28.js";import"./index.bff48780.js";import"./index.649f6c77.js";import"./index.a32fb6e5.js";import"./style.9c8f6403.js";import"./toNumber.6efebd6a.js";import"./_baseClone.eeff2792.js";import"./index.79f78425.js";import"./scroll.4888a9e9.js";import"./debounce.730e1961.js";import"./index.9a83ee01.js";import"./validator.b73911a9.js";import"./index.a3d8335f.js";import"./refs.d2253dd4.js";/* empty css                                                               */import"./plugin-vue_export-helper.21dcd24c.js";import"./plugin-vue_export-helper.41ffa612.js";import"./logo-transparent.1566007e.js";import"./index.10f642b2.js";import"./aria.75ec5909.js";const L=J.global.t,ve=()=>U.get(w("TableRelation/list")),ye=b=>U.post(w("TableRelation/Deletes"),b,void 0,{successMessage:L("common.deleteSuccess")}),Be=()=>U.get(w("TableRelation/getTablesAndFields")),Ve=()=>U.get(w("TableRelation/getRelationTypes")),Ae=b=>U.post(w("TableRelation/post"),b,void 0,{successMessage:L("common.createSuccess")}),Re=b=>U.get(w("TableRelation/isUniqueName"),{name:b},{hiddenLoading:!0,hiddenError:!0}),ge=K({props:{modelValue:{type:Boolean}},emits:["create-success"],setup(b,{emit:r}){const v=b,{t:s}=j(),{visible:c,handleClose:R}=X(v,r),y=h(),t=se({name:"",tableA:"",fieldA:"",tableB:"",fieldB:"",relation:""}),d={name:[E(s("common.fieldRequiredTips")),Y(1,50),Z(Re,s("common.RelationNameExistsTips"))],tableA:E(s("common.fieldRequiredTips")),fieldA:E(s("common.fieldRequiredTips")),tableB:E(s("common.fieldRequiredTips")),fieldB:E(s("common.fieldRequiredTips")),relation:E(s("common.fieldRequiredTips"))},u=h([]),_=h([]),q=$(()=>u.value.find(f=>f.name===t.tableA)),a=$(()=>u.value.find(f=>f.name===t.tableB));me(()=>c.value,f=>{var n;f&&((n=y.value)==null||n.resetFields(),P())});function P(){return x(this,null,function*(){const[f,n]=yield Promise.all([Be(),Ve()]);u.value=f,_.value=n;const T=u.value[0];if(T){t.tableA=T.name,t.tableB=T.name;const V=T.fields[0];V&&(t.fieldA=V,t.fieldB=V)}const B=_.value[0];B&&(t.relation=B.type)})}function M(){var f;(f=y.value)==null||f.validate(n=>x(this,null,function*(){n&&(yield Ae(t),R(),r("create-success"))}))}return(f,n)=>{const T=le,B=te,V=ae,N=ne,G=oe,H=ie;return m(),A(H,{modelValue:e(c),"onUpdate:modelValue":n[7]||(n[7]=l=>re(c)?c.value=l:null),width:"600px","close-on-click-modal":!1,title:e(s)("common.tableRelation"),onClose:e(R)},{footer:o(()=>[i(ee,{"confirm-label":e(s)("common.create"),onConfirm:M,onCancel:e(R)},null,8,["confirm-label","onCancel"])]),default:o(()=>[i(G,{ref_key:"createForm",ref:y,model:e(t),rules:e(d),"label-position":"top",onSubmit:n[6]||(n[6]=Q(()=>{},["prevent"])),onKeydown:W(M,["enter"])},{default:o(()=>[i(B,{prop:"name",label:e(s)("common.name")},{default:o(()=>[i(T,{modelValue:e(t).name,"onUpdate:modelValue":n[0]||(n[0]=l=>e(t).name=l)},null,8,["modelValue"])]),_:1},8,["label"]),i(B,{prop:"tableA",label:e(s)("common.tableA")},{default:o(()=>[i(N,{modelValue:e(t).tableA,"onUpdate:modelValue":n[1]||(n[1]=l=>e(t).tableA=l)},{default:o(()=>[(m(!0),k(F,null,D(u.value,l=>(m(),A(V,{key:l.name,value:l.name},{default:o(()=>[C(p(l.name),1)]),_:2},1032,["value"]))),128))]),_:1},8,["modelValue"])]),_:1},8,["label"]),S(i(B,{prop:"fieldA",label:e(s)("common.fieldA")},{default:o(()=>[e(q)?(m(),A(N,{key:0,modelValue:e(t).fieldA,"onUpdate:modelValue":n[2]||(n[2]=l=>e(t).fieldA=l)},{default:o(()=>[(m(!0),k(F,null,D(e(q).fields,l=>(m(),A(V,{key:l,value:l},{default:o(()=>[C(p(l),1)]),_:2},1032,["value"]))),128))]),_:1},8,["modelValue"])):O("",!0)]),_:1},8,["label"]),[[I,e(q)]]),i(B,{prop:"relation",label:e(s)("common.relation")},{default:o(()=>[i(N,{modelValue:e(t).relation,"onUpdate:modelValue":n[3]||(n[3]=l=>e(t).relation=l)},{default:o(()=>[(m(!0),k(F,null,D(_.value,l=>(m(),A(V,{key:l.type,value:l.type,label:l.displayName},{default:o(()=>[C(p(l.displayName),1)]),_:2},1032,["value","label"]))),128))]),_:1},8,["modelValue"])]),_:1},8,["label"]),i(B,{prop:"tableB",label:e(s)("common.tableB")},{default:o(()=>[i(N,{modelValue:e(t).tableB,"onUpdate:modelValue":n[4]||(n[4]=l=>e(t).tableB=l)},{default:o(()=>[(m(!0),k(F,null,D(u.value,l=>(m(),A(V,{key:l.name,value:l.name},{default:o(()=>[C(p(l.name),1)]),_:2},1032,["value"]))),128))]),_:1},8,["modelValue"])]),_:1},8,["label"]),S(i(B,{prop:"fieldB",label:e(s)("common.fieldB")},{default:o(()=>[e(a)?(m(),A(N,{key:0,modelValue:e(t).fieldB,"onUpdate:modelValue":n[5]||(n[5]=l=>e(t).fieldB=l)},{default:o(()=>[(m(!0),k(F,null,D(e(a).fields,l=>(m(),A(V,{key:l,value:l},{default:o(()=>[C(p(l),1)]),_:2},1032,["value"]))),128))]),_:1},8,["modelValue"])):O("",!0)]),_:1},8,["label"]),[[I,e(a)]])]),_:1},8,["model","rules","onKeydown"])]),_:1},8,["modelValue","title","onClose"])}}}),Te={class:"p-24"},Ee={class:"flex items-center py-24 space-x-16"},ke=["title"],Ce=["title"],we=["title"],Ue={class:"text-green","data-cy":"relation"},qe=["title"],Ne=["title"],hl=K({setup(b){const{t:r}=j(),v=h(),s=h(!1);de(()=>{c()});function c(){return x(this,null,function*(){v.value=yield ve()})}function R(y){return x(this,null,function*(){yield ce(y.length);const t=y.map(d=>d.id);yield ye({ids:t}),c()})}return(y,t)=>{const d=fe,u=be,_=_e,q=ue("hasPermission");return m(),k("div",Te,[i(pe,{name:e(r)("common.tableRelation")},null,8,["name"]),g("div",Ee,[S((m(),A(u,{round:"","data-cy":"create-table-relation",onClick:t[0]||(t[0]=a=>s.value=!0)},{default:o(()=>[i(d,{class:"iconfont icon-a-addto"}),C(" "+p(e(r)("common.new")),1)]),_:1})),[[q,{feature:"tableRelation",action:"edit"}]])]),i(e(z),{data:v.value,"show-check":"",permission:{feature:"tableRelation",action:"delete"},onDelete:R},{default:o(()=>[i(_,{label:e(r)("common.name"),prop:"name"},{default:o(({row:a})=>[g("span",{class:"ellipsis",title:a.name,"data-cy":"name"},p(a.name),9,ke)]),_:1},8,["label"]),i(_,{label:e(r)("common.tableA"),prop:"tableA"},{default:o(({row:a})=>[g("span",{class:"ellipsis",title:a.tableA,"data-cy":"table-A"},p(a.tableA),9,Ce)]),_:1},8,["label"]),i(_,{label:e(r)("common.fieldA"),prop:"fieldA"},{default:o(({row:a})=>[g("span",{class:"ellipsis",title:a.fieldA,"data-cy":"field-A"},p(a.fieldA),9,we)]),_:1},8,["label"]),i(_,{label:e(r)("common.relation")},{default:o(({row:a})=>[g("span",Ue,p(a.relationName),1)]),_:1},8,["label"]),i(_,{label:e(r)("common.tableB"),prop:"tableB"},{default:o(({row:a})=>[g("span",{class:"ellipsis",title:a.tableB,"data-cy":"table-B"},p(a.tableB),9,qe)]),_:1},8,["label"]),i(_,{label:e(r)("common.fieldB"),prop:"fieldB"},{default:o(({row:a})=>[g("span",{class:"ellipsis",title:a.fieldB,"data-cy":"field-B"},p(a.fieldB),9,Ne)]),_:1},8,["label"])]),_:1},8,["data"]),i(ge,{modelValue:s.value,"onUpdate:modelValue":t[1]||(t[1]=a=>s.value=a),onCreateSuccess:c},null,8,["modelValue"])])}}});export{hl as default};