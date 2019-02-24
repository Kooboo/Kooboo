//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Configuration;
using Microsoft.Win32.TaskScheduler;

namespace Kooboo.App
{
    public class KoobooAutoStart
    {
        private readonly static string _appKey = "KoobooApp";
        private readonly static string _isFirstTimeStart = "IsFirstTimeStart";
        private readonly static string _path = System.Windows.Forms.Application.ExecutablePath;

        public static bool IsFirstTimeStart()
        {
            try
            {
                return ConfigurationManager.AppSettings.AllKeys.All(f => f != _isFirstTimeStart)

                //compatible old code
                && !File.Exists(Path.Combine(_path, "_Admin", "View"));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsAutoStart()
        {
            try
            {
                var task = TaskService.Instance.AllTasks.FirstOrDefault(f => f.Name == _appKey);
                return task != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void AutoStart(bool auto)
        {
            try
            {
                if (auto)
                {
                    var taskDefinition = TaskService.Instance.NewTask();
                    taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;
                    taskDefinition.Triggers.AddNew(TaskTriggerType.Logon);
                    var action = taskDefinition.Actions.Add(_path, "", Path.GetDirectoryName(_path));
                    TaskService.Instance.RootFolder.RegisterTaskDefinition(_appKey, taskDefinition);
                }
                else
                {
                    TaskService.Instance.RootFolder.DeleteTask(_appKey, false);
                }

                if (IsFirstTimeStart()) SetNotFirstTimeStart();
            }
            catch (Exception)
            {
            }
        }

        public static bool OldCodeHadSetAutoSart()
        {
            try
            {
                var rk = Registry.LocalMachine;
                using (var subKey = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run"))
                {
                    object value = subKey.GetValue(_appKey, null);
                    if (value != null) subKey.DeleteSubKey(_appKey);
                    return _path.Equals(value);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void SetNotFirstTimeStart()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Add(_isFirstTimeStart, "false");
            config.AppSettings.SectionInformation.ForceSave = true;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}