using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Client
{
    [Service]
    public class DemoService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("DemoService", "DemoService started");


            DoWork();


            return  StartCommandResult.RedeliverIntent;
        }

        public void DoWork()
        {
            var t = new Thread(() => {
                Log.Debug("DemoService", "Doing work");
                Thread.Sleep(5000);
                Log.Debug("DemoService", "Work complete");
                StopSelf();
            }
            );
            t.Start();
        }
    }
}