using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For the mana pool owned by a player
[RequireComponent(typeof(ViewManaPool))]
public class ManaPool : MonoBehaviour {

    public Player plyr;
    public Mana manaOwned;
    public Mana manaReservedToPay; //Keep track of any mana that is planned to be used to pay for a cost, but hasn't yet been spent
    public Mana manaUsableToPay { //Get the amount of mana that we have free to pay for new costs (that hasn't been reserved yet)
        get {
            return Mana.SubMana(manaOwned, manaReservedToPay);
        }
    }

    public Subject subManaChange = new Subject();

    public void SetPlayer(Player _plyr) {
        plyr = _plyr;
    }

    //Quickly check if we have enough mana to pay the provided cost
    public bool CanPayManaCost(ManaCost manaCost) {

        //Just treat our entire mana pool as a Mana payment and see if it could cover the cost
        return manaCost.CanBePaidWith(manaOwned);
    }

    //Quickly check if we've got enough un-reserved mana to pay for another cost
    public bool HaveEnoughUsableMana(ManaCost manaCost) {

        //Pass along all our un-committed mana to the mana cost to see if it would be enough
        return manaCost.CanBePaidWith(manaUsableToPay);
    }


    public void ChangeMana(Mana.MANATYPE manaType, int nAmount = 1) {
        manaOwned.ChangeMana(manaType, nAmount);
        subManaChange.NotifyObs(this, manaType);
    }

    public void ChangeMana(Mana manaToAdd) {
        for(int i=0; i<=(int)Mana.MANATYPE.EFFORT; i++) {
            if (manaToAdd[i] == 0) continue;
            ChangeMana((Mana.MANATYPE)i, manaToAdd[i]);
        }
    }

    public void ReserveMana(Mana.MANATYPE manaType) {
        manaReservedToPay.ChangeMana(manaType);
        subManaChange.NotifyObs(this, manaType);
    }

    public void ReserveMana(Mana manaToReserve) {
        for (int i = 0; i <= (int)Mana.MANATYPE.EFFORT; i++) {
            if (manaToReserve[i] == 0) continue;
            manaReservedToPay.ChangeMana((Mana.MANATYPE)i, manaToReserve[i]);
            subManaChange.NotifyObs(this, (Mana.MANATYPE)i);
        }
    }

    public void UnreserveMana(Mana.MANATYPE manaType) {
        manaReservedToPay.ChangeMana(manaType, -1);
        subManaChange.NotifyObs(this, manaType);
    }

    public void UnreserveMana(Mana manaToUnreserve) {
        for (int i = 0; i <= (int)Mana.MANATYPE.EFFORT; i++) {
            if (manaToUnreserve[i] == 0) continue;
            manaReservedToPay.ChangeMana((Mana.MANATYPE)i, -manaToUnreserve[i]);
            subManaChange.NotifyObs(this, (Mana.MANATYPE)i);
        }
    }

    public void ResetReservedMana() {
        for (int i = 0; i <= (int)Mana.MANATYPE.EFFORT; i++) {
            manaReservedToPay[i] = 0;
            subManaChange.NotifyObs(this, (Mana.MANATYPE)i);
        }
    }


    public void PayManaPaymentForCost(Mana manaPaid, ManaCost manaCost) {
        Debug.Assert(manaCost.CanBePaidWith(manaPaid));

        manaOwned.ChangeMana(Mana.GetNegatedMana(manaPaid));
    }


    //Auto-generate a possible payment we can use to pay for the provided cost
    public Mana GetPaymentForManaCost(ManaCost manaCost) {
        Debug.Assert(HaveEnoughUsableMana(manaCost));

        Mana manaFinalCost = manaCost.pManaCost.Get();

        //Initially cover the mandatory portions
        Mana manaPayment = new Mana(manaFinalCost[0], manaFinalCost[1], manaFinalCost[2], manaFinalCost[3]);

        //Now we just have to cover the effort portion
        if(manaUsableToPay[Mana.MANATYPE.EFFORT] >= manaFinalCost[Mana.MANATYPE.EFFORT]) {
            //If we have enough raw effort mana, just use that directly and return
            manaPayment[Mana.MANATYPE.EFFORT] = manaFinalCost[Mana.MANATYPE.EFFORT];
            return manaPayment;
        }

        //Dump all the raw effort mana we have into the payment (up to the amount of the full cost)
        manaPayment[Mana.MANATYPE.EFFORT] = Mathf.Min(manaUsableToPay[Mana.MANATYPE.EFFORT], manaFinalCost[Mana.MANATYPE.EFFORT]);

        int nEffortLeftToPay = manaFinalCost[Mana.MANATYPE.EFFORT] - manaPayment[Mana.MANATYPE.EFFORT];
        //Now generate a Mana struct representing the mana we have that we haven't yet used in our payment
        Mana manaUsableForEffort = Mana.SubMana(manaUsableToPay, manaPayment);

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
        manaOwned = new Mana(4, 4, 4, 4, 2);
        manaReservedToPay = new Mana(0, 0, 0, 0, 0);

        ResetReservedMana();

    }
}
