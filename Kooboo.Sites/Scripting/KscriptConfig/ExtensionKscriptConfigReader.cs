using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Kooboo.Data;
using System.Xml.Linq;

namespace Kooboo.Sites.Scripting.KscriptConfig
{
    public class ExtensionKscriptConfigReader
    {
        private string kscriptSettingFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "kscript.config");

        public ExtensionKscriptConfigReader()
        {

        }

        public ExtensionKscriptConfigReader(string file)
        {
            kscriptSettingFile = file;
        }

        public KConfig ReadConfig()
        {
            var kconfig = new KConfig();
            if (File.Exists(kscriptSettingFile))
            {
                var str = File.ReadAllText(kscriptSettingFile);

                try
                {
                    var config = Lib.Helper.XmlHelper.DeSerialize(str);
                    var rootEl = config.Root;

                    foreach (var node in rootEl.Nodes())
                    {
                        if (node is XElement)
                        {
                            var xnode = node as XElement;
                            if (xnode.Name.LocalName.Equals("extensiondlls", StringComparison.OrdinalIgnoreCase))
                            {
                               var value= xnode.Attribute("value");
                                if (value != null)
                                {
                                    kconfig.ExtensionDlls = value.Value;
                                }
                                
                            }
                            else if(xnode.Name.LocalName.Equals("add", StringComparison.OrdinalIgnoreCase))
                            {
                                var keyAttr = xnode.Attribute("key");
                                var valueAttr = xnode.Attribute("value");
                                if (keyAttr != null && valueAttr != null
                                    && !string.IsNullOrEmpty(keyAttr.Value)
                                    && !string.IsNullOrEmpty(valueAttr.Value))
                                {
                                    kconfig.Kscripts[keyAttr.Value] = valueAttr.Value;
                                }
                            }
                           
                        }
                    }

                }
                catch (Exception ex)
                {

                }
            }
            return kconfig;
        }
    }
}
