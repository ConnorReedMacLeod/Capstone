using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSerenade : Action {

    public Healing heal;
    public int nBaseHealing;

    public ActionSerenade(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        arArgs[0] = new TargetArgChr(Action.IsFriendly);

        sName = "Serenade";
        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 1, 0, 0 });

        nCd = 8;
        nFatigue = 4;
        nActionCost = 1;

        nBaseHealing = 25;
        //Create a base Healing object that this action will apply
        heal = new Healing(this.chrSource, null, nBaseHealing);

        sDescription1 = "Heal 25 to the chosen ally.";


        SetArgOwners();
    }

    override public void Execute(int[] lstTargettingIndices) {

        Chr tarChr = Chr.GetTargetByIndex(lstTargettingIndices[0]);

        stackClauses.Push(new Clause() {
            fExecute = () => {

                //Make a copy of the heal object to give to the executable
                Healing healToApply = new Healing(heal);
                //Give the healing object its target
                healToApply.SetChrTarget(tarChr);

                ContAbilityEngine.Get().AddExec(new ExecHeal() {
                    chrSource = this.chrSource,
                    chrTarget = tarChr,

                    heal = healToApply,

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = "Healing " + tarChr.sName
                });
            }
        });

    }
}
