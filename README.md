# Mobile local notifications WIP (WORK IN PROGRESS)

## Overview
Using the power of mobile notifications package included with Unity, you can now target users quickly. Combined with the power of DDNA campaigns
You can trigger notifications in a few minutes after a certain action is completed in the game.

You will learn how to: 

- Go beyond limitations of remote push notifications using the power of local notifications.
- Trigger notifications as little as few minutes after an action based on event triggered campaigns

You can watch the following video : TBD

/<div align="center">
      <a href="https://www.youtube.com/watch?v=nVFiKqHmFLw&t=0s">
     <img 
      src="https://i.ytimg.com/vi/nVFiKqHmFLw/hqdefault.jpg?sqp=-oaymwEZCPYBEIoBSFXyq4qpAwsIARUAAIhCGAFwAQ==&amp;rs=AOn4CLApk_nJo0cOmRkVnAQh3mhs2VeQnw" 
      alt="Dynamic Ad Placement" 
      style="width:100%;">
      </a>
    </div>

## Scenarios
This tutorial demonstrates:
1) Basics of local notifications
2) DeltaDNA campaign triggered push notifications on mission completion

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




## Code

The following code snippet shows the Game Parameter callback.
```csharp
TBD
```
