using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionReverberate : Action {

    public ActionReverberate(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //We don't need to target anything, since we always deal damage to the enemy team
        // arArgs[0] = new TargetArgTeam((own, tar) => true); 

        sName = "Reverberate";
        type = ActionType.ACTIVE;

        //Physical, Mental, Energy, Blood, Effort
        arCost = new int[] { 0, 0, 0, 0, 0 };

        nCd = 8;
        nFatigue = 4;
        nActionCost = 1;

        sDescription = "Deal 5 damage to all enemies";

        SetArgOwners();
    }

    override public void Execute() {
        //It's a bit awkward that you have to do this typecasting, 
        // but at least it's eliminated from the targetting lambda

        int indexTargetPlayer = 0;
        //Ensure we target the other player
        if (indexTargetPlayer == chrOwner.plyrOwner.id) { 
            indexTargetPlayer = 1;
        }

        Player tar = Match.Get().arPlayers[indexTargetPlayer];

        stackClauses.Push(new Clause() {
            fExecute = () => {
                for (int i = 0; i < tar.arChr.Length; i++) {
                    Debug.Log("This Reverberate Clause put an ExecDamage on the stack");
                    //TODO:: Organize this in the correct order
                    ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                        chrOwner = this.chrOwner,
                        chrTarget = tar.arChr[i],
                        nDamage = 5,
                        fDelay = 1.0f,
                        sLabel = "Reverberating " + tar.arChr[i].sName
                    });
                }
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;
        base.Execute();
    }

}