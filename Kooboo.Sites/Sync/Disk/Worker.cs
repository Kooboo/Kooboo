using Kooboo.Data.Events.Global;
using System;
using System.Timers;
using Kooboo.Data.Context;
using Kooboo.Data.Models;

namespace Kooboo.Sites.Sync.Disk
{
      
    public class Worker
    {
        public static Worker Instance = new Worker();

        private bool _running;
        private Timer _timer;

        public Worker()
        {
            this._timer = new Timer(2000);
            this._timer.Elapsed += RunJobs;
        }

        public void Start()
        {
            this._timer.Start();
        }

        private void RunJobs(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (_running)
            {
                return;
            }

            _running = true; 
            try
            {
                SiteManager.CheckDisk();
            }
            catch (Exception ex)
            {
                Kooboo.Data.Log.Instance.Exception.WriteException(ex); 
            }
            _running = false;
        }
         

        public void Stop()
        {
            _timer.Stop();
            _timer = null;
        }
    }


    public class StartDiskMirrorTask : Data.Events.IHandler<ApplicationStartUp>
    {
        public void Handle(ApplicationStartUp theEvent, RenderContext context)
        {
            Worker.Instance.Start(); 
        }
 
    }

}
