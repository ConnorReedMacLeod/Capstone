using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewManaCalendar : MonoBehaviour {

    public ViewManaDate[] arViewManaDate;
    public Text txtTitle;

    public ManaCalendar modManaCalendar;

    // Start is called before the first frame update
    public void Start() {
        
        //txtTitle.text = string.Format("Mana Calendar for Player {0}", modManaCalendar.plyrOwner.id + 1);

    }

}
