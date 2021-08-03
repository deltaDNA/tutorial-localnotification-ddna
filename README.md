# Mobile local notifications WIP (WORK IN PROGRESS)

## Overview
Using the power of mobile notifications package included with Unity, you can now target users quickly. Combined with the power of DDNA campaigns
You can trigger notifications in a few minutes after a certain action is completed in the game.

## Packages used:
- Unity Mobile Notifications 1.4.2
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
      src="https://i.ytimg.com/vi/nVFiKqHmFLw/hqdefault.jpg?sqp=-oaymwEZCPYBEIoBSFXyq4qpAwsIARUAAIhCGAFwAQ==&amp;rs=AOn4CLApk_nJo0cOmRkVnAQh3mhs2VeQnw" 
      alt="DDNA Unity Mobile Notifications" 
      style="width:75%;">
      </a>
</div>

    
## Scenarios
This tutorial demonstrates:
1) Basics of local notifications
2) DeltaDNA campaign triggered push notifications on mission completion being able to cancel a notification for players who complete 

## Event Triggered Campaigns
As their name suggests, Event Triggered Campaigns use the events that your player is generating whilst playing to trigger a campaign that performs an action in the game immediately. 

Game Parameter Actions are used to send data to the game.

Game Parameters will be used to instruct the game to show an local notification

## Event Managment
A new event called **localNotification** should be created with the following game parameters. This event will be used to record opening the local push notification

![Local notification event schema](Images\localnotificationevent.png)

## Game Parameters

Game Parameters are used in this tutorial to Send information to local notifcations and record which campaign was sent

**localNotifTitle** -The title used for the local notification

**localNotifDesc** - Description used for the local notification

**localNotifTime** - The time triggered for notification

Additional parameters can be added to record data such as notificationID if managed by ddna campaigns. Campaign IDs...


## In Game Action
In game action can be created as follows, time is in minutes in this example
The main parameters are mentioned above
![Local notification event schema](Images\localnotificationaction.png)

## Campaign 
Campaign setup for Mission completed
![Local notification event schema](Images\camp1.png)
Target Segment/Audience in this scenario we are targeting all players
![Local notification event schema](Images\camp2.png)
Trigger where in the game this campaign will activate
![Local notification event schema](Images\camp3.png)
We will send the game parameters for the notifcations
![Local notification event schema](Images\camp4.png)


## Code

The following code snippet shows the Game Parameter callback.
```csharp
TODO
```
