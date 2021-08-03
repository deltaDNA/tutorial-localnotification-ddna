using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeltaDNA;
using System;

public class DDNA_Manager : MonoBehaviour
{
    [SerializeField]  private  MobileNotifications oMobileNotif;

    // Start is called before the first frame update
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
}
