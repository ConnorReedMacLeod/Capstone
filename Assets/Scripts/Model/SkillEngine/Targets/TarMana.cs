using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarMana : Target {

    public ManaCost manaCostRequired;

    public static byte GetIndividualCost(int nManaTypeIndex, int nSerialized) {
        //Costs are 6 bits each (for a max of 64 cost for any individual mana type)
        //  organized as: <2 bits unused><6 bits Phys><6 bits Mental><6 bits Energy><6 bits Blood>
        if(nManaTypeIndex < 0 || nManaTypeIndex > 5) {
            Debug.LogError("Can't get the cost for mana type: " + nManaTypeIndex);
        }

        return (byte)((nSerialized & (63 << (6 * (5 - nManaTypeIndex)))) >> (6 * (5 - nManaTypeIndex)));
    }

    public override int Serialize(object objToSerialize) {
        Mana mana = (Mana)objToSerialize;


        return (mana[0] << 24) + (mana[1] << 18) + (mana[2] << 12) + (mana[3] << 6) + mana[4];
    }

    public override object Unserialize(int nSerialized) {

        return new Mana(GetIndividualCost(0, nSerialized),
            GetIndividualCost(1, nSerialized),
            GetIndividualCost(2, nSerialized),
            GetIndividualCost(3, nSerialized),
            GetIndividualCost(4, nSerialized));
    }

    public static FnValidSelection COVERSCOST(ManaCost manaCostRequired) {
        return (object manaPaid, Selections selections) => (manaCostRequired.CanBePaidWith((Mana)manaPaid));
    }

    //If no additional requirements are present, just enforce that the proposed mana amount covers the required cost
    public TarMana(Skill _skill, ManaCost _manaCostRequired) : base(_skill, COVERSCOST(_manaCostRequired)) {
        manaCostRequired = _manaCostRequired;
    }

    //If we have any extra requirements, we can AND those together with the basic "Can it cover the cost" check on the proposed mana
    public TarMana(Skill _skill, ManaCost _manaCostRequired, FnValidSelection _IsValidSelection) : base(_skill, Target.AND(_IsValidSelection, COVERSCOST(_manaCostRequired))) {
        manaCostRequired = _manaCostRequired;
    }

    //This doesn't really make sense for this targetting type, so just return an empty list
    public override IEnumerable<object> GetSelactableUniverse() {
        return null;
    }

    public override object GetRandomSelectable() {
        return skill.chrOwner.plyrOwner.mana.GetPaymentForManaCost(manaCostRequired);
    }

    public override void InitTargetDescription() {
        sTargetDescription = "Pay for the Required Manacost";
    }


    //Hooked up to the 'submit' button/trigger for after the mana payments have been selected
    public override void cbClickSelectable(Object target, params object[] args) {
        //Pass along the built-up mana selection 
        AttemptSelection(target);
    }

    protected override void OnStartLocalSelection() {

        //Bring out the ViewTarMana and initialize it to be requesting payment for this cost
        ViewTarMana.Get().StartPayment(this);

    }

    protected override void OnEndLocalSelection() {

        //Now that we're done paying, have the ViewTarMana clean itself up
        ViewTarMana.Get().CleanUp();

    }
}
