using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypePassive : TypeAction {

	public TypePassive(Action act) : base(act) {

    }

    public override string getName() {
        return "Passive";
    }

    public override TYPE Type() {
        return TYPE.PASSIVE;
    }

    public override bool Usable() {
        //Passive's cannot be used

        return false;
    }

    public override int GetActionPointCost() {
        Debug.Log("Shouldn't be getting the action point cost of a passive");
        return 0;
    }

    public override void UseAction() {

        Debug.LogError("Can't use a passive action");

    }
}
