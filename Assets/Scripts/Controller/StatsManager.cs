using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class StatsManager : MonoBehaviour {

    //From strings -> T
    public Dictionary<string, int> dictStats;


    public void Start() {
        dictStats = new Dictionary<string, int>();
        ReadStatsFile();

        Chr.subAllDeath.Subscribe(cbCharacterDied);
        Player.subAllPlayerLost.Subscribe(cbPlayerLost);
    }

    public void OnApplicationQuit() {
        Debug.Log("Ending Game");
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
        string path = "Assets/Resources/stats.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, false);
        
        foreach(KeyValuePair<string, int> entry in dictStats) {
            
            writer.WriteLine(entry.Key + ":" + entry.Value);

        }

        writer.Close();

    }

    public void ReadStatsFile() {
        string path = "Assets/Resources/stats.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
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
