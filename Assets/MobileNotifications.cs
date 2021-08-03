using System;
using System.Collections.Generic;

#if UNITY_ANDROID
using Unity.Notifications;
#elif UNITY_IOS
using Unity.Notifications.iOS;
#endif

using UnityEngine;
using UnityEngine.UI;
using DeltaDNA;

public class MobileNotifications : MonoBehaviour
{
    public Text txtStatus;
    int notificationID;

    private void Awake()
    {
#if UNITY_ANDROID
        var channel = new Android.AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
#elif UNITY_IOS
      /*  using (var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true))
        {
            while (!req.IsFinished)
            {
                yield return null;
            };

            string result = "\n RequestAuthorization: \n";
            result += "\n finished: " + req.IsFinished;
            result += "\n granted :  " + req.Granted;
            result += "\n error:  " + req.Error;
            result += "\n deviceToken:  " + req.DeviceToken;
            Debug.Log(result);
        }*/
#endif

    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID
         AndroidNotificationCenter.NotificationReceivedCallback receivedNotificationHandler =
        delegate (AndroidNotificationIntentData data)
        {
            var msg = "Notification received : " + data.Id + "\n";
            msg += "\n Notification received: ";
            msg += "\n .Title: " + data.Notification.Title;
            msg += "\n .Body: " + data.Notification.Text;
            msg += "\n .Channel: " + data.Channel;
            
            Debug.Log(msg);
            
            txtStatus.text = msg;

          //Handle events for reporting
          //  GameEvent localNotification = new GameEvent("localNotification")
          //.AddParam("campaignId", gameParameters["campaignId"].ToString())
          //.AddParam("campaignName", gameParameters["campaignName"].ToString())
          //.AddParam("cohortId", gameParameters["cohortId"].ToString())
          //.AddParam("cohortName", gameParameters["cohortName"].ToString())
          //.AddParam("communicationSender", "Unity Mobile Notifications")
          //.AddParam("communicationState", "OPEN")
          //.AddParam("localNotifTitle", notification.Title)
          //.AddParam("localNotifDesc", notification.Text)
          //.AddParam("localNotifTime", notification.FireTime);

          //  // Record the missionStarted event event with some event parameters. 
          //  DDNA.Instance.RecordEvent(localNotification).Run();


        };

        //Get intentinfo
        AndroidNotificationCenter.OnNotificationReceived += receivedNotificationHandler;
        var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();
        if (notificationIntentData != null)
        {
            var id = notificationIntentData.Id;
            var channel = notificationIntentData.Channel;
            var notification = notificationIntentData.Notification;

            txtStatus.text += "\n INTENT" + id.ToString() + " " + channel.ToString() + " " + notification.IntentData;
        }
#endif
    }



    /// <summary>
    /// Basics of sending a simple ANDROID notification
    /// </summary>
    public void SendSimpleNotification()
    {
        #if UNITY_ANDROID
        var notification = new AndroidNotification();
        notification.SmallIcon = "my_custom_icon_id";
        notification.LargeIcon = "my_custom_large_icon_id";
        notification.Title = "Your Title";
        notification.Text = "Your Text";
        notification.FireTime = System.DateTime.Now.AddMinutes(1);

        //Get Notification id for later
        notificationID = AndroidNotificationCenter.SendNotification(notification, "channel_id");
#endif
    }


    /// <summary>
    /// Basics of sending custom notification with intent information passed at the callback
    /// </summary>
    public void SendCustomNotificaition()
    {
#if UNITY_ANDROID
        var notification = new AndroidNotification();
        notification.IntentData = "{\"title\": \"Notification 1\", \"data\": \"200\"}";
        notification.SmallIcon = "my_custom_icon_id"; //PNG file needs to be placed in /Assets/Plugins/Android/res/drawable
        notification.LargeIcon = "my_custom_large_icon_id"; //PNG file needs to be placed in /Assets/Plugins/Android/res/drawable
        notification.Title = "Custom Notification Title";
        notification.Text = "Custom Text";
        AndroidNotificationCenter.SendNotification(notification, "channel_id");
#endif
    }

    /// <summary>
    /// Send a notification using DELTADNA Event Triggered campaign
    /// </summary>
    /// <param name="gameParameters"></param>
    public void SendDDNANotification(Dictionary<string, object> gameParameters)
    {

        //Prepare the unity mobile notificaitons
        notificationID = Convert.ToInt32(gameParameters["notificationId"]); //SET notification Id else comment this line to let the packagage generate one
#if UNITY_ANDROID
        var notification = new AndroidNotification();
        notification.IntentData = "{\"campaignId\": \"id 1\", \"campaignName\": \"name\",\"notificationId\": \"id 1\",}";
        notification.SmallIcon = "my_custom_icon_id";
        notification.LargeIcon = "my_custom_large_icon_id";
        notification.Title = gameParameters["localNotifTitle"].ToString();
        notification.Text = gameParameters["localNotifDesc"].ToString();
        notification.FireTime = System.DateTime.Now.AddMinutes(Convert.ToDouble(gameParameters["localNotifTime"]));
       
        //Send the notification with unity mobile notifications
        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "channel_id", notificationID);
#elif UNITY_IOS
        var notification = new iOSNotification();
#endif

        //Record the event for reporting services
        GameEvent localNotifications = new GameEvent("localNotifications");
        localNotifications.AddParam("notificationId", Convert.ToInt32(gameParameters["notificationId"]));
        localNotifications.AddParam("campaignId", Convert.ToInt32(gameParameters["campaignId"]));
        localNotifications.AddParam("campaignName", gameParameters["campaignName"].ToString());
        localNotifications.AddParam("cohortId", Convert.ToInt32(gameParameters["cohortId"]));
        localNotifications.AddParam("cohortName", gameParameters["cohortName"].ToString());
        localNotifications.AddParam("communicationSender", "Unity Mobile Notifications");
        localNotifications.AddParam("communicationState", "SENT");
        localNotifications.AddParam("localNotifTitle", notification.Title);
        localNotifications.AddParam("localNotifDesc", notification.Body); // TODO should be Text field on Android !?!
        localNotifications.AddParam("localNotifTime", Convert.ToInt32(gameParameters["localNotifTime"])); 

        // Record the missionStarted event event with some event parameters. 
        DDNA.Instance.RecordEvent(localNotifications).Run();

        //Get Notification id for later (Can be ignored we pass the notificaitonid with DDNA campaign game parameters)
        //notificationID = AndroidNotificationCenter.SendNotification(notification, "channel_id");

    }

    public void CancelScheduledNotification(int notifId)
    {
#if UNITY_ANDROID
        var notificationStatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(notifId);

        if (notificationStatus == NotificationStatus.Scheduled)
        {
            // Replace the scheduled notification with a new notification.
            AndroidNotificationCenter.CancelNotification(notifId);

            txtStatus.text = "Notification Cancelled "; 
        }
//#elif
#endif
    }

    /// <summary>
    /// Replace an schedule notification
    /// This is great to be used if for example a carrot seed has grown into a carrot. previous queued notificaiton is no longer valid
    /// </summary>
    public void HandleScheduledNotifications()
    {

#if UNITY_ANDROID

        var newNotification = new AndroidNotification();
        newNotification.SmallIcon = "my_custom_icon_id";
        newNotification.LargeIcon = "my_custom_large_icon_id";
        newNotification.Title = "New Notifications Message";
        newNotification.Text = "This is a new notification message not original";
        newNotification.FireTime = System.DateTime.Now.AddMinutes(1);


        var notificationStatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationID);

        if (notificationStatus == NotificationStatus.Scheduled)
        {
            // Replace the scheduled notification with a new notification.
            AndroidNotificationCenter.UpdateScheduledNotification(notificationID, newNotification, "channel_id");
        }
        else if (notificationStatus == NotificationStatus.Delivered)
        {
            // Remove the previously shown notification from the status bar.
            AndroidNotificationCenter.CancelNotification(notificationID);
        }
        else if (notificationStatus == NotificationStatus.Unknown)
        {
            AndroidNotificationCenter.SendNotification(newNotification, "channel_id");
        }
#endif
    }
}
