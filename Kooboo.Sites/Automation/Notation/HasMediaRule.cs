//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Kooboo.Sites.Automation.Notation
//{
//   public class HasMediaRule : INotationAnalyzer
//    {
//        public string Name
//        {
//            get { return "hasmediarule"; }
//        }

//        public Type ReturnType
//        {
//            get { return typeof(bool); }
//        }

//        public object Execute(Dom.Element element)
//        {
           
//            string value = string.Empty;

//            foreach (var item in element.ownerDocument.StyleSheets page.medialrules.item)
//            {
//                if (item.type == enumCSSRuleType.STYLE_RULE)
//                {
//                    CSSStyleRule stylerule = item as CSSStyleRule;
//                    if (stylerule != null && element.matches(stylerule.selectors))
//                    {
//                        value = "yes";
//                        if (stylerule.style.hasPartialProperty("width"))
//                        {
//                            value = "width";
//                            if (!element.notation.ContainsKey(NotationKey.ToLower()))
//                            {
//                                element.notation.Add(NotationKey.ToLower(), value);
//                            }
//                            return;
//                        }

//                    }
//                }
//            }

//            if (!element.notation.ContainsKey(NotationKey.ToLower()))
//            {
//                element.notation.Add(NotationKey.ToLower(), value);
//            }
//        }

//        public List<string> ReturnStringValueList
//        {
//            get { throw new NotImplementedException(); }
//        }
//    }
//}
