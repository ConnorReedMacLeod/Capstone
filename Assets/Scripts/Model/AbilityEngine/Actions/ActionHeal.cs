using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHeal : Action {

    public Healing heal;
    public int nBaseHealing;

    public ActionHeal(Chr _chrOwner) : base(1, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        arArgs[0] = new TargetArgAlly((own, tar) => true);

        sName = "Heal (Cantrip)";
        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 0 });

        nCd = 3;
        nFatigue = 3;
        nActionCost = 0;

        nBaseHealing = 5;

        //Create a base Healing object that this action will apply
        heal = new Healing(this.chrSource, null, nBaseHealing);

        sDescription = "Restore 5 health to target ally";


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

                Debug.Log("This Heal Clause put an ExecHeal on the stack");
                ContAbilityEngine.Get().AddExec(new ExecHeal() {
                    chrSource = this.chrSource,
                    chrTarget = tarChr,

                    heal = healToApply,

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = "Healing"
                });
            }
        });

    }

}
