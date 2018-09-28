using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Globalization;

namespace Kooboo.ServiceProcess
{
    public class IntegratedInstaller : Installer
    {
        public IntegratedInstaller(string[] commandLine)
        {
            UseNewContext = true;
            CommandLine = commandLine;
        }

        public string[] CommandLine { get; set; }

        public bool UseNewContext { get; set; }

        public string Path
        {
            get
            {
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName);
            }
        }

        public override void Install(IDictionary savedState)
        {
            this.PrintStartText(Res.GetString("InstallActivityInstalling"));

            Hashtable hashtable = new Hashtable();
            savedState = hashtable;
            try
            {
                base.Install(savedState);
            }
            finally
            {
                FileStream output = new FileStream(this.GetInstallStatePath(this.Path), FileMode.Create);
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    CheckCharacters = false,
                    CloseOutput = false
                };
                XmlWriter writer = XmlWriter.Create(output, settings);
                try
                {
                    new NetDataContractSerializer().WriteObject(writer, savedState);
                }
                finally
                {
                    writer.Close();
                    output.Close();
                }
            }
        }

        public override void Commit(IDictionary savedState)
        {
            this.PrintStartText(Res.GetString("InstallActivityCommitting"));
            string installStatePath = this.GetInstallStatePath(this.Path);
            FileStream input = new FileStream(installStatePath, FileMode.Open, FileAccess.Read);
            XmlReaderSettings settings = new XmlReaderSettings
            {
                CheckCharacters = false,
                CloseInput = false
            };
            XmlReader reader = null;
            if (input != null)
            {
                reader = XmlReader.Create(input, settings);
            }
            try
            {
                if (reader != null)
                {
                    NetDataContractSerializer serializer = new NetDataContractSerializer();
                    savedState = (Hashtable)serializer.ReadObject(reader);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (input != null)
                {
                    input.Close();
                }
                if (base.Installers.Count == 0)
                {
                    base.Context.LogMessage(Res.GetString("RemovingInstallState"));
                    File.Delete(installStatePath);
                }
            }
            base.Commit(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {
            this.PrintStartText(Res.GetString("InstallActivityUninstalling"));
            string installStatePath = this.GetInstallStatePath(this.Path);
            if ((installStatePath != null) && File.Exists(installStatePath))
            {
                FileStream input = new FileStream(installStatePath, FileMode.Open, FileAccess.Read);
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    CheckCharacters = false,
                    CloseInput = false
                };
                XmlReader reader = null;
                if (input != null)
                {
                    reader = XmlReader.Create(input, settings);
                }
                try
                {
                    if (reader != null)
                    {
                        NetDataContractSerializer serializer = new NetDataContractSerializer();
                        savedState = (Hashtable)serializer.ReadObject(reader);
                    }
                }
                catch
                {
                    base.Context.LogMessage(Res.GetString("InstallSavedStateFileCorruptedWarning", new object[] { this.Path, installStatePath }));
                    savedState = null;
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    if (input != null)
                    {
                        input.Close();
                    }
                }
            }
            else
            {
                savedState = null;
            }
            base.Uninstall(savedState);
            if ((installStatePath != null) && (installStatePath.Length != 0))
            {
                try
                {
                    File.Delete(installStatePath);
                }
                catch
                {
                    throw new InvalidOperationException(Res.GetString("InstallUnableDeleteFile", new object[] { installStatePath }));
                }
            }
        }

        public override void Rollback(IDictionary savedState)
        {
            this.PrintStartText(Res.GetString("InstallActivityRollingBack"));
            string installStatePath = this.GetInstallStatePath(this.Path);
            FileStream input = new FileStream(installStatePath, FileMode.Open, FileAccess.Read);
            XmlReaderSettings settings = new XmlReaderSettings
            {
                CheckCharacters = false,
                CloseInput = false
            };
            XmlReader reader = null;
            if (input != null)
            {
                reader = XmlReader.Create(input, settings);
            }
            try
            {
                if (reader != null)
                {
                    NetDataContractSerializer serializer = new NetDataContractSerializer();
                    savedState = (Hashtable)serializer.ReadObject(reader);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (input != null)
                {
                    input.Close();
                }
            }
            try
            {
                base.Rollback(savedState);
            }
            finally
            {
                File.Delete(installStatePath);
            }
        }

        private string GetInstallStatePath(string path)
        {
            string str2 = base.Context.Parameters["InstallStateDir"];
            path = System.IO.Path.ChangeExtension(Path, ".InstallState");
            if (!string.IsNullOrEmpty(str2))
            {
                return System.IO.Path.Combine(str2, System.IO.Path.GetFileName(path));
            }
            return path;
        }

        private void PrintStartText(string activity)
        {
            if (this.UseNewContext)
            {
                InstallContext context = this.CreateInstallContext();
                if (base.Context != null)
                {
                    base.Context.LogMessage(Res.GetString("InstallLogContent", new object[] { this.Path }));
                    base.Context.LogMessage(Res.GetString("InstallFileLocation", new object[] { context.Parameters["logfile"] }));
                }
                base.Context = context;
            }
            base.Context.LogMessage(string.Format(CultureInfo.InvariantCulture, activity, new object[] { this.Path }));
            base.Context.LogMessage(Res.GetString("InstallLogParameters"));
            if (base.Context.Parameters.Count == 0)
            {
                base.Context.LogMessage("   " + Res.GetString("InstallLogNone"));
            }
            IDictionaryEnumerator enumerator = (IDictionaryEnumerator) base.Context.Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string key = (string) enumerator.Key;
                string str2 = (string) enumerator.Value;
                if (key.Equals("password", StringComparison.InvariantCultureIgnoreCase))
                {
                    str2 = "********";
                }
                base.Context.LogMessage("   " + key + " = " + str2);
            }
        }

        private InstallContext CreateInstallContext()
        {
            InstallContext context = new InstallContext(System.IO.Path.ChangeExtension(this.Path, ".InstallLog"), this.CommandLine);
            if (base.Context != null)
            {
                context.Parameters["logtoconsole"] = base.Context.Parameters["logtoconsole"];
            }
            context.Parameters["assemblypath"] = this.Path;
            return context;
        }
    }
}
