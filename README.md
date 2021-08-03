# Mobile local notifications WIP (WORK IN PROGRESS)

## Overview
Using the power of mobile notifications package included with Unity, you can now target users quickly. Combined with the power of DDNA campaigns
You can trigger notifications in a few minutes after a certain action is completed in the game.

## Requirements:
- Unity 2020.3.12f1 
- Unity Mobile Notifications
https://docs.unity3d.com/Packages/com.unity.mobile.notifications@1.4/manual/index.html
- Unity DeltaDNA SDK
https://github.com/deltaDNA/unity-sdk/releases


You will learn how to: 

- Go beyond limitations of remote push notifications using the power of local notifications.
- Trigger notifications as little as few minutes after an action based on event triggered campaigns

You can watch the following video : TODO

<div align="center">
      <a href="https://www.youtube.com/watch?v=cpcmpwYe1Jk">
     <img 
      src="Images/game.png" 
      alt="DDNA Unity Mobile Notifications" 
      style="width:25%;">
      </a>
</div>

    
## Scenarios
This tutorial demonstrates:
1) Basics of Unity local notifications local notifications
2) DeltaDNA campaign triggered push notifications on mission completion 
3) Cancel a notification for players who completeded the next level




## Event Managment
A new event called **localNotification** should be created with the following game parameters. This event will be used to record opening the local push notification

![Local notification event](Images/localnotificationevent.png)

## Game Parameters

Game Parameters are used in this tutorial to Send information to local notifcations and record which campaign was sent

**localNotifTitle** -The title used for the local notification

**localNotifDesc** - Description used for the local notification

**localNotifTime** - The time triggered for notification

Additional parameters can be added to record data such as notificationID if managed by ddna campaigns. Campaign IDs...


## In Game Action
In game action can be created as follows, time is in minutes in this example
The main parameters are mentioned above

![Local notification game parameters](Images/localnotificationaction.png)

## Event Triggered Campaigns
As their name suggests, Event Triggered Campaigns use the events that your player is generating whilst playing to trigger a campaign that performs an action in the game immediately. 

Game Parameter Actions are used to send data to the game.

Game Parameters will be used to instruct the game to show an local notification

Campaign setup for Mission completed

![Local notification campaigns](Images/camp1.png)

Target Segment/Audience in this scenario we are targeting all players

![Local notification campaigns](Images/camp2.png)

Trigger where in the game this campaign will activate

![Local notification campaigns](Images/camp3.png)

We will send the game parameters for the notifcations

![Local notification campaigns](Images/camp4.png)

We also can create a CANCEL notification this would be used to cancel a scheduled notification.

Action with parameters for cancel specify notificaiton ID and action

![Local notification campaigns](Images/cancelnotif.png)

Campaign setup

![Local notification campaigns](Images/cancelnotif1.png)

Trigger action

![Local notification campaigns](Images/cancelnotif2.png)

## Code
These are the main snippets of code for a full comprehensive on mobile notifcations feel free to check out the code in github


Register the game parameter handler
```csharp
 void Start()
    {
        DDNA.Instance.SetLoggingLevel(DeltaDNA.Logger.Level.DEBUG);
        DDNA.Instance.StartSDK();
        DDNA.Instance.Settings.DefaultGameParameterHandler = new GameParametersHandler(gameParameters =>
        {
            // do something with the game parameters
            myGameParameterHandler(gameParameters);
        });
   
    }
```
Game handler to handle the campaign send
```csharp
 private void myGameParameterHandler(Dictionary<string, object> gameParameters)
    {
        // Generic Game Parameter Handler
        Debug.Log("Received game parameters from Engage campaign: " + DeltaDNA.MiniJSON.Json.Serialize(gameParameters));

        //If we have a localnotificaiton game parameter being sent
        if (gameParameters.ContainsKey("localNotifTitle"))
        {
            if(gameParameters["localNotifTitle"].ToString().Contains("CANCEL"))
            {
                oMobileNotif.CancelScheduledNotification(Convert.ToInt32(gameParameters["notificationId"]));
            }
            else
            {
                oMobileNotif.SendDDNANotification(gameParameters);
            }
        }
    }
```

The following code snippet is how we actually send the local notification
```csharp
public void SendDDNANotification(Dictionary<string, object> gameParameters)
    {
        //Prepare the unity mobile notificaitons
        notificationID = Convert.ToInt32(gameParameters["notificationId"]); //SET notification Id else comment this line to let the packagage generate one
        var notification = new AndroidNotification();
        notification.IntentData = "{\"campaignId\": \"id 1\", \"campaignName\": \"name\",\"notificationId\": \"id 1\",}";
        notification.SmallIcon = "my_custom_icon_id";
        notification.LargeIcon = "my_custom_large_icon_id";
        notification.Title = gameParameters["localNotifTitle"].ToString();
        notification.Text = gameParameters["localNotifDesc"].ToString();
        notification.FireTime = System.DateTime.Now.AddMinutes(Convert.ToDouble(gameParameters["localNotifTime"])); // Time in minutes
       
        //Send the notification with unity mobile notifications
        AndroidNotificationCenter.SendNotificationWithExplicitID(notification, "channel_id", notificationID);


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
        localNotifications.AddParam("localNotifDesc", notification.Text);
        localNotifications.AddParam("localNotifTime", Convert.ToInt32(gameParameters["localNotifTime"])); 

        // Record the missionStarted event event with some event parameters. 
        DDNA.Instance.RecordEvent(localNotifications).Run();

        //Get Notification id for later (Can be ignored we pass the notificaitonid with DDNA campaign game parameters)
        //notificationID = AndroidNotificationCenter.SendNotification(notification, "channel_id");

    }
```

