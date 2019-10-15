using Kooboo.Data.Definition.KModel.Attributes;
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Definition
{
    public static class KMetaProvider
    {
        public static KModel.KMeta PraseMeta(Type modelType)
        {
            KModel.KMeta meta = new KModel.KMeta();

            foreach (var item in modelType.GetProperties())
            {
                if (item.CanRead && item.CanWrite && item.PropertyType.IsPublic)
                {
                    Kooboo.Data.Definition.KModel.Column col = new KModel.Column {Name = item.Name};

                    var allattributes = item.GetCustomAttributes(false);

                    foreach (var att in allattributes)
                    {
                        if (att is IMetaAttribute metaAtt)
                        {
                            if (metaAtt.IsHeader)
                            {
                                col.Header[metaAtt.PropertyName] = metaAtt.Value();
                            }
                            else
                            {
                                col.Cell[metaAtt.PropertyName] = metaAtt.Value();
                            }
                        }
                    }

                    meta.Columns.Add(col);
                }
            }
            EnsureDefaultValues(meta);
            return meta;
        }

        public static void EnsureDefaultValues(KModel.KMeta meta)
        {
            foreach (var item in meta.Columns)
            {
                SetDefault(item.Header, "displayName", item.Name);
            }

            void SetDefault(Dictionary<string, string> dict, string key, string defaultValue)
            {
                if (!dict.ContainsKey(key))
                {
                    dict[key] = defaultValue;
                }
            }
        }
    }
}