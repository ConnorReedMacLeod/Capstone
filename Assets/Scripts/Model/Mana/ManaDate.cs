using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaDate : MonoBehaviour {
    public ManaCalendar manacalendar;

    public int nDay;
    
    public Property<Mana> pmanaScheduled;

    public Subject subBecomeActiveDate = new Subject();
    public Subject subBecomeInactiveDate = new Subject();

    private bool bStarted = false;

    public void Start() {
        if (bStarted) return;
        bStarted = true;
        
        //Note that any scheduled effort mana will be distributed as random coloured mana
        pmanaScheduled = new Property<Mana>(new Mana(0, 0, 0, 0, 1));
    }


}
