using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class LogManager : MonoBehaviour {

    System.DateTimeOffset timeStart;
    string sLogfilePath;
    StreamWriter swFileWriter;

    public const string sLOGSPATH = "Logs/";
    public const int nMAXLOGFILES = 5;

    public void Start() {

        CleanOldLogFiles();

        InitLogFilePath();

        PrintRandomSeed();

        //Subscribe to all events that we support log printing for
        Chr.subAllDeath.Subscribe(cbCharacterDied);
        Player.subAllPlayerLost.Subscribe(cbPlayerLost);
    }

    public void CleanOldLogFiles() {

        //Get a list of all log files currently in the logs folder and only keep the 5 newest ones
        foreach (FileInfo fileInfo in new DirectoryInfo(sLOGSPATH).GetFiles().OrderByDescending(x => x.LastWriteTime).Skip(nMAXLOGFILES))
            fileInfo.Delete();

    }

    public void InitLogFilePath() {

        //Initialize the file name we'll want for the log for this match
        timeStart = new System.DateTimeOffset(System.DateTime.UtcNow).ToLocalTime();
        Debug.Log("Current time is " + timeStart.ToString());

        sLogfilePath = sLOGSPATH + "log-" + timeStart.Year + "-" + timeStart.Month + "-" + timeStart.Day + "-" + timeStart.Hour + "-" + timeStart.Minute + ".txt";

        //Delete the file if it already exists
        if(File.Exists(sLogfilePath)) File.Delete(sLogfilePath);

        Debug.Log(sLogfilePath);

        File.Create(sLogfilePath);
  

        //Initialize the writer
        swFileWriter = new StreamWriter(sLogfilePath, true);

    }

    public void PrintRandomSeed() {
        WriteLogFile(Random.state.ToString());
    }

    public void OnApplicationQuit() {

        WriteLogFile("ApplicationQuit");

        //Close the writer
        swFileWriter.Close();

    }


    public void cbPlayerLost(Object target, params object[] args) {


    }


    public void cbCharacterDied(Object target, params object[] args) {

    }

    public void WriteLogFile(string sText) {

        swFileWriter.WriteLine(sText);

    }


}
