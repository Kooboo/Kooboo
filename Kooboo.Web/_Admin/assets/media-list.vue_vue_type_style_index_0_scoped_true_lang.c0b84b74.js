import{d as i}from"./i18n.bcd18f8a.js";import{d as t}from"./main.582f9de6.js";import{a}from"./replace-all.d441bf14.js";const d=i.global.t,n=(e,s)=>t.get(a("Media/PagedList"),e,{hiddenLoading:s}),l=(e,s)=>t.get(a("Media/PagedListBy"),e,{hiddenLoading:s}),p=e=>a(`/_api/v2/Upload/Image?provider=${e||"default"}`),m=e=>t.post(a("Media/CreateFolder"),e,void 0,{successMessage:d("common.createSuccess")}),u=(e,s)=>t.post(a(`Media/DeleteFolders?provider=${s||"default"}`),e,void 0,{successMessage:d("common.deleteSuccess")}),M=(e,s)=>t.post(a(`Media/DeleteImages?provider=${s||"default"}`),e,void 0,{successMessage:d("common.deleteSuccess")}),v=(e,s)=>t.post(a(`Download/Images?provider=${s}`),e,void 0,{}),_=e=>t.get(a("Media/Get"),e),U=e=>t.post(a("Media/ImageUpdate"),e,void 0,{successMessage:d("common.saveSuccess")}),f=(e,s,o)=>t.get(a(`Upload/IsUniqueKey?provider=${e}&type=media`),{name:s,oldName:o},{hiddenLoading:!0,hiddenError:!0});export{U as a,u as b,M as c,v as d,n as e,l as f,p as g,m as h,f as i,_ as j};