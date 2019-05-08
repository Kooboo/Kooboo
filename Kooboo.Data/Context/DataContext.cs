//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Kooboo.Data.Context
{
    public class DataContext
    {

        private RenderContext renderContext;

        public DataContext()
        {

        }

        public DataContext(RenderContext context)
        {
            this.renderContext = context;
        }

        private LinkedList<IDictionary> stack = new LinkedList<IDictionary>();

        private RepeatCondition _repeatcounter;
        public RepeatCondition RepeatCounter
        {
            get
            {
                if (_repeatcounter == null)
                {
                    _repeatcounter = new RepeatCondition();
                }
                return _repeatcounter;
            }
            set
            {
                _repeatcounter = value;
            }
        }

        /// <summary>
        /// The culture value from render content.
        /// </summary>
        public string Culture { get; set; }

        public List<string> Keys
        {
            get
            {
                List<string> keylist = new List<string>();

                foreach (var dic in this.stack)
                {
                    foreach (string key in dic.Keys)
                    {
                        keylist.Add(key);
                    }
                }
                return keylist;
            }
        }

        public bool ContainsKeyPrefix(string prefix)
        {
            return Keys.Any(k => k.StartsWith(prefix));
        }

        public IEnumerable<string> GetKeysByPrefix(string prefix)
        {
            return Keys.Where(k => k.StartsWith(prefix));
        }
        /// <summary>
        /// Get Value by object Type, like News.Title. return any datasource that has the object type of news, and the title. 
        /// </summary>
        /// <param name="FullPropertyName"></param>
        /// <returns></returns>
        public object GetValueByObjectType(string FullPropertyName)
        {
            // We assume that the {} has been taken off. 
            // It must be in the format of News.Title. 
            string ObjectType = null;
            string PropertyName = null;

            int dotindex = FullPropertyName.IndexOf(".");
            if (dotindex < 1)
            {
                ObjectType = FullPropertyName.ToLower();
            }
            else
            {
                ObjectType = FullPropertyName.Substring(0, dotindex).ToLower();
                PropertyName = FullPropertyName.Substring(dotindex + 1);
            }
            foreach (var dictitems in this.stack)
            {
                foreach (var item in dictitems.Values)
                {
                    if (item is DataMethodResult)
                    {
                        var result = GetValueFromMethodResultByObjectType(item as DataMethodResult, ObjectType, PropertyName);
                        if (result != null)
                        { return result; }
                    }
                    else if (item.GetType().IsClass)
                    {
                        var type = item.GetType();
                        if (type.Name.ToLower() == ObjectType)
                        {
                            if (!string.IsNullOrEmpty(PropertyName))
                            {
                                var result = getMember(item, PropertyName);
                                if (result != null)
                                {
                                    return result;
                                }
                            }
                            else
                            {
                                return item;
                            }
                        }
                    }
                }
            }

            return null;

        }

        public object GetValueByMemberName(string MemberName)
        {
            if (MemberName.Contains("."))
            {
                return null;
            }

            foreach (var dictitems in this.stack)
            {
                foreach (var item in dictitems.Values)
                {
                    if (Kooboo.Lib.Reflection.TypeHelper.IsGenericCollection(item.GetType()))
                    {
                        continue;
                    }
                    if (item is DataMethodResult)
                    {
                        var methodresult = item as DataMethodResult;
                        var objectvalue = methodresult.Value;

                        if (objectvalue != null)
                        {
                            var result = getMember(objectvalue, MemberName);
                            if (result != null)
                            {
                                return result;
                            }
                        }

                    }
                    else if (item.GetType().IsClass)
                    {

                        var result = getMember(item, MemberName);
                        if (result != null)
                        {
                            return result;
                        }

                    }
                }
            }

            return null;

        }

        internal object GetValueFromMethodResultByObjectType(DataMethodResult MethodResult, string ObjectType, string PropertyName)
        {
            ObjectType = ObjectType.ToLower();
            // the closest children first. 
            if (MethodResult.HasChildren)
            {
                foreach (var item in MethodResult.Children)
                {
                    var result = GetValueFromMethodResultByObjectType(item.Value, ObjectType, PropertyName);
                    if (result != null)
                    { return result; }
                }
            }
            if (MethodResult.ObjectType == ObjectType)
            {
                var result = getMember(MethodResult.Value, PropertyName);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private object getDictProperty(IDictionary dic, string FullPropertyName)
        {
            var result = _getDictValueCaseInsensitive(dic, FullPropertyName);
            if (result != null)
            {
                return result;
            }

            int dotindex = FullPropertyName.IndexOf(".");

            if (dotindex > -1)
            {
                string key = FullPropertyName.Substring(0, dotindex);

                string subProperty = FullPropertyName.Substring(dotindex + 1);

                result = _getDictValueCaseInsensitive(dic, key);

                if (result == null)
                {
                    return null;
                }
                else
                {
                    return getObjectProperty(result, subProperty);
                }

            }
            return null;
        }

        private object _getDictValueCaseInsensitive(IDictionary dict, string FullKey)
        {
            if (FullKey == "")
            {
                if (dict.Contains(""))
                {
                    return dict[""];
                }
                else
                {
                    return null;
                }
            }
            string lowerkey = FullKey.ToLower();
            foreach (var item in dict.Keys)
            {
                if (item.ToString().ToLower() == lowerkey)
                {
                    return dict[item];
                }
            }
            return null;
        }

        private object getObjectProperty(object obj, string PropertyName)
        {
            if (obj is DataMethodResult)
            {
                DataMethodResult methodresult = obj as DataMethodResult;

                object value = getObjectProperty(methodresult.Value, PropertyName);

                if (value == null)
                {
                    value = getDictProperty(methodresult.Children, PropertyName);
                }

                return value;
            }
            else
            {
                int dotindex = PropertyName.IndexOf(".");
                if (dotindex > -1)
                {
                    string key = PropertyName.Substring(0, dotindex);
                    string subProperty = PropertyName.Substring(dotindex + 1);

                    object value = getMember(obj, key);

                    if (value == null)
                    {
                        return null;
                    }
                    else
                    {
                        return getObjectProperty(value, subProperty);
                    }
                }
                else
                {
                    return getMember(obj, PropertyName);
                }
            }
        }

        private object getMember(object obj, string PropertyName)
        {
            if (obj is IDynamic)
            {
                var content = obj as IDynamic;
                if (this.renderContext == null)
                {
                    return content.GetValue(PropertyName);
                }
                else
                {
                    return content.GetValue(PropertyName, renderContext);
                }
            }
            else if (obj is IDictionary)
            {
                var dict = obj as IDictionary;
                if (dict.Contains(PropertyName))
                {
                    return dict[PropertyName];
                }
                return null;
            }

            else if (obj is JObject)
            {
                return Lib.Helper.JsonHelper.GetObject(obj as JObject, PropertyName);
            }
            else if (obj is XDocument)
            {
                return Lib.Helper.XmlHelper.GetMember(obj as XDocument, PropertyName);
            }
            else if (obj is XElement)
            {
                return Lib.Helper.XmlHelper.GetMember(obj as XElement, PropertyName);
            }

            else if (obj is System.Dynamic.ExpandoObject)
            {
                IDictionary<String, Object> value = obj as IDictionary<String, Object>;
                if (value != null)
                {
                    object result;
                    value.TryGetValue(PropertyName, out result);
                    return result;
                }
                return null;
            }
            else if (obj is IDictionary<string, object>)
            {
                IDictionary<string, object> value = obj as IDictionary<string, object>;
                if (value != null)
                {
                    object result;
                    value.TryGetValue(PropertyName, out result);
                    return result;
                }
                return null;
            }

            else if (obj is IDictionary<string, string>)
            {
                IDictionary<string, string> value = obj as IDictionary<string, string>;
                if (value != null)
                {
                    string result;
                    value.TryGetValue(PropertyName, out result);
                    return result;
                }
                return null;
            }
            else if (obj is Jint.Native.JsValue)
            {
                var value = obj as Jint.Native.JsValue;

                var jsObject = value.ToObject();
                if (jsObject == null)
                {
                    return null;
                }

                if (jsObject is IDynamic)
                {
                    var content = jsObject as IDynamic;
                    if (this.renderContext == null)
                    {
                        return content.GetValue(PropertyName);
                    }
                    else
                    {
                        return content.GetValue(PropertyName, renderContext);
                    }
                }

                else  if (jsObject is IDictionary<string, object>)
                {
                    IDictionary<String, Object> rightvalue = jsObject as IDictionary<String, Object>;
                    if (rightvalue != null)
                    {
                        object result;
                        rightvalue.TryGetValue(PropertyName, out result);
                        return result;
                    }
                }
                else if (jsObject is IDictionary<string, string>)
                {
                    IDictionary<String, string> rightvalue = jsObject as IDictionary<String, string>;
                    if (rightvalue != null)
                    {
                        string result;
                        rightvalue.TryGetValue(PropertyName, out result);
                        return result;
                    }
                }

                else
                {
                    if (jsObject is System.Dynamic.ExpandoObject)
                    {
                        IDictionary<String, Object> expvalue = obj as IDictionary<String, Object>;
                        if (expvalue != null)
                        {
                            object result;
                            expvalue.TryGetValue(PropertyName, out result);
                            return result;
                        }
                        return null;
                    }
                    else
                    {
                        return Kooboo.Lib.Reflection.Dynamic.GetObjectMember(jsObject, PropertyName);
                    }
                }

            }

            return Kooboo.Lib.Reflection.Dynamic.GetObjectMember(obj, PropertyName);
        }

        internal object GetValueFromStackItem(IDictionary StackItem, GetValueQuery query)
        {
            object result;
            result = _getDictValueCaseInsensitive(StackItem, query.FullPropertyName);
            if (result != null)
            {
                return result;
            }

            if (query.IsMember)
            {
                foreach (var item in StackItem.Values)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    if (item is DataMethodResult)
                    {
                        var methodresult = item as DataMethodResult;
                        var objectvalue = methodresult.Value;

                        if (objectvalue != null)
                        {
                            result = getMember(objectvalue, query.MemberName);
                            if (result != null)
                            {
                                return result;
                            }
                        }

                    }
                    else
                    {

                        if (item is IDictionary)
                        {
                            var idic = item as IDictionary;
                            result = _getDictValueCaseInsensitive(idic, query.MemberName);
                            if (result != null)
                            {
                                return result;
                            }
                        }
                        else
                        { 
                            var type = item.GetType();

                            if (!type.IsValueType || !type.IsPrimitive)
                            {
                                result = getMember(item, query.MemberName);
                                if (result != null)
                                {
                                    return result;
                                }
                            }
                        }

                    }
                }
            }

            else
            {

                result = _getDictValueCaseInsensitive(StackItem, query.Key);

                if (result != null)
                {
                    return getObjectProperty(result, query.SubProperty);
                }

                // by object type...
                string objectType = query.Key.ToLower();

                foreach (var item in StackItem.Values)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    if (item is DataMethodResult)
                    {
                        result = GetValueFromMethodResultByObjectType(item as DataMethodResult, query.Key, query.SubProperty);
                        if (result != null)
                        { return result; }
                    }
                    else if (item.GetType().IsClass)
                    {
                        var type = item.GetType();
                        if (type.Name.ToLower() == objectType)
                        {
                            if (!string.IsNullOrEmpty(query.SubProperty))
                            {
                                result = getMember(item, query.SubProperty);
                                if (result != null)
                                {
                                    return result;
                                }
                            }
                            else
                            {
                                return item;
                            }
                        }
                    }
                }

            }

            return null;
        }


        internal object GetValueFromKScript(GetValueQuery query)
        {
            var item = this.renderContext.GetItem<Jint.Engine>();
            if (item == null)
            {
                var debugger = this.renderContext.GetItem<Jint.Engine>("__kooboodebugger");
                var result = GetValueFromJsEngine(query, debugger);

                if (result != null)
                {
                    return result;
                }
            }
            else
            {
                var result = GetValueFromJsEngine(query, item);

                if (result != null)
                {
                    return result;
                }
                else
                {
                    var debugger = this.renderContext.GetItem<Jint.Engine>("__kooboodebugger");
                    result = GetValueFromJsEngine(query, debugger);

                    if (result != null)
                    {
                        return result;
                    }

                }
            }

            return null;
        }

        internal object GetValueFromJsEngine(GetValueQuery query, Jint.Engine engine)
        {

            if (engine == null)
            {
                return null;
            }

            if (query.IsMember)
            {
                var jsvalue = engine.GetValue(query.MemberName);
                if (jsvalue != null && jsvalue.Type != Jint.Runtime.Types.Undefined)
                {
                    return jsvalue.ToObject();
                }
            }
            else
            {
                var value = engine.GetValue(query.Key);

                if (value != null && value.Type != Jint.Runtime.Types.Undefined)
                {
                    string[] subs = query.SubProperty.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    object rightvalue = value;

                    foreach (var sub in subs)
                    {
                        rightvalue = getMember(rightvalue, sub);
                        if (rightvalue == null)
                        {
                            break;
                        }
                    }

                    if (rightvalue != null)
                    {
                        if (rightvalue is Jint.Native.JsValue)
                        {
                            var jsvalue = rightvalue as Jint.Native.JsValue;
                            if (jsvalue != null && jsvalue.Type != Jint.Runtime.Types.Undefined)
                            {
                                return jsvalue.ToObject();
                            }
                        }
                        else
                        {
                            return rightvalue;
                        }

                    }
                }
            }

            return null;

        }

        public object GetValue(string FullPropertyName)
        {
            var query = new GetValueQuery(FullPropertyName);
            return GetValueByQuery(query);
        }

        public object GetValueByQuery(GetValueQuery query)
        {
            object result = null;

            foreach (var item in this.stack)
            {
                result = GetValueFromStackItem(item, query);
                if (result != null)
                {
                    return result;
                }
            }

            // Get Value from KScript variables... 
            if (hasvalidchar(query))
            {
                var jsresult  =  GetValueFromKScript(query);
                if (jsresult !=null)
                {
                    var type = jsresult.GetType(); 
                    if (!type.Name.Contains("Func"))
                    {
                       return jsresult; 
                    }
                }
                
            }
            return null;
        }

        private bool hasvalidchar(GetValueQuery query)
        {
            string checkkey = null;
            if (query.IsMember)
            {
                checkkey = query.MemberName;
            }
            else
            {
                checkkey = query.Key;
            }

            if (!string.IsNullOrEmpty(checkkey))
            {
                for (int i = 0; i < checkkey.Length; i++)
                {
                    var currentchar = checkkey[i];
                    if (!Lib.Helper.CharHelper.isAlphanumeric(currentchar) && currentchar != '_')
                    {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }

        public void Push(string key, object value)
        {
            Push(new Dictionary<string, object> { { key, value } });
        }

        public void Push(IDictionary data)
        {
            foreach (var item in data.Keys)
            {
                if (item.ToString() == "id")
                {
                    var fuck = item;
                }
            }
            stack.AddFirst(data);
            OnDataPush?.Invoke(data);
        }

        public void Pop()
        {
            if (stack.Count > 0)
            {
                stack.RemoveFirst();
            }
        }

        public Action<IDictionary> OnDataPush { get; set; }
    }

    public class GetValueQuery
    {
        public GetValueQuery(string FullPropertyName)
        {
            if (FullPropertyName.IndexOf("{") > -1 && FullPropertyName.IndexOf("}") > -1)
            {
                int start = FullPropertyName.IndexOf("{");
                int end = FullPropertyName.LastIndexOf("}");

                this.FullPropertyName = FullPropertyName.Substring(start + 1, end - start - 1);

                if (start > 0 || end < FullPropertyName.Length - 1)
                {
                    this.PartialMerge = true;
                    this.OriginalMergeField = "{" + this.FullPropertyName + "}";
                }
            }
            else
            {
                this.FullPropertyName = FullPropertyName;
            }

            int dotindex = this.FullPropertyName.IndexOf(".");
            if (dotindex < 1)
            {
                this.IsMember = true;
                this.MemberName = this.FullPropertyName;
            }
            else
            {
                this.Key = this.FullPropertyName.Substring(0, dotindex);
                this.SubProperty = this.FullPropertyName.Substring(dotindex + 1);
            }

        }
        public string FullPropertyName { get; set; }

        public string Key { get; set; }

        public string SubProperty { get; set; }

        public bool IsMember { get; set; }

        public string MemberName { get; set; }

        public bool PartialMerge { get; set; }

        public string OriginalMergeField { get; set; }
    }

    public class RepeatCondition
    {

        public Stack<RepeaterCounter> stack = new Stack<RepeaterCounter>();

        public void Push(int Total)
        {
            RepeaterCounter counter = new RepeaterCounter();
            counter.Total = Total;
            counter.Current = 0;

            this.stack.Push(counter);
        }

        public void Pop()
        {
            this.stack.Pop();
        }


        public RepeaterCounter CurrentCounter
        {
            get
            {
                if (this.stack.Count == 0)
                { return new RepeaterCounter(); }
                return this.stack.First();
            }
        }


        public bool Check(string condition)
        {

            if (string.IsNullOrEmpty(condition))
            {
                return false;
            }

            string lower = condition.ToLower().Trim();

            if (lower == "odd")
            {
                return IsOdd(this.CurrentCounter.Current);
            }
            else if (lower == "even")
            {
                return !IsOdd(this.CurrentCounter.Current);
            }
            else if (lower == "first")
            {
                return this.CurrentCounter.Current == 1;
            }
           else if (lower == "!first" || lower == "nonfirst")
            {
                return this.CurrentCounter.Current != 1; 
            }
            else if (lower == "last")
            {
                return this.CurrentCounter.Current == this.CurrentCounter.Total;
            }
            else if (lower == "!last" || lower == "nonlast")
            {
                return this.CurrentCounter.Current != this.CurrentCounter.Total;
            }
            else
            {
                int counter;

                if (int.TryParse(lower, out counter))
                {
                    return counter == this.CurrentCounter.Current;
                }

                var nth = GetNth(lower);
                if (nth != null)
                {
                    var left = this.CurrentCounter.Current % nth.N;
                    return left == nth.Th;
                }

                return false;
            }
        }

        private bool IsOdd(int value)
        {
            return value % 2 != 0;
        }

        private Nth GetNth(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }
            string ntype = input.ToLower().Replace("/", " ");
            ntype = ntype.Replace("\\", " ");
            ntype = ntype.Replace("+", " ");
            ntype = ntype.Replace("n", " ");
            ntype = ntype.Replace("x", " ");
            ntype = ntype.Replace("(", " ");
            ntype = ntype.Replace(")", " ");

            int index = ntype.IndexOf(" ");

            string begin = null;
            string end = null;
            if (index > -1)
            {
                begin = ntype.Substring(0, index);
                end = ntype.Substring(index);
            }

            if (string.IsNullOrWhiteSpace(begin))
            {
                return null;
            }

            Nth result = new Nth();

            int beginint = 0;
            if (!int.TryParse(begin.Trim(), out beginint))
            {
                return null;
            }

            result.N = beginint;

            int thint = 0;

            if (!string.IsNullOrWhiteSpace(end))
            {
                int.TryParse(end.Trim(), out thint);
                result.Th = thint;
            }
            return result;
        }

        public class RepeaterCounter
        {
            public int Total { get; set; }
            public int Current { get; set; }
        }

        public class Nth
        {
            public int N { get; set; }

            public int Th { get; set; }
        }

    }
}
