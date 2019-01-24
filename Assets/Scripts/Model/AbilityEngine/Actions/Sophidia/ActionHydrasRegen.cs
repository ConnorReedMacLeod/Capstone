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

        sDescription = "For 4 turns, while channeling heal 10 at the end of turn";

        SetArgOwners();
    }

    public void onTurnEnd() {

        ContAbilityEngine.Get().AddClause(new Clause() {
            fExecute = () => {

                ContAbilityEngine.Get().AddExec(new ExecHeal() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,
                    
                    heal = this.heal, //I don't think we need to make a separate copy
                    fDelay = 1.0f,
                    sLabel = chrSource.sName + " is regenerating"
                });
            }
        });

    }


    override public void Execute() {

       

    }

}