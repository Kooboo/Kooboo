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
using Jint.Native;

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
            renderContext = context;
        }

        private LinkedList<IDictionary> _stack = new LinkedList<IDictionary>();

        private RepeatCondition _repeatcounter;
        public RepeatCondition RepeatCounter
        {
            get => _repeatcounter ?? (_repeatcounter = new RepeatCondition());
            set => _repeatcounter = value;
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

                foreach (var dic in _stack)
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
        /// <param name="fullPropertyName"></param>
        /// <returns></returns>
        public object GetValueByObjectType(string fullPropertyName)
        {
            // We assume that the {} has been taken off. 
            // It must be in the format of News.Title. 
            string objectType;
            string propertyName = null;

            int dotindex = fullPropertyName.IndexOf(".");
            if (dotindex < 1)
            {
                objectType = fullPropertyName.ToLower();
            }
            else
            {
                objectType = fullPropertyName.Substring(0, dotindex).ToLower();
                propertyName = fullPropertyName.Substring(dotindex + 1);
            }
            foreach (var dictitems in _stack)
            {
                foreach (var item in dictitems.Values)
                {
                    if (item is DataMethodResult methodResult)
                    {
                        var result = GetValueFromMethodResultByObjectType(methodResult, objectType, propertyName);
                        if (result != null)
                        { return result; }
                    }
                    else if (item.GetType().IsClass)
                    {
                        var type = item.GetType();
                        if (type.Name.ToLower() == objectType)
                        {
                            if (!string.IsNullOrEmpty(propertyName))
                            {
                                var result = GetMember(item, propertyName);
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

        public object GetValueByMemberName(string memberName)
        {
            if (memberName.Contains("."))
            {
                return null;
            }

            foreach (var dictitems in _stack)
            {
                foreach (var item in dictitems.Values)
                {
                    if (Kooboo.Lib.Reflection.TypeHelper.IsGenericCollection(item.GetType()))
                    {
                        continue;
                    }
                    if (item is DataMethodResult methodresult)
                    {
                        var objectvalue = methodresult.Value;

                        if (objectvalue != null)
                        {
                            var result = GetMember(objectvalue, memberName);
                            if (result != null)
                            {
                                return result;
                            }
                        }

                    }
                    else if (item.GetType().IsClass)
                    {

                        var result = GetMember(item, memberName);
                        if (result != null)
                        {
                            return result;
                        }

                    }
                }
            }

            return null;

        }

        internal object GetValueFromMethodResultByObjectType(DataMethodResult methodResult, string objectType, string propertyName)
        {
            objectType = objectType.ToLower();
            // the closest children first. 
            if (methodResult.HasChildren)
            {
                foreach (var item in methodResult.Children)
                {
                    var result = GetValueFromMethodResultByObjectType(item.Value, objectType, propertyName);
                    if (result != null)
                    { return result; }
                }
            }
            if (methodResult.ObjectType == objectType)
            {
                var result = GetMember(methodResult.Value, propertyName);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private object GetDictProperty(IDictionary dic, string fullPropertyName)
        {
            var result = _getDictValueCaseInsensitive(dic, fullPropertyName);
            if (result != null)
            {
                return result;
            }

            int dotindex = fullPropertyName.IndexOf(".");

            if (dotindex > -1)
            {
                string key = fullPropertyName.Substring(0, dotindex);

                string subProperty = fullPropertyName.Substring(dotindex + 1);

                result = _getDictValueCaseInsensitive(dic, key);

                if (result == null)
                {
                    return null;
                }

                return GetObjectProperty(result, subProperty);

            }
            return null;
        }

        private object _getDictValueCaseInsensitive(IDictionary dict, string fullKey)
        {
            if (fullKey == "")
            {
                if (dict.Contains(""))
                {
                    return dict[""];
                }

                return null;
            }
            string lowerkey = fullKey.ToLower();
            foreach (var item in dict.Keys)
            {
                if (item.ToString().ToLower() == lowerkey)
                {
                    return dict[item];
                }
            }
            return null;
        }

        private object GetObjectProperty(object obj, string propertyName)
        {
            if (obj is DataMethodResult methodresult)
            {
                object value = GetObjectProperty(methodresult.Value, propertyName) ?? GetDictProperty(methodresult.Children, propertyName);

                return value;
            }

            int dotindex = propertyName.IndexOf(".");
            if (dotindex > -1)
            {
                string key = propertyName.Substring(0, dotindex);
                string subProperty = propertyName.Substring(dotindex + 1);

                object value = GetMember(obj, key);

                if (value == null)
                {
                    return null;
                }

                return GetObjectProperty(value, subProperty);
            }

            return GetMember(obj, propertyName);
        }

        private object GetMember(object obj, string propertyName)
        {
            if (obj is IDynamic content1)
            {
                if (renderContext == null)
                {
                    return content1.GetValue(propertyName);
                }

                return content1.GetValue(propertyName, renderContext);
            }

            if (obj is IDictionary dict)
            {
                if (dict.Contains(propertyName))
                {
                    return dict[propertyName];
                }
                return null;
            }

            if (obj is JObject jObject)
            {
                return Lib.Helper.JsonHelper.GetObject(jObject, propertyName);
            }

            if (obj is XDocument document)
            {
                return Lib.Helper.XmlHelper.GetMember(document, propertyName);
            }

            if (obj is XElement element)
            {
                return Lib.Helper.XmlHelper.GetMember(element, propertyName);
            }

            if (obj is System.Dynamic.ExpandoObject)
            {
                IDictionary<String, Object> value = (IDictionary<String, Object>) obj;
                {
                    value.TryGetValue(propertyName, out var result);
                    return result;
                }
            }

            if (obj is IDictionary<string, object> value1)
            {
                {
                    value1.TryGetValue(propertyName, out var result);
                    return result;
                }
            }

            if (obj is IDictionary<string, string> value2)
            {
                {
                    value2.TryGetValue(propertyName, out var result);
                    return result;
                }
            }

            if (obj is JsValue value3)
            {
                var jsObject = value3.ToObject();
                if (jsObject == null)
                {
                    return null;
                }

                if (jsObject is IDynamic content)
                {
                    return renderContext == null ? content.GetValue(propertyName) : content.GetValue(propertyName, renderContext);
                }

                if (jsObject is IDictionary<string, object> rightvalue1)
                {
                    {
                        rightvalue1.TryGetValue(propertyName, out var result);
                        return result;
                    }
                }

                if (jsObject is IDictionary<string, string> rightvalue)
                {
                    {
                        rightvalue.TryGetValue(propertyName, out var result);
                        return result;
                    }
                }

                return Kooboo.Lib.Reflection.Dynamic.GetObjectMember(jsObject, propertyName);

            }

            return Kooboo.Lib.Reflection.Dynamic.GetObjectMember(obj, propertyName);
        }

        internal object GetValueFromStackItem(IDictionary stackItem, GetValueQuery query)
        {
            object result;
            result = _getDictValueCaseInsensitive(stackItem, query.FullPropertyName);
            if (result != null)
            {
                return result;
            }

            if (query.IsMember)
            {
                foreach (var item in stackItem.Values)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    if (item is DataMethodResult methodresult)
                    {
                        var objectvalue = methodresult.Value;

                        if (objectvalue != null)
                        {
                            result = GetMember(objectvalue, query.MemberName);
                            if (result != null)
                            {
                                return result;
                            }
                        }

                    }
                    else
                    {

                        if (item is IDictionary idic)
                        {
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
                                result = GetMember(item, query.MemberName);
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

                result = _getDictValueCaseInsensitive(stackItem, query.Key);

                if (result != null)
                {
                    return GetObjectProperty(result, query.SubProperty);
                }

                // by object type...
                string objectType = query.Key.ToLower();

                foreach (var item in stackItem.Values)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    if (item is DataMethodResult methodResult)
                    {
                        result = GetValueFromMethodResultByObjectType(methodResult, query.Key, query.SubProperty);
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
                                result = GetMember(item, query.SubProperty);
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
            var item = renderContext.GetItem<Jint.Engine>();
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

                var debugger = renderContext.GetItem<Jint.Engine>("__kooboodebugger");
                result = GetValueFromJsEngine(query, debugger);

                if (result != null)
                {
                    return result;
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
                        rightvalue = GetMember(rightvalue, sub);
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
                            if (jsvalue.Type != Jint.Runtime.Types.Undefined)
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

        public object GetValue(string fullPropertyName)
        {
            var query = new GetValueQuery(fullPropertyName);
            return GetValueByQuery(query);
        }

        public object GetValueByQuery(GetValueQuery query)
        {
            object result;

            foreach (var item in _stack)
            {
                result = GetValueFromStackItem(item, query);
                if (result != null)
                {
                    return result;
                }
            }

            // Get Value from KScript variables... 
            if (Hasvalidchar(query))
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

        private bool Hasvalidchar(GetValueQuery query)
        {
            var checkkey = query.IsMember ? query.MemberName : query.Key;

            return !string.IsNullOrEmpty(checkkey) && checkkey.All(currentchar => Lib.Helper.CharHelper.isAlphanumeric(currentchar) || currentchar == '_');
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
            _stack.AddFirst(data);
            OnDataPush?.Invoke(data);
        }

        public void Pop()
        {
            if (_stack.Count > 0)
            {
                _stack.RemoveFirst();
            }
        }

        public Action<IDictionary> OnDataPush { get; set; }
    }

    public class GetValueQuery
    {
        public GetValueQuery(string fullPropertyName)
        {
            if (fullPropertyName.IndexOf("{") > -1 && fullPropertyName.IndexOf("}") > -1)
            {
                int start = fullPropertyName.IndexOf("{");
                int end = fullPropertyName.LastIndexOf("}");

                FullPropertyName = fullPropertyName.Substring(start + 1, end - start - 1);

                if (start > 0 || end < fullPropertyName.Length - 1)
                {
                    PartialMerge = true;
                    OriginalMergeField = "{" + FullPropertyName + "}";
                }
            }
            else
            {
                FullPropertyName = fullPropertyName;
            }

            int dotindex = FullPropertyName.IndexOf(".");
            if (dotindex < 1)
            {
                IsMember = true;
                MemberName = FullPropertyName;
            }
            else
            {
                Key = FullPropertyName.Substring(0, dotindex);
                SubProperty = FullPropertyName.Substring(dotindex + 1);
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

        public void Push(int total)
        {
            RepeaterCounter counter = new RepeaterCounter {Total = total, Current = 0};

            stack.Push(counter);
        }

        public void Pop()
        {
            stack.Pop();
        }


        public RepeaterCounter CurrentCounter => stack.Count == 0 ? new RepeaterCounter() : stack.First();


        public bool Check(string condition)
        {

            if (string.IsNullOrEmpty(condition))
            {
                return false;
            }

            string lower = condition.ToLower().Trim();

            switch (lower)
            {
                case "odd":
                    return IsOdd(CurrentCounter.Current);
                case "even":
                    return !IsOdd(CurrentCounter.Current);
                case "first":
                    return CurrentCounter.Current == 1;
                case "!first":
                case "nonfirst":
                    return CurrentCounter.Current != 1;
                case "last":
                    return CurrentCounter.Current == CurrentCounter.Total;
                case "!last":
                case "nonlast":
                    return CurrentCounter.Current != CurrentCounter.Total;
                default:
                {
                    if (int.TryParse(lower, out var counter))
                    {
                        return counter == CurrentCounter.Current;
                    }

                    var nth = GetNth(lower);
                    if (nth != null)
                    {
                        var left = CurrentCounter.Current % nth.N;
                        return left == nth.Th;
                    }

                    return false;
                }
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

            if (!int.TryParse(begin.Trim(), out var beginint))
            {
                return null;
            }

            result.N = beginint;

            if (!string.IsNullOrWhiteSpace(end))
            {
                int.TryParse(end.Trim(), out var thint);
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
