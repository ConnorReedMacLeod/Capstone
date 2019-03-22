using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionForcedEvolution : Action {

    public ActionForcedEvolution(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Note that we don't have any targets for this ability
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner);

        sName = "ForcedEvolution";
        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 0 });

        nCd = 6;
        nFatigue = 1;
        nActionCost = 1;

        sDescription1 = "Lose 5 health.  Gain 5 POWER.";

        SetArgOwners();
    }


    override public void Execute(int[] lstTargettingIndices) {

        stackClauses.Push(new Clause() {
            fExecute = () => {
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,

                    funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {

                        return new SoulEvolved(_chrSource, _chrTarget);

                    },

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = sName + " is evolving"
                });
            }
        });

        stackClauses.Push(new Clause() {
            fExecute = () => {
                ContAbilityEngine.Get().AddExec(new ExecLoseLife() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,
                    nLifeLoss = 5,

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = this.chrSource.sName + " is going berserk"
                });
            }
        });
    }
}
