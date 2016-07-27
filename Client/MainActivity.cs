using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Util;

namespace Client
{
    [Activity(Label = "Client", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, IActivityConnection<ServiceBinder<StorageService>>
    {
        private int _clickCounter;

        private bool _isConfigurationChange;
        private ServiceConnection<MainActivity, ServiceBinder<StorageService>> _storageServiceConnection;

        protected Button BtnStartBoundService;
        protected Button BtnStartService;
        protected Button BtnTestClickMe;
        protected Button BtnOpenNewWindow;
        protected TextView LblTimesClicked;

        public ServiceBinder<StorageService> Binder { get; set; }
        public bool IsBound { get; set; }

        public MainActivity()
        {
            _clickCounter = 0;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            FindViews();
            AssignEventHablders();
            RestoreServiceConnection();
        }

        protected override void OnStart()
        {
            base.OnStart();

            var storageServiceIntent = new Intent("com.devmentor.sitransgruamobile");
            _storageServiceConnection = new ServiceConnection<MainActivity, ServiceBinder<StorageService>>(this);

            // The BindService method should be called from the ApplicationContext rather than from the Activity
            // for handling configuration changes (such as screen rotate).
            // The BindService is an asyncronous call and should always be executed in OnStart because if don't 
            // no-one guarantees that it will be initialized when we will need it for example in a button click.
            ApplicationContext.BindService(storageServiceIntent, _storageServiceConnection, Bind.AutoCreate);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_isConfigurationChange) return;

            if (!IsBound) return;

            IsBound = false;
            UnbindService(_storageServiceConnection);
        }

        /// <summary>
        /// Return the service connection if there is a configuration change.
        /// This method is used to prevent unbinding due to a configuration change, and would return the ServiceConnection instance
        /// </summary>
        /// <returns></returns>
        public override Java.Lang.Object OnRetainNonConfigurationInstance()
        {
            base.OnRetainNonConfigurationInstance();

            _isConfigurationChange = true;

            return _storageServiceConnection;
        }
        
        private void FindViews()
        {
            BtnStartBoundService = FindViewById<Button>(Resource.Id.btnStartBoundService);
            BtnStartService = FindViewById<Button>(Resource.Id.btnStartService);
            BtnTestClickMe = FindViewById<Button>(Resource.Id.btnClickMeTest);
            BtnOpenNewWindow = FindViewById<Button>(Resource.Id.btnOpenNewWnd);
            LblTimesClicked = FindViewById<TextView>(Resource.Id.lblClickMeResult);
        }

        private void AssignEventHablders()
        {
            BtnStartBoundService.Click += BtnStartBoundService_Click;
            BtnStartService.Click += BtnStartService_Click;
            BtnTestClickMe.Click += BtnTestClickMe_Click;
            BtnOpenNewWindow.Click += BtnOpenNewWindow_Click;
        }

        #region Button Click Handlers
        private void BtnStartBoundService_Click(object sender, EventArgs e)
        {
            RunBoundService();
        }

        private void BtnStartService_Click(object sender, EventArgs e)
        {
            StartService(new Intent("com.devmentor.sitransgruamobile"));
        }
        
        private void BtnOpenNewWindow_Click(object sender, EventArgs e)
        {
            var typeOfT = typeof(SecondActivity);
            var intent = new Intent(this, typeOfT);
            StartActivity(intent);
        }

        private void BtnTestClickMe_Click(object sender, EventArgs e)
        {
            LblTimesClicked.Text = $"Times Clicked: {++_clickCounter}";
        }
        #endregion

        /// <summary>
        /// Restore the connection if there was a configuration change, such as a device rotation
        /// </summary>
        private void RestoreServiceConnection()
        {
#pragma warning disable 618
            //TODO: LastNonConfigurationInstance is obsolete. Find a workaround.
            if (LastNonConfigurationInstance != null)
                _storageServiceConnection = LastNonConfigurationInstance as ServiceConnection<MainActivity, ServiceBinder<StorageService>>;
#pragma warning restore 618

            if (_storageServiceConnection != null) Binder = _storageServiceConnection.Binder;
        }

        private void RunBoundService()
        {
            if (!IsBound) return;

            RunOnUiThread(() => {
                Log.Debug("AppDebuggingFlow", "[MainActivity][RoundBoundService][RunOnUiThread] Entered");

                var binder = Binder.GetServiceInstance();
                binder.SetCurrentActivity(this);

                binder.DoWork("This is a message from RunBoundService method!");

                Log.Debug("AppDebuggingFlow", "[MainActivity][RoundBoundService][RunOnUiThread] Finished");
            });
        }
    }
}

