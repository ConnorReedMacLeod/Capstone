using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//To be used to manage a cost of mana that must/may be played
public class ManaCost {

    public Property<Mana> pManaCost;

    public bool bXCost; //Whether this cost has an X component (can pay any amount of mana)

    public ManaCost(Mana manaCost) : this(manaCost[0], manaCost[1], manaCost[2], manaCost[3], manaCost[4]) {

    }

    public ManaCost(int nPhys, int nMental, int nEnergy, int nBlood, int nEffort = 0) {
        pManaCost = new Property<Mana>(new Mana(nPhys, nMental, nEnergy, nBlood, nEffort));
    }


    public bool CanBePaidWith(Mana manaPaid) {

        Mana manaFinalCost = pManaCost.Get();

        for(int i = 0; i < (int)Mana.MANATYPE.EFFORT; i++) {
            if(manaPaid[i] < manaFinalCost[i]) {
                //The paid amount can't affored the cost for mana type i
                return false;
            }
        }
        if(manaPaid.GetTotalMana() < manaFinalCost.GetTotalMana()) {
            //The total amount of mana paid wasn't enough (so if all the coloured mana
            //   was enough, then the total amount of extra coloured mana can't have covered the effort cost)
            return false;
        }

        return true;
    }

    public override string ToString() {
        return pManaCost.Get().ToString();
    }
}
