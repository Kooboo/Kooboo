using System;
using System.Collections.Generic;
using Kooboo.Mail.MassMailing.Model;

namespace Kooboo.Mail.MassMailing
{
    public class ISPContainer
    {

        public ISPContainer()
        {
            InitList();
        }

        public static ISPContainer instance { get; set; } = new ISPContainer();


        public List<ISP> List
        {
            get; set;
        }

        public Dictionary<string, ISP> DictList { get; set; } = new Dictionary<string, ISP>();


        private List<ISP> DefaultList()
        {
            List<ISP> isp = new List<ISP>();
            var gmail = new ISP() { Name = "gmail", RootDomains = new HashSet<string>() { "gmail.com", "google.com" }, MxDomains = new HashSet<string>() { "google.com", "googlemail.com" }, StartTls = true, PipeLining = true };

            var tele2 = new ISP() { Name = "tele2", RootDomains = new HashSet<string>() { "zonnet.nl", "tele2.nl", "versatel.nl" }, MxDomains = new HashSet<string>() { "isp-net.nl" } };

            var neteast = new ISP() { Name = "163", RootDomains = new HashSet<string>() { "163.com", "126.com" }, MxDomains = new HashSet<string>() { "163.com" }, StartTls = true, PipeLining = true };

            var outlook = new ISP() { Name = "outlook", RootDomains = new HashSet<string>() { "live.com", "outlook.com", "hotmail.com" }, MxDomains = new HashSet<string>() { "hotmail.com", "outlook.com", "live.com" }, StartTls = true, PipeLining = true };

            isp.Add(gmail);
            isp.Add(tele2);
            isp.Add(neteast);
            isp.Add(outlook); 
            return isp;
        }


        public void InitList()
        {
            foreach (var item in DefaultList())
            {
                if (!DictList.ContainsKey(item.Name))
                {
                    DictList[item.Name] = item;
                }
            }
        }

        public void Add(ISP isp)
        {
            this.DictList[isp.Name] = isp;
            var FileName = ispFileName(isp.Name);
            var json = System.Text.Json.JsonSerializer.Serialize(isp);
            System.IO.File.WriteAllText(FileName, json);
        }

        public ISP Get(string ISPName)
        {
            if (ISPName != default && this.DictList.ContainsKey(ISPName))
            {
                return this.DictList[ISPName];
            }
            var FileName = ispFileName(ISPName);

            if (System.IO.File.Exists(FileName))
            {
                var json = System.IO.File.ReadAllText(FileName);
                return System.Text.Json.JsonSerializer.Deserialize<ISP>(json);
            }
            return new ISP() { Name = ISPName };
        }

        public void SaveDisk(ISP isp)
        {
            var FileName = ispFileName(isp.Name);
            var json = System.Text.Json.JsonSerializer.Serialize(isp);
            System.IO.File.WriteAllText(FileName, json);
        }

        private string ispFileName(string name)
        {
            return System.IO.Path.Combine(DiskFolder, name.ToLower() + ".json");
        }


        private string _diskFolder;
        private string DiskFolder
        {
            get
            {
                if (_diskFolder == null)
                {
                    var root = AppDomain.CurrentDomain.BaseDirectory;
                    root = System.IO.Path.Combine(root, "AppData", "ISP");

                    Kooboo.Lib.Helper.IOHelper.EnsureDirectoryExists(root);

                    _diskFolder = root;
                }
                return _diskFolder;

            }
        }

    }
}
