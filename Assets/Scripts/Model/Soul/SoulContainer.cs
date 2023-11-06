using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SoulContainer : MonoBehaviour {
    public List<Soul> lstSoul;//TODO:: Lists have pretty bad runtime as we're using them  - worth changing?

    public int nMaxVisibleSoul;

    public Subject subVisibleSoulUpdate;
    public Observer observer;

    bool bStarted;

    //returns a list of the visible events in the soul (oldest elements first)
    public List<Soul> GetVisibleSoul() {
        //TODO:: Just make this consistently maintained so we don't have to recalculate it each time
        List<Soul> lstVisibleSoul = new List<Soul>();

        for (int i = 0; i < lstSoul.Count; i++) {
            if (lstSoul[i].bVisible == true) {
                lstVisibleSoul.Add(lstSoul[i]);
            }
        }

        Debug.Assert(lstVisibleSoul.Count <= nMaxVisibleSoul);

        return lstVisibleSoul;
    }

    public void cbReduceDurations(Object target, params object[] args) {

        //TODO:: Consider if there are any weird corner cases with removing soul effects that trigger placement of new soul effects
        //   - don't want an effect to be queue'd up to be removed via duration, then being also removed by a 4th soul effect pushing
        //     the oldest soul effect off (could maybe just add a check to ensure we only call the RemoveSoul method on soul effects in
        //     lstExpiredSoul that are still present in lstSoul?

        List<Soul> lstExpiredSoul = new List<Soul>();

        //Search through the list from the back (most recently added) to the front
        for (int i = lstSoul.Count - 1; i >= 0; i--) {
            if (lstSoul[i].bDuration == true) {
                lstSoul[i].nCurDuration--;

                if (lstSoul[i].nCurDuration == 0) {
                    //If this soul effect has finished its duration

                    //Then add it to the list of effects to be removed
                    lstExpiredSoul.Add(lstSoul[i]);
                }
            }
        }

        //Remove each effect that was noted as having no duration left
        foreach (Soul SoulToRemove in lstExpiredSoul) {
            //If this soul effect somehow isn't in the list of active soul effects, don't try to remove it again
            if (lstSoul.Contains(SoulToRemove) == false) continue;
            RemoveSoul(SoulToRemove);
        }

        //Let others know that the visible soul MAY have changed (not necessarily)
        subVisibleSoulUpdate.NotifyObs(this);

    }

    public abstract void LetOwnerNotifySoulRemoved(Soul soulRemoved);

    public void RemoveAllSoul() {
        Debug.LogFormat("Removing all Soul effects from {0}", GetOwnerName());

        while (lstSoul.Count > 0) {
            Soul soulToRemove = lstSoul[0];
            Debug.LogFormat("Removing {0} soul effect", soulToRemove);
            RemoveSoul(soulToRemove);
        }
    }

    public void RemoveSoul(Soul toRemove) {

        bool bRemoveSuccessfully = lstSoul.Remove(toRemove);

        if (bRemoveSuccessfully == false) {
            Debug.Log("Couldn't remove " + toRemove.sName + " from " + toRemove.GetNameOfAppliedTo() + " since it's already removed");
            return;
        }

        toRemove.OnRemoval();

        //Let others know that the visible soul MAY have changed (not necessarily)
        subVisibleSoulUpdate.NotifyObs(this);

        LetOwnerNotifySoulRemoved(toRemove);

        Debug.Log("After removing " + toRemove.sName);

        if (ContSkillEngine.bDEBUGENGINE) PrintAllSoul();

    }

    public abstract void LetOwnerNotifySoulApplied(Soul soulApplied);

    public void ApplySoul(Soul newSoul) {

        if (newSoul.bVisible == true) {
            //Then check if we have enough slots
            List<Soul> lstVisibleSoul = GetVisibleSoul();

            if (lstVisibleSoul.Count == nMaxVisibleSoul) {
                //Then were already using all of our slots

                //TODO:: Add in a check for locked events
                //So remove the oldest visible effect
                Soul soulToRemove = lstVisibleSoul[0];

                RemoveSoul(soulToRemove);

                OnOverfillingSoul();
            }

        }

        lstSoul.Add(newSoul);

        //Perform any effect that needs to be done on application
        newSoul.OnApply(this);

        //Let others know that the visible soul MAY have changed (not necessarily)
        subVisibleSoulUpdate.NotifyObs(this);

        LetOwnerNotifySoulApplied(newSoul);

        if (ContSkillEngine.bDEBUGENGINE) PrintAllSoul();

    }

    public abstract string GetOwnerName();

    public virtual void OnOverfillingSoul() {
        //By default, we don't need to do anything extra
    }

    public void PrintAllSoul() {
        Debug.Log("********** Printing all Soul for " + GetOwnerName() + "*****************");
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


    public abstract void InitMaxVisibleSoul();

    // Use this for initialization
    public void Start() {
        if (bStarted) return;
        bStarted = true;

        lstSoul = new List<Soul>();

        subVisibleSoulUpdate = new Subject();
        observer = new Observer();
        InitMaxVisibleSoul();

        observer.Observe(ExecTurnEndTurn.subAllPostTrigger, cbReduceDurations);
        //TODO::  At somepoint, define a fixed order of notifications (e.g., based on original acting character order)

    }
}
