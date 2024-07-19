//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Kooboo.Lib
{
    public class AppSettingsUtility
    {
        static Configuration _configuration;

        public static Configuration Configuration => _configuration;

        static AppSettingsUtility()
        {
            var name = Assembly.GetEntryAssembly().GetName().Name;
            var path = Path.Combine(AppContext.BaseDirectory, $"{name}.dll.config");
            _configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
            {
                ExeConfigFilename = path
            }, ConfigurationUserLevel.None);
        }

        public static string Get(string name, string defaultValue = null)
        {
            var val = _configuration.AppSettings.Settings[name]?.Value;

            if (string.IsNullOrEmpty(val))
            {
                val = GetEnvironmentVariable($"AppSettings__{name}");
                if (!string.IsNullOrEmpty(val))
                {
                    return val;
                }
                return defaultValue;
            }

            return val;
        }

        public static bool GetBool(string name, bool defaultValue = false)
        {
            var value = Get(name, null);
            if (value == null) Get(name.ToLower(), null);
            if (bool.TryParse(value, out bool result)) return result;
            return defaultValue;
        }

        public static int GetInt(string name, int defaultValue = 0)
        {
            var value = Get(name, null);
            if (value == null) Get(name.ToLower(), null);
            if (int.TryParse(value, out int result)) return result;
            return defaultValue;
        }

        public static void AddOrSave(string name, string value)
        {
            var el = _configuration.AppSettings.Settings[name];
            if (el == null) el = _configuration.AppSettings.Settings[name.ToLower()];

            if (el == null)
            {
                _configuration.AppSettings.Settings.Add(name, value);
            }
            else
            {
                el.Value = value;
            }

            _configuration.AppSettings.SectionInformation.ForceSave = true;
            _configuration.Save(ConfigurationSaveMode.Modified);
        }

        private static string GetEnvironmentVariable(string name)
        {
            var value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);
        }
    }
}
