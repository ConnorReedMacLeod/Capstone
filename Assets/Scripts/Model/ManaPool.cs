using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For the mana pool owned by a player
[RequireComponent(typeof(ViewMana))]
public class ManaPool : MonoBehaviour {

    public Player plyr;
    public Mana manaOwned;

    public Subject subManaChange = new Subject();

    public void SetPlayer(Player _plyr) {
        plyr = _plyr;
    }

    //Quickly check if we have enough mana to pay the provided cost
    public bool CanPayManaCost(ManaCost manaCost) {

        //Just treat our entire mana pool as a Mana payment and see if it could cover the cost
        return manaCost.CanBePaidWith(manaOwned);
    }


    public void ChangeMana(Mana.MANATYPE manaType, int nAmount = 1) {
        Mana manaToAdd = new Mana(0, 0, 0, 0, 0);
        manaToAdd[manaType] = nAmount;
        ChangeMana(manaToAdd);
    }

    public void ChangeMana(Mana manaToAdd) {
        manaOwned.ChangeMana(manaToAdd);
    }

    public void PayManaPaymentForCost(Mana manaPaid, ManaCost manaCost) {
        Debug.Assert(manaCost.CanBePaidWith(manaPaid));

        manaOwned.ChangeMana(Mana.GetNegatedMana(manaPaid));
    }


    //Auto-generate a possible payment we can use to pay for the provided cost
    public Mana GetPaymentForManaCost(ManaCost manaCost) {
        Debug.Assert(CanPayManaCost(manaCost));

        Mana manaFinalCost = manaCost.pManaCost.Get();

        //Initially cover the mandatory portions
        Mana manaPayment = new Mana(manaFinalCost[0], manaFinalCost[1], manaFinalCost[2], manaFinalCost[3]);

        //Now we just have to cover the effort portion
        if(manaOwned[Mana.MANATYPE.EFFORT] >= manaFinalCost[Mana.MANATYPE.EFFORT]) {
            //If we have enough raw effort mana, just use that directly and return
            manaPayment[Mana.MANATYPE.EFFORT] = manaFinalCost[Mana.MANATYPE.EFFORT];
            return manaPayment;
        }

        //Dump all the raw effort mana we have into the payment
        manaPayment[Mana.MANATYPE.EFFORT] = manaOwned[Mana.MANATYPE.EFFORT];

        int nEffortLeftToPay = manaFinalCost[Mana.MANATYPE.EFFORT] - manaPayment[Mana.MANATYPE.EFFORT];
        //Now generate a Mana struct representing the mana we have that we haven't yet used in our payment
        Mana manaUsableForEffort =
            new Mana(manaOwned[0] - manaPayment[0],
            manaOwned[1] - manaPayment[1],
            manaOwned[2] - manaPayment[2],
            manaOwned[3] - manaPayment[3]);

        //Repeatedly select a random owned mana and add it to the payment
        for(; nEffortLeftToPay > 0; nEffortLeftToPay--) {

            int nRandomManaType = Random.Range(0, (int)Mana.MANATYPE.EFFORT);
            for(int i = 0; i < (int)Mana.MANATYPE.EFFORT; i++) {
                //Cycle through the types of mana until we find one that we still have a supply of
                int nManaTypeIndex = (nRandomManaType + i) % (int)Mana.MANATYPE.EFFORT;
                if(manaUsableForEffort[nManaTypeIndex] > 0) {
                    //We have at least 1 of this mana to use for paying effort
                    manaUsableForEffort[nManaTypeIndex]--;
                    break;
                }
            }

        }

        if(manaCost.CanBePaidWith(manaPayment) == false) {
            Debug.LogError("Generated " + manaPayment + " but it can't pay for " + manaCost);
        }

        return manaPayment;
    }

    public void Start() {

        ChangeMana(new Mana(4, 4, 4, 4));

    }
}
