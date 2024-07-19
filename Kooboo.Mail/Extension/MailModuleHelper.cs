using System.IO;
using System.IO.Compression;
using System.Text.Json;
using Kooboo.Data.Context;

namespace Kooboo.Mail.Extension
{
    public static class MailModuleHelper
    {

        public static string ReadReadme(byte[] moduleZip)
        {
            return ReadFileFromZip(moduleZip, "Readme.md");
        }

        // used by template. 
        public static string ReadConfig(byte[] moduleZip)
        {
            return ReadFileFromZip(moduleZip, "module.config");
        }

        private static string ReadFileFromZip(byte[] ZipFile, string name)
        {
            if (name == null)
            { return null; }

            name = name.ToLower();
            using (MemoryStream memory = new MemoryStream(ZipFile))
            {
                ZipArchive zip = new ZipArchive(memory);
                //var name = Settings.ModuleConfigFileName;
                foreach (var entry in zip.Entries)
                {
                    if (!string.IsNullOrEmpty(entry.Name) && entry.Name.ToLower() == name)
                    {
                        MemoryStream file = new MemoryStream();
                        entry.Open().CopyTo(file);
                        var bytes = file.ToArray();
                        return System.Text.Encoding.UTF8.GetString(bytes);
                    }
                }
                return null;
            }
        }

        public static ConfigValue PraseConfig(string ModuleConfig)
        {
            var json = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(ModuleConfig);
            ConfigValue value = new ConfigValue();

            if (json.TryGetProperty("name", out JsonElement nameel))
            {
                value.Name = nameel.GetString();
            }
            else if (json.TryGetProperty("Name", out JsonElement Nameel))
            {
                value.Name = Nameel.GetString();
            }


            if (json.TryGetProperty("version", out JsonElement versionel))
            {
                value.Version = versionel.GetString();
            }
            else if (json.TryGetProperty("Version", out JsonElement Versionel))
            {
                value.Version = Versionel.GetString();
            }

            if (json.TryGetProperty("description", out JsonElement desc))
            {
                value.Description = desc.GetString();
            }
            else if (json.TryGetProperty("Description", out JsonElement Desc))
            {
                value.Description = Desc.GetString();
            }

            return value;
        }


        public static string ModuleRootFolder(RenderContext context)
        {
            var orgId = context.User.CurrentOrgId;
            return Kooboo.Data.AppSettings.GetDbName(orgId, "__mailmodule");
        }

        public static void UpdateTaskJs(MailModuleContext context, string newBody)
        {
            var module = context.Module;
            module.TaskJs = newBody;
            context.OrgDb.Module.AddOrUpdate(module);
        }


    }


}
