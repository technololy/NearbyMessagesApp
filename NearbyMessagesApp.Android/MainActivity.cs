using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Common.Apis;
using Android.Gms.Nearby;
using Android.Gms.Nearby.Messages;
using NearbyMessage = Android.Gms.Nearby.Messages.Message;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Text;
using Android.Net;

[assembly: MetaData("com.google.android.nearby.messages.API_KEY", Value = "AIzaSyDwSWO33q1sQ18kUSNtEaBCAg7ybv4vgEs")]

namespace NearbyMessagesApp.Droid
{
    [Activity(Label = "NearbyMessagesApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity :
global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity,
GoogleApiClient.IOnConnectionFailedListener,
        GoogleApiClient.IConnectionCallbacks
    {
        GoogleApiClient googleApiClient;
        NearbyMessageListener emotionsMsgListener;
        NearbyMessage publishedMessage;
        // Android.Support.V4.App.FragmentActivity Fragment;
        //global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
        TaskCompletionSource<bool> tcsConnected = new TaskCompletionSource<bool>();

        public MainActivity()
        {
            MessagingCenter.Subscribe<MainPage>(this, "Publish", (sender) =>
            {
                Publish();
            });

            MessagingCenter.Subscribe<MainPage>(this, "Subscribe", (sender) =>
            {
                Subscribe();
            });
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            // TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // Setup our GoogleApiClient connection
            googleApiClient = new GoogleApiClient.Builder(this)
                .AddApi(NearbyClass.MessagesApi)
                .AddConnectionCallbacks(this)
                //.EnableAutoManage(frg, this)
                .AddOnConnectionFailedListener(this)
                .Build();

            //googleApiClient.Connect();

            emotionsMsgListener = new NearbyMessageListener
            {
                OnFoundHandler = msg =>
                {
                    App.Current.MainPage.DisplayAlert(string.Empty, msg?.message, "Okay");
                },
                OnLostHandler = msg =>
                {
                    App.Current.MainPage.DisplayAlert(string.Empty, msg?.message, "Okay");

                }
            };

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        async Task RequestNearbyPermissionsAsync()
        {
            // Wait until Google Play Services is connected
            if (googleApiClient.IsConnected)
            {

                // Request permissions
                var permStatus = await NearbyClass.Messages.GetPermissionStatusAsync(googleApiClient);

                // If our request failed, we'll need to see if there's a way to resolve
                if (!permStatus.IsSuccess)
                {
                    LogMessage("Nearby permission request failed...");

                    // If we have a resolution for requesting permissions, start it
                    if (permStatus.HasResolution)
                    {
                        LogMessage("Has resolution for Nearby permission request failure...");
                        permStatus.StartResolutionForResult(this, 1001);
                    }
                    else
                    {
                        // No resolution, just abandon the app
                        Toast.MakeText(this, "Nearby Messaging Disabled, exiting App", ToastLength.Long).Show();
                        Finish();
                    }
                }
                else
                {
                    // Permission request succeeded, continue with subscribing and publishing messages
                    await Subscribe();
                    await Publish();
                }
            }
        }

        public void OnConnectionFailed(Android.Gms.Common.ConnectionResult result)
        {
            // Connection failed to Google Play services, set our result false
            //await RequestNearbyPermissionsAsync();

            tcsConnected.TrySetResult(false);

            // There's no way to recover at this point, so let the user know, and exit
            Toast.MakeText(this, "Failed to connect to Google Play Services", ToastLength.Long).Show();
            Finish();
        }

        public void OnConnected(Bundle connectionHint)
        {
            // Google Play services connected, set our completion source to true!
            tcsConnected.TrySetResult(true);
        }

        public void OnConnectionSuspended(int cause)
        {
            // Not doing anything currently when connection is suspended
        }

        Task<bool> IsConnected()
        {


            //return Task.FromResult(true);
            timer.Start();
            return tcsConnected.Task;
        }
        System.Timers.Timer timer = new System.Timers.Timer();



        bool IsConnectedToNetwork
        {
            get
            {
                var connManager = (ConnectivityManager)GetSystemService(ConnectivityService);
                NetworkInfo info = connManager.GetNetworkInfo(ConnectivityType.Wifi);

                return (info != null && info.IsConnectedOrConnecting);
            }
        }

        async Task Subscribe()
        {
            // Wait for a connection to GPS
            if (!await IsConnected())
                return;

            var status = await NearbyClass.Messages.SubscribeAsync(googleApiClient, emotionsMsgListener);
            if (!status.IsSuccess)
                await App.Current.MainPage.DisplayAlert(string.Empty, status.StatusMessage, "Okay");

        }

        // Unsubscribes from the Messages API
        async Task Unsubscribe()
        {
            // Wait for a connection to GPS (although this should always be the case if it's called)
            if (!await IsConnected())
                return;

            var status = await NearbyClass.Messages.UnsubscribeAsync(googleApiClient, emotionsMsgListener);
            if (!status.IsSuccess)
                await App.Current.MainPage.DisplayAlert(string.Empty, status.StatusMessage, "Okay");
        }

        async Task Publish()
        {
            try
            {
                // Wait for connection
                if (!await IsConnected())
                    return;

                //if (!IsConnectedToNetwork)
                //{
                //    await App.Current.MainPage.DisplayAlert(string.Empty, "Not connected to network", "Okay");
                //    return;
                //}

                // Create new Nearby message to publish with the spinner choices
                dynamic Message = new
                {
                    message = "hello"
                };
                // var sevenItems = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
                var array = Encoding.ASCII.GetBytes(new string(' ', 100));


                // Remove any existing messages for this user from our list
                // Add the new message and update the dataset


                // If we already published a message, unpublish it first
                if (publishedMessage != null)
                    await Unpublish();

                // Create a new nearby message with our serialized object
                //publishedMessage = new NearbyMessage(Message?.Serialize());
                publishedMessage = new NearbyMessage(array, "lolade1", "lolade2");
                var getMsg = NearbyClass.GetMessagesClient(this);
                // Publish our new message
                if (!googleApiClient.IsConnected)
                {
                    googleApiClient.Connect();
                }
                var status = await NearbyClass.Messages.PublishAsync(googleApiClient, publishedMessage);
                if (!status.IsSuccess)
                    await App.Current.MainPage.DisplayAlert(string.Empty, status.StatusMessage, "Okay");
                else

                    await App.Current.MainPage.DisplayAlert(string.Empty, status.StatusMessage, "Okay");

            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(string.Empty, ex.ToString(), "Okay");
            }

        }

        async Task Unpublish()
        {
            // Wait for GPS connection
            if (!await IsConnected())
                return;

            // If we actually published a message, unpublish it
            if (publishedMessage != null)
            {
                var status = await NearbyClass.Messages.Unpublish(googleApiClient, publishedMessage);
                if (!status.Status.IsSuccess)
                    await App.Current.MainPage.DisplayAlert(string.Empty, status.Status.StatusMessage, "Okay");
            }
        }

        protected override void OnStop()
        {
            // Unpublish messages and then when it's done, unsubscribe
            Unpublish().ContinueWith(t => Unsubscribe());
            MessagingCenter.Unsubscribe<MainPage>(this, "Publish");
            base.OnStop();
            googleApiClient.Disconnect();

        }
        protected override void OnStart()
        {
            googleApiClient.Connect();
            RequestNearbyPermissionsAsync();
            base.OnStart();
            var array = Encoding.ASCII.GetBytes(new string(' ', 100));

            publishedMessage = new NearbyMessage(array, "lolade1", "lolade2");
            //googleApiClient.Connect();
            NearbyClass.GetMessagesClient(this).Publish(publishedMessage);
            NearbyClass.GetMessagesClient(this).Subscribe(emotionsMsgListener);



        }
        void LogMessage(string format, params object[] args)
        {
            Android.Util.Log.Debug("NEARBY-APP", string.Format(format, args));
        }
    }
}