using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSmokeCover : Action {

    public ActionSmokeCover(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Note that we don't have any targets for this ability
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner);

        sName = "SmokeCover";
        type = new TypeCantrip(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 1, 0, 0, 0 });

        nCd = 10;
        nFatigue = 2;
        nActionCost = 0;//0 since this is a cantrip

        sDescription = "[Cantrip]  Gain \"Immune to damage to damage\" for 4 turns.\n" +
            "If this character blocks, dispel";

        SetArgOwners();
    }


    override public void Execute() {
        //No typecasting is needed since we have no targets
        //Chr tar = ((TargetArgChr)arArgs[0]).chrTar;

        stackClauses.Push(new Clause() {
            fExecute = () => {
                
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrSource = this.chrSource,
                    chrTarget = this.chrSource,

                    funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {

                        return new SoulSmokeCover(_chrSource, _chrTarget);

                    },

                    fDelay = ContTurns.fDelayStandard,
                    sLabel = "Applying SmokeCover"
                });
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

        base.Execute();
    }
}
