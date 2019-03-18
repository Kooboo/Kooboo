//using Kooboo.Data.Context;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Kooboo.Sites.Scripting.Global
//{
     
//    public class kPay
//    {
//        private RenderContext context { get; set; }
//        public kPay(RenderContext context)
//        {
//            this.context = context;
//        }

//        public IPaymentMethod GetTable(string Name)
//        {
//            var db = Kooboo.Data.DB.GetKDatabase(this.context.WebSite);
//            var tb = db.GetOrCreateTable(Name);
//            return new IPaymentMethod(tb);
//        }

//        public IPaymentMethod Table(string Name)
//        {
//            var db = Kooboo.Data.DB.GetKDatabase(this.context.WebSite);
//            var tb = db.GetOrCreateTable(Name);
//            return new IPaymentMethod(tb);
//        }

//        public IPaymentMethod this[string key]
//        {
//            get
//            {
//                return GetTable(key);
//            }
//            set
//            {

//            }
//        }

//    }

//}
