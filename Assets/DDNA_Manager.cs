using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeltaDNA;
using System;

public class DDNA_Manager : MonoBehaviour
{
    [SerializeField]  private  MobileNotifications oMobileNotif;

    // Get the DDNA SDK running asap, Awake() is called before Start(). 
    void Awake()
    {
        DDNA.Instance.SetLoggingLevel(DeltaDNA.Logger.Level.DEBUG);
        DDNA.Instance.StartSDK();

        DDNA.Instance.Settings.DefaultGameParameterHandler = new GameParametersHandler(gameParameters =>
        {
            // do something with the game parameters received from DDNA In-Game campaigns
            myGameParameterHandler(gameParameters);
        });
   
    }


    void OnApplicationPause(bool pauseStatus)
    {
        // Record a gamePaused or gameResumed event based on the pauseStatus       
        string eventName = pauseStatus == true ? "gamePaused" : "gameResumed";

        // Take Note - The DDNA SDK is started on Awake(), not on Start()
        // as OnApplicationPause() occurs before Start()       
        DDNA.Instance.RecordEvent(eventName).Run();        
    }



     public void MissionCompleted(int level)
    {
        GameEvent missionComplete = new GameEvent("missionCompleted")
            .AddParam("missionName", "Mission " + level.ToString())
            .AddParam("missionID", level.ToString());

        // Record the missionStarted event event with some event parameters. 
        DDNA.Instance.RecordEvent(missionComplete).Run();
    }



    private void myGameParameterHandler(Dictionary<string, object> gameParameters)
    {
        // Generic Game Parameter Handler
        Debug.Log("Received game parameters from Engage campaign: " + DeltaDNA.MiniJSON.Json.Serialize(gameParameters));

        // Catch Game parameters thaty control local notifications 
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
}
