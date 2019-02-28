//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Kooboo.Sites.Scripting.Helper;
using System.Xml.Serialization;
using System.Xml;
using Kooboo.Sites.Scripting.Helper;

namespace Kooboo.Sites.Scripting
{
    public class ViewHelpReader
    {
        private static Dictionary<string, ViewSetting> _models;
        public static Dictionary<string, ViewSetting> Models
        {
            get
            {
                if (_models == null)
                {
                    _models = Read();
                }
                return _models;
            }
        }
        private static Dictionary<string, ViewSetting> Read()
        {
            var path = GetPath();
            Dictionary<string, ViewSetting> modelDic = new Dictionary<string, ViewSetting>();
            if (!Directory.Exists(path)) return modelDic;

            var files = Directory.GetFiles(path);
            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var reader = new StreamReader(file);
                try
                {
                    var serializer = new XmlSerializer(typeof(ViewSetting));

                    var model = serializer.Deserialize(reader) as ViewSetting;

                    if (!modelDic.ContainsKey(model.Name))
                    {
                        model.Description = model.DisplayName;
                        modelDic.Add(model.Name, model);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return modelDic;
        }

        public static Tree GetTree()
        {
            var models = Models;
            var tree = new Tree();
            tree.Nodes = new List<Node>();
            var rootName = DocumentHelper.RootName;
            if (models.ContainsKey(rootName))
            {
                var node = new Node()
                {
                    ShowName=rootName,
                    setting = models[rootName],
                    Url=DocumentHelper.GetViewUrl(rootName)
                };
                List<Node> child = new List<Node>();
                foreach (var keypair in models)
                {
                    if (keypair.Key != rootName)
                    {
                        child.Add(new Node()
                        {
                            ShowName = keypair.Value.Name,
                            setting = keypair.Value,
                            
                            Url = DocumentHelper.GetViewUrl(keypair.Value.Name)
                        });
                    }
                }
                node.Nodes = child;
                tree.Nodes.Add(node);
            }
            return tree;
        }

        private static string GetPath()
        {
            var path = Kooboo.Lib.Compatible.CompatibleManager.Instance.System.CombinePath(Kooboo.Data.AppSettings.RootPath, @"_Admin\help\kView");

            return path;
        }
        
    }
}
