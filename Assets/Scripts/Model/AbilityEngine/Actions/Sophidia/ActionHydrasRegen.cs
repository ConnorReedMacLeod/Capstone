using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRegenerate : Action {

    public Healing heal;
    public int nBaseHealing;

    public ActionRegenerate(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner);

        sName = "Regenerate";
        sDisplayName = "Regenerate";

        SoulChannel soulChannelEffect = new SoulChannel(this) {
            
            lstTriggers = new List<Soul.TriggerEffect>() {
                new Soul.TriggerEffect() {
                    sub = ExecTurnEndTurn.subAllPostTrigger,
                    cb = (target, args) => {

                        onTurnEnd();

                    }
                }
            }
        };

        type = new TypeChannel(this, 4, soulChannelEffect);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 0 });

        nCd = 6;
        nFatigue = 1;
        nActionCost = 1;

        nBaseHealing = 10;
        //Create a base Healing object that this action will apply
        heal = new Healing(this.chrSource, this.chrSource, nBaseHealing);

        sDescription1 = "While channeling, at the end of turn heal 10.";

        SetArgOwners();
    }

    public void onTurnEnd() {

        ContAbilityEngine.Get().AddClause(new Clause() {
            fExecute = () => {

                //Make a copy of the heal object to give to the executable
                Healing healToApply = new Healing(heal);

                ContAbilityEngine.Get().AddExec(new ExecHeal() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,
                    
                    heal = healToApply, 
                    fDelay = ContTurns.fDelayStandard,
                    sLabel = chrSource.sName + " is regenerating"
                });
            }
        });

    }


    override public void Execute(int[] lstTargettingIndices) {

       

    }

}