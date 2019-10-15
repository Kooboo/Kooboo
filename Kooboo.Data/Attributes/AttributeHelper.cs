//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Attributes;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kooboo.Attributes
{
    public static class AttributeHelper
    {
        public static bool HasAttribute(ISiteObject item, Type attibuteType, bool inherit = false)
        {
            return item.GetType().IsDefined(attibuteType, inherit);
        }

        private static bool HasAttribute(Type SiteObjectType, Type attibuteType, bool inherit = false)
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
                if (item is Diskable disktable)
                {
                    if (disktable.DiskType == DiskType.Binary)
                    {
                        if (ModelItem is IBinaryFile)
                        {
                            return DiskType.Binary;
                        }

                        return DiskType.Json;
                    }

                    if (disktable.DiskType == DiskType.Text)
                    {
                        if (ModelItem is ITextObject)
                        {
                            return DiskType.Text;
                        }

                        return DiskType.Json;
                    }

                    return disktable.DiskType;
                }
            }
            return DiskType.Json;
        }

        public static DiskType GetDiskType(Type modelType)
        {
            var allattributes = modelType.GetCustomAttributes(false);

            foreach (var item in allattributes)
            {
                if (item is Diskable disktable)
                {
                    if (disktable.DiskType == DiskType.Binary)
                    {
                        if (Lib.Reflection.TypeHelper.HasInterface(modelType, typeof(IBinaryFile)))
                        {
                            return DiskType.Binary;
                        }

                        return DiskType.Json;
                    }

                    if (disktable.DiskType == DiskType.Text)
                    {
                        if (Lib.Reflection.TypeHelper.HasInterface(modelType, typeof(ITextObject)))
                        {
                            return DiskType.Text;
                        }

                        return DiskType.Json;
                    }

                    return disktable.DiskType;
                }
            }
            return DiskType.Json;
        }

        public static ActivityInfo GetActivityInfo(Type modelType)
        {
            var allattributes = modelType.GetCustomAttributes(false);

            foreach (var item in allattributes)
            {
                if (item is ActivityEvent activtydefinition)
                {
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

        public static bool IsRoutable(Type modelType)
        {
            return HasAttribute(modelType, typeof(Routable));
        }

        public static bool IsNameAsId(ISiteObject item)
        {
            return HasAttribute(item, typeof(NameAsID));
        }

        public static bool IsSummaryIgnore(MemberInfo fieldOrProperty)
        {
            return fieldOrProperty.IsDefined(typeof(SummaryIgnore));
        }

        public static bool RequireFolder(MemberInfo fieldOrProperty)
        {
            return fieldOrProperty.IsDefined(typeof(RequireFolder));
        }

        public static bool RequireProductType(MemberInfo fieldOrProperty)
        {
            return fieldOrProperty.IsDefined(typeof(RequireProductType));
        }

        public static bool IsCoreObject(Type siteObjectType)
        {
            return Lib.Reflection.TypeHelper.HasInterface(siteObjectType, typeof(ICoreObject));
        }

        public static List<OptionalParameter> GetOptionalParameters(MethodInfo methodinfo)
        {
            List<OptionalParameter> result = new List<OptionalParameter>();
            var allattributes = methodinfo.GetCustomAttributes(false);
            foreach (var item in allattributes)
            {
                if (item is OptionalParameter)
                {
                    var op = item as OptionalParameter;
                    result.Add(op);
                }
            }
            return result;
        }
    }
}