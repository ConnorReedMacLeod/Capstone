using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionFortissimo : Action {

    public ActionFortissimo(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Note that we don't have any targets for this ability
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner);

        sName = "Fortissimo";
        type = ActionTypes.TYPE.CANTRIP;

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCd = 8;
        nFatigue = 0;
        nActionCost = 0;//0 since this is a cantrip

        sDescription = "[Cantrip]  +10 [Power] and +10 [Defense] for 4 turns";

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

                        return new SoulFortissimo(_chrSource, _chrTarget);

                    },

                    fDelay = 1.0f,
                    sLabel = "Applying Fortissimo"
                });
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;

        base.Execute();
    }
}
