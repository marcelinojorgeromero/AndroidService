using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

namespace Client
{
    [Service]
    [IntentFilter(new[] { "com.company.app.syncservice" })]
    public class SyncService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        // OnStartCommand is executed only when the StartService(intent) is called!
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("AppDebuggingFlow", $"[SyncService][OnStartCommand] started at { DateTime.Now.ToString("T") }");

            return StartCommandResult.NotSticky;
        }
    }
}