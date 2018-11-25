using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionFortissimo : Action {

    public ActionFortissimo(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Note that we don't have any targets for this ability
        //arArgs[0] = new TargetArgChr((own, tar) => own.plyrOwner != tar.plyrOwner);

        sName = "Fortissimo";
        type = ActionType.ACTIVE;

        //Physical, Mental, Energy, Blood, Effort
        arCost = new int[] { 0, 0, 0, 0, 0 };

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
                Debug.Log("Putting Fortissimo buff on self");
                ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                    chrOwner = this.chrOwner,
                    soulContainerTarget = chrOwner.soulContainer,

                    funcCreateSoul = () => {
                        return new Soul() {
                            sName = "Fortissimo",
                            bVisible = true,
                            bDuration = true,

                            nMaxDuration = 4,

                            funcOnApplication = () => {
                                Debug.Log("TODO:: Apply a Power and Defense Buff now");
                            },

                            funcOnRemoval = () => {
                                Debug.Log("TODO:: Remove the Power and Defense Buff now");
                            }
                        };
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
