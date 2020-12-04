using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Google.Nearby;
using UIKit;
using Xamarin.Forms;

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
        public static IPublication PublishedMessage { get; set; }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            
            LoadApplication(new App());
            setUp();
            return base.FinishedLaunching(app, options); 
        }


        private void setUp()
        {
            try
            {
                manager = new MessageManager("AIzaSyBEOVENhxhTy1aps3gg-okeFwgNJ7edtGg");
                subscription = manager.Subscription(MessageFound, MessageLost);
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert(string.Empty, "Exception", "okay");
            }

            MessagingCenter.Subscribe<MainPage>(this, "Publish", (sender) =>
            {
                Publish();
            });
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

        void Publish()
        {
            var myMessage = new EmotionMessage
            {
                UserId = "Nero",
                Name = "hi",
                Species = "",
                Emotion = ""
            };
            AppDelegate.PublishedMessage = manager.Publication(Message.Create(NSData.FromArray(myMessage.Serialize())));
        }
    }
}
