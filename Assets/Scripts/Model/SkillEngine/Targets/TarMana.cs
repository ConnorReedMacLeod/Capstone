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

        return (byte)((nSerialized & (63 << (6 * (4 - nManaTypeIndex)))) >> (6 * (4 - nManaTypeIndex)));
    }

    public static int SerializeMana(Mana mana) {
        return (mana[0] << 24) + (mana[1] << 18) + (mana[2] << 12) + (mana[3] << 6) + mana[4];
    }

    public static Mana UnserializeMana(int nSerialized) {
        return new Mana(GetIndividualCost(0, nSerialized),
            GetIndividualCost(1, nSerialized),
            GetIndividualCost(2, nSerialized),
            GetIndividualCost(3, nSerialized),
            GetIndividualCost(4, nSerialized));
    }



    public override int Serialize(object objToSerialize) {
        return SerializeMana((Mana)objToSerialize);
    }

    public override object Unserialize(int nSerialized, List<object> lstSelectionsSoFar) {
        return UnserializeMana(nSerialized);
    }

    public static FnValidSelection COVERSCOST(ManaCost manaCostRequired) {
        return (object manaPaid, Selections selections) => (manaCostRequired.CanBePaidWith((Mana)manaPaid));
    }

    public static TarMana AddTarget(Skill _skill, ManaCost _manaCostRequired) {
        return AddTarget(_skill, _manaCostRequired, COVERSCOST(_manaCostRequired));
    }

    public static TarMana AddTarget(Skill _skill, ManaCost _manaCostRequried, FnValidSelection fnValidSelection) {
        TarMana tarmana = new TarMana(_skill, _manaCostRequried, fnValidSelection);
        _skill.lstTargets.Add(tarmana);

        return tarmana;
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
    public override IEnumerable<object> GetSelectableUniverse() {
        return null;
    }

    public override object GetRandomSelectable() {
        return skill.chrOwner.plyrOwner.manapool.GetPaymentForManaCost(manaCostRequired);
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
