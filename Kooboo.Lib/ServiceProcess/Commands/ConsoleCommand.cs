//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.ServiceProcess
{
    public class ConsoleCommand : ICommand
    {
        public ApplicationStarter Starter { get; set; }

        public bool Execute(ICommandOptions options)
        {
            try
            {
                if (!Environment.UserInteractive)
                    return false;

                Starter.OnStart(options);
                while (true)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ReadKey();
            }
            finally
            {
                Starter.OnStop();
            }
            return true;
        }
    }
}
