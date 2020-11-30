using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Google.Nearby;
using UIKit;

namespace NearbyMessagesApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        MessageManager manager;
        ISubscription subscription;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            manager = new MessageManager("");
            subscription = manager.Subscription(MessageFound, MessageLost);

            return base.FinishedLaunching(app, options);
        }

        // Method to be called when a monkey published his/her message
        void MessageFound(Message message)
        {
            //var emotionMessage = EmotionMessage.Deserialize(message.Content.ToArray());
            App.Current.MainPage.DisplayAlert(string.Empty, "Got a message", "okay");
        }

        // Method to be called when a monkey unpublished his/her message
        void MessageLost(Message message)
        {
            App.Current.MainPage.DisplayAlert(string.Empty, "Unpublished", "okay");
        }
    }
}
