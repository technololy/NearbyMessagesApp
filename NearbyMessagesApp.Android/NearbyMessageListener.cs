using System;
using Android.Gms.Nearby.Messages;
using NearbyMessage = Android.Gms.Nearby.Messages.Message;

namespace NearbyMessagesApp.Droid
{
    public class NearbyMessageListener : MessageListener
    {
        public Action<dynamic> OnFoundHandler { get; set; }
        public Action<dynamic> OnLostHandler { get; set; }

        public override void OnFound(NearbyMessage message)
        {
            OnFoundHandler?.Invoke(new { message = "Message received" });
        }

        public override void OnLost(NearbyMessage message)
        {
            base.OnLost(message);
            OnLostHandler?.Invoke(new { message = "Message lost" });
        }
    }
}
