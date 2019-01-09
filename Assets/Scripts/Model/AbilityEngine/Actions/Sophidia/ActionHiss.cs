using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHiss : Action {


    public ActionHiss(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        //arArgs[0] = new TargetArgTeam((own, tar) => true); // No selection necessary

        sName = "Hiss";
        type = ActionType.CANTRIP;

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 1 });

        nCd = 10;
        nFatigue = 1;
        nActionCost = 0;

        sDescription = "[CANTRIP] All enemies lose 10 [Power] for 3 turns";

        SetArgOwners();
    }

    override public void Execute() {

        int indexTargetPlayer = 0;
        //Ensure we target the other player
        if (indexTargetPlayer == chrSource.plyrOwner.id) {
            indexTargetPlayer = 1;
        }

        Player tar = Match.Get().arPlayers[indexTargetPlayer];

        stackClauses.Push(new Clause() {
            fExecute = () => {
                for (int i = 0; i < tar.arChr.Length; i++) {
                    //TODO:: Organize this in the correct order
                    ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                        chrSource = this.chrSource,
                        chrTarget = tar.arChr[i],

                        funcCreateSoul = (_chrSource, _chrTarget) => {
                            return new SoulSpooked(_chrSource, _chrTarget);
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