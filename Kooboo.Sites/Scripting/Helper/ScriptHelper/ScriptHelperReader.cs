//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Scripting.Helper;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Kooboo.Sites.Scripting.Helper.ScriptHelper
{
    public class ScriptHelperReader
    {

        public static Dictionary<string,SettingBase> Settings { get; set; }

        
        public static Tree GetTree()
        {
            Settings = ReadSettings();
         
            var tree = new Tree();
            tree.Nodes = new List<Node>();

            var rootName = DocumentHelper.RootName;
            if (Settings.ContainsKey(rootName))
            {
                var setting = Settings[rootName];
                if (setting != null)
                {
                    var node = new Node()
                    {
                        ShowName= rootName,
                        setting = setting,
                        Url=DocumentHelper.GetTypeUrl(rootName)
                    };
                    node.Nodes = new List<Node>();
                    var kscriptSetting = setting as KScriptSetting;

                    foreach (var prop in kscriptSetting.Props)
                    {
                        var childNode = new Node()
                        {
                            ShowName = prop.Name,
                            setting = prop,
                            Url = DocumentHelper.GetTypeUrl(prop.Type)
                        };
                        if (prop.Childrens.Count > 0)
                        {
                            foreach(var child in prop.Childrens)
                            {
                                childNode.Nodes = new List<Node>();
                                var childSetting = Settings[child.ToLower()];
                                if(childSetting!=null)
                                {
                                    childNode.Nodes.Add(new Node()
                                    {
                                        ShowName = child,
                                        setting = Settings[child.ToLower()],
                                        Url = DocumentHelper.GetTypeUrl(child)
                                    });
                                }
                                
                            }
                            
                        }
                        node.Nodes.Add(childNode);

                    }

                    foreach (var method in kscriptSetting.Methods)
                    {
                        var paramBuilder = new StringBuilder();
                        foreach (var param in method.Params)
                        {
                            paramBuilder.Append(param.Name);
                        }

                        node.Nodes.Add(new Node()
                        {
                            ShowName = method.Name,
                            setting = method,
                            Url=DocumentHelper.GetMethodUrl("k",method.Name, paramBuilder.ToString())
                        });
                    }
                    
                    tree.Nodes.Add(node);
                }
                
            }
            
            return tree;
        }

        private static Dictionary<string,SettingBase> ReadSettings()
        {
            //XmlSerializer serializer = new XmlSerializer(typeof(KScriptSetting));

            XmlSerializer serializer =
XmlSerializer.FromTypes(new[] { typeof(KScriptSetting) })[0];

            var path = GetPath();
            var settings = new Dictionary<string,SettingBase>();
            var files = Directory.GetFiles(path);
            for (var i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var reader = new StreamReader(file);
                try
                {  
                    KScriptSetting setting = serializer.Deserialize(reader) as KScriptSetting;

                    var fileName = Path.GetFileNameWithoutExtension(file).ToLower();
                    settings.Add(fileName, setting);
                }
                catch(Exception ex)
                {

                }
                finally
                {
                    reader.Close();
                }

            }
            return settings;

        }
        public static string GetPath()
        {
            var path = Kooboo.Lib.Compatible.CompatibleManager.Instance.System.CombinePath(Kooboo.Data.AppSettings.RootPath, @"_Admin\help\kScript");

            return path;
        }
    }
}
