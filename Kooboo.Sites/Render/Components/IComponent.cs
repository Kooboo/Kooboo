//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kooboo.Sites.Repository;
using Kooboo.Data.Context;

namespace Kooboo.Sites.Render.Components
{ 
    public interface IComponent
    {
        string TagName { get; }

        // if regular html tag, require the rel=
        bool IsRegularHtmlTag { get;   }

        // for component that use regular html tag and has special objectStore to keep the content... 
        // This is the name of the engine that will be kept in the special objectstore..
        string StoreEngineName { get;   }

        // The const type of object store... In order to calculate the relation.
        // 0 == this will not calculate the relation. 
        byte StoreConstType { get;  }

        string DisplayName(RenderContext Context);
         
        Task<string> RenderAsync(RenderContext context, ComponentSetting settings);

        Dictionary<string, string> Setttings { get; }

        List<ComponentInfo> AvaiableObjects(SiteDb SiteDb);

        string Preview(SiteDb SiteDb, string NameOrId);
    }

    public class ComponentInfo
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
    }

}
