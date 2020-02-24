using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTantrum : Action {

    public ActionTantrum(Chr _chrOwner) : base(_chrOwner, 0) {

        sName = "Tantrum";
        sDisplayName = "Tantrum";

        type = new TypeActive(this);

        //Physical, Mental, Energy, Blood, Effort
        parCost = new Property<int[]>(new int[] { 1, 0, 0, 1, 0 });

        nCd = 9;
        nFatigue = 5;
        nActionCost = 1;

        lstClauses = new List<Clause>() {
            new Clause1(this)
        };
    }

    class Clause1 : ClauseSpecial {

        Damage dmgEnemy;
        public int nEnemyDamage = 20;

        Damage dmgAlly;
        public int nAllyDamage = 5;

        public Clause1(Action _act) : base(_act) {
            //TODO - add tags as needed
            
            dmgEnemy = new Damage(action.chrSource, null, nEnemyDamage);
            dmgAlly = new Damage(action.chrSource, null, nAllyDamage);
        }

        public override string GetDescription() {

            return string.Format("Deal {0} damage to all enemies and {1} damage to all other allies", dmgEnemy.Get(), dmgAlly.Get());
        }

        public override void ClauseEffect() {

            //First, deal damage to all enemies
            List<Chr> lstChrEnemy = action.chrSource.plyrOwner.GetEnemyPlayer().GetActiveChrs();

            for (int i = 0; i < lstChrEnemy.Count; i++) {
                //For each ally, deal our dmgAlly to them
                ContAbilityEngine.PushSingleExecutable(new ExecDealDamage(action.chrSource, lstChrEnemy[i], dmgEnemy) {
                    sLabel = "WAAAAAAAHWAAHWAHHH"
                });
            }

            //Then damage each of our allies
            List<Chr> lstChrAlly = action.chrSource.plyrOwner.GetActiveChrs();

            for(int i=0; i<lstChrAlly.Count; i++) {
                //For each ally, deal our dmgAlly to them
                ContAbilityEngine.PushSingleExecutable(new ExecDealDamage(action.chrSource, lstChrAlly[i], dmgAlly) {
                    sLabel = "Really, dude?"
                });
            }

        }

    };

}