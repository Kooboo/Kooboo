//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Kooboo.Events
//{
//    public class SiteObjectChangeEventHierarchyResolver : IEventHierarchyResolver
//    {
//        public bool Resolve(Type type, HashSet<HierarchyEntry> hierarchy)
//        {
//            var genericBaseType = typeof(Kooboo.Events.Cms.SiteObjectChangeEvent<>);
//            var isSiteObjectChange = type.IsGenericType && type.GetGenericTypeDefinition() == genericBaseType;
//            if (!isSiteObjectChange)
//                return true;

//            var argumentType = type.GetGenericArguments()[0];
//            var baseType = argumentType.BaseType;
//            while (baseType != null && baseType != typeof(object) && baseType.GetInterface("ISiteObject") != null)
//            {
//                var toType = genericBaseType.MakeGenericType(baseType);
//                hierarchy.Add(new HierarchyEntry
//                {
//                    EventType = toType,
//                    Convert = o => Convert(o, toType)
//                });

//                baseType = baseType.BaseType;
//            }

//            return true;
//        }

//        private IEvent Convert(IEvent e, Type toType)
//        {
//            var from = e as Kooboo.Events.Cms.SiteObjectEvent;
//            var to = Activator.CreateInstance(toType) as Kooboo.Events.Cms.SiteObjectEvent;
//            to.ChangeType = from.ChangeType;
//            to.OldValue = from.OldValue;
//            to.Value = from.Value; 
//            to.SiteDb = from.SiteDb;
//            return to;
//        }
//    }
//}
