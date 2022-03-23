using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tracks the mana gain scheduled for a player at the beginning of each turn
public class ManaCalendar : MonoBehaviour {

    public const int NDAYS = 12;

    public Player plyrOwner;
    
    public ManaDate[] arManaDates = new ManaDate[NDAYS];

    public void Start() {
        ContTurns.Get().subTurnChange.Subscribe(cbOnDateChange);
    }

    public void cbOnDateChange(Object tar, params object[] args) {

        //Pass along the notification to the dates for the previous active day, and the new active day
        GetPreviousManaDate().subBecomeInactiveDate.NotifyObs();
        GetCurrentManaDate().subBecomeActiveDate.NotifyObs();

    }

    public ManaDate GetCurrentManaDate() {

        int nCurDay = ContTurns.Get().nTurnNumber % NDAYS;

        return arManaDates[nCurDay];

    }

    public ManaDate GetPreviousManaDate() {
        int nPrevDay = (ContTurns.Get().nTurnNumber - 1) % NDAYS;

        return arManaDates[nPrevDay];
    }


}
