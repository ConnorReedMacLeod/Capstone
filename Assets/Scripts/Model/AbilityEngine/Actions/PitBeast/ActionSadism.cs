using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSadism : Action {

    public Soul soulPassive;

    public ActionSadism(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner); 

        sName = "Sadism";
        type = new TypePassive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCd = 0;
        nFatigue = 0;
        nActionCost = 0;

        sDescription1 = "When " + _chrOwner.sName + " would deal damage to a character with greater health, heal 5.";

        SetArgOwners();
    }

    override public void Execute(int[] lstTargettingIndices) {

        Debug.LogError("Shouldn't be able to use " + sName + " since it's a passive ability");
    }

    public override void OnEquip() {

        stackClauses.Push(new Clause() {
            fExecute = () => {

                //Save a reference to the buff we're applying
                soulPassive = new SoulSadism(this.chrSource, this.chrSource);

                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,

                    funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {
                        return soulPassive;
                    },

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = chrSource.sName + " is sadistic"
                });
            }
        });

        base.OnEquip();
    }

    public override void OnUnequip() {
       
        stackClauses.Push(new Clause() {
            fExecute = () => {

                ContAbilityEngine.Get().AddExec(new ExecRemoveSoul() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,

                    soulToRemove = this.soulPassive,

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = chrSource.sName + " is no longer sadistic"
                });
            }
        });

        base.OnUnequip();
    }
}
