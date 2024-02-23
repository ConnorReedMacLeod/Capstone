using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPriorityList : Singleton<ViewPriorityList> {

    public GameObject pfHeadshot;
    public Dictionary<Chr, ViewPriorityHeadshot> dictHeadshots;

    public bool bRefreshNeeded; //If true, then we have priority changes that we should update ourselves to account for

    public List<Chr> lstChrPriority {
        get { return ContTurns.Get().lstChrPriority; }
    }
    
    public void AddHeadshot(Chr chr) {

        Debug.LogFormat("Adding {0}'s headshot", chr);

        GameObject goNewHeadshot = Instantiate(pfHeadshot, this.transform);
        ViewPriorityHeadshot viewNewHeadshot = goNewHeadshot.GetComponent<ViewPriorityHeadshot>();

        viewNewHeadshot.SetChrDisplaying(chr);

        dictHeadshots.Add(chr, viewNewHeadshot);
        Debug.LogFormat("{0} has been added to dictHeadshots", chr);

        bRefreshNeeded = true;
    }

    public void cbAddHeadshot(Object target, params object[] args) {
        Debug.LogFormat("Received cbAddHeadshot for {0}", target);
        AddHeadshot((Chr)target);
    }

    

    public void RemoveHeadshot(Chr chr) {

        ViewPriorityHeadshot viewHeadshotToRemove = dictHeadshots[chr];

        dictHeadshots.Remove(chr);

        viewHeadshotToRemove.DestroyHeadshot();

        bRefreshNeeded = true;
    }

    public void cbRemoveHeadshot(Object target, params object[] args) {
        RemoveHeadshot((Chr)target);
    }


    public void UpdateHeadshotPositions() {

        for (int i = 0; i < lstChrPriority.Count; i++) {
            dictHeadshots[lstChrPriority[i]].transform.SetSiblingIndex(i);
        }
    }

    public void cbUpdateHeadshots(Object target, params object[] args) {

        bRefreshNeeded = true;

    }


    public void InitViewPriorityList() {
        Debug.Log("InitViewPriorityList");

        //Subscribe to newly added characters, remove characters, or priority shuffling
        ContTurns.Get().subChrAddedPriority.Subscribe(cbAddHeadshot);
        ContTurns.Get().subChrRemovedPriority.Subscribe(cbRemoveHeadshot);
        ContTurns.Get().subAllPriorityChange.Subscribe(cbUpdateHeadshots);

    }

    // Use this for initialization
    public override void Init() {
        dictHeadshots = new Dictionary<Chr, ViewPriorityHeadshot>();
        bRefreshNeeded = false;
    }

    public void Update() {

        if (bRefreshNeeded) {
            bRefreshNeeded = false;
            UpdateHeadshotPositions();
        }

    }
}
