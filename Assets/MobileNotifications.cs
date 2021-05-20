using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.UI;
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
        };

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

        //SET notification Id
        //notificationID = 10000;
        //AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "channel_id", notificationId);



    }

    public void SendCustomNotificaition()
    {
        var notification = new AndroidNotification();
        notification.IntentData = "{\"title\": \"Notification 1\", \"data\": \"200\"}";
        notification.SmallIcon = "my_custom_icon_id";
        notification.LargeIcon = "my_custom_large_icon_id";
        notification.Title = "Custom Notification Title";
        notification.Text = "Custom Text";
        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }

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
