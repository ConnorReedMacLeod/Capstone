using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class StatsManager : SingletonPersistent<StatsManager> {

    //From strings -> T
    public Dictionary<string, int> dictStats;

    public const string sSTATSPATH = "Stats/stats.txt";

    public override void Init() {
        dictStats = new Dictionary<string, int>();
        ReadStatsFile();

        Chr.subAllDeath.Subscribe(cbCharacterDied);
        Player.subAllPlayerLost.Subscribe(cbPlayerLost);
    }

    public void OnApplicationQuit() {
        WriteStatsFile();
    }


    public void cbPlayerLost(Object target, params object[] args) {
        if(((Player)target).id == 0) {
            dictStats["Losses"]++;
        } else {
            dictStats["Wins"]++;
        }
        
    }


    public void cbCharacterDied(Object target, params object[] args) {
        if(((Chr)target).plyrOwner.id == 0) {
            dictStats["CharactersLost"]++;
        }else if (((Chr)target).plyrOwner.id == 1) {
            dictStats["CharactersKilled"]++;
        }
    }

    public void WriteStatsFile() {

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(sSTATSPATH, false);
        
        foreach(KeyValuePair<string, int> entry in dictStats) {
            
            writer.WriteLine(entry.Key + ":" + entry.Value);

        }

        writer.Close();

    }

    public void ReadStatsFile() {
        

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(sSTATSPATH);
        while(reader.EndOfStream != true) {
            //Read the stat file in
            string sStat = reader.ReadLine();

            //Split the stat file and load it into the hashmap
            string[] splitArray = sStat.Split(':');
            
            string sCategory = splitArray[0];
            int nValue = int.Parse(splitArray[1]);
            dictStats.Add(sCategory, nValue);
        }
        reader.Close();



    }


}
