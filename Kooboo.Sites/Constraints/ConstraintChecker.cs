//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Models;
using System.Reflection;
using Kooboo.Lib.Reflection;

namespace Kooboo.Sites.Constraints
{
    public static class ConstraintChecker
    {
        private static object _locker = new object();

        private static List<CheckerInstance> _List;
        public static List<CheckerInstance> List
        {
            get
            {
                if (_List == null)
                {
                    lock (_locker)
                    {
                        if (_List == null)
                        {
                            _List = new List<CheckerInstance>();

                            var alltypes = AssemblyLoader.LoadTypeByGenericInterface(typeof(IConstraintChecker<>));
                            foreach (var item in alltypes)
                            {
                                CheckerInstance instance = new CheckerInstance();

                                var iteminstance = Activator.CreateInstance(item);
                                instance.ClassInstance = iteminstance;

                                instance.ModelType = TypeHelper.GetGenericType(item);

                                var meta = item.GetMethod("GetMeta").Invoke(iteminstance, null);
                                if (meta != null)
                                {
                                    var display = meta as DisplayMetaInfo;
                                    instance.Name = display.Name;
                                    instance.Description = display.Description;
                                }

                                instance.HasFix = (bool)item.GetProperty("HasFix").GetValue(iteminstance, null);

                                instance.HasCheck = (bool)item.GetProperty("HasCheck").GetValue(iteminstance, null);


                                instance.FixOnSave = (bool)item.GetProperty("AutoFixOnSave").GetValue(iteminstance, null);

                                instance.Fix = item.GetMethod("Fix");
                                instance.Check = item.GetMethod("Check");

                                _List.Add(instance);
                            }
                        }
                    }
                }

                return _List;
            }
        }

        private static Dictionary<string, List<CheckerInstance>> SiteObjectCheckers = new Dictionary<string, List<CheckerInstance>>(); 

        private static List<CheckerInstance> GetCheckers(Type SiteObjectType)
        {
            string name = SiteObjectType.FullName; 
            
           if (!SiteObjectCheckers.ContainsKey(name))
            {
                lock(_locker)
                {
                    List<CheckerInstance> checkerlist = new List<CheckerInstance>(); 
                    foreach (var item in List)
                    {
                        if (TypeHelper.IsOfBaseTypeOrInterface(SiteObjectType, item.ModelType))
                        {
                            checkerlist.Add(item);  
                        }
                    } 
                    SiteObjectCheckers[name] = checkerlist; 
                } 
            } 
            return SiteObjectCheckers[name];  
        }

        public static void FixOnSave(SiteDb SiteDb, SiteObject SiteObject, string Language = null)
        {
            if (SiteDb == null || SiteObject == null)
            {
                return;
            }
            foreach (var item in GetCheckers(SiteObject.GetType()))
            {
                if (item.FixOnSave)
                {
                    List<object> paras = new List<object>();
                    paras.Add(SiteDb);
                    paras.Add(SiteObject);
                    paras.Add(Language);

                    item.Fix.Invoke(item.ClassInstance, paras.ToArray()); 
                }
            }
        }

        public static void FixConstraint(SiteDb SiteDb, SiteObject SiteObject, string ConstrainName, string Language = null)
        {
            if (string.IsNullOrEmpty(ConstrainName) || SiteDb == null || SiteObject == null)
            {
                return;
            }

            foreach (var item in GetCheckers(SiteObject.GetType()))
            {
                if (item.HasFix && item.Name.ToLower() == ConstrainName.ToLower())
                {
                    List<object> paras = new List<object>();
                    paras.Add(SiteDb);
                    paras.Add(SiteObject);
                    paras.Add(Language);

                    item.Fix.Invoke(item.ClassInstance, paras.ToArray());
                }
            }
        }

        public static List<ConstraintResponse> Check(SiteDb SiteDb, SiteObject SiteObject, string Language)
        {
            List<ConstraintResponse> result = new List<ConstraintResponse>();

            if (SiteDb == null || SiteObject == null)
            {
                return result;
            }
            foreach (var item in GetCheckers(SiteObject.GetType()))
            {
                if (item.HasCheck)
                {
                    List<object> paras = new List<object>();
                    paras.Add(SiteDb);
                    paras.Add(SiteObject);
                    paras.Add(Language);

                    var oneresult = item.Check.Invoke(item.ClassInstance, paras.ToArray());

                    if (oneresult != null)
                    {
                        var listresult = oneresult as List<ConstraintResponse>;
                        if (listresult != null && listresult.Count() > 0)
                        {
                            result.AddRange(listresult);
                        }
                    } 
                }
            }

            return result;
        }

        public static List<ConstraintResponse> CheckConstraint(SiteDb SiteDb, SiteObject SiteObject, string ConstrainName, string Language = null)
        {

            List<ConstraintResponse> result = new List<ConstraintResponse>();

            if (SiteDb == null || SiteObject == null)
            {
                return result;
            }
            foreach (var item in List)
            {
                if (item.HasCheck && TypeHelper.IsOfBaseTypeOrInterface(SiteObject.GetType(), item.ModelType))
                {
                    if (item.Name.ToLower() == ConstrainName.ToLower())
                    {
                        List<object> paras = new List<object>();
                        paras.Add(SiteDb);
                        paras.Add(SiteObject);
                        paras.Add(Language);

                        var oneresult = item.Check.Invoke(item.ClassInstance, paras.ToArray());

                        if (oneresult != null)
                        {
                            var listresult = oneresult as List<ConstraintResponse>;
                            if (listresult != null && listresult.Count() > 0)
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
