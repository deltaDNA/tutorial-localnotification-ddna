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

   
    public void OnDDNAClick()
    {

        GameEvent missionStartedEvent = new GameEvent("missionCompleted")
            .AddParam("missionName", "Mission 01")
            .AddParam("missionID", "1");

        // Record the missionStarted event event with some event parameters. 
        DDNA.Instance.RecordEvent(missionStartedEvent).Run();
    }

   


    private void myGameParameterHandler(Dictionary<string, object> gameParameters)
    {
        // Generic Game Parameter Handler
        Debug.Log("Received game parameters from Engage campaign: " + DeltaDNA.MiniJSON.Json.Serialize(gameParameters));

        // Handle ADS commands received from DDNA
        if (gameParameters.ContainsKey("localNotifTitle"))
        {
            //Do something with this game parameter
            
            oMobileNotif.SendDDNANotification(gameParameters);
           
        }
    }
}
