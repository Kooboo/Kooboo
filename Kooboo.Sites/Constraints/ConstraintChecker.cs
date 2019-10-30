//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Lib.Reflection;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kooboo.Sites.Constraints
{
    public static class ConstraintChecker
    {
        private static object _locker = new object();

        private static List<CheckerInstance> _list;

        public static List<CheckerInstance> List
        {
            get
            {
                if (_list == null)
                {
                    lock (_locker)
                    {
                        if (_list == null)
                        {
                            _list = new List<CheckerInstance>();

                            var alltypes = AssemblyLoader.LoadTypeByGenericInterface(typeof(IConstraintChecker<>));
                            foreach (var item in alltypes)
                            {
                                CheckerInstance instance = new CheckerInstance();

                                var iteminstance = Activator.CreateInstance(item);
                                instance.ClassInstance = iteminstance;

                                instance.ModelType = TypeHelper.GetGenericType(item);

                                var meta = item.GetMethod("GetMeta")?.Invoke(iteminstance, null);
                                if (meta != null)
                                {
                                    var display = meta as DisplayMetaInfo;
                                    instance.Name = display?.Name;
                                    instance.Description = display.Description;
                                }

                                instance.HasFix = (bool)item.GetProperty("HasFix")?.GetValue(iteminstance, null);

                                instance.HasCheck = (bool)item.GetProperty("HasCheck")?.GetValue(iteminstance, null);

                                instance.FixOnSave = (bool)item.GetProperty("AutoFixOnSave")?.GetValue(iteminstance, null);

                                instance.Fix = item.GetMethod("Fix");
                                instance.Check = item.GetMethod("Check");

                                _list.Add(instance);
                            }
                        }
                    }
                }

                return _list;
            }
        }

        private static Dictionary<string, List<CheckerInstance>> SiteObjectCheckers = new Dictionary<string, List<CheckerInstance>>();

        private static List<CheckerInstance> GetCheckers(Type siteObjectType)
        {
            string name = siteObjectType.FullName;

            if (!SiteObjectCheckers.ContainsKey(name))
            {
                lock (_locker)
                {
                    List<CheckerInstance> checkerlist = new List<CheckerInstance>();
                    foreach (var item in List)
                    {
                        if (TypeHelper.IsOfBaseTypeOrInterface(siteObjectType, item.ModelType))
                        {
                            checkerlist.Add(item);
                        }
                    }
                    SiteObjectCheckers[name] = checkerlist;
                }
            }
            return SiteObjectCheckers[name];
        }

        public static void FixOnSave(SiteDb siteDb, SiteObject siteObject, string language = null)
        {
            if (siteDb == null || siteObject == null)
            {
                return;
            }
            foreach (var item in GetCheckers(siteObject.GetType()))
            {
                if (item.FixOnSave)
                {
                    List<object> paras = new List<object> {siteDb, siteObject, language};

                    item.Fix.Invoke(item.ClassInstance, paras.ToArray());
                }
            }
        }

        public static void FixConstraint(SiteDb siteDb, SiteObject siteObject, string constrainName, string language = null)
        {
            if (string.IsNullOrEmpty(constrainName) || siteDb == null || siteObject == null)
            {
                return;
            }

            foreach (var item in GetCheckers(siteObject.GetType()))
            {
                if (item.HasFix && item.Name.ToLower() == constrainName.ToLower())
                {
                    List<object> paras = new List<object> {siteDb, siteObject, language};

                    item.Fix.Invoke(item.ClassInstance, paras.ToArray());
                }
            }
        }

        public static List<ConstraintResponse> Check(SiteDb siteDb, SiteObject siteObject, string language)
        {
            List<ConstraintResponse> result = new List<ConstraintResponse>();

            if (siteDb == null || siteObject == null)
            {
                return result;
            }
            foreach (var item in GetCheckers(siteObject.GetType()))
            {
                if (item.HasCheck)
                {
                    List<object> paras = new List<object> {siteDb, siteObject, language};

                    var oneresult = item.Check.Invoke(item.ClassInstance, paras.ToArray());

                    if (oneresult != null)
                    {
                        if (oneresult is List<ConstraintResponse> listresult && listresult.Any())
                        {
                            result.AddRange(listresult);
                        }
                    }
                }
            }

            return result;
        }

        public static List<ConstraintResponse> CheckConstraint(SiteDb siteDb, SiteObject siteObject, string constrainName, string language = null)
        {
            List<ConstraintResponse> result = new List<ConstraintResponse>();

            if (siteDb == null || siteObject == null)
            {
                return result;
            }
            foreach (var item in List)
            {
                if (item.HasCheck && TypeHelper.IsOfBaseTypeOrInterface(siteObject.GetType(), item.ModelType))
                {
                    if (item.Name.ToLower() == constrainName.ToLower())
                    {
                        List<object> paras = new List<object> {siteDb, siteObject, language};

                        var oneresult = item.Check.Invoke(item.ClassInstance, paras.ToArray());

                        if (oneresult != null)
                        {
                            if (oneresult is List<ConstraintResponse> listresult && listresult.Any())
                            {
                                result.AddRange(listresult);
                            }
                        }
                    }
                }
            }

            return result;
        }
    }

    public class CheckerInstance
    {
        public string Name { get; set; }

        public bool HasFix { get; set; }

        public bool HasCheck { get; set; }

        public bool FixOnSave { get; set; }

        public string Description { get; set; }

        public object ClassInstance { get; set; }

        /// <summary>
        /// The type or interface of the siteobject that this constraint is designed for...
        /// </summary>
        public Type ModelType { get; set; }

        public MethodInfo Fix { get; set; }

        public MethodInfo Check { get; set; }
    }
}