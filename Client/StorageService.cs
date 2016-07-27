using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

namespace Client
{
    [Service]
    [IntentFilter(new[] { "com.devmentor.sitransgruamobile" })]
    public class StorageService : Service
    {
        private ServiceBinder<StorageService> _binder;
        private Type _currentActivityType;

        public Lazy<DataOperations<Table>> DataTableOperations;

        // This is executed only when the service binds
        public override IBinder OnBind(Intent intent)
        {
            _binder = new ServiceBinder<StorageService>(this);

            Initialize();

            return _binder;
        }

        // OnStartCommand is executed only when the StartService(intent) is called!
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("AppDebuggingFlow", $"[StorageService][OnStartCommand] started at {GetNowTime()}");

            return StartCommandResult.RedeliverIntent;
        }

        private void Initialize()
        {
            var random = new Random();

            var table = new Table
            {
                Id = $"abcd-{random.Next(0, 9999):D4}",
                Name = "Name of the table"
            };

            Log.Debug("AppDebuggingFlow", $"[StorageService][OnStartCommand] table created with Id: {table.Id}");

            DataTableOperations = new Lazy<DataOperations<Table>>(() => new DataOperations<Table>(table));
        }

        public void SetCurrentActivity(Activity activity)
        {
            _currentActivityType = activity.GetType();
        }

        public void DoWork(string andPrintMsg)
        {
            var t = new Thread(() => {

                SendNotification();

                Log.Debug("AppDebuggingFlow", $"[StorageService][DoWork] Starting foreground work at {GetNowTime()}");
                Log.Debug("AppDebuggingFlow", $"[StorageService][DoWork] Prints message: {andPrintMsg}");
                Thread.Sleep(10000);

                Log.Debug("AppDebuggingFlow", $"[StorageService][DoWork] Stopping foreground work at {GetNowTime()}");

                /*
                To remove the service from the foreground, simply call StopForeground, passing a Boolean that indicates whether to remove the notification as well.
                StopForeground removes the service from the foreground so that the system will be able to stop it under memory pressure, but it does not stop the service.
                When the service is stopped, if the notification is still present, it will be removed.
                StopForeground is used with StartForeground method.
                Ref: https://developer.xamarin.com/guides/android/application_fundamentals/services/part_1_-_started_services/#Foreground_Services
                 */
                //StopForeground(true);

                /*
                 StopSelf() stops the current service. Applies only to long running services created by StartService.
                 If service is not running StopSelf does nothing.
                 */
                StopSelf(); 
            });

            t.Start();
        }

        private string GetNowTime()
        {
            return DateTime.Now.ToString("T");
        }

        public void SendNotification(Type activityType)
        {
            if (activityType == null) throw new ActivityNotFoundException(nameof(activityType));

            var pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, activityType), 0);

            var builder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetTicker("Message from demo service")
                .SetContentTitle("Demo Service Notification")
                .SetContentText("Message from demo service")
                //.SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            var notification = builder.Build();


            // Generate Random Id for notification
            var r = new Random();
            var notificationId = r.Next(int.MinValue, int.MaxValue);

            var nMgr = (NotificationManager)GetSystemService(NotificationService);
            nMgr.Notify(notificationId, notification);

            // Deprecated method:
            //var nMgr = (NotificationManager)GetSystemService (NotificationService);
            //var notification = new Notification (Resource.Drawable.icon, "Message from demo service");
            //var pendingIntent = PendingIntent.GetActivity (this, 0, new Intent (this, typeof(DemoActivity)), 0);
            //notification.SetLatestEventInfo (this, "Demo Service Notification", "Message from demo service", pendingIntent);
            //nMgr.Notify (0, notification);
        }

        public void SendNotification()
        {
            if (_currentActivityType == null) throw new ActivityNotFoundException("Specify an activity by calling SetCurrentActivity()");
            SendNotification(_currentActivityType);
        }
    }
}