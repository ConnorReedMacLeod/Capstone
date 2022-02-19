using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class LogManager : SingletonPersistent<LogManager> {

    System.DateTimeOffset timeStart;
    string sLogfilePath;
    StreamWriter swFileWriter;

    public const string sLOGSDIR = "Logs/";
    public const int nMAXLOGFILES = 5;

    public override void Init() {
        
    }

    public void InitMatchLog() {

        CleanOldLogFiles();

        CreateMatchLogFile();

        LogMatchSetup();

    }

    public void CleanOldLogFiles() {

        Debug.Log(new DirectoryInfo(sLOGSDIR).GetFiles().Length + " is the number of files in our log directory");

        //Get a list of all log files currently in the logs folder and only keep the N newest ones
        foreach (FileInfo fileInfo in new DirectoryInfo(sLOGSDIR).GetFiles().OrderByDescending(x => x.LastWriteTime).Skip(nMAXLOGFILES - 1)) {
            Debug.Log(fileInfo.Name + " is old, so we're deleting it");
            fileInfo.Delete();
        }

    }

    public string GetDateTime() {

        System.DateTimeOffset timeCur = new System.DateTimeOffset(System.DateTime.UtcNow).ToLocalTime();

        return string.Format("{0}-{1}-{2}-{3}-{4}", timeCur.Year, timeCur.Month, timeCur.Day, timeCur.Hour, timeCur.Minute);
    }

    public string GetTimestamp() {

        System.DateTimeOffset timeCur = new System.DateTimeOffset(System.DateTime.UtcNow).ToLocalTime();

        return string.Format("{0}-{1}", timeCur.Minute, timeCur.Second);
    }

    public void CreateMatchLogFile() {

        //Initialize the file name we'll want for the log for this match
        timeStart = new System.DateTimeOffset(System.DateTime.UtcNow).ToLocalTime();
        Debug.Log("Current time is " + timeStart.ToString());

        sLogfilePath = string.Format("{0}log-{1}.txt", sLOGSDIR, GetDateTime());

        //Delete the file if it already exists
        if(File.Exists(sLogfilePath)) File.Delete(sLogfilePath);

        Debug.LogFormat("Creating {0}", sLogfilePath);

        //Initialize the writer
        swFileWriter = new StreamWriter(sLogfilePath, false);

    }

    public void CloseLogFile() {

        WriteToMatchLogFile("Log Complete");

        swFileWriter.Close();
    }

    public void LogMatchSetup() {

        LogPlayerOwners();
        LogInputTypes();
        LogCharacterSelections(0);
        LogCharacterSelections(1);
        LogLoadouts(0);
        LogLoadouts(1);
        LogPositionCoords(0);
        LogPositionCoords(1);
        LogRandomizationSeed();

    }

    public void LoadLoggedMatchSetup(string sLogFilePath) {

        string[] arsLogLines = File.ReadAllLines(sLogFilePath);

        foreach(string sLine in arsLogLines) {
            string[] arsSplitLine = sLine.Split(':');

            switch (arsSplitLine[0]) {
                case "po":
                    LoadLoggedPlayerOwners(arsSplitLine);
                    break;

                case "it":
                    LoadLoggedInputTypes(arsSplitLine);
                    break;

                case "cs":
                    LoadLoggedCharacterSelections(arsSplitLine);
                    break;

                case "lo":
                    LoadLoggedLoadouts(arsSplitLine);
                    break;

                case "pc":
                    LoadLoggedPositionCoords(arsSplitLine);
                    break;

                case "rs":
                    LoadLoggedRandomizationSeed(arsSplitLine);
                    break;

                default:
                    Debug.LogFormat("Nothing to load for entry: {0}", arsSplitLine[0]);
                    break;
            }
        }

        LoadLoggedPlayerOwners(arsLogLines[0].Split(':'));
        LoadLoggedInputTypes(arsLogLines[1].Split(':'));
        LoadLoggedCharacterSelections(arsLogLines[2].Split(':'));
        LoadLoggedCharacterSelections(arsLogLines[3].Split(':'));
        LoadLoggedLoadouts(arsLogLines[4].Split(':'));
        LoadLoggedLoadouts(arsLogLines[5].Split(':'));
        LoadLoggedPositionCoords(arsLogLines[6].Split(':'));
        LoadLoggedPositionCoords(arsLogLines[7].Split(':'));
        LoadLoggedRandomizationSeed(arsLogLines[8].Split(':'));

    }

    public void LogPlayerOwners() {
        WriteToMatchLogFile(string.Format("po:{0}:{1}", NetworkMatchSetup.GetPlayerOwner(0), NetworkMatchSetup.GetPlayerOwner(1)));
    }

    public void LoadLoggedPlayerOwners(string[] arsSplitLogs) {

        Debug.Assert(arsSplitLogs[0] == "po");

        int playerowner0, playerowner1;

        if (int.TryParse(arsSplitLogs[1], out playerowner0) == false || playerowner0 < 0) {
            Debug.LogErrorFormat("Error! {0} was not a valid player owner to be loaded", arsSplitLogs[1]);
            return;
        }
        if (int.TryParse(arsSplitLogs[2], out playerowner1) == false || playerowner1 < 0) {
            Debug.LogErrorFormat("Error! {0} was not a valid player owner to be loaded", arsSplitLogs[2]);
            return;
        }

        NetworkMatchSetup.SetPlayerOwner(0, playerowner0);
        NetworkMatchSetup.SetPlayerOwner(1, playerowner1);
    }

    public void LogInputTypes() {
        WriteToMatchLogFile(string.Format("it:{0}:{1}", (int)NetworkMatchSetup.GetInputType(0), (int)NetworkMatchSetup.GetInputType(1)));
    }

    public void LoadLoggedInputTypes(string[] arsSplitLogs) {

        Debug.Assert(arsSplitLogs[0] == "it");

        int inputtype0, inputtype1;

        if (int.TryParse(arsSplitLogs[1], out inputtype0) == false || inputtype0 < 0) {
            Debug.LogErrorFormat("Error! {0} was not a valid input type to be loaded", arsSplitLogs[1]);
            return;
        }
        if (int.TryParse(arsSplitLogs[2], out inputtype1) == false || inputtype1 < 0) {
            Debug.LogErrorFormat("Error! {0} was not a valid input type to be loaded", arsSplitLogs[2]);
            return;
        }

        NetworkMatchSetup.SetInputType(0, (Player.InputType)inputtype0);
        NetworkMatchSetup.SetInputType(1, (Player.InputType)inputtype1);
    }

    public void LogCharacterSelections(int iPlayer) {

        for(int iChr = 0; iChr < Player.MAXCHRS; iChr++) {
            CharType.CHARTYPE chartype = NetworkMatchSetup.GetCharacterSelection(iPlayer, iChr);

            WriteToMatchLogFile(string.Format("cs:{0}-{1}:{2}:{3}", iPlayer, iChr, (int)chartype, CharType.GetChrName(chartype)));
        }
    }

    public void LoadLoggedCharacterSelections(string[] arsSplitLogs) {

        Debug.Assert(arsSplitLogs[0] == "cs");

        int iPlayer, iChr, nSerializedCoords;

        if (int.TryParse(arsSplitLogs[1], out iPlayer) == false || iPlayer < 0 || iPlayer >= Player.MAXPLAYERS) {
            Debug.LogErrorFormat("Error! {0} was not a valid player id to be loaded", arsSplitLogs[1]);
            return;
        }
        if (int.TryParse(arsSplitLogs[2], out iChr) == false || iChr < 0 || iChr >= Player.MAXCHRS) {
            Debug.LogErrorFormat("Error! {0} was not a valid chr id to be loaded", arsSplitLogs[2]);
            return;
        }
        if (int.TryParse(arsSplitLogs[3], out nSerializedCoords) == false) {
            Debug.LogErrorFormat("Error! {0} was not a valid serialized character selections to be loaded", arsSplitLogs[3]);
            return;
        }

        NetworkMatchSetup.SetPositionCoords(iPlayer, iChr, Position.UnserializeCoords(nSerializedCoords));
    }

    public void LogLoadouts(int iPlayer) {
        
        for(int iChr = 0; iChr < Player.MAXCHRS; iChr++) {
            LoadoutManager.Loadout loadout = NetworkMatchSetup.GetLoadout(iPlayer, iChr);

            string sLoadout = string.Format("lo:{0}-{1}", iPlayer, iChr);

            int[] arnSerializedLoadout = LoadoutManager.SerializeLoadout(loadout);

            //For each entry of our loadout, add it to our string to log
            for(int i=0; i<arnSerializedLoadout.Length; i++) {
                sLoadout += ":" + arnSerializedLoadout[i].ToString();
            }

            sLoadout += "\n" + loadout;

            WriteToMatchLogFile(sLoadout);
        }

    }

    public void LoadLoggedLoadouts(string[] arsSplitLogs) {

        Debug.Assert(arsSplitLogs[0] == "lo");

        int iPlayer, iChr;                                                                  

        if (int.TryParse(arsSplitLogs[1], out iPlayer) == false || iPlayer < 0 || iPlayer >= Player.MAXPLAYERS) {
            Debug.LogErrorFormat("Error! {0} was not a valid player id to be loaded", arsSplitLogs[1]);
            return;
        }
        if (int.TryParse(arsSplitLogs[2], out iChr) == false || iChr < 0 || iChr >= Player.MAXCHRS) {
            Debug.LogErrorFormat("Error! {0} was not a valid chr id to be loaded", arsSplitLogs[2]);
            return;
        }

        int[] arnSerializedLoadout = new int[arsSplitLogs.Length - 3];

        //Copy and translate all the logged strings into serialized ints in an array
        for(int i=0; i<arnSerializedLoadout.Length; i++) {
            if (int.TryParse(arsSplitLogs[i+3], out arnSerializedLoadout[i]) == false) {
                Debug.LogErrorFormat("Error! {0} was not a valid serialized loadout entry to be loaded", arsSplitLogs[i+3]);
                return;
            }
        }

        NetworkMatchSetup.SetLoadout(iPlayer, iChr, LoadoutManager.UnserializeLoadout(arnSerializedLoadout));
    }

    public void LogPositionCoords(int iPlayer) {

        for(int iChr=0; iChr < Player.MAXCHRS; iChr++) {
            Position.Coords poscoords = NetworkMatchSetup.GetPositionCoords(iPlayer, iChr);

            WriteToMatchLogFile(string.Format("pc:{0}-{1}:{2}:{3}", iPlayer, iChr, Position.SerializeCoords(poscoords), poscoords));
        }
    }

    public void LoadLoggedPositionCoords(string[] arsSplitLogs) {

        Debug.Assert(arsSplitLogs[0] == "pc");

        int iPlayer, iChr, nSerializedCoords;

        if (int.TryParse(arsSplitLogs[1], out iPlayer) == false || iPlayer < 0 || iPlayer >= Player.MAXPLAYERS) {
            Debug.LogErrorFormat("Error! {0} was not a valid player id to be loaded", arsSplitLogs[1]);
            return;
        }
        if (int.TryParse(arsSplitLogs[2], out iChr) == false || iChr < 0 || iChr >= Player.MAXCHRS) {
            Debug.LogErrorFormat("Error! {0} was not a valid chr id to be loaded", arsSplitLogs[2]);
            return;
        }
        if (int.TryParse(arsSplitLogs[3], out nSerializedCoords) == false) {
            Debug.LogErrorFormat("Error! {0} was not a valid serialized coordinates to be loaded", arsSplitLogs[3]);
            return;
        }

        NetworkMatchSetup.SetPositionCoords(iPlayer, iChr, Position.UnserializeCoords(nSerializedCoords));
    }

    public void LogRandomizationSeed() {
        WriteToMatchLogFile(string.Format("rs:{0}", NetworkMatchSetup.GetRandomizationSeed().ToString()));
    }

    public void LoadLoggedRandomizationSeed(string[] arsSplitLogs) {

        Debug.Assert(arsSplitLogs[0] == "rs");

        int nRandomizationSeed;

        if (int.TryParse(arsSplitLogs[1], out nRandomizationSeed) == false) {
            Debug.LogErrorFormat("Error! {0} was not a valid randomization seed to be loaded", arsSplitLogs[1]);
            return;
        }
        NetworkMatchSetup.SetRandomizationSeed(nRandomizationSeed);
    }

    public void LogMatchInput() {
        MatchInput curinput = NetworkMatchReceiver.Get().GetCurMatchInput();
        WriteToMatchLogFile(string.Format("mi:{0}:{1}:{2}", NetworkMatchReceiver.Get().indexCurMatchInput, curinput.Serialize(), curinput));
    }

    public void OnApplicationQuit() {

        WriteToMatchLogFile("ApplicationQuit");

        CloseLogFile();

    }
    

    public void WriteToMatchLogFile(string sText, bool bTimestamp = false) {

        swFileWriter.WriteLine(sText);

    }


}
