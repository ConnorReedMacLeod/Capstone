﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LogManager : SingletonPersistent<LogManager> {

    System.DateTimeOffset timeStart;
    string sLogfilePath;
    StreamWriter swFileWriter;

    public const string sLOGSDIR = "Logs/";
    public const int nMAXLOGFILES = 5;
    public const string sLogFileHeader = "log-match";

    public List<FileInfo> lstLogFiles;

    //Holds all the serialized match inputs that we've loaded in from the current log file
    public List<int[]> lstLoggedSerializedMatchInputs;

    private bool bLoggingInfo;

    public override void Init() {

        SceneManager.sceneUnloaded += OnLeaveScene;
    }

    public void InitMatchLog() {

        try {
            bLoggingInfo = true;

            CleanOldLogFiles();

            CreateMatchLogFile();

            LogMatchSetup();
        } catch(IOException e) {
            Debug.Log("Encountered " + e);
            OnFileSharingError();
        }

    }

    public void OnFileSharingError() {
        Debug.Log(LibDebug.AddColor("Warning! Log File Use Conflict - Disabling Log File Usage", LibDebug.Col.RED));

        //Turn off logging since there was an issue with this instance of the game recording log information
        bLoggingInfo = false;
    }

    public bool IsValidLogFile(FileInfo fileinfo) {
        //Do some verification checks to determine if this file is indeed a log file

        //Currently just checking that the file header is correct
        if(File.ReadLines(string.Concat(sLOGSDIR, fileinfo.Name)).First() != sLogFileHeader) {
            //Debug.Log(File.ReadLines(string.Concat(sLOGSDIR, fileinfo.Name)).First() + " isn't a valid header for a log file");
            return false;
        }

        return true;
    }

    public void UpdateRecognizedLogFiles() {

        EnsureLogDirExists();

        FileInfo[] arFilesInLogDir = new DirectoryInfo(sLOGSDIR).GetFiles();
        lstLogFiles = new List<FileInfo>();

        foreach(FileInfo fileinfo in arFilesInLogDir) {
            if(IsValidLogFile(fileinfo)) {
                lstLogFiles.Add(fileinfo);
            }
        }
    }

    public void CleanOldLogFiles() {
        UpdateRecognizedLogFiles();

        //Get a list of all log files currently in the logs folder and only keep the N newest ones
        foreach(FileInfo fileInfo in lstLogFiles.OrderByDescending(x => x.LastWriteTime).Skip(nMAXLOGFILES - 1)) {
            fileInfo.Delete();
        }

    }

    public string GetDateTime() {

        System.DateTimeOffset timeCur = new System.DateTimeOffset(System.DateTime.UtcNow).ToLocalTime();

        return string.Format("{0}-{1}-{2}-{3}-{4}", timeCur.Year, timeCur.Month.ToString().PadLeft(2, '0'), timeCur.Day.ToString().PadLeft(2, '0'),
            timeCur.Hour.ToString().PadLeft(2, '0'), timeCur.Minute.ToString().PadLeft(2, '0'));
    }

    public string GetTimestamp() {

        System.DateTimeOffset timeCur = new System.DateTimeOffset(System.DateTime.UtcNow).ToLocalTime();

        return string.Format("{0}-{1}", timeCur.Minute, timeCur.Second);
    }

    public void EnsureLogDirExists() {
        //Note that this won't do anythign if the directory already exists (which works out nicely)
        Directory.CreateDirectory(sLOGSDIR);

    }

    public void CreateMatchLogFile() {

        EnsureLogDirExists();

        //Initialize the file name we'll want for the log for this match
        timeStart = new System.DateTimeOffset(System.DateTime.UtcNow).ToLocalTime();

        sLogfilePath = string.Format("{0}log-{1}.txt", sLOGSDIR, GetDateTime());

        //Attempt to initialize the writer
        swFileWriter = new StreamWriter(sLogfilePath, false);

        Debug.LogFormat("Created {0}", sLogfilePath);

        //Add a log file header to the file
        WriteLogFileHeader();
    }

    public void WriteLogFileHeader() {

        WriteToMatchLogFile(sLogFileHeader);

    }

    public void CloseLogFile() {

        WriteToMatchLogFile("Log Complete");

        swFileWriter.Close();

        swFileWriter.Dispose();
    }

    public void LogMatchSetup() {

        LogMaster();
        LogPlayerOwners();
        LogInputTypes();
        LogCharacterOrdering(0);
        LogCharacterOrdering(1);
        LogLoadouts(0);
        LogLoadouts(1);
        LogPositionCoords(0);
        LogPositionCoords(1);
        LogRandomizationSeed();

    }

    public void LoadLoggedMatchSetup(FileInfo fileinfoLog) {

        //Initialize the array of lists of MatchInputs we'll be storing chosen skills in
        lstLoggedSerializedMatchInputs = new List<int[]>();

        string[] arsLogLines = File.ReadAllLines(string.Concat(sLOGSDIR, fileinfoLog.Name));

        foreach(string sLine in arsLogLines) {
            string[] arsSplitLine = sLine.Split(':');

            switch(arsSplitLine[0]) {
            case "po":
                LoadLoggedPlayerOwners(arsSplitLine);
                break;

            case "it":
                LoadLoggedInputTypes(arsSplitLine);
                break;

            case "co":
                LoadLoggedCharacterOrdering(arsSplitLine);
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

            case "mi":
                LoadLoggedMatchInput(arsSplitLine);
                break;

            default:
                //Debug.LogFormat("Nothing to load for entry: {0}", arsSplitLine[0]);
                break;
            }
        }

        //Now that all the match setup parameters have been loaded from the log file, have the NetworkConnectionManager move us into the Match scene
        //We'll do a small delay just to ensure that the submitted setup information has been properly setup in the room
        //TODO - Have a safer way of confirming that the sent parameters have been recieved and the room properties have been updated before
        //        automatically moving into the match scene
        NetworkConnectionManager.Get().Invoke("TransferToMatchScene", 1.0f);
    }

    public void LogMaster() {
        WriteToMatchLogFile(string.Format("ma:{0}", PhotonNetwork.IsMasterClient));
    }

    public void LogPlayerOwners() {
        WriteToMatchLogFile(string.Format("po:{0}:{1}", NetworkMatchSetup.GetPlayerOwner(0), NetworkMatchSetup.GetPlayerOwner(1)));
    }

    public void LoadLoggedPlayerOwners(string[] arsSplitLogs) {

        Debug.Assert(arsSplitLogs[0] == "po");

        int playerowner0, playerowner1;

        if(int.TryParse(arsSplitLogs[1], out playerowner0) == false || playerowner0 < 0) {
            Debug.LogErrorFormat("Error! {0} was not a valid player owner to be loaded", arsSplitLogs[1]);
            return;
        }
        if(int.TryParse(arsSplitLogs[2], out playerowner1) == false || playerowner1 < 0) {
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

        if(int.TryParse(arsSplitLogs[1], out inputtype0) == false || inputtype0 < 0) {
            Debug.LogErrorFormat("Error! {0} was not a valid input type to be loaded", arsSplitLogs[1]);
            return;
        }
        if(int.TryParse(arsSplitLogs[2], out inputtype1) == false || inputtype1 < 0) {
            Debug.LogErrorFormat("Error! {0} was not a valid input type to be loaded", arsSplitLogs[2]);
            return;
        }

        NetworkMatchSetup.SetInputType(0, (LocalInputType.InputType)inputtype0);
        NetworkMatchSetup.SetInputType(1, (LocalInputType.InputType)inputtype1);
    }

    public void LogCharacterOrdering(int iPlayer) {

        for(int iChr = 0; iChr < Match.NINITIALCHRSPERTEAM; iChr++) {
            CharType.CHARTYPE chartype = NetworkMatchSetup.GetCharacterOrdering(iPlayer, iChr);

            WriteToMatchLogFile(string.Format("co:{0}:{1}:{2}:{3}", iPlayer, iChr, (int)chartype, CharType.GetChrName(chartype)));
        }
    }

    public void LoadLoggedCharacterOrdering(string[] arsSplitLogs) {

        Debug.Assert(arsSplitLogs[0] == "co");

        int iPlayer, iChr, nchrSelection;

        if(int.TryParse(arsSplitLogs[1], out iPlayer) == false || iPlayer < 0 || iPlayer >= Match.NPLAYERS) {
            Debug.LogErrorFormat("Error! {0} was not a valid player id to be loaded", arsSplitLogs[1]);
            return;
        }
        if(int.TryParse(arsSplitLogs[2], out iChr) == false || iChr < 0 || iChr >= Match.NINITIALCHRSPERTEAM) {
            Debug.LogErrorFormat("Error! {0} was not a valid chr id to be loaded", arsSplitLogs[2]);
            return;
        }
        if(int.TryParse(arsSplitLogs[3], out nchrSelection) == false) {
            Debug.LogErrorFormat("Error! {0} was not a valid serialized character selections to be loaded", arsSplitLogs[3]);
            return;
        }

        NetworkMatchSetup.SetCharacterOrdering(iPlayer, iChr, (CharType.CHARTYPE)nchrSelection);
    }

    public void LogLoadouts(int iPlayer) {

        for(int iChr = 0; iChr < Match.NINITIALCHRSPERTEAM; iChr++) {
            LoadoutManager.Loadout loadout = NetworkMatchSetup.GetLoadout(iPlayer, iChr);

            string sLoadout = string.Format("lo:{0}:{1}", iPlayer, iChr);

            int[] arnSerializedLoadout = LoadoutManager.SerializeLoadout(loadout);

            sLoadout += LibConversions.ArToStr(arnSerializedLoadout);

            sLoadout += "\n" + loadout;

            WriteToMatchLogFile(sLoadout);
        }

    }

    public void LoadLoggedLoadouts(string[] arsSplitLogs) {

        Debug.Assert(arsSplitLogs[0] == "lo");

        int iPlayer, iChr;

        if(int.TryParse(arsSplitLogs[1], out iPlayer) == false || iPlayer < 0 || iPlayer >= Match.NPLAYERS) {
            Debug.LogErrorFormat("Error! {0} was not a valid player id to be loaded", arsSplitLogs[1]);
            return;
        }
        if(int.TryParse(arsSplitLogs[2], out iChr) == false || iChr < 0 || iChr >= Match.NINITIALCHRSPERTEAM) {
            Debug.LogErrorFormat("Error! {0} was not a valid chr id to be loaded", arsSplitLogs[2]);
            return;
        }

        int[] arnSerializedLoadout = new int[arsSplitLogs.Length - 3];

        //Copy and translate all the logged strings into serialized ints in an array
        for(int i = 0; i < arnSerializedLoadout.Length; i++) {
            if(int.TryParse(arsSplitLogs[i + 3], out arnSerializedLoadout[i]) == false) {
                Debug.LogErrorFormat("Error! {0} was not a valid serialized loadout entry to be loaded", arsSplitLogs[i + 3]);
                return;
            }
        }

        NetworkMatchSetup.SetLoadout(iPlayer, iChr, LoadoutManager.UnserializeLoadout(arnSerializedLoadout));
    }

    public void LogPositionCoords(int iPlayer) {

        for(int iChr = 0; iChr < Match.NMINACTIVECHRSPERTEAM; iChr++) {
            Position.Coords poscoords = NetworkMatchSetup.GetPositionCoords(iPlayer, iChr);

            WriteToMatchLogFile(string.Format("pc:{0}:{1}:{2}:{3}", iPlayer, iChr, Serializer.SerializeByte(poscoords), poscoords));
        }
    }

    public void LoadLoggedPositionCoords(string[] arsSplitLogs) {

        Debug.Assert(arsSplitLogs[0] == "pc");

        int iPlayer, iChr, nSerializedCoords;

        if(int.TryParse(arsSplitLogs[1], out iPlayer) == false || iPlayer < 0 || iPlayer >= Match.NPLAYERS) {
            Debug.LogErrorFormat("Error! {0} was not a valid player id to be loaded", arsSplitLogs[1]);
            return;
        }
        if(int.TryParse(arsSplitLogs[2], out iChr) == false || iChr < 0 || iChr >= Match.NMINACTIVECHRSPERTEAM) {
            Debug.LogErrorFormat("Error! {0} was not a valid chr id to be loaded", arsSplitLogs[2]);
            return;
        }
        if(int.TryParse(arsSplitLogs[3], out nSerializedCoords) == false) {
            Debug.LogErrorFormat("Error! {0} was not a valid serialized coordinates to be loaded", arsSplitLogs[3]);
            return;
        }

        NetworkMatchSetup.SetPositionCoords(iPlayer, iChr, Serializer.DeserializeCoords(nSerializedCoords));
    }

    public void LogRandomizationSeed() {
        WriteToMatchLogFile(string.Format("rs:{0}", NetworkMatchSetup.GetRandomizationSeed().ToString()));
    }

    public void LoadLoggedRandomizationSeed(string[] arsSplitLogs) {

        Debug.Assert(arsSplitLogs[0] == "rs");

        int nRandomizationSeed;

        if(int.TryParse(arsSplitLogs[1], out nRandomizationSeed) == false) {
            Debug.LogErrorFormat("Error! {0} was not a valid randomization seed to be loaded", arsSplitLogs[1]);
            return;
        }
        NetworkMatchSetup.SetRandomizationSeed(nRandomizationSeed);
    }

    public void LogMatchInput(MatchInput matchinput) {
        Debug.Log(matchinput);
        Debug.Log(matchinput.plyrActing);
        string sMatchInput = string.Format("mi:{0}", matchinput.plyrActing.id);

        int[] arnSerializedMatchInput = matchinput.Serialize();

        sMatchInput += LibConversions.ArToStr(arnSerializedMatchInput);

        //Also print out a friendly human-readable input
        sMatchInput += "\n" + matchinput.ToString();

        WriteToMatchLogFile(sMatchInput);
    }

    public void LoadLoggedMatchInput(string[] arsSplitLogs) {

        Debug.Assert(arsSplitLogs[0] == "mi");

        int iPlayerActing;

        if(int.TryParse(arsSplitLogs[1], out iPlayerActing) == false || iPlayerActing < 0 || iPlayerActing >= Match.NPLAYERS) {
            Debug.LogErrorFormat("Error! {0} was not a valid player id to be loaded", arsSplitLogs[1]);
            return;
        }

        int[] arnSerializedMatchInput = new int[arsSplitLogs.Length - 2];

        //Copy and translate all the logged strings into serialized ints in an array
        for(int i = 0; i < arnSerializedMatchInput.Length; i++) {
            if(int.TryParse(arsSplitLogs[i + 2], out arnSerializedMatchInput[i]) == false) {
                Debug.LogErrorFormat("Error! {0} was not a valid serialized matchinput entry to be loaded", arsSplitLogs[i + 2]);
                return;
            }
        }

        //Now that we've recorded the matchinput data from the log file, let's store it for when we load it into some scripted player input
        lstLoggedSerializedMatchInputs.Add(arnSerializedMatchInput);
    }

    public void LoadStartingInputs() {

        //If we're not the master, then it's not our job to load any inputs
        if(PhotonNetwork.IsMasterClient == false) return;

        //If we don't have any logged match inputs, then we don't need to do anything here
        if(lstLoggedSerializedMatchInputs == null) return;

        //First, double check that the NetworkMatchSender has been started so we can ask it to send some input for us
        NetworkMatchSender.Get().Start();

        //If we have any loaded logged match inputs, then we can send these all to the matchnetworkreceiver's input buffer
        for(int i = 0; i < lstLoggedSerializedMatchInputs.Count; i++) {
            //For the index, just send 0, 1, 2,... in order since we'll be starting from the very beginning of the match
            NetworkMatchSender.Get().SendInput(i, lstLoggedSerializedMatchInputs[i]);
        }

        //Since we're done giving all our logged inputs to the networkreceiver, we cna clear out our locally stored lst of inputs
        lstLoggedSerializedMatchInputs = null;
    }

    void OnLeaveScene<Scene>(Scene scene) {

        if(swFileWriter == null) return;

        Debug.Log("Leaving scene - closing log file");

        WriteToMatchLogFile("LeftScene");

        CloseLogFile();
    }


    public void OnApplicationQuit() {

        if(swFileWriter == null) return;

        WriteToMatchLogFile("ApplicationQuit");

        CloseLogFile();

    }


    public void WriteToMatchLogFile(string sText, bool bTimestamp = false) {
        //Don't bother logging anything if we've been deactivated
        if(bLoggingInfo == false) return;

        //Consider if the stream writer should only be opened and closed for a short time to add one line of text rather than 
        //  the current method of keeping it open at all times
        swFileWriter.WriteLine(sText);

    }


}
