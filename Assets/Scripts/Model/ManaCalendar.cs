using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tracks the mana gain scheduled for a player at the beginning of each turn
public class ManaCalendar : MonoBehaviour {

    public const int NDAYS = 12;

    public Player plyrOwner;

    public class ManaDate {
        public ManaCalendar manacalendar;

        public int nDay;

        public int nRandomManaToGain;
        public Property<Mana> pmanaToGain;

        public ManaDate(ManaCalendar _manaCalendar, int _nDay, int _nRandomManaToGain) {
            manacalendar = _manaCalendar;
            nDay = _nDay;
            nRandomManaToGain = _nRandomManaToGain;
            pmanaToGain = new Property<Mana>(new Mana(0, 0, 0, 0));
        }

    }

    public ManaDate[] arManaDates;

    public void InitManaDays() {

        arManaDates = new ManaDate[NDAYS];

        for(int i=0; i<NDAYS; i++) {
            arManaDates[i] = new ManaDate(this, i, 1);
        }

    }

    public ManaDate GetCurrentManaDay() {

        int nCurDay = ContTurns.Get().nTurnNumber % NDAYS;

        return arManaDates[nCurDay];

    }


}
