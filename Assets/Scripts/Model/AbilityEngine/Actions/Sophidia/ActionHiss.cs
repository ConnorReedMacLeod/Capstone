using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHiss : Action {


    public ActionHiss(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //Since the base constructor initializes this array, we can start filling it
        //arArgs[0] = new TargetArgTeam((own, tar) => true); // No selection necessary

        sName = "Hiss";
        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 1 });

        nCd = 10;
        nFatigue = 1;
        nActionCost = 0;

        sDescription1 = "All enemies lose 10 POWER for 3 turns.";

        SetArgOwners();
    }

    override public void Execute(int[] lstTargettingIndices) {

        Player tarPlyr = chrSource.GetEnemyPlayer();

        stackClauses.Push(new Clause() {
            fExecute = () => {
                for (int i = 0; i < tarPlyr.arChr.Length; i++) {

                    //Don't target dead characters
                    if (tarPlyr.arChr[i].bDead) continue;

                    //TODO:: Organize this in the correct order
                    ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
                        chrSource = this.chrSource,
                        chrTarget = tarPlyr.arChr[i],

                        funcCreateSoul = (_chrSource, _chrTarget) => {
                            return new SoulSpooked(_chrSource, _chrTarget);
                        },

                        fDelay = ContTurns.fDelayNone,
                        sLabel = "Applying Hiss "
                    });
                }
            }
        });

    }

}