using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DeltaDNA;

#if UNITY_ANDROID
    using Unity.Notifications.Android;
#elif UNITY_IOS
    using Unity.Notifications.iOS;
#endif


public class MobileNotifications : MonoBehaviour
{
    public Text txtStatus;
    int notificationID;

       

    private void Awake()
    {
#if UNITY_ANDROID
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.High,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
#elif UNITY_IOS
        // IOS Seems to work OK without any explicit registration for local notifications
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
#elif UNITY_IOS
        iOSNotificationCenter.OnRemoteNotificationReceived += remoteNotification =>
        {
           Debug.Log("Remote notification received");
        };
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
        System.DateTime date = System.DateTime.Now.AddMinutes(Convert.ToDouble(gameParameters["localNotifTime"]));

        notification.Identifier = notificationID.ToString();
        notification.Title = gameParameters["localNotifTitle"].ToString();
        notification.Body = gameParameters["localNotifDesc"].ToString();
        notification.Trigger = notification.Trigger = new iOSNotificationCalendarTrigger
        {
            Year = date.Year,
            Month = date.Month,
            Day = date.Day,
            Hour = date.Hour,
            Minute = date.Minute,
            Second = date.Second
        };
        notification.CategoryIdentifier = "GOLD_REWARD";

        //Send the notification with unity mobile notifications
        iOSNotificationCenter.ScheduleNotification(notification);
        Debug.Log("Sceduled Notification");               

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

#if UNITY_IOS
        localNotifications.AddParam("localNotifDesc", notification.Body); 
#elif UNITY_ANDROID
         localNotifications.AddParam("localNotifDesc", notification.Text); 
#endif
        localNotifications.AddParam("localNotifTime", Convert.ToInt32(gameParameters["localNotifTime"])); 

        // Record the missionStarted event event with some event parameters. 
        DDNA.Instance.RecordEvent(localNotifications).Run();

    }

    public void CancelScheduledNotification(int notifId)
    {
#if UNITY_ANDROID
        var notificationStatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(notifId);

        if (notificationStatus == NotificationStatus.Scheduled)
        {
            // Cancel the scheduled notification.
            AndroidNotificationCenter.CancelNotification(notifId);

            txtStatus.text = "Android Notification Cancelled " + notifId.ToString(); 
        }
#elif UNITY_IOS
        var scheduledNotifications = iOSNotificationCenter.GetScheduledNotifications();

        foreach (iOSNotification n in scheduledNotifications)
        {
            if (n.Identifier == notifId.ToString())
            {
                iOSNotificationCenter.RemoveScheduledNotification(notifId.ToString());
                txtStatus.text = "IOS Notification Cancelled " + notifId.ToString();
            }
        }        
#endif
    }
}