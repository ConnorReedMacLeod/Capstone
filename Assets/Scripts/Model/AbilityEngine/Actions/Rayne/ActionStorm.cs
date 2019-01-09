using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionStorm : Action {

    public int nEnemyDamage;
    public int nStunDuration;

    public ActionStorm(Chr _chrOwner) : base(0, _chrOwner) {//number of target arguments

        //We don't need to target anything, since we always deal damage to everyone
        // arArgs[0] = new TargetArgTeam((own, tar) => true); 

        sName = "Storm";
        type = ActionType.ACTIVE;

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 0, 1, 1, 0, 0 });

        nCd = 10;
        nFatigue = 5;
        nActionCost = 1;

        sDescription = "Deal 15 damage and 2 fatigue to all enemies.";

        nEnemyDamage = 15;
        nStunDuration = 2;

        SetArgOwners();
    }

    override public void Execute() {

        Player enemy = chrSource.GetEnemyPlayer();

        stackClauses.Push(new Clause() {
            fExecute = () => {
                //Deal damage to all enemies
                for (int i = 0; i < enemy.arChr.Length; i++) {
                    Damage dmgToDeal = new Damage(chrSource, enemy.arChr[i], nEnemyDamage);

                    //TODO:: Organize this in the correct order
                    ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                        chrSource = this.chrSource,
                        chrTarget = enemy.arChr[i],
                        dmg = dmgToDeal,

                        fDelay = 1.0f,
                        sLabel = enemy.arChr[i].sName + " is caught in the storm"
                    });
                }

                //Stun all enemies
                for (int i = 0; i < enemy.arChr.Length; i++) {

                    //TODO:: Organize this in the correct order
                    ContAbilityEngine.Get().AddExec(new ExecStun() {
                        chrSource = this.chrSource,
                        chrTarget = enemy.arChr[i],
                        nAmount = nStunDuration,

                        fDelay = 1.0f,
                        sLabel = chrSource.plyrOwner.arChr[i].sName + " is being stunned"
                    });
                }
            }
        });


        //NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;
        base.Execute();
    }

}