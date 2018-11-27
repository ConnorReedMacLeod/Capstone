using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHiss : Action {


    public ActionHiss(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        //arArgs[0] = new TargetArgTeam((own, tar) => true); // No selection necessary

        sName = "Hiss";
        type = ActionType.ACTIVE;

        //Physical, Mental, Energy, Blood, Effort
        arCost = new int[] { 0, 0, 0, 0, 1 };

        nCd = 10;
        nFatigue = 1;
        nActionCost = 0;

        sDescription = "All enemies lose 10 [Power] for 3 turns";

        SetArgOwners();
    }

    override public void Execute() {

        int indexTargetPlayer = 0;
        //Ensure we target the other player
        if (indexTargetPlayer == chrOwner.plyrOwner.id) {
            indexTargetPlayer = 1;
        }

        Player tar = Match.Get().arPlayers[indexTargetPlayer];

        stackClauses.Push(new Clause() {
            fExecute = () => {
                for (int i = 0; i < tar.arChr.Length; i++) {
                    Debug.Log("This Hiss Clause put an ApplySoul on the stack");
                    //TODO:: Organize this in the correct order
                    ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                        chrOwner = this.chrOwner,
                        soulContainerTarget = tar.arChr[i].soulContainer,

                        funcCreateSoul = () => {
                            return new Soul() {
                                sName = "Spooked",
                                bVisible = true,
                                bDuration = true,

                                nMaxDuration = 3,

                                funcOnApplication = () => {
                                    Debug.Log("TODO:: Apply a Power debuff now");
                                },

                                funcOnRemoval = () => {
                                    Debug.Log("TODO:: Remove the Power debuff now");
                                }
                            };
                        },

                        fDelay = 1.0f,
                        sLabel = "Applying Hiss "
                    });
                }
            }
        });

        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;


        base.Execute();
    }

}