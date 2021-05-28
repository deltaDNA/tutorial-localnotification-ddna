using System;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.UI;
using DeltaDNA;

public class MobileNotifications : MonoBehaviour
{
    public Text txtStatus;
    int notificationID;

    private void Awake()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    // Start is called before the first frame update
    void Start()
    {
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



    }



    /// <summary>
    /// Basics of sending a simple ANDROID notification
    /// </summary>
    public void SendSimpleNotification()
    {
        var notification = new AndroidNotification();
        notification.SmallIcon = "my_custom_icon_id";
        notification.LargeIcon = "my_custom_large_icon_id";
        notification.Title = "Your Title";
        notification.Text = "Your Text";
        notification.FireTime = System.DateTime.Now.AddMinutes(1);

        //Get Notification id for later
        notificationID = AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }


    /// <summary>
    /// Basics of sending custom notification with intent information passed at the callback
    /// </summary>
    public void SendCustomNotificaition()
    {
        var notification = new AndroidNotification();
        notification.IntentData = "{\"title\": \"Notification 1\", \"data\": \"200\"}";
        notification.SmallIcon = "my_custom_icon_id"; //PNG file needs to be placed in /Assets/Plugins/Android/res/drawable
        notification.LargeIcon = "my_custom_large_icon_id"; //PNG file needs to be placed in /Assets/Plugins/Android/res/drawable
        notification.Title = "Custom Notification Title";
        notification.Text = "Custom Text";
        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }

    /// <summary>
    /// Send a notification using DELTADNA Event Triggered campaign
    /// </summary>
    /// <param name="gameParameters"></param>
    public void SendDDNANotification(Dictionary<string, object> gameParameters)
    {
        notificationID = Convert.ToInt32(gameParameters["notificationId"]); //SET notification Id else comment this line to let the packagage generate one
        var notification = new AndroidNotification();
        notification.IntentData = "{\"campaignId\": \"id 1\", \"campaignName\": \"name\",\"notificationId\": \"id 1\",}";
        notification.SmallIcon = "my_custom_icon_id";
        notification.LargeIcon = "my_custom_large_icon_id";
        notification.Title = gameParameters["localNotifTitle"].ToString();
        notification.Text = gameParameters["localNotifDesc"].ToString();
        notification.FireTime = System.DateTime.Now.AddMinutes(Convert.ToDouble(gameParameters["localNotifTime"]));
       

        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "channel_id", notificationID);

        GameEvent localNotifications = new GameEvent("localNotifications");
        localNotifications.AddParam("notificationId", Convert.ToInt32(gameParameters["notificationId"]));
        localNotifications.AddParam("campaignId", Convert.ToInt32(gameParameters["campaignId"]));
        localNotifications.AddParam("campaignName", gameParameters["campaignName"].ToString());
        localNotifications.AddParam("cohortId", Convert.ToInt32(gameParameters["cohortId"]));
        localNotifications.AddParam("cohortName", gameParameters["cohortName"].ToString());
        localNotifications.AddParam("communicationSender", "Unity Mobile Notifications");
        localNotifications.AddParam("communicationState", "SENT");
        localNotifications.AddParam("localNotifTitle", notification.Title);
        localNotifications.AddParam("localNotifDesc", notification.Text);
        localNotifications.AddParam("localNotifTime", Convert.ToInt32(gameParameters["localNotifTime"])); 

        //GameEvent localNotifications = new GameEvent("localNotifications")
        //   .AddParam("notificationId", Convert.ToInt32(gameParameters["notificationId"]))
        //   .AddParam("campaignId", Convert.ToInt32(gameParameters["campaignId"]))
        //   .AddParam("campaignName", gameParameters["campaignName"].ToString())
        //   .AddParam("cohortId", Convert.ToInt32(gameParameters["cohortId"]))
        //   .AddParam("cohortName", gameParameters["cohortName"].ToString())
        //   .AddParam("communicationSender", "Unity Mobile Notifications")
        //   .AddParam("communicationState", "SENT")
        //   .AddParam("localNotifTitle", notification.Title)
        //   .AddParam("localNotifDesc", notification.Text)
        //   .AddParam("localNotifTime", notification.FireTime);

        // Record the missionStarted event event with some event parameters. 
        DDNA.Instance.RecordEvent(localNotifications).Run();



        //Get Notification id for later
        //notificationID = AndroidNotificationCenter.SendNotification(notification, "channel_id");

    }

    public void CancelScheduledNotification(int notifId)
    {
        var notificationStatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(notifId);

        if (notificationStatus == NotificationStatus.Scheduled)
        {
            // Replace the scheduled notification with a new notification.
            AndroidNotificationCenter.CancelNotification(notifId);

            txtStatus.text = "Notification Cancelled "; 
        }
    }

    /// <summary>
    /// Replace an schedule notification
    /// This is great to be used if for example a carrot seed has grown into a carrot. previous queued notificaiton is no longer valid
    /// </summary>
    public void HandleScheduledNotifications()
    {
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
    }



}
