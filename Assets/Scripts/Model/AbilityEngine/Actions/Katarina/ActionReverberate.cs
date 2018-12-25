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
        parCost = new Property<int[]>(new int[] { 0, 0, 0, 0, 0 });

        nCd = 8;
        nFatigue = 4;
        nActionCost = 1;

        sDescription = "Deal 5 damage to all enemies";

        SetArgOwners();
    }

    override public void Execute() {

        Player tar = chrSource.GetEnemyPlayer();

        stackClauses.Push(new Clause() {
            fExecute = () => {
                for (int i = 0; i < tar.arChr.Length; i++) {
                    Debug.Log("This Reverberate Clause put an ExecDamage on the stack");
                    Damage dmgToDeal = new Damage(chrSource, tar.arChr[i], 5);

                    //TODO:: Organize this in the correct order
                    ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                        chrSource = this.chrSource,
                        chrTarget = tar.arChr[i],
                        dmg = dmgToDeal,

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