using Android.App;
using Android.Content;
using Android.OS;

namespace Client
{
    public class ServiceConnection<TActivity, TBinder> : Java.Lang.Object, IServiceConnection 
        where TActivity : Activity, IActivityConnection<TBinder> 
        where TBinder : Binder
    {
        private readonly TActivity _activity;
        public TBinder Binder { get; private set; }

        public ServiceConnection(TActivity activity)
        {
            _activity = activity;
        }

        /// <summary>
        /// Callback called by BindService (serviceIntent, serviceConnection, Bind.AutoCreate);
        /// </summary>
        /// <param name="name"></param>
        /// <param name="service"></param>
        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            var storageServiceBinder = service as TBinder;
            if (storageServiceBinder == null) return;

            _activity.Binder = storageServiceBinder;
            _activity.IsBound = true;

            // keep instance for preservation across configuration changes
            Binder = storageServiceBinder;
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            _activity.IsBound = false;
        }
    }
}