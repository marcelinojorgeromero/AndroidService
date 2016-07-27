using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;

namespace Client
{
    [Activity(Label = "SecondActivity")]
    public class SecondActivity : Activity, IActivityConnection<ServiceBinder<StorageService>>
    {
        private int _clickCounter;
        private bool _isConfigurationChange;

        private ServiceConnection<SecondActivity, ServiceBinder<StorageService>> _storageServiceConnection;

        protected Button BtnStartNewLongBoundService;
        protected Button BtnClickMeResponse;
        protected TextView LblTimesClicked;

        public ServiceBinder<StorageService> Binder { get; set; }
        public bool IsBound { get; set; }

        public SecondActivity()
        {
            _clickCounter = 0;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Second);

            FindViews();
            AssignEventHablders();
            RestoreServiceConnection();
        }

        protected override void OnStart()
        {
            base.OnStart();

            var storageServiceIntent = new Intent("com.devmentor.sitransgruamobile");
            
            Log.Debug("AppDebuggingFlow", "[SecondActivity][OnStart] About to initialize constructor of ServiceConnection()");
            _storageServiceConnection = new ServiceConnection<SecondActivity, ServiceBinder<StorageService>>(this);
            Log.Debug("AppDebuggingFlow", "[SecondActivity][OnStart] Constructor of ServiceConnection() finished initializing");

            Log.Debug("AppDebuggingFlow", "[SecondActivity][OnStart] About to execute BindService()");
            // The BindService method should be called from the ApplicationContext rather than from the Activity
            // for handling configuration changes (such as screen rotate).
            // The BindService is an asyncronous call and should always be executed in OnStart because if don't 
            // no-one guarantees that it will be initialized when we will need it for example in a button click.
            ApplicationContext.BindService(storageServiceIntent, _storageServiceConnection, Bind.AutoCreate);
            Log.Debug("AppDebuggingFlow", "[SecondActivity][OnStart] BindService() Finished");
        }

        /// <summary>
        /// Restore the connection if there was a configuration change, such as a device rotation
        /// </summary>
        private void RestoreServiceConnection()
        {
#pragma warning disable 618
            if (LastNonConfigurationInstance != null)
                _storageServiceConnection = LastNonConfigurationInstance as ServiceConnection<SecondActivity, ServiceBinder<StorageService>>;
#pragma warning restore 618

            if (_storageServiceConnection != null) Binder = _storageServiceConnection.Binder;
        }

        /// <summary>
        /// Cuando se cierra la pantalla se libera la conección al servicio
        /// </summary>
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
        [Obsolete("deprecated")]
        public override Java.Lang.Object OnRetainNonConfigurationInstance()
        {
            base.OnRetainNonConfigurationInstance();

            _isConfigurationChange = true;

            return _storageServiceConnection;
        }

        private void FindViews()
        {
            BtnStartNewLongBoundService = FindViewById<Button>(Resource.Id.btnNewServiceBoundLongRun);
            BtnClickMeResponse = FindViewById<Button>(Resource.Id.btnClickMeResponseSecondAct);
            LblTimesClicked = FindViewById<TextView>(Resource.Id.lblClickMeResultSecondAct);
        }

        private void AssignEventHablders()
        {
            BtnStartNewLongBoundService.Click += BtnStartNewLongBoundService_Click;
            BtnClickMeResponse.Click += BtnClickMeResponse_Click;
        }

        private void BtnClickMeResponse_Click(object sender, EventArgs e)
        {
            LblTimesClicked.Text = $"Times Clicked: {++_clickCounter}";
        }

        private async void BtnStartNewLongBoundService_Click(object sender, EventArgs e)
        {
            Log.Debug("AppDebuggingFlow", "[SecondActivity][BtnStartNewLongBoundService] About to execute StartService(intent)");
            StartService(new Intent("com.devmentor.sitransgruamobile"));
            Log.Debug("AppDebuggingFlow", "[SecondActivity][BtnStartNewLongBoundService] StartService(intent) executed");

            if (!IsBound)
            {
                Log.Debug("AppDebuggingFlow", "[SecondActivity][BtnStartNewLongBoundService] Service is not bound. Exiting button click handler.");
                return;
            }

            RunBoundService();

            // Si habilito este código que hace UnbindService arroja error
            //Log.Debug("BtnStartNewLongBoundService", "Service unbounding...");
            //UnbindService(_storageServiceConnection);
            //Log.Debug("AppDebuggingFlow", "[SecondActivity][BtnStartNewLongBoundService] Service unbounded. ");
            // -----------------------------------------------------------

            var binder = Binder.GetServiceInstance();
            binder.SetCurrentActivity(this);

            Log.Debug("AppDebuggingFlow", "[SecondActivity][BtnStartNewLongBoundService] DoSomethingLonger() is about to run... ");
            await binder.DataTableOperations.Value.DoSomethingLonger("Before message!", "After message!!");
            Log.Debug("AppDebuggingFlow", "[SecondActivity][BtnStartNewLongBoundService] DoSomethingLonger() finished.");
        }

        private void RunBoundService()
        {
            if (!IsBound) return;

            RunOnUiThread(async () => {
                Log.Debug("AppDebuggingFlow", "[SecondActivity][RunBoundService][RunOnUiThread] Entered");

                var binder = Binder.GetServiceInstance();
                binder.SetCurrentActivity(this);

                await binder.DataTableOperations.Value.DoSomethingLong();

                Log.Debug("AppDebuggingFlow", "[SecondActivity][RunBoundService][RunOnUiThread] Service method DoSomethingLong() executed");
            });
        }
    }
}