using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPriorityList : Singleton<ViewPriorityList> {

    public GameObject pfHeadshot;
    public Dictionary<Chr, GameObject> dictHeadshots;

    public List<Chr> lstChrPriority {
        get { return ContTurns.Get().lstChrPriority; }
    }

    public void AddHeadshot(Chr chr) {

        Debug.LogFormat("Adding {0}'s headshot", chr);

        GameObject newHeadshot = Instantiate(pfHeadshot, this.transform);

        string sImgPath = "Images/Chrs/" + chr.sName + "/img" + chr.sName + "Headshot";

        Sprite sprChr = Resources.Load(sImgPath, typeof(Sprite)) as Sprite;

        newHeadshot.GetComponent<SpriteRenderer>().sprite = sprChr;

        dictHeadshots.Add(chr, newHeadshot);

        UpdateHeadshotPositions();
    }

    public void cbAddHeadshot(Object target, params object[] args) {
        AddHeadshot((Chr)target);
    }

    

    public void RemoveHeadshot(Chr chr) {
        Debug.LogFormat("Remove {0}'s headshot", chr);

        GameObject goHeadshotToRemove = dictHeadshots[chr];
        dictHeadshots.Remove(chr);

        Destroy(goHeadshotToRemove);

        UpdateHeadshotPositions();
    }

    public void cbRemoveHeadshot(Object target, params object[] args) {
        RemoveHeadshot((Chr)target);
    }


    public void UpdateHeadshotPositions() {

        Debug.LogFormat("Updating headshots at time {0}", Time.timeSinceLevelLoad);

        for (int i = 0; i < lstChrPriority.Count; i++) {
            dictHeadshots[lstChrPriority[i]].transform.position = new Vector3();
        }
    }

    public void cbUpdateHeadshots(Object target, params object[] args) {

        UpdateHeadshotPositions();
    }


    public void InitViewPriorityList() {
        //Subscribe to newly added characters, remove characters, or priority shuffling
        ContTurns.Get().subChrAddedPriority.Subscribe(cbAddHeadshot);
        ContTurns.Get().subChrRemovedPriority.Subscribe(cbRemoveHeadshot);
        ContTurns.Get().subAllPriorityChange.Subscribe(cbUpdateHeadshots);

        //Initially set up the headshots
        UpdateHeadshotPositions();
    }

    // Use this for initialization
    public override void Init() {

    }
}
