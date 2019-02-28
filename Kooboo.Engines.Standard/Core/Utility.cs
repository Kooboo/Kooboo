//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.Core {
    using System;
    using System.IO;
    using System.Reflection;

    public static class Utility {
        public static string ResourceAsString(string resource, Type scope) {
            using (var resourceStream = GetManifestResourceStream(scope.Assembly,scope,resource))
            using (var reader = new StreamReader(resourceStream))
                return reader.ReadToEnd();
        }

        public static Stream GetManifestResourceStream(Assembly assembly, Type type, string resource)
        {
            var nameSpace = type.Namespace.Replace("SassAndCoffee", "Kooboo.Engines");
            var resourceName = string.Format("{0}.{1}", nameSpace, resource);
            return assembly.GetManifestResourceStream(resourceName);
        }
    }
}
