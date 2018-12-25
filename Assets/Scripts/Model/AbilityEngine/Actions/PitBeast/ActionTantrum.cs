using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTantrum : Action {

    public ActionTantrum(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //We don't need to target anything, since we always deal damage to everyone
        // arArgs[0] = new TargetArgTeam((own, tar) => true); 

        sName = "Tantrum";
        type = ActionType.ACTIVE;

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 1, 0, 0, 1, 0 });

        nCd = 9;
        nFatigue = 5;
        nActionCost = 1;

        sDescription = "Deal 20 damage to all enemies and 5 damage to all other allies";

        SetArgOwners();
    }

    override public void Execute() {

        Player enemy = chrSource.GetEnemyPlayer();

        stackClauses.Push(new Clause() {
            fExecute = () => {
                //Deal 20 damage to all enemies
                for (int i = 0; i < enemy.arChr.Length; i++) {
                    Debug.Log("This Tantrum Clause put an ExecDamage on the stack");
                    Damage dmgToDeal = new Damage(chrSource, enemy.arChr[i], 20);

                    //TODO:: Organize this in the correct order
                    ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                        chrSource = this.chrSource,
                        chrTarget = enemy.arChr[i],
                        dmg = dmgToDeal,

                        fDelay = 1.0f,
                        sLabel = enemy.arChr[i].sName + " is caught in the tantrum"
                    });
                }

                //Deal 5 damage to all other allies
                for (int i = 0; i < chrSource.plyrOwner.arChr.Length; i++) {
                    if (chrSource.plyrOwner.arChr[i] == chrSource) continue; //Don't hurt yourself

                    Debug.Log("This Tantrum Clause put an ExecDamage on the stack");
                    Damage dmgToDeal = new Damage(chrSource, chrSource.plyrOwner.arChr[i], 5);

                    //TODO:: Organize this in the correct order
                    ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                        chrSource = this.chrSource,
                        chrTarget = chrSource.plyrOwner.arChr[i],
                        dmg = dmgToDeal,

                        fDelay = 1.0f,
                        sLabel = chrSource.plyrOwner.arChr[i].sName + " is caught in the tantrum"
                    });
                }
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;
        base.Execute();
    }

}