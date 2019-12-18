declare const k: Kooboo.Web.Frontend.KScriptDefine.KScript;
declare namespace Kooboo.Web.Frontend.KScriptDefine {
   interface KScript {
       /** method or property description */
       user:User;
       execute(code:string,user:User):void;
   }

   interface User {
       name:string;
       pwd:string;
       age:number;
       id:number;
       kType:KType;
   }

   enum KType {
       aaa=0,
       bbb=1,
   }

}
