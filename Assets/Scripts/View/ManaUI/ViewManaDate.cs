using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewManaDate : MonoBehaviour {

    public Text txtManaScheduled;


    public void SetScheduledManaText(Mana manaScheduled, int nRandomManaScheduled) {
        Mana manaTotalScheduled = new Mana(manaScheduled);

        //Displayer all extra random mana to be generated as effort icons
        manaTotalScheduled.ChangeMana(Mana.MANATYPE.EFFORT, nRandomManaScheduled);

        txtManaScheduled.text = manaTotalScheduled.ToPrettyString();
    }
}
