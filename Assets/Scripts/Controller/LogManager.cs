using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LogManager : MonoBehaviour {

    string sLogPath;

    public void Start() {

        InitLogFileName();

        //Subscribe to all events that we support log printing for
        Chr.subAllDeath.Subscribe(cbCharacterDied);
        Player.subAllPlayerLost.Subscribe(cbPlayerLost);
    }

    public void InitLogFileName() {

        var Timestamp = new System.DateTimeOffset(System.DateTime.UtcNow).ToLocalTime();
        Debug.Log("Current time is " + Timestamp.ToString());

    }

    public void OnApplicationQuit() {

    }


    public void cbPlayerLost(Object target, params object[] args) {


    }


    public void cbCharacterDied(Object target, params object[] args) {

    }

    public void WriteStatsFile() {

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(sLogPath, true);


        writer.Close();

    }


}
