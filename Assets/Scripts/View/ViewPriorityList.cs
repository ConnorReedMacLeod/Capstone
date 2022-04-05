using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPriorityList : Singleton<ViewPriorityList> {

    public GameObject[] arHeadshots = new GameObject[6];

    public List<Chr> lstChrPriority {
        get { return ContTurns.Get().lstChrPriority; }
    }

    //Ensure the character in position i is correctly displayed
    public void SetHeadshot(int i) {


        string sImgPath = "Images/Chrs/" + lstChrPriority[i].sName + "/img" + lstChrPriority[i].sName + "Headshot";

        Sprite sprChr = Resources.Load(sImgPath, typeof(Sprite)) as Sprite;

        arHeadshots[i].GetComponent<SpriteRenderer>().sprite = sprChr;
    }

    public void cbUpdateHeadshots(Object target, params object[] args) {

        for(int i = 0; i < arHeadshots.Length; i++) {
            SetHeadshot(i);
        }
    }


    public void InitViewPriorityList() {
        ContTurns.Get().subAllPriorityChange.Subscribe(cbUpdateHeadshots);

        //Initially set up the headshots
        cbUpdateHeadshots(null);
    }

    // Use this for initialization
    public override void Init() {

    }
}
