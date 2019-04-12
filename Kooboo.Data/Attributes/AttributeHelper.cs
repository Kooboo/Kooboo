//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kooboo.Attributes
{
  public static  class AttributeHelper
    {
        public static bool HasAttribute(ISiteObject item, Type attibuteType, bool inherit = false)
        {
           return item.GetType().IsDefined(attibuteType, inherit); 
        }

        private static bool HasAttribute(Type SiteObjectType,  Type attibuteType, bool inherit = false)
        {
            return SiteObjectType.IsDefined(attibuteType, inherit);
        }

        public static bool IsDiskable(ISiteObject item)
        {
            return HasAttribute(item, typeof(Diskable)); 
        }

        public static bool IsDiskable(Type SiteObjectType)
        {
            return HasAttribute(SiteObjectType, typeof(Diskable));
        }

        public static DiskType GetDiskType(ISiteObject ModelItem)
        { 
            var allattributes = ModelItem.GetType().GetCustomAttributes(false);

            foreach (var item in allattributes)
            {
                if (item is Diskable)
                {
                    var disktable = item as Diskable;

                    if (disktable.DiskType == DiskType.Binary)
                    {
                        if (ModelItem is IBinaryFile)
                        {
                            return DiskType.Binary; 
                        }
                        else
                        {
                            return DiskType.Json; 
                        }
                    }

                    if (disktable.DiskType == DiskType.Text )
                    {
                        if (ModelItem is ITextObject)
                        { 
                            return DiskType.Text; 
                        }
                        else
                        {
                            return DiskType.Json; 
                        }
                    }

                    return disktable.DiskType;
                }
            }
            return DiskType.Json; 
        }

        public static DiskType GetDiskType(Type ModelType)
        {
            var allattributes = ModelType.GetCustomAttributes(false);

            foreach (var item in allattributes)
            {
                if (item is Diskable)
                {
                    var disktable = item as Diskable;

                    if (disktable.DiskType == DiskType.Binary)
                    {
                        if (Lib.Reflection.TypeHelper.HasInterface(ModelType, typeof(IBinaryFile)))
                        {
                            return DiskType.Binary;
                        }
                        else
                        {
                            return DiskType.Json;
                        }
                    }

                    if (disktable.DiskType == DiskType.Text)
                    {
                        if (Lib.Reflection.TypeHelper.HasInterface(ModelType, typeof(ITextObject)))
                        {
                            return DiskType.Text;
                        }
                        else
                        {
                            return DiskType.Json;
                        }
                    }

                    return disktable.DiskType;
                }
            }
            return DiskType.Json;
        }

        public static ActivityInfo GetActivityInfo(Type ModelType)
        {
            var allattributes = ModelType.GetCustomAttributes(false);

            foreach (var item in allattributes)
            {
                if (item is ActivityEvent)
                {
                    var activtydefinition = item as ActivityEvent;
                    return new ActivityInfo()
                    {
                        GroupName = activtydefinition.GroupName,
                        ActivityName = activtydefinition.ActivityName
                    };  
                }
            }
            return null; 
        }

        public static bool IsRoutable(ISiteObject item)
        {
            return HasAttribute(item, typeof(Routable));
        } 

        public static bool IsRoutable(Type ModelType)
        {
            return HasAttribute(ModelType, typeof(Routable));  
        }

        public static bool IsNameAsId(ISiteObject item)
        {
            return HasAttribute(item, typeof(NameAsID));
        }

        public static bool IsSummaryIgnore(MemberInfo FieldOrProperty)
        {
            return FieldOrProperty.IsDefined(typeof(SummaryIgnore)); 
        }

        public static bool RequireFolder(MemberInfo FieldOrProperty)
        {
            return FieldOrProperty.IsDefined(typeof(RequireFolder));
        }

        public static bool RequireProductType(MemberInfo FieldOrProperty)
        {
            return FieldOrProperty.IsDefined(typeof(RequireProductType));
        }

        public static bool IsCoreObject(Type SiteObjectType)
        {
            return Lib.Reflection.TypeHelper.HasInterface(SiteObjectType, typeof(ICoreObject)); 
        }
         
        public static List<OptionalParameter> GetOptionalParameters(MethodInfo methodinfo)
        {
            List<OptionalParameter> Result = new List<OptionalParameter>();  
            var allattributes = methodinfo.GetCustomAttributes(false); 
            foreach (var item in allattributes)
            {
                if (item is OptionalParameter)
                {
                    var op = item as OptionalParameter; 
                    Result.Add(op);  
                }
            }
            return Result;
        } 
    }
}
