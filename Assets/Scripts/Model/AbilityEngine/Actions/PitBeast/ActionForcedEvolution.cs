using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionForcedEvolution : Action {

    public ActionForcedEvolution(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Note that we don't have any targets for this ability
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner);

        sName = "ForcedEvolution";
        type = ActionType.ACTIVE;

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 1, 0 });

        nCd = 6;
        nFatigue = 1;
        nActionCost = 1;

        sDescription = "Gain 5 [Power].  Lose 5 Life";

        SetArgOwners();
    }


    override public void Execute() {
        //No typecasting is needed since we have no targets
        //Chr tar = ((TargetArgChr)arArgs[0]).chrTar;

        stackClauses.Push(new Clause() {
            fExecute = () => {
                Debug.Log("Putting ForcedEvolution buff on self");
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,

                    funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {

                        return new SoulEvolved(_chrSource, _chrTarget);

                    },

                    fDelay = 1.0f,
                    sLabel = sName + " is evolving"
                });
            }
        });

        stackClauses.Push(new Clause() {
            fExecute = () => {
                Debug.Log("This Evolution Clause put an LoseLife on the stack");
                ContAbilityEngine.Get().AddExec(new ExecLoseLife() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,
                    nLifeLoss = 5,

                    fDelay = 1.0f,
                    sLabel = this.chrSource.sName + " is going berserk"
                });
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

        base.Execute();
    }
}
