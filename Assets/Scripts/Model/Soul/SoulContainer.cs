using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulContainer : MonoBehaviour {

    public List<Soul> lstSoul = new List<Soul>();//TODO:: Lists have pretty bad runtime as we're using them  - worth changing?

    public Chr chrOwner;

    public int nMaxVisibleSoul;

    public Subject subVisibleSoulUpdate = new Subject();

    //returns a list of the visible events in the soul (oldest elements first)
    public List<Soul> GetVisibleSoul() {
        //TODO:: Just make this consistently maintained so we don't have to recalculate it each time
        List<Soul> lstVisibleSoul = new List<Soul>();

        for(int i=0; i < lstSoul.Count; i++) {
            if(lstSoul[i].bVisible == true) {
                lstVisibleSoul.Add(lstSoul[i]);
            }
        }

        Debug.Assert(lstVisibleSoul.Count <= nMaxVisibleSoul);

        return lstVisibleSoul;
    }

    public void cbReduceDurations(Object target, params object[] args) {

        //MAJOR TODO:: Should probably not even have this method
        //Instead, have each soul subscribe to the ExecTurnEnd.SubAllEndTurn trigger
        //So that each can trigger one by one

        //TODO:: Fix this so that we don't potentially get errors when elements get removed mid-list-traversal

        List<Soul> lstExpiredSoul = new List<Soul>();

        //Search through the list from the back (most recently added) to the front
        for (int i = lstSoul.Count - 1; i >= 0; i--) {
            if (lstSoul[i].bDuration == true) {
                lstSoul[i].nCurDuration--;

                if(lstSoul[i].nCurDuration == 0) {
                    //If this soul effect has finished its duration

                    //Then add it to the list of effects to be removed
                    lstExpiredSoul.Add(lstSoul[i]);
                }
            }
        }

        //Remove each effect that was noted as having no duration left
        foreach (Soul SoulToRemove in lstExpiredSoul) {
            RemoveSoul(SoulToRemove);
        }

        //Let others know that the visible soul MAY have changed (not necessarily)
        subVisibleSoulUpdate.NotifyObs(this);

    }

    public void RemoveSoul(Soul toRemove) {

        bool bRemoveSuccessfully = lstSoul.Remove(toRemove);

        if(bRemoveSuccessfully == false) {
            Debug.Log("Couldn't remove " + toRemove.sName + " since it's already removed");
            return;
        }

        toRemove.OnRemoval();

        //Let others know that the visible soul MAY have changed (not necessarily)
        subVisibleSoulUpdate.NotifyObs(this);

        //Debug.Log("After removing " + toRemove.sName);

        if (ContAbilityEngine.bDEBUGENGINE) PrintAllSoul();

    }

    public void ApplySoul(Soul newSoul) {
        
        if(newSoul.bVisible == true) {
            //Then check if we have enough slots
            List<Soul> lstVisibleSoul = GetVisibleSoul();

            if(lstVisibleSoul.Count == nMaxVisibleSoul) {
                //Then were already using all of our slots

                //So remove the oldest visible effect
                Soul soulRemoved = lstVisibleSoul[0];

                lstSoul.Remove(lstVisibleSoul[0]);

                //Perform any action that needs to be done when this Soul is removed;
                soulRemoved.OnRemoval();

                //TODO:: Add in a check for locked events
            }

        }

        lstSoul.Add(newSoul);

        //Perform any action that needs to be done on application
        newSoul.OnApply(this);

        //Let others know that the visible soul MAY have changed (not necessarily)
        subVisibleSoulUpdate.NotifyObs(this);

        if (ContAbilityEngine.bDEBUGENGINE) PrintAllSoul();

    }

    public void PrintAllSoul() {
        Debug.Log("********** Printing all Soul for " + chrOwner.sName + "*****************");
        for (int i = 0; i < lstSoul.Count; i++) {
            string sVisible = "";
            string sDuration = "";
            if (lstSoul[i].bVisible == true) {
                sVisible = "(Visible)";
            }
            if (lstSoul[i].bDuration == true) {
                sDuration = " with duration " + lstSoul[i].nCurDuration + "/" + lstSoul[i].pnMaxDuration.Get();
            }
            Debug.Log("[" + i + "] - " + lstSoul[i].sName + " " + sVisible + sDuration);
        }
        Debug.Log("*************************************************************************");
    }




	// Use this for initialization
	void Start () {

        nMaxVisibleSoul = 3;

        ExecTurnEndTurn.subAllPostTrigger.Subscribe(cbReduceDurations);
        //TODO::  At somepoint, fix the order of the notifications to be sent out to next-character-to-act first


    }
}
